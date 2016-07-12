using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using CmsData;
using CmsData.Codes;
using CmsData.View;
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
        public OrgDrop(int id)
            : this()
        {
            Id = id;
            var org = DbUtil.Db.LoadOrganizationById(id);
            OrgName = org.OrganizationName;
            Count = People(DbUtil.Db.CurrentOrg).Count();

            Group = "People"; // default
            switch (DbUtil.Db.CurrentOrg.GroupSelect)
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

        public IQueryable<OrgPerson> People(ICurrentOrg co)
        {
            var q = from p in DbUtil.Db.OrgPeople(Id, co.GroupSelect,
                        co.First(), co.Last(), co.SgFilter, co.ShowHidden,
                        Util2.CurrentTagName, Util2.CurrentTagOwnerId,
                        co.FilterIndividuals, co.FilterTag, false, Util.UserPeopleId)
                    select p;
            return q;
        }

        private List<int> pids;
        public void Process(CMSDataContext db)
        {
            // running has not started yet, start it on a separate thread
            pids = (from p in People(db.CurrentOrg) select p.PeopleId.Value).ToList();
            Started = DateTime.Now;
            var lop = new LongRunningOp()
            {
                Started = Started,
                Count = pids.Count,
                Processed = 0,
                Id = Id,
                Operation = Op,
            };
            db.LongRunningOps.InsertOnSubmit(lop);
            db.SubmitChanges();
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoWork(this));
        }

        private void DoWork(OrgDrop model)
        {
            var db = DbUtil.Create(model.host);
            var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOp lop = null;
            foreach (var pid in pids)
            {
                db.Dispose();
                db = DbUtil.Create(model.host);
                var om = db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == Id);
                if (DropDate.HasValue)
                    om.Drop(db, DropDate.Value);
                else
                    om.Drop(db);
                db.SubmitChanges();
                if (RemoveFromEnrollmentHistory)
                    db.ExecuteCommand("DELETE dbo.EnrollmentTransaction WHERE PeopleId = {0} AND OrganizationId = {1}", pid, Id);
                lop = FetchLongRunningOp(db, model.Id, Op);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                db.SubmitChanges();
                db.LogActivity($"Org{Group} Drop{(RemoveFromEnrollmentHistory ? " w/history" : "")}", Id, pid, UserId);
            }
            // finished
            lop = FetchLongRunningOp(db, model.Id, Op);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
        }
    }
}
