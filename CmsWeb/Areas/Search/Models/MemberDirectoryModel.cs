/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Search.Controllers;
using CmsWeb.Models;
using UtilityExtensions;
using HandlebarsDotNet;

namespace CmsWeb.Areas.Search.Models
{
    public class MemberDirectoryModel : PagerModel2
    {
        private int? count;
        private IQueryable<Person> members;
        private MemberDirectoryController ctl;

        public MemberDirectoryModel(MemberDirectoryController ctl)
        {
            GetCount = Count;
            Sort = "Family";
            this.ctl = ctl;
            Handlebars.RegisterHelper("PagerTop", (writer, context, args) =>
            {
                writer.Write(ViewExtensions2.RenderPartialViewToString(ctl, "PagerTop", this));
            });
            Handlebars.RegisterHelper("PagerBottom", (writer, context, args) =>
            {
                writer.WriteLine(ViewExtensions2.RenderPartialViewToString(ctl, "PagerBottom", this));
                writer.WriteLine(Hidden("totcnt", Count().ToString("N0")));
                writer.WriteLine(Hidden("Page", Page));
                writer.WriteLine(Hidden("Sort", Sort));
                writer.WriteLine(Hidden("Direction", Direction));
            });
        }
        public static string Hidden(string name, object value)
        {
            var tb = new TagBuilder("input");
            tb.MergeAttribute("id", name);
            tb.MergeAttribute("type", "hidden");
            tb.MergeAttribute("name", name);
            tb.MergeAttribute("value", value.ToString());
            return tb.ToString();
        }

        public string Name { get; set; }
        public int OrgId { get; set; }
        public bool FamilyOption { get; set; }

        public IEnumerable<DirectoryInfo> MemberList()
        {
            members = FetchMembers();
            if (!count.HasValue)
                count = Count();

            var q1 = members.AsQueryable();

            if (Sort == "Birthday")
                q1 = from p in q1
                     orderby DbUtil.Db.NextBirthday(p.PeopleId)
                     select p;
            else
            {
                var qf = (from p in members
                          let famname = p.Family.People.Single(hh => hh.PeopleId == hh.Family.HeadOfHouseholdId).Name2
                          group p by new {famname, p.FamilyId}
                          into g
                          orderby g.Key.famname, g.Key.FamilyId
                          select g.Max(pp => pp.FamilyId)).Skip(StartRow).Take(PageSize);
                ;
                q1 = from p in q1
                     where qf.Contains(p.FamilyId)
                     let pos = (p.PositionInFamilyId == 10 ? p.GenderId : 1000 - (p.Age ?? 0))
                     let famname = p.Family.People.Single(hh => hh.PeopleId == hh.Family.HeadOfHouseholdId).Name2
                     orderby famname, p.FamilyId, p.PositionInFamilyId == 10 ? p.GenderId : 1000 - (p.Age ?? 0)
                     select p;
            }

            var q2 = from p in q1
                     select new DirectoryInfo
                     {
                         Family = p.LastName,
                         FamilyId = p.FamilyId,
                         Name = p.PreferredName,
                         Suffix = p.SuffixCode,
                         Birthday = p.BirthDate.ToString2("m"),
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         CityStateZip = p.CityStateZip,
                         Cell = p.CellPhone.FmtFone("C"),
                         Home = p.HomePhone.FmtFone("H"),
                         Email = (p.SendEmailAddress1 ?? true) ? p.EmailAddress : "",
                         Email2 = (p.SendEmailAddress2 ?? false) ? p.EmailAddress2 : "",
                         DoNotPublishPhones = p.DoNotPublishPhones
                     };

            return q2;
        }

        public int Count()
        {
            if (!count.HasValue)
            {
                if (Sort == "Family")
                    count = (from pp in FetchMembers()
                             group pp by pp.FamilyId
                             into g
                             select g.Key).Count();
                else
                    count = FetchMembers().Count();
            }
            return count.Value;
        }

        private IQueryable<Person> FetchMembers()
        {
            if (members != null)
                return members;

            var q = from o in DbUtil.Db.Organizations
                    where o.OrganizationId == OrgId
                    select o.PublishDirectory;
            FamilyOption = q.Single() == 2;

            if (FamilyOption)
                members = from p in DbUtil.Db.People
                          where p.Family.People.Any(pp =>
                              pp.OrganizationMembers.Any(mm =>
                                  mm.OrganizationId == OrgId
                                  && (mm.Pending ?? false) == false
                                  && mm.MemberTypeId != MemberTypeCode.InActive
                                  && mm.MemberTypeId != MemberTypeCode.Prospect))
                          where p.DeceasedDate == null
                          select p;
            else
                members = from p in DbUtil.Db.People
                          where p.OrganizationMembers.Any(mm =>
                              mm.OrganizationId == OrgId
                              && (mm.Pending ?? false) == false
                              && mm.MemberTypeId != MemberTypeCode.InActive
                              && mm.MemberTypeId != MemberTypeCode.Prospect)
                          where p.DeceasedDate == null
                          select p;

            if (Name.HasValue())
                members = from p in members
                          where p.Name.Contains(Name)
                          select p;
            return members;
        }
    }
}
