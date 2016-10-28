/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData.View;
using CmsWeb.Code;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Search.Models
{
    public class TaskSearchModel : PagedTableModel<Task, TaskInfo>
    {
        public TaskSearchInfo SearchParameters { get; set; }

        public TaskSearchModel()
            : base("Date", "desc", true)
        {
            SearchParameters = new TaskSearchInfo();
        }

        public override IQueryable<Task> DefineModelList()
        {
//            var db = DbUtil.Db;
//
//            IQueryable<int> ppl = null;
//
//            if (Util2.OrgLeadersOnly)
//                ppl = db.OrgLeadersOnlyTag2().People(db).Select(pp => pp.PeopleId);
//
//            var u = DbUtil.Db.CurrentUser;
//            var roles = u.UserRoles.Select(uu => uu.Role.RoleName.ToLower()).ToArray();
//            var managePrivateContacts = HttpContext.Current.User.IsInRole("ManagePrivateContacts");
//            var q = from c in DbUtil.Db.Contacts
//                   where (c.LimitToRole ?? "") == "" || roles.Contains(c.LimitToRole) || managePrivateContacts
//                   select c;
//
//            if (ppl != null && Util.UserPeopleId != null)
//                q = from c in q
//                    where c.contactsMakers.Any(cm => cm.PeopleId == Util.UserPeopleId.Value)
//                    select c;
//
//            if (SearchParameters.ContacteeName.HasValue())
//                q = from c in q
//                    where
//                        c.contactees.Any(p => p.person.Name.Contains(SearchParameters.ContacteeName)) ||
//                        c.organization.OrganizationName.Contains(SearchParameters.ContacteeName)
//                    select c;
//
//            if (SearchParameters.ContactorName.HasValue())
//                q = from c in q
//                    where c.contactsMakers.Any(p => p.person.Name.Contains(SearchParameters.ContactorName))
//                    select c;
//
//            if (SearchParameters.CreatedBy.HasValue())
//            {
//                var pid = SearchParameters.CreatedBy.ToInt();
//                if (pid > 0)
//                    q = from c in q
//                        where DbUtil.Db.Users.Any(uu => c.CreatedBy == uu.UserId && uu.Person.PeopleId == pid)
//                        select c;
//                else
//                    q = from c in q
//                        where
//                            DbUtil.Db.Users.Any(
//                                uu => c.CreatedBy == uu.UserId && uu.Username == SearchParameters.CreatedBy)
//                        select c;
//            }
//            if (SearchParameters.Private)
//            {
//                q = from c in q
//                    where (c.LimitToRole ?? "") != ""
//                    select c;
//            }
//            if (SearchParameters.Incomplete)
//            {
//                q = from c in q
//                    where c.MinistryId == null
//                          || c.ContactReasonId == null
//                          || c.ContactTypeId == null
//                          || !c.contactees.Any()
//                          || !c.contactsMakers.Any()
//                    select c;
//            }
//
//            DateTime startDateRange;
//            DateTime endDateRange;
//            if (SearchParameters.StartDate.HasValue)
//            {
//                startDateRange = SearchParameters.StartDate.Value;
//                endDateRange = SearchParameters.EndDate?.AddHours(+24) ?? DateTime.Today;
//            }
//            else if (SearchParameters.EndDate.HasValue)
//            {
//                startDateRange = DateTime.Parse("01/01/1800");
//                endDateRange = SearchParameters.EndDate.Value.AddHours(+24);
//            }
//            else
//            {
//                startDateRange = DateTime.Parse("01/01/1800");
//                endDateRange = Util.Now.Date.AddHours(+24);
//            }
//
//            q = from c in q
//                where c.ContactDate >= startDateRange && c.ContactDate < endDateRange
//                select c;
//
//            if ((SearchParameters.ContactReason.Value.ToInt()) != 0)
//                q = from c in q
//                    where c.ContactReasonId == SearchParameters.ContactReason.Value.ToInt()
//                    select c;
//
//            if ((SearchParameters.ContactType.Value.ToInt()) != 0)
//                q = from c in q
//                    where c.ContactTypeId == SearchParameters.ContactType.Value.ToInt()
//                    select c;
//
//            if ((SearchParameters.Ministry.Value.ToInt()) != 0)
//                q = from c in q
//                    where c.MinistryId == SearchParameters.Ministry.Value.ToInt()
//                    select c;
//
//            switch (SearchParameters.ContactResult.Value)
//            {
//                case "Gospel Shared":
//                    q = from c in q
//                        where c.GospelShared == true
//                        select c;
//                    break;
//                case "Attempted/Not Available":
//                    q = from c in q
//                        where c.NotAtHome == true
//                        select c;
//                    break;
//                case "Left Note Card":
//                    q = from c in q
//                        where c.LeftDoorHanger == true
//                        select c;
//                    break;
//                case "Left Message":
//                    q = from c in q
//                        where c.LeftMessage == true
//                        select c;
//                    break;
//                case "Contact Made":
//                    q = from c in q
//                        where c.ContactMade == true
//                        select c;
//                    break;
//                case "Prayer Request Received":
//                    q = from c in q
//                        where c.PrayerRequest == true
//                        select c;
//                    break;
//                case "Gift Bag Given":
//                    q = from c in q
//                        where c.GiftBagGiven == true
//                        select c;
//                    break;
//            }
//            return q;
            return null;
        }

        public override IQueryable<Task> DefineModelSort(IQueryable<Task> q)
        {
//            switch (sortexpression)
//            {
//                case "id":
//                    return from c in q
//                           orderby c.contactid
//                           select c;
//                case "date":
//                    return from c in q
//                           orderby c.contactdate
//                           select c;
//                case "reason":
//                    return from c in q
//                           orderby c.contactreasonid, c.contactdate descending
//                           select c;
//                case "type":
//                    return from c in q
//                           orderby c.contacttypeid, c.contactdate descending
//                           select c;
//                case "id desc":
//                    return from c in q
//                           orderby c.contactid descending
//                           select c;
//                case "reason desc":
//                    return from c in q
//                           orderby c.contactreasonid descending, c.contactdate descending
//                           select c;
//                case "type desc":
//                    return from c in q
//                           orderby c.contacttypeid descending, c.contactdate descending
//                           select c;
//                case "date desc":
//                default:
//                    return from c in q
//                           orderby c.contactdate descending
//                           select c;
//            }
            return null;
        }

        public override IEnumerable<TaskInfo> DefineViewList(IQueryable<Task> q)
        {
            return from o in q
                   select new TaskInfo
                   {
                   };
        }

        public IEnumerable<TaskSummaryInfo> ContactorSummary()
        {
            int ministryid = SearchParameters.Ministry.Value.ToInt();
            var q = from c in DbUtil.Db.Contactors
                   where c.contact.ContactDate >= SearchParameters.StartDate || SearchParameters.StartDate == null
                   where c.contact.ContactDate <= SearchParameters.EndDate || SearchParameters.EndDate == null
                   where ministryid == 0 || ministryid == c.contact.MinistryId
                   group c by new
                   {
                       c.PeopleId,
                       c.person.Name,
                       c.contact.ContactType.Description,
                       c.contact.MinistryId,
                       c.contact.Ministry.MinistryName
                   } into g
                   where g.Key.MinistryId != null
                   orderby g.Key.MinistryId
                   select new TaskSummaryInfo
                   {
                       ContactType = g.Key.Description,
                       Ministry = g.Key.MinistryName,
                       Count = g.Count()
                   };
            return q;
        }

        public IEnumerable<TaskSummaryInfo> TaskSummary()
        {
            //return from i in dbutil.db.contactsummary(
            //    searchparameters.startdate,
            //    searchparameters.enddate,
            //    searchparameters.ministry.value.toint(),
            //    searchparameters.contacttype.value.toint(),
            //    searchparameters.contactreason.value.toint())
            //       select new contactsummaryinfo()
            //           {
            //               count = i.count ?? 0,
            //               contacttype = i.contacttype,
            //               reasontype = i.reasontype,
            //               ministry = i.ministry,
            //               hascomments = i.comments,
            //               hasdate = i.contactdate,
            //               hascontactor = i.contactor,
            //           };
            return null;
        }

        public IEnumerable<ContactTypeTotal> TaskTypeTotals()
        {
            return from c in DbUtil.Db.ContactTypeTotals(SearchParameters.StartDate, SearchParameters.EndDate, SearchParameters.Ministry.Value.ToInt())
                    orderby c.Count descending
                    select c;
        }

        public bool CanDeleteTotal()
        {
            return HttpContext.Current.User.IsInRole("Developer") 
                && !SearchParameters.StartDate.HasValue 
                && !SearchParameters.EndDate.HasValue 
                && SearchParameters.Ministry.Value.ToInt() == 0;
        }
        public static void DeleteContactsForType(int id)
        {
            DbUtil.Db.ExecuteCommand("DELETE dbo.Contactees FROM dbo.Contactees ce JOIN dbo.Contact c ON ce.ContactId = c.ContactId WHERE c.ContactTypeId = {0}", id);
            DbUtil.Db.ExecuteCommand("DELETE dbo.Contactors FROM dbo.Contactors co JOIN dbo.Contact c ON co.ContactId = c.ContactId WHERE c.ContactTypeId = {0}", id);
            DbUtil.Db.ExecuteCommand("DELETE dbo.Contact WHERE ContactTypeId = {0}", id);
        }
        public Guid ConvertToQuery()
        {
            var c = DbUtil.Db.ScratchPadCondition();
            c.Reset();
            var clause = c.AddNewClause(QueryType.MadeContactTypeAsOf, CompareType.Equal, "1,True");
            clause.Program = SearchParameters.Ministry.Value;
            clause.StartDate = SearchParameters.StartDate ?? DateTime.Parse("1/1/2000");
            clause.EndDate = SearchParameters.EndDate ?? DateTime.Today;
            var cvc = new CodeValueModel();
            var q = from v in cvc.ContactTypeList()
                    where v.Id == SearchParameters.ContactType.Value.ToInt()
                    select v.IdValue;
            var idvalue = q.Single();
            clause.CodeIdValue = idvalue;
            c.Save(DbUtil.Db);
            return c.Id;
        }
        public static Guid ContactTypeQuery(int id)
        {
            var c = DbUtil.Db.ScratchPadCondition();
            c.Reset();
            var comp = CompareType.Equal;
            var clause = c.AddNewClause(QueryType.RecentContactType, comp, "1,True");
            clause.Days = 10000;
            var cvc = new CodeValueModel();
            var q = from v in cvc.ContactTypeList()
                    where v.Id == id
                    select v.IdValue;
            clause.CodeIdValue = q.Single();
            c.Save(DbUtil.Db);
            return c.Id;
        }

        private const string STR_ContactSearch = "ContactSearch2";
        internal void GetFromSession()
        {
            var os = HttpContext.Current.Session[STR_ContactSearch] as ContactSearchInfo;
            if (os != null)
                SearchParameters.CopyPropertiesFrom(os);
        }
        internal void SaveToSession()
        {
            var os = new ContactSearchInfo();
            SearchParameters.CopyPropertiesTo(os);
            HttpContext.Current.Session[STR_ContactSearch] = os;
        }

        internal void ClearSession()
        {
            HttpContext.Current.Session.Remove(STR_ContactSearch);
        }
    }
}