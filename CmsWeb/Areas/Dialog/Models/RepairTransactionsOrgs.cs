using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using CmsData;
using CmsWeb.Areas.Search.Models;
using Newtonsoft.Json;
using UtilityExtensions;
using Task = System.Threading.Tasks.Task;

namespace CmsWeb.Areas.Dialog.Models
{
    public class RepairTransactionsOrgs : LongRunningOp
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
            Id = Util.UserPeopleId ?? 0;
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
            var lop = new LongRunningOp()
            {
                Started = DateTime.Now,
                Count = orginfos.Count,
                Processed = 0,
                Id = Id,
                Operation = Op,
            };
            db.LongRunningOps.InsertOnSubmit(lop);
            db.SubmitChanges();
            Task.Run(() => DoWork(this));
		}
        public static void DoWork(RepairTransactionsOrgs model)
        {
			Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
			var db = DbUtil.Create(model.host);
            db.CommandTimeout = 2200;
		    var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);
            LongRunningOp lop = null;
            foreach (var orginfo in model.orginfos)
		    {
	            db.RepairTransactionsOrgs(orginfo.Id);
                lop = FetchLongRunningOp(db, model.Id, Op);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                lop.CustomMessage = $"Working on {orginfo.Name} ({orginfo.Id})";
                db.SubmitChanges();
		    }
            // finished
            lop = FetchLongRunningOp(db, model.Id, Op);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
		}
    }
}
