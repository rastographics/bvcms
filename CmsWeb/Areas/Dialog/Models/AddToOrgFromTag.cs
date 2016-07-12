using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using UtilityExtensions;
using Tasks = System.Threading.Tasks;

namespace CmsWeb.Areas.Dialog.Models
{
    public class AddToOrgFromTag : LongRunningOp
    {
        public const string Op = "addtoorgfromtag";

        public int UserId { get; set; }
        public string OrgName { get; set; }
        public AddToOrgFromTag() { }
        public AddToOrgFromTag(int id, string group)
        {
            Id = id;
            Group = group;
            UserId = Util.UserId;
            if (group == GroupSelectCode.Previous)
            {
                var org = DbUtil.Db.LoadOrganizationById(id);
                OrgName = org.OrganizationName;
            }
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
                        return "Pending";
                    case GroupSelectCode.Prospect:
                        return "Prospects";
                    case GroupSelectCode.Inactive:
                        return "Inactive";
                    case GroupSelectCode.Previous:
                        return "Previous";
                }
                return null;
            }
        }

        internal List<int> pids;

        public bool TagHasBeenSelected => Count.HasValue;

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
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoWork(this));
        }

        private static void DoWork(AddToOrgFromTag model)
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
                        Organization.AddAsPreviousMember(db, model.Id, pid, model.OrgName, MemberTypeCode.InActive, DateTime.Now, DateTime.Now, model.UserId);
                        break;
                }
                lop = FetchLongRunningOp(db, model.Id, Op);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                db.SubmitChanges();
                db.LogActivity($"Org{model.GroupName} AddFromTag", model.Id, pid);
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
