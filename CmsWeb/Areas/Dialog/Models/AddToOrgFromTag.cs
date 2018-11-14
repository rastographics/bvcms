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
        public AddToOrgFromTag() { }

        private OrgFilter filter;
        public OrgFilter Filter => filter ?? (filter = DbUtil.Db.OrgFilter(QueryId));
        public int OrgId => Filter.Id;
        public AddToOrgFromTag(Guid id)
        {
            QueryId = id;
            UserId = Util.UserId;
            if (Filter.GroupSelect == GroupSelectCode.Previous)
            {
                var org = DbUtil.Db.LoadOrganizationById(OrgId);
                OrgName = org.OrganizationName;
            }
            Tag = new CodeInfo("0", "Tag");
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
            pids = FetchPeopleIds(DbUtil.Db, Tag.Value.ToInt()).ToList();
            var lop = new LongRunningOperation()
            {
                Started = DateTime.Now,
                Count = pids.Count,
                Processed = 0,
                QueryId = QueryId,
                Operation = Op,
            };
            DbUtil.Db.LongRunningOperations.InsertOnSubmit(lop);
            DbUtil.Db.SubmitChanges();
            HostingEnvironment.QueueBackgroundWorkItem(ct => DoWork(this));
        }

        private static void DoWork(AddToOrgFromTag model)
        {
            var db = DbUtil.Create(model.Host);
            var cul = DbUtil.Db.Setting("Culture", "en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cul);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cul);

            LongRunningOperation lop = null;
            foreach (var pid in model.pids)
            {
                DbUtil.Db.Dispose();
                db = DbUtil.Create(model.Host);
                switch (model.filter.GroupSelect)
                {
                    case GroupSelectCode.Member:
                        OrganizationMember.InsertOrgMembers(DbUtil.Db, model.OrgId, pid, MemberTypeCode.Member, DateTime.Now, null, pending: false);
                        break;
                    case GroupSelectCode.Pending:
                        OrganizationMember.InsertOrgMembers(DbUtil.Db, model.OrgId, pid, MemberTypeCode.Member, DateTime.Now, null, pending: true);
                        break;
                    case GroupSelectCode.Prospect:
                        OrganizationMember.InsertOrgMembers(DbUtil.Db, model.OrgId, pid, MemberTypeCode.Prospect, DateTime.Now, null, pending: false);
                        break;
                    case GroupSelectCode.Inactive:
                        OrganizationMember.InsertOrgMembers(DbUtil.Db, model.OrgId, pid, MemberTypeCode.InActive, DateTime.Now, DateTime.Now, pending: false);
                        break;
                    case GroupSelectCode.Previous:
                        Organization.AddAsPreviousMember(DbUtil.Db, model.OrgId, pid, model.OrgName, MemberTypeCode.InActive, DateTime.Now, DateTime.Now, model.UserId);
                        break;
                }
                lop = FetchLongRunningOperation(DbUtil.Db, Op, model.QueryId);
                Debug.Assert(lop != null, "r != null");
                lop.Processed++;
                DbUtil.Db.SubmitChanges();
                DbUtil.Db.LogActivity($"Org{model.DisplayGroup} AddFromTag", model.OrgId, pid);
            }
            // finished
            lop = FetchLongRunningOperation(DbUtil.Db, Op, model.QueryId);
            lop.Completed = DateTime.Now;
            DbUtil.Db.SubmitChanges();
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
                var q = FetchPeopleIds(DbUtil.Db, Tag.Value.ToInt());
                Count = q.Count();
                DbUtil.Db.SubmitChanges();
                return true;
            }
            return false;
        }

        public static IQueryable<int> FetchPeopleIds(CMSDataContext db, int tagid)
        {
            return tagid == -1
                ? DbUtil.Db.PeopleQueryLast().Select(pp => pp.PeopleId)
                : from t in DbUtil.Db.TagPeople
                  where t.Id == tagid
                  select t.PeopleId;
        }
    }
}
