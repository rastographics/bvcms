using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using UtilityExtensions;
using Tasks = System.Threading.Tasks;

namespace CmsWeb.Models
{
    public class AddToOrgFromTag : LongRunningOp
    {
        public const string Op = "addtoorgfromtag";

        public AddToOrgFromTag() { }
        public AddToOrgFromTag(int id, string group)
        {
            Id = id;
            Group = group;
            Tag = new CodeInfo("0", "Tag");
        }
        [DisplayName("Choose A Tag")]
        public CodeInfo Tag { get; set; }
        public string Group { get; set; }

        public string GroupName
        {
            get
            {
                switch (Group)
                {
                    case GroupSelectCode.Member:
                        return "Members";
                    case GroupSelectCode.Pending:
                        return "Pending Members";
                    case GroupSelectCode.Prospect:
                        return "Prospects";
                    case GroupSelectCode.Inactive:
                        return "Inactive Members";
                    case GroupSelectCode.Previous:
                        return "Previous Members";
                }
                return null;
            }
        }

        internal List<int> pids;
  
        public bool TagHasBeenSelected
        {
            get { return Count.HasValue; }
        }

        public void Process(CMSDataContext db)
        {
            // running has not started yet, start it on a separate thread
            pids = FetchPeopleIds(db, Tag.Value.ToInt()).ToList();
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
            Tasks.Task.Run(() => DoWork(this));
        }

        private static void DoWork(AddToOrgFromTag model)
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            var db = new CMSDataContext(Util.GetConnectionString(model.host));
            db.Host = model.host;
            var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOp lop = null;
            foreach (var pid in model.pids)
            {
                db.Dispose();
                db = new CMSDataContext(Util.GetConnectionString(model.host));
                switch (model.Group)
                {
                    case GroupSelectCode.Member:
                        OrganizationMember.InsertOrgMembers(db, model.Id, pid, MemberTypeCode.Member, DateTime.Now, null, pending: false);
                        break;
                    case GroupSelectCode.Pending:
                        OrganizationMember.InsertOrgMembers(db, model.Id, pid, MemberTypeCode.Member, DateTime.Now, null, pending: true);
                        break;
                    case GroupSelectCode.Prospect:
                        OrganizationMember.InsertOrgMembers(db, model.Id, pid, MemberTypeCode.Prospect, DateTime.Now, null, pending: false);
                        break;
                    case GroupSelectCode.Inactive:
                        OrganizationMember.InsertOrgMembers(db, model.Id, pid, MemberTypeCode.InActive, DateTime.Now, DateTime.Now, pending: false);
                        break;
                    case GroupSelectCode.Previous:
                        Organization.AddAsPreviousMember(db, model.Id, pid, MemberTypeCode.InActive, DateTime.Now, DateTime.Now);
                        break;
                }
                lop = FetchLongRunningOp(db, model.Id, Op);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                db.SubmitChanges();
                //Thread.Sleep(1000);
            }
            // finished
            lop = FetchLongRunningOp(db, model.Id, Op);
            lop.Completed = DateTime.Now;
            db.SubmitChanges();
        }

        public void Validate(ModelStateDictionary modelState)
        {
            if (Tag != null && Tag.Value == "0") // They did not choose a tag
                modelState.AddModelError("Tag", "Must choose a tag");
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