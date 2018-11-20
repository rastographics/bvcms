using CmsData;
using CmsWeb.Areas.Search.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class RepairTransactionsOrgs : LongRunningOperation
    {
        private class OrgInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
        }
        public const string Op = "RepairTransactionsOrgs";
        public string Orgs { get; set; }

        public RepairTransactionsOrgs() { }
        public RepairTransactionsOrgs(OrgSearchModel m)
        {
            QueryId = Guid.NewGuid();
            var q = (from o in m.FetchOrgs()
                     select new OrgInfo
                     {
                         Id = o.OrganizationId,
                         Name = o.OrganizationName,
                         Count = o.MemberCount ?? 0
                     }).ToList();
            Count = q.Count();
            Orgs = JsonConvert.SerializeObject(q);
        }

        private List<OrgInfo> orginfos;
        public void Process(CMSDataContext db)
        {
            // running has not started yet, start it on a separate thread
            orginfos = JsonConvert.DeserializeObject<List<OrgInfo>>(Orgs);
            var lop = new LongRunningOperation()
            {
                Started = DateTime.Now,
                Count = orginfos.Count,
                Processed = 0,
                QueryId = QueryId,
                Operation = Op,
            };
            db.LongRunningOperations.InsertOnSubmit(lop);
            db.SubmitChanges();
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoWork(this));
        }
        public static void DoWork(RepairTransactionsOrgs model)
        {
            var db = DbUtil.Create(model.Host);
            db.CommandTimeout = 2200;
            var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
            LongRunningOperation lop = null;
            foreach (var orginfo in model.orginfos)
            {
                db.RepairTransactionsOrgs(orginfo.Id);
                lop = FetchLongRunningOperation(db, Op, model.QueryId);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                lop.CustomMessage = $"Working on {orginfo.Name.Truncate(170)} ({orginfo.Id})";
                db.SubmitChanges();
            }
            // finished
            lop = FetchLongRunningOperation(db, Op, model.QueryId);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
        }
    }
}
