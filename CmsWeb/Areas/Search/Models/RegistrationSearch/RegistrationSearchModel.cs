/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using CmsData;
using CmsData.View;
using CmsWeb.Constants;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class RegistrationSearchModel : PagedTableModel<RegistrationList, RegistrationList>, IDbBinder
    {
        public RegistrationSearchInfo SearchParameters { get; set; }

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public RegistrationSearchModel()
        {
            Init();
        }

        public RegistrationSearchModel(CMSDataContext db) : base(db)
        {
            Init();
        }

        override protected void Init()
        {
            Sort = "Date";
            Direction = "desc";
            AjaxPager = true;
            SearchParameters = new RegistrationSearchInfo();
            base.Init();
        }

        public override IQueryable<RegistrationList> DefineModelList()
        {
            var roles = CurrentDatabase.CurrentRoles();

            var q = from r in CurrentDatabase.ViewRegistrationLists
                    join org in CurrentDatabase.Organizations on r.OrganizationId equals org.OrganizationId
                    where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                    where r.Cnt > 0
                    where r.First.Length > 0 && r.Last.Length > 0
                    select r;
            if (SearchParameters.Registrant.HasValue())
            {
                Util.NameSplit(SearchParameters.Registrant, out string first, out string last);
                q = from c in q
                    where first == null || first == "" || c.First.StartsWith(first)
                    where last == null || last == "" || c.Last.StartsWith(last)
                    select c;
            }

            if (SearchParameters.Organization.HasValue())
            {
                if (SearchParameters.Organization.AllDigits())
                {
                    q = from c in q
                        where c.OrganizationId == SearchParameters.Organization.ToInt()
                        select c;
                }
                else
                {
                    q = from c in q
                        where c.OrganizationName.Contains(SearchParameters.Organization)
                        select c;
                }
            }

            if (SearchParameters.Count.HasValue())
            {
                var cnt = SearchParameters.Count.GetDigits().ToInt();
                if (SearchParameters.Count.AllDigits())
                {
                    q = q.Where(cc => cc.Cnt == cnt);
                }
                else if (SearchParameters.Count.StartsWith(">="))
                {
                    q = q.Where(cc => cc.Cnt >= cnt);
                }
                else if (SearchParameters.Count.StartsWith(">"))
                {
                    q = q.Where(cc => cc.Cnt > cnt);
                }
                else if (SearchParameters.Count.StartsWith("<="))
                {
                    q = q.Where(cc => cc.Cnt <= cnt);
                }
                else if (SearchParameters.Count.StartsWith("<"))
                {
                    q = q.Where(cc => cc.Cnt < cnt);
                }
            }

            switch (SearchParameters.Complete.Value)
            {
                case "All":
                    break;
                case "No":
                    q = from r in q
                        where (r.Completed ?? false) == false
                        select r;
                    break;
                case "Yes":
                    q = from r in q
                        where (r.Completed ?? false)
                        select r;
                    break;
                case "InProgress":
                    q = from r in q
                        let o = CurrentDatabase.Organizations.Single(oo => oo.OrganizationId == r.OrganizationId)
                        where (r.Completed ?? false)
                        select r;
                    break;
            }
            switch (SearchParameters.Active.Value)
            {
                case "All":
                    break;
                case "No":
                    q = from r in q
                        where (r.Expired ?? false)
                        select r;
                    break;
                case "Yes":
                    q = from r in q
                        where (r.Expired ?? false) == false
                        select r;
                    break;
            }
            switch (SearchParameters.Abandoned.Value)
            {
                case "Yes":
                    q = from r in q
                        where (r.Abandoned ?? false)
                        select r;
                    break;
                case "No":
                    q = from r in q
                        where (r.Abandoned ?? false) == false
                        select r;
                    break;
            }
            if (SearchParameters.FromMobileAppOnly)
            {
                q = from r in q
                    where (r.Mobile == true)
                    select r;
            }

            DateTime startDateRange;
            DateTime endDateRange;
            if (SearchParameters.StartDate.HasValue)
            {
                startDateRange = SearchParameters.StartDate.Value;
                if (SearchParameters.EndDate.HasValue)
                {
                    endDateRange = SearchParameters.EndDate.Value.AddHours(+24);
                }
                else
                {
                    endDateRange = DateTime.Today.AddHours(24);
                }
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
                where c.Stamp >= startDateRange && c.Stamp < endDateRange
                select c;

            return q;
        }

        public override IQueryable<RegistrationList> DefineModelSort(IQueryable<RegistrationList> q)
        {
            switch (SortExpression)
            {
                case "Date":
                    return from r in q
                           orderby r.Stamp
                           select r;
                case "Registrant":
                    return from r in q
                           orderby r.First
                           select r;
                case "Organization":
                    return from r in q
                           orderby r.OrganizationName
                           select r;
                case "Date desc":
                    return from r in q
                           orderby r.Stamp descending
                           select r;
                case "Organization desc":
                    return from r in q
                           orderby r.OrganizationName descending
                           select r;
            }
            return q.OrderByDescending(r => r.Id);
        }

        public override IEnumerable<RegistrationList> DefineViewList(IQueryable<RegistrationList> q)
        {
            return q;
        }

        //        private const string STR_RegistrationSearch = "RegistrationSearch";
        //        internal void GetFromSession()
        //        {
        //            var os = HttpContextFactory.Current.Session[STR_RegistrationSearch] as RegistrationSearchInfo;
        //            if (os != null)
        //                SearchParameters.CopyPropertiesFrom(os);
        //        }
        //        internal void SaveToSession()
        //        {
        //            var os = new RegistrationSearchInfo();
        //            SearchParameters.CopyPropertiesTo(os);
        //            HttpContextFactory.Current.Session[STR_RegistrationSearch] = os;
        //        }
        //
        //        internal void ClearSession()
        //        {
        //            HttpContextFactory.Current.Session.Remove(STR_RegistrationSearch);
        //        }
    }
}
