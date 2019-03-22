using CmsData;
using CmsData.Classes.RoleChecker;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.Code;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgPeopleModel : PagedTableModel<OrgFilterPerson, OrgFilterPerson>
    {
        public CMSDataContext Db { get; set; }
        public Guid QueryId { get; set; }
        public IPrincipal User { get; set; }

        public OrgPeopleModel()
            : base("Name", "asc", true)
        {
        }

        private Organization org;
        public Organization Org
        {
            get
            {
                if (org == null)
                {
                    if (Id == null)
                    {
                        Id = Db.CurrentSessionOrgId;
                    }

                    org = Db.LoadOrganizationById(Id);
                    CheckNameLinks();
                }
                return org;
            }
        }

        public override IQueryable<OrgFilterPerson> DefineModelList()
        {
            var filter = Db.OrgFilter(QueryId);
            filter.CopyPropertiesFrom(this);
            filter.TagId = (int?)Db.TagCurrent().Id;
            filter.LastUpdated = DateTime.Now;
            Db.SubmitChanges();
            var q = from p in Db.OrgFilterPeople(QueryId, ShowMinistryInfo)
                    select p;
            return q;
        }

        private Tag orgTag;
        public Tag OrgTag => orgTag ??
                             (orgTag = Db.FetchOrCreateTag(QueryId.ToString(), Util.UserPeopleId, DbUtil.TagTypeId_OrgMembers));

        private List<int> currentList;

        public List<int> CurrentList()
        {
            if (currentList != null)
            {
                return currentList;
            }

            return currentList = DefineModelList().Select(vv => vv.PeopleId.Value).ToList();
        }
        public List<int> CurrentNotChecked()
        {
            return CurrentList().Except(CurrentChecked()).ToList();
        }

        public List<int> AllChecked()
        {
            return OrgTag.People(Db).Select(pp => pp.PeopleId).ToList();
        }
        public List<int> CurrentChecked()
        {
            return AllChecked().Intersect(CurrentList()).ToList();
        }

        public Dictionary<int, string> NameLinks;
        private void CheckNameLinks()
        {
            if (org == null)
            {
                return;
            }

            var ev = org.GetExtra(Db, "ShowNameLinks");
            if (ev.HasValue())
            {
                var namelinks = Regex.Replace(ev,
                    @"\[(?<text>.*?)\]\((?<url>[^\s]*)\s*(?<attr>.*?)\)",
                    "<a href=\"${url}\" ${attr}>${text}</a>");
                NameLinks = (
                    from pid in CurrentList()
                    select new
                    {
                        i = pid,
                        links = namelinks.Replace("{peopleid}", pid.ToString())
                            .Replace("{orgid}", Id.ToString())
                    }
               ).ToDictionary(vv => vv.i, vv => vv.links);
            }
        }


        public override IQueryable<OrgFilterPerson> DefineModelSort(IQueryable<OrgFilterPerson> q)
        {
            if (Direction == "asc")
            {
                switch (Sort)
                {
                    case "Name":
                        q = from p in q orderby p.Name2, p.PeopleId select p;
                        break;
                    case "Church":
                        q = from p in q orderby p.MemberStatus, p.Name2, p.PeopleId select p;
                        break;
                    case "MemberType":
                        q = from p in q orderby p.MemberCode select p;
                        break;
                    case "Primary Address":
                        q = from p in q orderby p.St, p.City, p.Address, p.PeopleId select p;
                        break;
                    case "BFTeacher":
                        q = from p in q orderby p.LeaderName, p.Name2, p.PeopleId select p;
                        break;
                    case "% Att.":
                        q = from p in q orderby p.AttPct select p;
                        break;
                    case "Age":
                        q = from p in q orderby p.BirthYear, p.BirthMonth, p.BirthDay select p;
                        break;
                    case "Bday":
                        q = from p in q orderby p.BirthMonth, p.BirthDay, p.Name2 select p;
                        break;
                    case "Last Attended":
                        q = from p in q orderby p.LastAttended, p.Name2 select p;
                        break;
                    case "Join Date":
                        q = from p in q orderby p.Joined, p.Name2 select p;
                        break;
                    case "Drop Date":
                        q = from p in q orderby p.Dropped, p.Joined, p.Name2 select p;
                        break;
                    case "Inactive Date":
                        q = from p in q orderby p.InactiveDate, p.Name2 select p;
                        break;
                    case "Contact Made":
                        q = from p in q orderby p.LastContactMadeDt ?? SqlDateTime.MinValue.Value, p.Name2 select p;
                        break;
                    case "Contact Received":
                        q = from p in q orderby p.LastContactReceivedDt, p.Name2 select p;
                        break;
                    case "Task About":
                        q = from p in q orderby p.TaskAboutDt ?? SqlDateTime.MinValue.Value, p.Name2 select p;
                        break;
                    case "Task Assigned":
                        q = from p in q orderby p.TaskDelegatedDt ?? SqlDateTime.MinValue.Value, p.Name2 select p;
                        break;
                    case "Tab":
                        q = from p in q orderby p.Tab, p.Name2 select p;
                        break;
                    case "Ck":
                        q = from p in q orderby p.IsChecked, p.Name2 select p;
                        break;
                }
            }
            else
            {
                switch (Sort)
                {
                    case "Church":
                        q = from p in q orderby p.MemberStatus descending, p.Name2, p.PeopleId descending select p;
                        break;
                    case "MemberType":
                        q = from p in q orderby p.MemberCode descending, p.Name2, p.PeopleId descending select p;
                        break;
                    case "Address":
                        q = from p in q orderby p.St descending, p.City descending, p.Address descending, p.PeopleId descending select p;
                        break;
                    case "BFTeacher":
                        q = from p in q orderby p.LeaderName descending, p.Name2, p.PeopleId descending select p;
                        break;
                    case "% Att.":
                        q = from p in q orderby p.AttPct descending select p;
                        break;
                    case "Name":
                        q = from p in q orderby p.Name2 descending, p.PeopleId descending select p;
                        break;
                    case "Bday":
                        q = from p in q orderby p.BirthMonth descending, p.BirthDay descending, p.Name2 descending select p;
                        break;
                    case "Last Attended":
                        q = from p in q orderby p.LastAttended descending, p.Name2 descending select p;
                        break;
                    case "Join Date":
                        q = from p in q orderby p.Joined descending, p.Dropped descending, p.Name2 descending select p;
                        break;
                    case "Drop Date":
                        q = from p in q orderby p.Dropped descending, p.Joined descending, p.Name2 descending select p;
                        break;
                    case "Inactive Date":
                        q = from p in q orderby p.InactiveDate descending, p.Name2 descending select p;
                        break;
                    case "Contact Made":
                        q = from p in q orderby p.LastContactMadeDt ?? SqlDateTime.MinValue.Value descending, p.Name2 descending select p;
                        break;
                    case "Contact Received":
                        q = from p in q orderby p.LastContactReceivedDt descending, p.Name2 descending select p;
                        break;
                    case "Task About":
                        q = from p in q orderby p.TaskAboutDt ?? SqlDateTime.MinValue.Value descending, p.Name2 descending select p;
                        break;
                    case "Task Assigned":
                        q = from p in q orderby p.TaskDelegatedDt ?? SqlDateTime.MinValue.Value descending, p.Name2 descending select p;
                        break;
                    case "Age":
                        q = from p in q orderby p.BirthYear descending, p.BirthMonth descending, p.BirthDay descending select p;
                        break;
                    case "Tab":
                        q = from p in q orderby p.Tab descending, p.Name2 descending select p;
                        break;
                    case "Ck":
                        q = from p in q orderby p.IsChecked descending, p.Name2 descending select p;
                        break;
                }
            }

            return q;
        }

        public override IEnumerable<OrgFilterPerson> DefineViewList(IQueryable<OrgFilterPerson> q)
        {
            return q;
        }

        public IEnumerable<SelectListItem> SmallGroups()
        {
            return from mt in Db.MemberTags
                   where mt.OrgId == Id
                   orderby mt.Name
                   select new SelectListItem
                   {
                       Text = mt.Name,
                       Value = mt.Id.ToString()
                   };
        }

        public string GroupOptions
        {
            get
            {
                var values = new List<string>();
                if (MultiSelect)
                {
                    values.Add("Multi");
                }

                if (ShowHidden)
                {
                    values.Add("Hidden");
                }

                if (ShowMinistryInfo)
                {
                    values.Add("Ministry");
                }

                if (values.Count == 0)
                {
                    values.Add("Options");
                }

                return string.Join(",", values);
            }
        }

        public bool MultiSelect { get; set; }
        public bool ShowMinistryInfo { get; set; }

        public string GroupActive(string group)
        {
            return GroupSelect.Contains(group) ? "active" : "";
        }
        public bool IsFiltered
        {
            get
            {
                return NameFilter.HasValue() ||
                        SgFilter.HasValue() ||
                        FilterIndividuals ||
                        FilterTag;
            }
        }

        public string MultiSelectActive => MultiSelect ? "active" : "";
        public string ShowMinistryInfoActive => ShowMinistryInfo ? "active" : "";
        public string ShowHiddenActive => ShowHidden ? "active" : "";
        public string ShowAddressActive => ShowAddress ? "active" : "";
        public string FilterTagActive => FilterTag ? "active" : "";
        public string FilterIndActive => FilterIndividuals ? "active" : "";

        private bool? orgLeaderAddDrop;
        public bool OrgLeaderAddDrop => orgLeaderAddDrop ?? (bool)(orgLeaderAddDrop = RoleChecker.HasSetting(SettingName.OrgMembersDropAdd, false));
        public bool DisablePeopleLink => RoleChecker.HasSetting(SettingName.DisablePersonLinks, false);
        public bool HideInactiveButton => RoleChecker.HasSetting(SettingName.HideInactiveOrgMembers, false);
        public bool HidePendingButton => RoleChecker.HasSetting(SettingName.HidePendingOrgMembers, false);
        public bool HideGuestsButton => RoleChecker.HasSetting(SettingName.HideGuestsOrgMembers, false);
        public bool HidePreviousButton => RoleChecker.HasSetting(SettingName.HidePreviousOrgMembers, false);

        public bool ShowOptions => RoleChecker.HasSetting(SettingName.Organization_ShowOptionsMenu, true);
        public bool ShowSubgroupFilters => RoleChecker.HasSetting(SettingName.Organization_ShowFiltersBar, true);
        public bool ShowBirthday => RoleChecker.HasSetting(SettingName.Organization_ShowBirthday, true);
        public bool ShowTagButtons => RoleChecker.HasSetting(SettingName.Organization_ShowTagButtons, true);
        public bool ShowShowAddress => RoleChecker.HasSetting(SettingName.Organization_ShowAddress, true);

        public bool ShowAddress { get; set; }
        public int? Id { get; set; }
        public string GroupSelect { get; set; }
        public string NameFilter { get; set; }
        public string SgFilter { get; set; }
        public bool ShowHidden { get; set; }
        public bool FilterTag { get; set; }
        public bool FilterIndividuals { get; set; }
        public bool ClearFilter { get; set; }

        public HtmlString GroupHelp => ViewExtensions2.Markdown(@"
* Click one of the buttons to see those people.
* You can work with them individually
* or combine them with the options dropdown.

When a single group is shown (not combined),
use the dropdown menu immediately to it's right
to `Add`, `Drop`, `Update` Members etc.
");
        public HtmlString NameFilterHelp => ViewExtensions2.Markdown(@"
**Match a Name**

* First and last name or just last name.
* You can put just the first few letters of each.
* Put a space after the first name to search first only.
* PeopleId works too.
");

        public HtmlString SgFilterHelp => ViewExtensions2.Markdown(@"
**Match a sub-group name.**

* Partial match just the first few letters of a sub-group when you follow with asterisk (`*`)
* Lead with a minus sign (`-`) to exclude a sub-group.
* Use a semi-colon (`;`) to separate multiple sub-groups.
* `NONE` to find no sub-groups assigned.
* `ALL:` to match people who are in each group specified.
* When there are more groups than fit into the textbox, most browsers will let you resize that box so you can see the rest.
");

        public bool GroupNeedsMenu(string group)
        {
            switch (group)
            {
                case GroupSelectCode.Guest:
                    return false;
                case GroupSelectCode.Previous:
                    return User.IsInRole("Developer") || User.IsInRole("Conversion");
                default:
                    return true;
            }
        }

        public bool Showdrop(string group)
        {
            if ((MultiSelect ? "" : GroupSelect) != group)
            {
                return false;
            }

            var u = HttpContextFactory.Current.User;
            return u.IsInRole("Edit")
                || RoleChecker.HasSetting(SettingName.OrgMembersDropAdd, false);
        }
    }
}
