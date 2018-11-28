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
    public class OrgSearchDrop : LongRunningOperation
    {
        private class OrgInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
        }
        public const string Op = "orgsearchdrop";

        public DateTime? DropDate { get; set; }
        public string Orgs { get; set; }
        public int OrgCount { get; set; }

        public OrgSearchDrop() { }
        public OrgSearchDrop(OrgSearchModel m)
        {

            QueryId = Guid.NewGuid();
            var q = (from o in m.FetchOrgs()
                     select new OrgInfo
                     {
                         Id = o.OrganizationId,
                         Name = o.OrganizationName,
                         Count = o.MemberCount ?? 0
                     }).ToList();
            Count = q.Sum(mm => mm.Count);
            OrgCount = q.Count();
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

        private void DoWork(OrgSearchDrop model)
        {
            var db = DbUtil.Create(model.Host);
            var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOperation lop = null;
            foreach (var orginfo in model.orginfos)
            {
                var pids = (from m in db.OrganizationMembers
                            where m.OrganizationId == orginfo.Id
                            select m.PeopleId
                    ).ToList();
                var n = 0;
                foreach (var pid in pids)
                {
                    n++;
                    //DbUtil.Db.Dispose();
                    //db = DbUtil.Create(model.Host);
                    var om = db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == orginfo.Id);
                    if (DropDate.HasValue)
                    {
                        om.Drop(db, DropDate.Value);
                    }
                    else
                    {
                        om.Drop(db);
                    }

                    lop = FetchLongRunningOperation(db, Op, model.QueryId);
                    Debug.Assert(lop != null, "r != null");
                    lop.Processed++;
                    lop.CustomMessage = $"Working on {orginfo.Name.Truncate(170)}, {n}/{pids.Count}";
                    db.SubmitChanges();
                }
                var o = db.LoadOrganizationById(orginfo.Id);
                o.OrganizationStatusId = CmsData.Codes.OrgStatusCode.Inactive;
                db.SubmitChanges();
            }
            // finished
            lop = FetchLongRunningOperation(db, Op, model.QueryId);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
        }
    }
}
