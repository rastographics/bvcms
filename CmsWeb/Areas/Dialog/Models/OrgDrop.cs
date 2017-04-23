using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class OrgDrop : LongRunningOp
    {
        public const string Op = "orgdrop";

        public string OrgName { get; set; }
        public DateTime? DropDate { get; set; }
        public bool RemoveFromEnrollmentHistory { get; set; }
        public string Group { get; set; }
        public int UserId { get; set; }

        public OrgDrop()
        {
            UserId = Util.UserId;
        }
        public OrgDrop(Guid id)
            : this()
        {
            QueryId = id;
            var filter = DbUtil.Db.OrgFilter(id);
            var org = DbUtil.Db.LoadOrganizationById(filter.Id);
            Id = filter.Id;
            OrgName = org.OrganizationName;
            pids = DbUtil.Db.OrgFilterIds(id)
                .Select(vv => vv.PeopleId.Value).ToList();
            Count = pids.Count;

            Group = "People"; // default
            switch (filter.GroupSelect)
            {
                case GroupSelectCode.Member:
                    Group = "Members";
                    break;
                case GroupSelectCode.Inactive:
                    Group = "Inactive";
                    break;
                case GroupSelectCode.Pending:
                    Group = "Pending";
                    break;
                case GroupSelectCode.Prospect:
                    Group = "Prospects";
                    break;
            }
        }

        private readonly List<int> pids;
        public void Process(CMSDataContext db)
        {
            // running has not started yet, start it on a separate thread
            Started = DateTime.Now;
            var lop = new LongRunningOp()
            {
                Started = Started,
                Count = pids.Count,
                Processed = 0,
                Id = Id,
                Operation = Op,
            };
            db.LogActivity($"OrgDrop {lop.Count} records", Id, uid: UserId);
            db.LongRunningOps.InsertOnSubmit(lop);
            db.SubmitChanges();
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoWork(this));
        }

        private static void DoWork(OrgDrop model)
        {
            var db = DbUtil.Create(model.host);
            var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOp lop = null;
            foreach (var pid in model.pids)
            {
                db.Dispose();
                db = DbUtil.Create(model.host);
                var om = db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == model.Id);
                if (model.DropDate.HasValue)
                    om.Drop(db, model.DropDate.Value);
                else
                    om.Drop(db);
                db.SubmitChanges();
                if (model.RemoveFromEnrollmentHistory)
                    db.ExecuteCommand("DELETE dbo.EnrollmentTransaction WHERE PeopleId = {0} AND OrganizationId = {1}", pid, model.Id);
                lop = FetchLongRunningOp(db, model.Id, Op);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                db.SubmitChanges();
                db.LogActivity($"Org{model.Group} Drop{(model.RemoveFromEnrollmentHistory ? " w/history" : "")}", model.Id, pid, uid: model.UserId);
            }
            // finished
            lop = FetchLongRunningOp(db, model.Id, Op);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
        }

        public void DropSingleMember(int orgId, int peopleId)
        {
            var org = DbUtil.Db.LoadOrganizationById(orgId);
            var om = org.OrganizationMembers.Single(mm => mm.PeopleId == peopleId);
            om.Drop(DbUtil.Db);
            DbUtil.Db.SubmitChanges();
        }
    }
}
