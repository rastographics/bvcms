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
using CmsData.Codes;
using Newtonsoft.Json;
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

            var u = db.CurrentUser;
            var roles = u.UserRoles.Select(uu => uu.Role.RoleName.ToLower()).ToArray();
            var managePrivateContacts = HttpContext.Current.User.IsInRole("ManagePrivateContacts");
            var manageTasks = HttpContext.Current.User.IsInRole("ManageTasks");
            var uid = Util.UserPeopleId;
            var q = from t in db.ViewTaskSearches
                    where (t.LimitToRole ?? "") == "" || roles.Contains(t.LimitToRole) || managePrivateContacts
                    where manageTasks || t.OrginatorId == uid || t.OwnerId == uid || t.CoOwnerId == uid
                    where t.Archive == SearchParameters.Archived
                    select t;


            if (SearchParameters.About.HasValue())
                if (SearchParameters.About.AllDigits())
                    q = from t in q
                        where t.WhoId == SearchParameters.About.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.About.Contains(SearchParameters.About)
                        select t;

            if (SearchParameters.Owner.HasValue())
                if (SearchParameters.Owner.AllDigits())
                    q = from t in q
                        where t.OwnerId == SearchParameters.Owner.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.Owner.Contains(SearchParameters.Owner)
                        select t;

            if (SearchParameters.Delegate.HasValue())
                if (SearchParameters.Delegate.AllDigits())
                    q = from t in q
                        where t.CoOwnerId == SearchParameters.Delegate.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.DelegateX.Contains(SearchParameters.Delegate)
                        select t;

            if (SearchParameters.Originator.HasValue())
                if (SearchParameters.Originator.AllDigits())
                    q = from t in q
                        where t.OrginatorId == SearchParameters.Originator.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.Originator.Contains(SearchParameters.Originator)
                        select t;

            if (SearchParameters.ExcludeNewPerson)
                q = from t in q
                    where !t.Description.StartsWith("New Person")
                    select t;

            if (SearchParameters.Description.HasValue())
                q = from t in q
                    where t.Description.Contains(SearchParameters.Description)
                    select t;

            if (SearchParameters.Notes.HasValue())
                q = from t in q
                    where t.Notes.Contains(SearchParameters.Notes)
                    select t;

            if (SearchParameters.Status.Value.ToInt() > 0)
                q = from t in q
                    where t.StatusId == SearchParameters.Status.Value.ToInt()
                    select t;

            if (SearchParameters.Lookback.HasValue)
            {
                var enddt = SearchParameters.EndDt;
                if(!enddt.HasValue)
                    enddt = DateTime.Today.AddDays(1);
                q = from t in q
                    where t.Created >= enddt.Value.AddDays(-SearchParameters.Lookback.Value)
                    select t;
            }
            if (SearchParameters.EndDt.HasValue)
                q = from t in q
                    where t.Created <= SearchParameters.EndDt.Value
                    select t;

            if (SearchParameters.IsPrivate)
                q = from t in q
                    where (t.LimitToRole ?? "") != ""
                    select t;

            if (SearchParameters.ExcludeCompleted)
                q = from t in q
                    where t.StatusId != TaskStatusCode.Complete
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
                case "Date":
                    return q.OrderBy(tt => tt.Created);
                case "Date desc":
                    return q.OrderByDescending(tt => tt.Created);
            }
            return q;
        }

        public override IEnumerable<TaskSearch> DefineViewList(IQueryable<TaskSearch> q)
        {
            return q;
        }

        private const string StrTaskSearch = "TaskSearch";

        private string NewTaskSearchString => JsonConvert.SerializeObject(new TaskSearchInfo());

        internal void GetPreference()
        {
            var os = JsonConvert.DeserializeObject<TaskSearchInfo>(
                DbUtil.Db.UserPreference(StrTaskSearch, NewTaskSearchString));
            if (os != null)
                SearchParameters.CopyPropertiesFrom(os);
        }

        internal void SavePreference()
        {
            DbUtil.Db.SetUserPreference(StrTaskSearch, 
                JsonConvert.SerializeObject(SearchParameters));
        }

        internal void ClearPreference()
        {
            DbUtil.Db.SetUserPreference(StrTaskSearch, NewTaskSearchString);
        }
    }
}