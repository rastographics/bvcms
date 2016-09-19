using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;
using System.Web.Mvc;
using CmsData.Codes;
using CmsData.View;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgPeopleModel : PagedTableModel<OrgPerson, OrgPerson>, ICurrentOrg
    {
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
                        Id = DbUtil.Db.CurrentOrgId0;
                    org = DbUtil.Db.LoadOrganizationById(Id);
                }
                return org;
            }
        }

        public override IQueryable<OrgPerson> DefineModelList()
        {
            var q = from p in DbUtil.Db.OrgPeople(Id, GroupSelect,
                        this.First(), this.Last(), SgFilter, ShowHidden,
                        Util2.CurrentTagName, Util2.CurrentTagOwnerId,
                        FilterIndividuals, FilterTag, ShowMinistryInfo, Util.UserPeopleId)
                    select p;
            return q;
        }

        private Tag orgTag;
        public Tag OrgTag => orgTag ??
                             (orgTag = DbUtil.Db.FetchOrCreateTag("Org-" + Id, Util.UserPeopleId, DbUtil.TagTypeId_OrgMembers));

        private List<int> currentList;

        public List<int> CurrentList()
        {
            if (currentList != null)
                return currentList;
            return currentList = (from p in DbUtil.Db.OrgPeople(Id, GroupSelect,
                        this.First(), this.Last(), SgFilter, ShowHidden,
                        filterchecked: false, filtertag: FilterTag,
                        currtag: Util2.CurrentTagName, currtagowner: Util2.CurrentTagOwnerId,
                        ministryinfo: false, userpeopleid: Util.UserPeopleId)
                                  select p.PeopleId.Value).ToList();
        }
        public List<int> CurrentNotChecked()
        {
            return CurrentList().Except(CurrentChecked()).ToList();
        }

        public List<int> AllChecked()
        {
            return OrgTag.People(DbUtil.Db).Select(pp => pp.PeopleId).ToList();
        }
        public List<int> CurrentChecked()
        {
            return AllChecked().Intersect(CurrentList()).ToList();
        }

        public override IQueryable<OrgPerson> DefineModelSort(IQueryable<OrgPerson> q)
        {
            if (Direction == "asc")
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
            else
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
            return q;
        }

        public override IEnumerable<OrgPerson> DefineViewList(IQueryable<OrgPerson> q)
        {
            return q;
        }

        public IEnumerable<SelectListItem> SmallGroups()
        {
            return from mt in DbUtil.Db.MemberTags
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
                    values.Add("Multi");
                if (ShowHidden)
                    values.Add("Hidden");
                if (ShowMinistryInfo)
                    values.Add("Ministry");
                if (values.Count == 0)
                    values.Add("Options");
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
        public bool ShowPeopleLink => HttpContext.Current.User.IsInRole("OrgLeadersOnly") && DbUtil.Db.Setting("UX-DisablePersonLinksForOrgLeaders");
        public bool OrgLeaderAddDrop => HttpContext.Current.User.IsInRole("OrgLeadersOnly") && DbUtil.Db.Setting("UX-OrgLeadersOnlyOrgMembersDropAdd");
        public bool HideInactiveButton => HttpContext.Current.User.IsInRole("OrgLeadersOnly") && DbUtil.Db.Setting("UX-HideInactiveOrgMembersForOrgLeadersOnly");
        public bool HidePendingButton => HttpContext.Current.User.IsInRole("OrgLeadersOnly") && DbUtil.Db.Setting("UX-HidePendingOrgMembersForOrgLeadersOnly");
        public bool HideGuestsButton => HttpContext.Current.User.IsInRole("OrgLeadersOnly") && DbUtil.Db.Setting("UX-HideGuestsOrgMembersForOrgLeadersOnly");

        public int? Id { get; set; }
        public string GroupSelect { get; set; }
        public string NameFilter { get; set; }
        public string SgFilter { get; set; }
        public bool ShowHidden { get; set; }
        public bool ShowAddress { get; set; }
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
                    var u = HttpContext.Current.User;
                    return u.IsInRole("Developer") || u.IsInRole("Conversion");
                default:
                    return true;
            }
        }

        public bool Showdrop(string group)
        {
            var u = HttpContext.Current.User;
            var orgLeader = u.IsInRole("OrgLeadersOnly");

            return (u.IsInRole("Edit") || (orgLeader && DbUtil.Db.Setting("UX-OrgLeadersOnlyOrgMembersDropAdd"))) &&
                (MultiSelect ? "" : GroupSelect) == group;
        }
    }
}
