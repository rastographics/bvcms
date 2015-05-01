using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using CmsData;
using CmsData.Codes;
using CmsData.View;
using UtilityExtensions;
using Task = System.Threading.Tasks.Task;

namespace CmsWeb.Areas.Dialog.Models
{
    public class OrgDrop : LongRunningOp
    {
        public const string Op = "orgdrop";

        public string OrgName { get; set; }
        public DateTime? DropDate { get; set; }
        public bool RemoveFromEnrollmentHistory { get; set; }

        public OrgDrop() { }
        public OrgDrop(int id)
        {
            Id = id;
            var org = DbUtil.Db.LoadOrganizationById(id);
            OrgName = org.OrganizationName;
            Count = People(DbUtil.Db.CurrentOrg).Count();
        }

        public IQueryable<OrgPerson> People(ICurrentOrg co)
        {
            var q = from p in DbUtil.Db.OrgPeople(Id, co.GroupSelect,
                        co.First(), co.Last(), co.SgFilter, co.ShowHidden,
                        Util2.CurrentTag, Util2.CurrentTagOwnerId,
                        co.FilterIndividuals, co.FilterTag, false, Util.UserPeopleId)
                    select p;
            return q;
        }

        public string Group
        {
            get
            {
                switch (DbUtil.Db.CurrentOrg.GroupSelect)
                {
                    case GroupSelectCode.Member:
                        return "Current Members";
                    case GroupSelectCode.Inactive:
                        return "Inactive Members";
                    case GroupSelectCode.Pending:
                        return "Pending Members";
                    case GroupSelectCode.Prospect:
                        return "Prospects";
                }
                return "People";
            }
        }

        private List<int> pids; 
        public void Process(CMSDataContext db)
        {
            // running has not started yet, start it on a separate thread
            pids = (from p in People(db.CurrentOrg) select p.PeopleId).ToList();
            var lop = new LongRunningOp()
            {
                Started = DateTime.Now,
                Count = pids.Count,
                Processed = 0,
                Id = Id,
                Operation = Op,
            };
            db.LongRunningOps.InsertOnSubmit(lop);
            db.SubmitChanges();
            Task.Run(() => DoWork(this));
        }

        private void DoWork(OrgDrop model)
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            var db = new CMSDataContext(Util.GetConnectionString(model.host));
            db.Host = model.host;
            var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOp lop = null;
            foreach (var pid in pids)
            {
                db.Dispose();
                db = new CMSDataContext(Util.GetConnectionString(model.host));
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
            }
            // finished
            lop = FetchLongRunningOp(db, model.Id, Op);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
        }
    }
}