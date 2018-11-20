using CmsData;
using CmsData.Codes;
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
    public class OrgDrop : LongRunningOperation
    {
        public const string Op = "orgdrop";

        public string DisplayGroup
        {
            get
            {
                switch (Filter.GroupSelect)
                {
                    case GroupSelectCode.Member:
                        return "Members";
                    case GroupSelectCode.Inactive:
                        return "Inactive";
                    case GroupSelectCode.Pending:
                        return "Pending";
                    case GroupSelectCode.Prospect:
                        return "Prospects";
                    default:
                        throw new Exception("Unknown group " + Filter.GroupSelect);
                }
            }
        }

        public DateTime? DropDate { get; set; }
        public bool RemoveFromEnrollmentHistory { get; set; }
        public int UserId { get; set; }


        public OrgDrop()
        {
            UserId = Util.UserId;
        }
        public OrgDrop(Guid id)
            : this()
        {
            QueryId = id;
        }

        private OrgFilter filter;
        public OrgFilter Filter => filter ?? (filter = DbUtil.Db.OrgFilter(QueryId));
        public int OrgId => Filter.Id;

        public int DisplayCount => Count ?? (Count = DbUtil.Db.OrgFilterIds(QueryId).Count()) ?? 0;

        private string orgname;
        public string OrgName => orgname ?? (orgname = DbUtil.Db.Organizations.Where(vv => vv.OrganizationId == Filter.Id).Select(vv => vv.OrganizationName).Single());

        private List<int> pids;
        private List<int> Pids => pids ?? (pids = (from p in DbUtil.Db.OrgFilterIds(QueryId)
                                                   select p.PeopleId.Value).ToList());

        public void Process(CMSDataContext db)
        {
            // running has not started yet, start it on a separate thread
            Started = DateTime.Now;
            var lop = new LongRunningOperation()
            {
                QueryId = QueryId,
                Started = Started,
                Count = Pids.Count,
                Processed = 0,
                Operation = Op,
            };
            DbUtil.Db.LogActivity($"OrgDrop {lop.Count} records", Filter.Id, uid: UserId);
            db.LongRunningOperations.InsertOnSubmit(lop);
            db.SubmitChanges();
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoWork(this));
        }

        private static void DoWork(OrgDrop model)
        {
            var db = DbUtil.Create(model.Host);
            var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOperation lop = null;
            foreach (var pid in model.Pids)
            {
                //DbUtil.Db.Dispose();
                //db = DbUtil.Create(model.Host);
                var om = db.OrganizationMembers.Single(mm => mm.PeopleId == pid && mm.OrganizationId == model.filter.Id);
                if (model.DropDate.HasValue)
                {
                    om.Drop(db, model.DropDate.Value);
                }
                else
                {
                    om.Drop(db);
                }

                db.SubmitChanges();
                if (model.RemoveFromEnrollmentHistory)
                {
                    db.ExecuteCommand("DELETE dbo.EnrollmentTransaction WHERE PeopleId = {0} AND OrganizationId = {1}", pid, model.filter.Id);
                }

                lop = FetchLongRunningOperation(db, Op, model.QueryId);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                db.SubmitChanges();
                db.LogActivity($"Org{model.DisplayGroup} Drop{(model.RemoveFromEnrollmentHistory ? " w/history" : "")}", model.filter.Id, pid, uid: model.UserId);
            }
            // finished
            lop = FetchLongRunningOperation(db, Op, model.QueryId);
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
