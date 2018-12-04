using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class AddAttendeesFromTag : LongRunningOperation
    {
        public const string Op = "addattendeesfromtag";

        public int UserId { get; set; }
        public string OrgName { get; set; }
        public int MeetingId { get; set; }
        public int OrgId { get; set; }
        public bool AddAsMembers { get; set; }
        public DateTime JoinDate { get; set; }

        public AddAttendeesFromTag() { }
        public AddAttendeesFromTag(int id)
        {
            QueryId = Guid.NewGuid();
            MeetingId = id;
            UserId = Util.UserId;
            var i = (from m in DbUtil.Db.Meetings
                     where m.MeetingId == id
                     select new
                     {
                         m.Organization.OrganizationName,
                         m.OrganizationId,
                         m.MeetingDate
                     }).Single();
            OrgName = i.OrganizationName;
            OrgId = i.OrganizationId;
            JoinDate = i.MeetingDate.Value.Date;
            Tag = new CodeInfo("0", "Tag");
        }
        [DisplayName("Choose A Tag")]
        public CodeInfo Tag { get; set; }

        internal List<int> pids;

        public bool TagHasBeenSelected => Count.HasValue;

        public void Process(CMSDataContext db)
        {
            // running has not started yet, start it on a separate thread
            pids = FetchPeopleIds(db, Tag.Value.ToInt()).ToList();
            var lop = new LongRunningOperation()
            {
                Started = DateTime.Now,
                Count = pids.Count,
                Processed = 0,
                QueryId = QueryId,
                Operation = Op,
            };
            db.LongRunningOperations.InsertOnSubmit(lop);
            db.SubmitChanges();
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoWork(this));
        }

        private static void DoWork(AddAttendeesFromTag model)
        {
            var db = DbUtil.Create(model.Host);
            var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOperation lop = null;
            foreach (var pid in model.pids)
            {
                //db.Dispose();
                //db = db.Create(model.Host);
                if (model.AddAsMembers)
                {
                    OrganizationMember.InsertOrgMembers(db, model.OrgId, pid,
                        MemberTypeCode.Member, model.JoinDate, null, false);
                }

                db.RecordAttendance(model.MeetingId, pid, true);
                lop = FetchLongRunningOperation(db, Op, model.QueryId);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                db.SubmitChanges();
            }
            // finished
            lop = FetchLongRunningOperation(db, Op, model.QueryId);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
        }

        public void Validate(ModelStateDictionary modelState)
        {
            if (Tag != null && Tag.Value == "0") // They did not choose a tag
            {
                modelState.AddModelError("Tag", "Must choose a tag");
            }
        }

        public bool ShowCount(CMSDataContext db)
        {
            if (Count == null && Tag != null)
            {
                var q = FetchPeopleIds(db, Tag.Value.ToInt());
                Count = q.Count();
                db.SubmitChanges();
                return true;
            }
            return false;
        }

        public static IQueryable<int> FetchPeopleIds(CMSDataContext db, int tagid)
        {
            return tagid == -1
                ? db.PeopleQueryLast().Select(pp => pp.PeopleId)
                : from t in db.TagPeople
                  where t.Id == tagid
                  select t.PeopleId;
        }
    }
}
