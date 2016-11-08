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
using CmsWeb.Models;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class TaskSearchModel : PagedTableModel<TaskSearch, TaskSearch>
    {
        public TaskSearchInfo Search { get; set; }

        public TaskSearchModel()
            : base("Date", "desc", true)
        {
            Search = new TaskSearchInfo();
        }

        public override IQueryable<TaskSearch> DefineModelList()
        {
            var db = DbUtil.Db;

            var q = GetBaseResults(db, Search.GetOptions());

            if (Search.About.HasValue())
                if (Search.About.AllDigits())
                    q = from t in q
                        where t.WhoId == Search.About.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.About2.StartsWith(Search.About)
                        select t;

            if (Search.Owner.HasValue())
                if (Search.Owner.AllDigits())
                    q = from t in q
                        where t.OwnerId == Search.Owner.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.Owner2.StartsWith(Search.Owner)
                        select t;

            if (Search.Delegate.HasValue())
                if (Search.Delegate.AllDigits())
                    q = from t in q
                        where t.CoOwnerId == Search.Delegate.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.Delegate2.StartsWith(Search.Delegate)
                        select t;

            if (Search.Originator.HasValue())
                if (Search.Originator.AllDigits())
                    q = from t in q
                        where t.OrginatorId == Search.Originator.ToInt()
                        select t;
                else
                    q = from t in q
                        where t.Originator2.StartsWith(Search.Originator)
                        select t;

            if (Search.Description.HasValue())
                q = from t in q
                    where t.Description.Contains(Search.Description)
                    select t;

            if (Search.Notes.HasValue())
                q = from t in q
                    where t.Notes.Contains(Search.Notes)
                    select t;

            if (Search.IsPrivate)
                q = from t in q
                    where (t.LimitToRole ?? "") != ""
                    select t;

            return q;
        }

        internal void Archive()
        {
            var q = DefineModelList().ToList();
            foreach (var t in q)
                DbUtil.Db.ExecuteCommand("UPDATE dbo.Task SET Archive = 1 WHERE Id = {0}", t.Id);
        }

        private static IQueryable<TaskSearch> GetBaseResults(CMSDataContext db, TaskSearchInfo.OptionInfo opt)
        {
            var u = db.CurrentUser;
            var roles = u.UserRoles.Select(uu => uu.Role.RoleName.ToLower()).ToArray();
            var managePrivateContacts = HttpContext.Current.User.IsInRole("ManagePrivateContacts");
            var manageTasks = HttpContext.Current.User.IsInRole("ManageTasks");
            var uid = Util.UserPeopleId;
            var q = from t in db.ViewTaskSearches
                where (t.LimitToRole ?? "") == "" || roles.Contains(t.LimitToRole) || managePrivateContacts
                where manageTasks || t.OrginatorId == uid || t.OwnerId == uid || t.CoOwnerId == uid
                where t.Archive == opt.Archived
                select t;

            if (opt.Status == 99)
                q = from t in q
                    where new[] {TaskStatusCode.Active, TaskStatusCode.Pending}.Contains(t.StatusId ?? 0)
                    select t;
            else if (opt.Status > 0)
                q = from t in q
                    where t.StatusId == opt.Status
                    select t;

            if (opt.ExcludeNewPerson)
                q = from t in q
                    where !t.Description.StartsWith("New Person")
                    select t;

            if (opt.Lookback.HasValue)
            {
                var ed = opt.EndDt;
                if (!ed.HasValue)
                    ed = DateTime.Today.AddDays(1);
                q = from t in q
                    where t.Created >= ed.Value.AddDays(-opt.Lookback.Value)
                    select t;
            }
            if (opt.EndDt.HasValue)
                q = from t in q
                    where t.Created <= opt.EndDt
                    select t;

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
                case "Status":
                    return q.OrderBy(tt => tt.Status).ThenByDescending(tt => tt.Created);
                case "Status desc":
                    return q.OrderByDescending(tt => tt.Status).ThenByDescending(tt => tt.Created);
                case "Originator":
                    return q.OrderBy(tt => tt.Originator2 ?? "zzz").ThenByDescending(tt => tt.Created);
                case "Originator desc":
                    return q.OrderByDescending(tt => tt.Originator2 ?? "zzz").ThenByDescending(tt => tt.Created);
                case "About":
                    return q.OrderBy(tt => tt.About2).ThenByDescending(tt => tt.Created);
                case "About desc":
                    return q.OrderByDescending(tt => tt.About2).ThenByDescending(tt => tt.Created);
                case "Owner":
                    return q.OrderBy(tt => tt.Owner2).ThenByDescending(tt => tt.Created);
                case "Owner desc":
                    return q.OrderByDescending(tt => tt.Owner2).ThenByDescending(tt => tt.Created);
                case "Delegate":
                    return q.OrderBy(tt => tt.Delegate2 ?? "zzz").ThenByDescending(tt => tt.Created);
                case "Delegate desc":
                    return q.OrderByDescending(tt => tt.Delegate2 ?? "zzz").ThenByDescending(tt => tt.Created);
            }
            return q;
        }

        public override IEnumerable<TaskSearch> DefineViewList(IQueryable<TaskSearch> q)
        {
            return q;
        }

        public static string[] FindNames(string type, string term, int limit, string optionstring)
        {
            var options = TaskSearchInfo.GetOptions(optionstring);
            var q = GetBaseResults(DbUtil.Db, options);

            switch (type)
            {
                case "Delegate":
                    return (from t in q
                        where t.Delegate2.StartsWith(term)
                        select t.Delegate2).Distinct().Take(limit).ToArray();
                case "About":
                    return (from t in q
                        where t.About2.StartsWith(term)
                        select t.About2).Distinct().Take(limit).ToArray();
                case "Owner":
                    return (from t in q
                        where t.Owner2.StartsWith(term)
                        select t.Owner2).Distinct().Take(limit).ToArray();
                case "Originator":
                    return (from t in q
                        where t.Originator2.StartsWith(term)
                        select t.Originator2).Distinct().Take(limit).ToArray();
            }
            return new string[0];
        }

        public HtmlString Icon(string which)
        {
            return Search.Icon(which);
        }
    }
}