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
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class TaskSearchModel : PagedTableModel<TaskSearch, TaskSearch>
    {
        public TaskSearchInfo SearchParameters { get; set; }

        public TaskSearchModel()
            : base("Date", "desc", true)
        {
            SearchParameters = new TaskSearchInfo();
        }

        public override IQueryable<TaskSearch> DefineModelList()
        {
            var db = DbUtil.Db;

            var u = DbUtil.Db.CurrentUser;
            var roles = u.UserRoles.Select(uu => uu.Role.RoleName.ToLower()).ToArray();
            var managePrivateContacts = HttpContext.Current.User.IsInRole("ManagePrivateContacts");
            var q = from t in DbUtil.Db.ViewTaskSearches
                   where (t.LimitToRole ?? "") == "" || roles.Contains(t.LimitToRole) || managePrivateContacts
                   select t;

            if(SearchParameters.About.HasValue())
                if (SearchParameters.About.AllDigits())
                    q = from t in q
                        where t.WhoId == SearchParameters.About.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.About.Contains(SearchParameters.About)
                        select t;

            if(SearchParameters.Owner.HasValue())
                if (SearchParameters.Owner.AllDigits())
                    q = from t in q
                        where t.OwnerId == SearchParameters.Owner.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.Owner.Contains(SearchParameters.Owner)
                        select t;

            if(SearchParameters.Delegate.HasValue())
                if (SearchParameters.Delegate.AllDigits())
                    q = from t in q
                        where t.CoOwnerId == SearchParameters.Delegate.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.DelegateX.Contains(SearchParameters.Delegate)
                        select t;

            if(SearchParameters.Originator.HasValue())
                if (SearchParameters.Originator.AllDigits())
                    q = from t in q
                        where t.OrginatorId == SearchParameters.Originator.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.Originator.Contains(SearchParameters.Originator)
                        select t;

            if (SearchParameters.Status.Value.ToInt() > 0)
                q = from t in q
                    where t.StatusId == SearchParameters.Status.Value.ToInt()
                    select t;

            if (SearchParameters.Archived.HasValue)
                q = from t in q
                    where t.Archive == SearchParameters.Archived.Value
                    select t;

            if (SearchParameters.StartDt.HasValue)
                q = from t in q
                    where t.Created >= SearchParameters.StartDt.Value
                    select t;
            else if (SearchParameters.Lookback.HasValue)
                q = from t in q
                    where t.Created >= DateTime.Today.AddDays(-SearchParameters.Lookback.Value)
                    select t;

            if (SearchParameters.EndDt.HasValue)
                q = from t in q
                    where t.Created >= SearchParameters.EndDt.Value
                    select t;

            if(SearchParameters.IsPrivate.HasValue)
                if (SearchParameters.IsPrivate.Value)
                    q = from t in q
                        where (t.LimitToRole ?? "") != ""
                        select t;

//            IQueryable<int> ppl = null;
//            if (Util2.OrgLeadersOnly)
//                ppl = db.OrgLeadersOnlyTag2().People(db).Select(pp => pp.PeopleId);

//            if (ppl != null && Util.UserPeopleId != null)
//                q = from c in q
//                    where c.contactsMakers.Any(cm => cm.PeopleId == Util.UserPeopleId.Value)
//                    select c;
            return q;
        }

        public override IQueryable<TaskSearch> DefineModelSort(IQueryable<TaskSearch> q)
        {
            switch (SortExpression)
            {
                case "created":
                    return q.OrderBy(tt => tt.Created);
                case "created desc":
                    return q.OrderByDescending(tt => tt.Created);
            }
            return q;
        }

        public override IEnumerable<TaskSearch> DefineViewList(IQueryable<TaskSearch> q)
        {
            return q;
        }


        private const string StrTaskSearch = "TaskSearch";
        internal void GetFromSession()
        {
            var os = HttpContext.Current.Session[StrTaskSearch] as TaskSearchInfo;
            if (os != null)
                SearchParameters.CopyPropertiesFrom(os);
        }
        internal void SaveToSession()
        {
            var os = new ContactSearchInfo();
            SearchParameters.CopyPropertiesTo(os);
            HttpContext.Current.Session[StrTaskSearch] = os;
        }

        internal void ClearSession()
        {
            HttpContext.Current.Session.Remove(StrTaskSearch);
        }
    }
}