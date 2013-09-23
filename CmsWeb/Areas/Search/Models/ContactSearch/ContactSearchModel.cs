/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using CmsWeb.Code;
using CmsWeb.Models;
using UtilityExtensions;
using System.Web.Mvc;
using CmsData;

namespace CmsWeb.Areas.Search.Models
{
    public class ContactSearchModel : PagedTableModel<Contact, ContactInfo>
    {
        public ContactSearchInfo SearchParameters { get; set; }

        public ContactSearchModel()
            : base("Date", "desc")
        {
            SearchParameters = new ContactSearchInfo();
        }

        public override IQueryable<Contact> DefineModelList()
        {
            var db = DbUtil.Db;

            IQueryable<int> ppl = null;

            if (Util2.OrgMembersOnly)
                ppl = db.OrgMembersOnlyTag2().People(db).Select(pp => pp.PeopleId);
            else if (Util2.OrgLeadersOnly)
                ppl = db.OrgLeadersOnlyTag2().People(db).Select(pp => pp.PeopleId);

            var u = DbUtil.Db.CurrentUser;
            var roles = u.UserRoles.Select(uu => uu.Role.RoleName.ToLower()).ToArray();
            var isAdminFinance = roles.Contains("admin") && roles.Contains("finance") && SearchParameters.Private;
            var q = from c in DbUtil.Db.Contacts
                    where c.LimitToRole == null || roles.Contains(c.LimitToRole) || isAdminFinance
                    select c;

            if (ppl != null && Util.UserPeopleId != null)
                q = from c in q
                    where c.contactsMakers.Any(cm => cm.PeopleId == Util.UserPeopleId.Value)
                    select c;

            if (SearchParameters.ContacteeName.HasValue())
                q = from c in q
                    where c.contactsMakers.Any(p => p.person.Name.Contains(SearchParameters.ContacteeName))
                    select c;

            if (SearchParameters.ContactorName.HasValue())
                q = from c in q
                    where c.contactsMakers.Any(p => p.person.Name.Contains(SearchParameters.ContactorName))
                    select c;

            if (SearchParameters.CreatedBy.HasValue())
            {
                var pid = SearchParameters.CreatedBy.ToInt();
                if (pid > 0)
                    q = from c in q
                        where DbUtil.Db.Users.Any(uu => c.CreatedBy == uu.UserId && uu.Person.PeopleId == pid)
                        select c;
                else
                    q = from c in q
                        where
                            DbUtil.Db.Users.Any(
                                uu => c.CreatedBy == uu.UserId && uu.Username == SearchParameters.CreatedBy)
                        select c;
            }
            if (SearchParameters.Private)
            {
                q = from c in q
                    where c.LimitToRole.Length > 0
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
                if (SearchParameters.EndDate.HasValue)
                    endDateRange = SearchParameters.EndDate.Value.AddHours(+24);
                else
                    endDateRange = DateTime.Today;

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
                q = from c in q
                    where c.ContactReasonId == SearchParameters.ContactReason.Value.ToInt()
                    select c;

            if ((SearchParameters.ContactType.Value.ToInt()) != 0)
                q = from c in q
                    where c.ContactTypeId == SearchParameters.ContactType.Value.ToInt()
                    select c;

            if ((SearchParameters.Ministry.Value.ToInt()) != 0)
                q = from c in q
                    where c.MinistryId == SearchParameters.Ministry.Value.ToInt()
                    select c;

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
            switch (Pager.SortExpression)
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
                default:
                case "Date desc":
                    return from c in q
                           orderby c.ContactDate descending
                           select c;
                case "Reason desc":
                    return from c in q
                           orderby c.ContactReasonId descending, c.ContactDate descending
                           select c;
                case "Type desc":
                    return from c in q
                           orderby c.ContactTypeId descending, c.ContactDate descending
                           select c;
            }
            return q;
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
                       ContacteeList = string.Join(", ", (from c in DbUtil.Db.Contactees
                                                          where c.ContactId == o.ContactId
                                                          select c.person.Name).Take(3)),
                       ContactorList = string.Join(", ", (from c in DbUtil.Db.Contactors
                                                          where c.ContactId == o.ContactId
                                                          select c.person.Name).Take(3))

                   };
        }
    }
}