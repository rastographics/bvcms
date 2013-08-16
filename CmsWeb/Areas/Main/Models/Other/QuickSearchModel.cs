/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using UtilityExtensions;
using System.Data.Linq.SqlClient;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace CmsWeb.Models
{
	public class QuickSearchModel
	{
		private const int CountMax = 60;
		public string text { get; set; }
		private CMSDataContext Db;

		public List<MailingController.PersonInfo> people;
		public List<OrgSearchModel.OrganizationInfo> orgs;
		string First, Last;

		public QuickSearchModel(string t)
		{
			text = t ?? "";
			Util.NameSplit(text, out First, out Last);
			Db = DbUtil.Db;
			people = PeopleList().ToList();
			orgs = Orglist().ToList();
		}

		private IEnumerable<MailingController.PersonInfo> PeopleList()
		{
			var qp = DbUtil.Db.People.AsQueryable();
			if (Util2.OrgMembersOnly)
				qp = DbUtil.Db.OrgMembersOnlyTag2().People(DbUtil.Db);
			else if (Util2.OrgLeadersOnly)
				qp = DbUtil.Db.OrgLeadersOnlyTag2().People(DbUtil.Db);

			var hasfirst = First.HasValue();
			var phone = "donotmatch";
			if (text.HasValue() && text.AllDigits() && text.Length == 7)
				phone = text;
			if (text.AllDigits())
			{
				qp = from p in qp
					 where
						 p.PeopleId == Last.ToInt()
						 || p.CellPhone.Contains(phone)
						 || p.Family.HomePhone.Contains(phone)
						 || p.WorkPhone.Contains(phone)
					 orderby p.Name2
					 select p;
			}
			else
				qp = from p in qp
					 where
						 (
						 (p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last)
						 || p.LastName.StartsWith(text) || p.MaidenName.StartsWith(text))
						 && (!hasfirst || p.FirstName.StartsWith(First) || p.NickName.StartsWith(First) || p.MiddleName.StartsWith(First)
						 || p.LastName.StartsWith(text) || p.MaidenName.StartsWith(text))
						 )
						 || p.PeopleId == Last.ToInt()
						 //|| p.EmployerOther.Contains(text)
						 || p.Family.AddressLineOne.StartsWith(text)
						 || p.Family.AddressLineTwo.StartsWith(text)
					 orderby p.Name2
					 select p;
			return PeopleList(qp.Take(CountMax));
		}
		private IEnumerable<OrgSearchModel.OrganizationInfo> Orglist()
		{
			var roles = Db.CurrentUser.UserRoles.Select(uu => uu.Role.RoleName).ToArray();
			var qo = from o in Db.Organizations
					 where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                     where o.OrganizationStatusId == OrgStatusCode.Active
					 select o;

			if (Util2.OrgMembersOnly)
				qo = from o in qo
					 where o.OrganizationMembers.Any(om => om.PeopleId == Util.UserPeopleId)
					 select o;
			else if (Util2.OrgLeadersOnly)
			{
				var oids = Db.GetLeaderOrgIds(Util.UserPeopleId);
				qo = Db.Organizations.Where(o => oids.Contains(o.OrganizationId));
			}
			var text1 = "";

			if (text.StartsWith("-") && text.Length > 1)
				text1 = text.Substring(1);
			if (text1.AllDigits())
				qo = from o in qo
					 where
						 o.OrganizationId == text1.ToInt()
					 orderby o.Division.Program.Name, o.Division.Name, o.OrganizationName
					 select o;
			else
				qo = from o in qo
					 where
						 o.OrganizationName.Contains(text)
						 || (o.LeaderName.Contains(First) && o.LeaderName.Contains(Last))
						 || o.DivOrgs.Any(t => t.Division.Name.Contains(text))
					 orderby o.Division.Program.Name, o.Division.Name, o.OrganizationName
					 select o;
			return OrgSearchModel.OrganizationList(qo.Take(CountMax), null, null);
		}

        private IEnumerable<MailingController.PersonInfo> PeopleList(IQueryable<Person> query)
        {
            var q = from p in query
                    select new MailingController.PersonInfo
                    {
                        PeopleId = p.PeopleId,
                        Name = p.Name,
                        BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                        Address = p.PrimaryAddress,
                        Address2 = p.PrimaryAddress2,
                        CityStateZip = Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                        HomePhone = p.HomePhone,
                        CellPhone = p.CellPhone,
                        WorkPhone = p.WorkPhone,
                        PhonePref = p.PhonePrefId,
                        MemberStatus = p.MemberStatus.Description,
                        Email = p.EmailAddress,
                        BFTeacher = p.BFClass.LeaderName,
                        BFTeacherId = p.BFClass.LeaderId,
                        Age = p.Age.ToString(),
                        Deceased = p.DeceasedDate.HasValue,
                    };
            return q;
        }

	}
}
