/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using CmsData;
using CmsData.View;
using CmsWeb.Code;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class ContactSearchModel : PagedTableModel<Contact, ContactInfo>
    {
        public ContactSearchInfo SearchParameters { get; set; }

        public ContactSearchModel()
            : base("Date", "desc", true)
        {
            SearchParameters = new ContactSearchInfo();
        }

        public override IQueryable<Contact> DefineModelList()
        {
            //var db = Db;

            IQueryable<int> ppl = null;

            if (Util2.OrgLeadersOnly)
            {
                ppl = DbUtil.Db.OrgLeadersOnlyTag2().People(DbUtil.Db).Select(pp => pp.PeopleId);
            }

            var u = DbUtil.Db.CurrentUser;
            var roles = u.UserRoles.Select(uu => uu.Role.RoleName.ToLower()).ToArray();
            var managePrivateContacts = HttpContextFactory.Current.User.IsInRole("ManagePrivateContacts");
            var q = from c in DbUtil.Db.Contacts
                    where (c.LimitToRole ?? "") == "" || roles.Contains(c.LimitToRole) || managePrivateContacts
                    select c;

            if (ppl != null && Util.UserPeopleId != null)
            {
                q = from c in q
                    where c.contactsMakers.Any(cm => cm.PeopleId == Util.UserPeopleId.Value)
                    select c;
            }

            if (SearchParameters.ContacteeName.HasValue())
            {
                q = from c in q
                    where
                        c.contactees.Any(p => p.person.Name.Contains(SearchParameters.ContacteeName)) ||
                        c.organization.OrganizationName.Contains(SearchParameters.ContacteeName)
                    select c;
            }

            if (SearchParameters.ContactorName.HasValue())
            {
                q = from c in q
                    where c.contactsMakers.Any(p => p.person.Name.Contains(SearchParameters.ContactorName))
                    select c;
            }

            if (SearchParameters.CreatedBy.HasValue())
            {
                var pid = SearchParameters.CreatedBy.ToInt();
                if (pid > 0)
                {
                    q = from c in q
                        where DbUtil.Db.Users.Any(uu => c.CreatedBy == uu.UserId && uu.Person.PeopleId == pid)
                        select c;
                }
                else
                {
                    q = from c in q
                        where
                            DbUtil.Db.Users.Any(
                                uu => c.CreatedBy == uu.UserId && uu.Username == SearchParameters.CreatedBy)
                        select c;
                }
            }
            if (SearchParameters.Private)
            {
                q = from c in q
                    where (c.LimitToRole ?? "") != ""
                    select c;
            }
            if (SearchParameters.Incomplete)
            {
                q = from c in q
                    where c.MinistryId == null
                          || c.ContactReasonId == null
                          || c.ContactTypeId == null
                          || !c.contactees.Any()
                          || !c.contactsMakers.Any()
                    select c;
            }

            DateTime startDateRange;
            DateTime endDateRange;
            if (SearchParameters.StartDate.HasValue)
            {
                startDateRange = SearchParameters.StartDate.Value;
                endDateRange = SearchParameters.EndDate?.AddHours(+24) ?? DateTime.Today;
            }
            else if (SearchParameters.EndDate.HasValue)
            {
                startDateRange = DateTime.Parse("01/01/1800");
                endDateRange = SearchParameters.EndDate.Value.AddHours(+24);
            }
            else
            {
                startDateRange = DateTime.Parse("01/01/1800");
                endDateRange = Util.Now.Date.AddHours(+24);
            }

            q = from c in q
                where c.ContactDate >= startDateRange && c.ContactDate < endDateRange
                select c;

            if ((SearchParameters.ContactReason.Value.ToInt()) != 0)
            {
                q = from c in q
                    where c.ContactReasonId == SearchParameters.ContactReason.Value.ToInt()
                    select c;
            }

            if ((SearchParameters.ContactType.Value.ToInt()) != 0)
            {
                q = from c in q
                    where c.ContactTypeId == SearchParameters.ContactType.Value.ToInt()
                    select c;
            }

            if ((SearchParameters.Ministry.Value.ToInt()) != 0)
            {
                q = from c in q
                    where c.MinistryId == SearchParameters.Ministry.Value.ToInt()
                    select c;
            }

            switch (SearchParameters.ContactResult.Value)
            {
                case "Gospel Shared":
                    q = from c in q
                        where c.GospelShared == true
                        select c;
                    break;
                case "Attempted/Not Available":
                    q = from c in q
                        where c.NotAtHome == true
                        select c;
                    break;
                case "Left Note Card":
                    q = from c in q
                        where c.LeftDoorHanger == true
                        select c;
                    break;
                case "Left Message":
                    q = from c in q
                        where c.LeftMessage == true
                        select c;
                    break;
                case "Contact Made":
                    q = from c in q
                        where c.ContactMade == true
                        select c;
                    break;
                case "Prayer Request Received":
                    q = from c in q
                        where c.PrayerRequest == true
                        select c;
                    break;
                case "Gift Bag Given":
                    q = from c in q
                        where c.GiftBagGiven == true
                        select c;
                    break;
            }
            return q;
        }

        public override IQueryable<Contact> DefineModelSort(IQueryable<Contact> q)
        {
            switch (SortExpression)
            {
                case "ID":
                    return from c in q
                           orderby c.ContactId
                           select c;
                case "Date":
                    return from c in q
                           orderby c.ContactDate
                           select c;
                case "Reason":
                    return from c in q
                           orderby c.ContactReasonId, c.ContactDate descending
                           select c;
                case "Type":
                    return from c in q
                           orderby c.ContactTypeId, c.ContactDate descending
                           select c;
                case "ID desc":
                    return from c in q
                           orderby c.ContactId descending
                           select c;
                case "Reason desc":
                    return from c in q
                           orderby c.ContactReasonId descending, c.ContactDate descending
                           select c;
                case "Type desc":
                    return from c in q
                           orderby c.ContactTypeId descending, c.ContactDate descending
                           select c;
                case "Date desc":
                default:
                    return from c in q
                           orderby c.ContactDate descending
                           select c;
            }
        }

        public override IEnumerable<ContactInfo> DefineViewList(IQueryable<CmsData.Contact> q)
        {
            return from o in q
                   select new ContactInfo
                   {
                       ContactId = o.ContactId,
                       ContactDate = o.ContactDate,
                       ContactReason = o.ContactReason.Description,
                       TypeOfContact = o.ContactType.Description,
                       Ministry = o.Ministry.MinistryName,
                       Results = (o.GospelShared == true ? "GS " : "")
                                 + (o.NotAtHome == true ? "NA " : "")
                                 + (o.LeftDoorHanger == true ? "LN " : "")
                                 + (o.LeftMessage == true ? "LM " : "")
                                 + (o.ContactMade == true ? "CM " : "")
                                 + (o.PrayerRequest == true ? "PR " : "")
                                 + (o.GiftBagGiven == true ? "GB " : ""),
                       ContacteeList = o.OrganizationId.HasValue ? o.organization.OrganizationName :
                                        string.Join(", ", (from c in DbUtil.Db.Contactees
                                                           where c.ContactId == o.ContactId
                                                           select c.person.Name).Take(3)),
                       ContactorList = string.Join(", ", (from c in DbUtil.Db.Contactors
                                                          where c.ContactId == o.ContactId
                                                          select c.person.Name).Take(3))

                   };
        }

        public IEnumerable<ContactorSummaryInfo> ContactorSummary()
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
                    select new ContactorSummaryInfo
                    {
                        PeopleId = g.Key.PeopleId,
                        Name = g.Key.Name,
                        ContactType = g.Key.Description,
                        Ministry = g.Key.MinistryName,
                        Count = g.Count()
                    };
            return q;
        }

        public IEnumerable<ContactSummaryInfo> ContactSummary()
        {
            return from i in DbUtil.Db.ContactSummary(
                SearchParameters.StartDate,
                SearchParameters.EndDate,
                SearchParameters.Ministry.Value.ToInt(),
                SearchParameters.ContactType.Value.ToInt(),
                SearchParameters.ContactReason.Value.ToInt())
                   select new ContactSummaryInfo()
                   {
                       Count = i.Count ?? 0,
                       ContactType = i.ContactType,
                       ReasonType = i.ReasonType,
                       Ministry = i.Ministry,
                       HasComments = i.Comments,
                       HasDate = i.ContactDate,
                       HasContactor = i.Contactor,
                   };
        }

        public IEnumerable<ContactTypeTotal> ContactTypeTotals()
        {
            return from c in DbUtil.Db.ContactTypeTotals(SearchParameters.StartDate, SearchParameters.EndDate, SearchParameters.Ministry.Value.ToInt())
                   orderby c.Count descending
                   select c;
        }

        public bool CanDeleteTotal()
        {
            return HttpContextFactory.Current.User.IsInRole("Developer")
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
            var os = HttpContextFactory.Current.Session[STR_ContactSearch] as ContactSearchInfo;
            if (os != null)
            {
                SearchParameters.CopyPropertiesFrom(os);
            }
        }
        internal void SaveToSession()
        {
            var os = new ContactSearchInfo();
            SearchParameters.CopyPropertiesTo(os);
            HttpContextFactory.Current.Session[STR_ContactSearch] = os;
        }

        internal void ClearSession()
        {
            HttpContextFactory.Current.Session.Remove(STR_ContactSearch);
        }
    }
}
