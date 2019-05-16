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
    public class AddToOrgFromTag : LongRunningOperation
    {
        public const string Op = "addtoorgfromtag";

        public int UserId { get; set; }
        public string OrgName { get; set; }
        public CMSDataContext CurrentDatabase { get; set; }
        public AddToOrgFromTag() { }

        private OrgFilter filter;
        public OrgFilter Filter { get; }
        public int OrgId { get; }

        public AddToOrgFromTag(Guid id, CMSDataContext db)
        {
            Host = db.Host;
            CurrentDatabase = db;
            QueryId = id;
            UserId = Util.UserId;
            Tag = new CodeInfo("0", "Tag");

            Filter = filter ?? (filter = db.OrgFilter(id));

            if (Filter.GroupSelect == GroupSelectCode.Previous)
            {
                var org = db.LoadOrganizationById(OrgId);
                OrgName = org.OrganizationName;
            }

            OrgId = Filter.Id;
        }

        [DisplayName("Choose A Tag")]
        public CodeInfo Tag { get; set; }

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
            this.Host = db.Host;
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoWork(this));
        }

        private static void DoWork(AddToOrgFromTag model)
        {
            var db = CMSDataContext.Create(model.Host);
            var cul = db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOperation lop = null;
            foreach (var pid in model.pids)
            {
                switch (model.filter.GroupSelect)
                {
                    case GroupSelectCode.Member:
                        OrganizationMember.InsertOrgMembers(db, model.OrgId, pid, MemberTypeCode.Member, DateTime.Now, null, pending: false);
                        break;
                    case GroupSelectCode.Pending:
                        OrganizationMember.InsertOrgMembers(db, model.OrgId, pid, MemberTypeCode.Member, DateTime.Now, null, pending: true);
                        break;
                    case GroupSelectCode.Prospect:
                        OrganizationMember.InsertOrgMembers(db, model.OrgId, pid, MemberTypeCode.Prospect, DateTime.Now, null, pending: false);
                        break;
                    case GroupSelectCode.Inactive:
                        OrganizationMember.InsertOrgMembers(db, model.OrgId, pid, MemberTypeCode.InActive, DateTime.Now, DateTime.Now, pending: false);
                        break;
                    case GroupSelectCode.Previous:
                        Organization.AddAsPreviousMember(db, model.OrgId, pid, model.OrgName, MemberTypeCode.InActive, DateTime.Now, DateTime.Now, model.UserId);
                        break;
                }
                lop = FetchLongRunningOperation(db, Op, model.QueryId);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                db.SubmitChanges();
                db.LogActivity($"Org{model.DisplayGroup} AddFromTag", model.OrgId, pid);
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
