using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models;
using UtilityExtensions;
using System.Web.Mvc;
using CmsData.View;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgPeopleModel : PagedTableModel<OrgPerson, OrgPerson>, ICurrentOrg
    {
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

        public string GroupSelect { get; set; }

        public OrgPeopleModel()
            : base("Name", "asc")
        {
        }

        public bool IsFiltered
        {
            get { return NameFilter.HasValue() || SgFilter.HasValue(); }
        }
        public IEnumerable<SelectListItem> SmallGroups()
        {
            var q = from mt in DbUtil.Db.MemberTags
                    where mt.OrgId == Id
                    orderby mt.Name
                    select new SelectListItem
                    {
                        Text = mt.Name,
                        Value = mt.Id.ToString()
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Value = "-1", Text = "(not assigned)" });
            list.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return list;
        }

        public override IQueryable<OrgPerson> DefineModelList()
        {
            var q = from p in DbUtil.Db.OrgPeople(Id, GroupSelect, 
                        this.First(), this.Last(), SgFilter, ShowHidden, 
                        Util2.CurrentTag, Util2.CurrentTagOwnerId)
                select p;
            return q;
        }

        public override IQueryable<OrgPerson> DefineModelSort(IQueryable<OrgPerson> q)
        {
            if (Direction == "asc")
                switch (Sort)
                {
                    case "Name":
                        q = from p in q
                            orderby p.Name2, p.PeopleId
                            select p;
                        break;
                    case "Church":
                        q = from p in q
                            orderby p.MemberStatus, p.Name2, p.PeopleId
                            select p;
                        break;
                    case "MemberType":
                        q = from p in q
                            orderby p.MemberCode
                            select p;
                        break;
                    case "Primary Address":
                        q = from p in q
                            orderby p.St,
                            p.City,
                            p.Address,
                            p.PeopleId
                            select p;
                        break;
                    case "BFTeacher":
                        q = from p in q
                            orderby p.LeaderName,
                            p.Name2,
                            p.PeopleId
                            select p;
                        break;
                    case "% Att.":
                        q = from p in q
                            orderby p.AttPct
                            select p;
                        break;
                    case "Age":
                        q = from p in q
                            orderby p.BirthYear, p.BirthMonth, p.BirthDay
                            select p;
                        break;
                    case "Bday":
                        q = from p in q
                            orderby p.BirthMonth, p.BirthDay,
                            p.Name2
                            select p;
                        break;
                    case "Last Att.":
                        q = from p in q
                            orderby p.LastAttended, p.Name2
                            select p;
                        break;
                    case "Joined":
                        q = from p in q
                            orderby p.Joined, p.Name2
                            select p;
                        break;
                }
            else
                switch (Sort)
                {
                    case "Church":
                        q = from p in q
                            orderby p.MemberStatus descending,
                            p.Name2,
                            p.PeopleId descending
                            select p;
                        break;
                    case "MemberType":
                        q = from p in q
                            orderby p.MemberCode descending,
                            p.Name2,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Address":
                        q = from p in q
                            orderby p.St descending,
                                   p.City descending,
                                   p.Address descending,
                                   p.PeopleId descending
                            select p;
                        break;
                    case "BFTeacher":
                        q = from p in q
                            orderby p.LeaderName descending,
                            p.Name2,
                            p.PeopleId descending
                            select p;
                        break;
                    case "% Att.":
                        q = from p in q
                            orderby p.AttPct descending
                            select p;
                        break;
                    case "Name":
                        q = from p in q
                            orderby p.Name2,
                            p.PeopleId descending
                            select p;
                        break;
                    case "Bday":
                        q = from p in q
                            orderby p.BirthMonth descending, p.BirthDay descending,
                            p.Name2 descending
                            select p;
                        break;
                    case "Last Att.":
                        q = from p in q
                            orderby p.LastAttended descending, p.Name2 descending
                            select p;
                        break;
                    case "Joined":
                        q = from p in q
                            orderby p.Joined descending, p.Name2 descending
                            select p;
                        break;
                    case "Age":
                        q = from p in q
                            orderby p.BirthYear descending, p.BirthMonth descending, p.BirthDay descending
                            select p;
                        break;
                }
            return q;
        }

        public override IEnumerable<OrgPerson> DefineViewList(IQueryable<OrgPerson> q)
        {
            return q;
        }

        public int? Id { get; set; }
        public string NameFilter { get; set; }
        public string SgFilter { get; set; }
        public bool ShowHidden { get; set; }
        public bool ClearFilter { get; set; }
    }
}
