using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using CmsData;
using Newtonsoft.Json;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class OrgSearchDrop : LongRunningOp
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
            Id = Util.UserPeopleId ?? 0;
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
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoWork(this));
        }

        private void DoWork(OrgSearchDrop model)
        {
            var db = DbUtil.Create(model.host);
            var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOp lop = null;
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
                    db.Dispose();
                    db = DbUtil.Create(model.host);
                    var om = db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == orginfo.Id);
                    if (DropDate.HasValue)
                        om.Drop(db, DropDate.Value);
                    else
                        om.Drop(db);
                    lop = FetchLongRunningOp(db, model.Id, Op);
                    Debug.Assert(lop != null, "r != null");
                    lop.Processed++;
                    lop.CustomMessage = $"Working on {orginfo.Name}, {n}/{pids.Count}";
                    db.SubmitChanges();
                }
                var o = db.LoadOrganizationById(orginfo.Id);
                o.OrganizationStatusId = CmsData.Codes.OrgStatusCode.Inactive;
                db.SubmitChanges();
            }
            // finished
            lop = FetchLongRunningOp(db, model.Id, Op);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
        }
    }
}
