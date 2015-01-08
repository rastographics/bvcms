/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using CmsData;
using CmsData.Codes;

namespace CmsWeb.Areas.Reports.Models
{
	public class RollsheetModel
	{
		public class PersonInfo
		{
			public int PeopleId { get; set; }
			public string Name { get; set; }
			public string Name2 { get; set; }
			public string BirthDate { get; set; }
			public string Age { get; set; }
			public string Address { get; set; }
			public string Address2 { get; set; }
			public string CityStateZip { get; set; }
			public int PhonePref { get; set; }
			public string HomePhone { get; set; }
			public string CellPhone { get; set; }
			public string WorkPhone { get; set; }
			public string MemberStatus { get; set; }
			public string Email { get; set; }
			public bool HasTag { get; set; }
			public string BFTeacher { get; set; }
			public int? BFTeacherId { get; set; }
			public DateTime? LastAttended { get; set; }
		}
		public class PersonMemberInfo : PersonInfo
		{
			public string MemberTypeCode { get; set; }
			public string MemberType { get; set; }
			public int MemberTypeId { get; set; }
			public DateTime? InactiveDate { get; set; }
			public decimal? AttendPct { get; set; }
			public DateTime? Joined { get; set; }
            public string MedicalDescription { get; set; }
		}
		public class PersonVisitorInfo : PersonInfo
		{
			public string VisitorType { get; set; }
			public string NameParent1 { get; set; }
			public string NameParent2 { get; set; }
		}
		// This gets current org members
		public static IEnumerable<PersonMemberInfo> FetchOrgMembers(int orgid, int[] groups)
		{
			if (groups == null)
				groups = new int[] { 0 };
			var tagownerid = Util2.CurrentTagOwnerId;
			var q = from om in DbUtil.Db.OrganizationMembers
					where om.OrganizationId == orgid
					where om.OrgMemMemTags.Any(mt => groups.Contains(mt.MemberTagId)) || (groups[0] == 0)
					where !groups.Contains(-1) || (groups.Contains(-1) && om.OrgMemMemTags.Count() == 0)
					where (om.Pending ?? false) == false
					where om.MemberTypeId != MemberTypeCode.InActive
					where om.MemberTypeId != MemberTypeCode.Prospect
					where om.EnrollmentDate <= Util.Now
					orderby om.Person.LastName, om.Person.FamilyId, om.Person.Name2
					let p = om.Person
                    let rr = p.RecRegs.SingleOrDefault()
					select new PersonMemberInfo
					{
						PeopleId = p.PeopleId,
						Name = p.Name,
						Name2 = p.Name2,
						BirthDate = Util.FormatBirthday(
							p.BirthYear,
							p.BirthMonth,
							p.BirthDay),
						Address = p.PrimaryAddress,
						Address2 = p.PrimaryAddress2,
						CityStateZip = Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
						PhonePref = p.PhonePrefId,
						HomePhone = p.HomePhone,
						CellPhone = p.CellPhone,
						WorkPhone = p.WorkPhone,
						MemberStatus = p.MemberStatus.Description,
						Email = p.EmailAddress,
						BFTeacher = p.BFClass.LeaderName,
						BFTeacherId = p.BFClass.LeaderId,
						Age = p.Age.ToString(),
						MemberTypeCode = om.MemberType.Code,
						MemberType = om.MemberType.Description,
						MemberTypeId = om.MemberTypeId,
						InactiveDate = om.InactiveDate,
						AttendPct = om.AttendPct,
						LastAttended = om.LastAttended,
						HasTag = p.Tags.Any(t => t.Tag.Name == Util2.CurrentTagName && t.Tag.PeopleId == tagownerid),
						Joined = om.EnrollmentDate,
                        MedicalDescription = rr.MedicalDescription
					};
			return q;
		}
		// This gets OrgMembers as of the date of the meeting
		private static IEnumerable<PersonMemberInfo> FetchOrgMembers(int OrganizationId, DateTime MeetingDate, bool CurrentMembers = false)
		{
			var tagownerid = Util2.CurrentTagOwnerId;
			IEnumerable<PersonMemberInfo> q = null;
		    if (CurrentMembers)
		        q = from m in DbUtil.Db.OrganizationMembers
		            where m.OrganizationId == OrganizationId
		            let p = m.Person
		            let enrolled = m.EnrollmentDate
		            where m.MemberTypeId != MemberTypeCode.InActive
		            where m.MemberTypeId != MemberTypeCode.Prospect
		            orderby p.LastName, p.FamilyId, p.PreferredName
		            select new PersonMemberInfo
		                       {
		                           PeopleId = p.PeopleId,
		                           Name = p.Name,
		                           Name2 = p.Name2,
		                           BirthDate = Util.FormatBirthday(
		                               p.BirthYear,
		                               p.BirthMonth,
		                               p.BirthDay),
		                           Address = p.PrimaryAddress,
		                           Address2 = p.PrimaryAddress2,
		                           CityStateZip = Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
		                           PhonePref = p.PhonePrefId,
		                           HomePhone = p.HomePhone,
		                           CellPhone = p.CellPhone,
		                           WorkPhone = p.WorkPhone,
		                           MemberStatus = p.MemberStatus.Description,
		                           Email = p.EmailAddress,
		                           BFTeacher = p.BFClass.LeaderName,
		                           BFTeacherId = p.BFClass.LeaderId,
		                           Age = p.Age.ToString(),
		                           MemberTypeCode = m.MemberType.Code,
		                           MemberType = m.MemberType.Description,
		                           MemberTypeId = m.MemberTypeId,
		                           Joined = m.EnrollmentDate
		                       };
		    else
		        q = from m in DbUtil.Db.OrgMembersAsOfDate(OrganizationId, MeetingDate)
					orderby m.LastName, m.FamilyId, m.PreferredName
		            select new PersonMemberInfo
		            {
						PeopleId = m.PeopleId,
						Name = m.PreferredName + " " + m.LastName,
						Name2 = m.LastName + ", " + m.PreferredName,
						BirthDate = Util.FormatBirthday(
							m.BirthYear,
							m.BirthMonth,
							m.BirthDay),
						Address = m.PrimaryAddress,
						Address2 = m.PrimaryAddress2,
						CityStateZip = Util.FormatCSZ(m.PrimaryCity, m.PrimaryState, m.PrimaryZip),
						HomePhone = m.HomePhone,
						CellPhone = m.CellPhone,
						WorkPhone = m.WorkPhone,
						MemberStatus = m.MemberStatus,
						Email = m.EmailAddress,
						BFTeacher = m.BFTeacher,
						BFTeacherId = m.BFTeacherId,
						Age = m.Age.ToString(),
						MemberType = m.MemberType,
						MemberTypeId = m.MemberTypeId,
						Joined = m.Joined
		            };
			return q;
		}

		private static int[] VisitAttendTypes = new int[] 
        { 
            AttendTypeCode.VisitingMember,
            AttendTypeCode.RecentVisitor, 
            AttendTypeCode.NewVisitor,
            AttendTypeCode.OtherClass
        };

		public static IEnumerable<PersonVisitorInfo> FetchVisitors(int orgid, DateTime MeetingDate, bool NoCurrentMembers, bool UseAltNames = false)
		{
		    var q = from p in DbUtil.Db.OrgVisitorsAsOfDate(orgid, MeetingDate, NoCurrentMembers)
					orderby p.LastName, p.FamilyId, p.PreferredName
		            select new PersonVisitorInfo()
		            {
						VisitorType = p.VisitorType,
						PeopleId = p.PeopleId,
						Name = p.PreferredName + " " + p.LastName,
						Name2 = p.LastName + ", " + p.PreferredName,
						BirthDate = Util.FormatBirthday(
							p.BirthYear,
							p.BirthMonth,
							p.BirthDay),
						Address = p.PrimaryAddress,
						Address2 = p.PrimaryAddress2,
						CityStateZip = Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
						HomePhone = p.HomePhone,
						CellPhone = p.CellPhone,
						WorkPhone = p.WorkPhone,
						MemberStatus = p.MemberStatus,
						Email = p.Email,
						BFTeacher = p.BFTeacher,
						BFTeacherId = p.BFTeacherId,
						Age = p.Age.ToString(),
						LastAttended = p.LastAttended,
                   };
			return q;
		}
		public static IEnumerable<AttendInfo> RollList(int? MeetingId, int OrgId, DateTime MeetingDate, bool SortByName = false, bool CurrentMembers = false)
		{
			// people who attended, members or visitors
			var attends = (from a in DbUtil.Db.Attends
						   where a.MeetingId == MeetingId
						   where a.EffAttendFlag == null || a.EffAttendFlag == true || a.Commitment != null
						   select new
						   {
							   a,
							   a.Person.Name2,
							   a.Person.Age,
							   a.Person.EmailAddress
						   }).ToList();

			// Members at the time of the meeting
			var members = FetchOrgMembers(OrgId, MeetingDate, CurrentMembers).ToList();

			// the list that will appear at the top, 
			// members who should attend and members who did attend
			var memberlist = from p in members
							 join pa in attends on p.PeopleId equals pa.a.PeopleId into j
							 from pa in j.DefaultIfEmpty()
							 let cid = pa != null ? pa.a.Commitment : null
							 where CurrentMembers || MeetingDate > p.Joined// they were either a member at the time
								 // or they attended as a member (workaround for bad transaction history)
									|| (pa != null && !VisitAttendTypes.Contains(pa.a.AttendanceTypeId.Value))
							 select new AttendInfo
							 {
								 PeopleId = p.PeopleId,
								 Name = p.Name2,
								 Email = p.Email,
								 Attended = pa != null && pa.a.AttendanceFlag,
								 AttendCommitmentId = cid,
								 Commitment = CmsData.Codes.AttendCommitmentCode.Lookup(cid ?? 99),
								 Member = true,
								 CurrMemberType = p.MemberType,
								 MemberType = pa != null ? (pa.a.MemberType != null ? pa.a.MemberType.Description : "") : "",
								 AttendType = pa != null ? (pa.a.AttendType != null ? pa.a.AttendType.Description : "") : "",
								 Age = p.Age,
								 OtherAttend = pa != null ? (int?)pa.a.OtherAttends : null
							 };

			// recent visitors and new visitors
			var visitors = FetchVisitors(OrgId, MeetingDate, NoCurrentMembers: false).ToList();

			// the list that appears at the bottom in bold, 
			// visitors who attended, 
			// recent visitors who did not attend excluding those who have since become members in the previous list
			var visitorlist = from pvisitor in visitors
							  where members.All(mm => mm.PeopleId != pvisitor.PeopleId)
							  join pattender in attends on pvisitor.PeopleId equals pattender.a.PeopleId into j
							  from pattender in j.DefaultIfEmpty()
							  let cid = pattender != null ? pattender.a.Commitment : null
							  select new AttendInfo
							  {
								  PeopleId = pvisitor.PeopleId,
								  Name = pvisitor.Name2,
								  Email = pvisitor.Email,
								  Attended = pattender != null && pattender.a.AttendanceFlag,
								  AttendCommitmentId = cid,
								  Commitment = CmsData.Codes.AttendCommitmentCode.Lookup(cid ?? 99),
								  Member = false,
								  CurrMemberType = "",
								  MemberType = pattender != null ? (pattender.a.MemberType != null ? pattender.a.MemberType.Description : "") : "",
								  AttendType = pattender != null ? (pattender.a.AttendType != null ? pattender.a.AttendType.Description : "") : "",
								  Age = pvisitor.Age,
								  OtherAttend = pattender != null ? (int?)pattender.a.OtherAttends : null
							  };

			var otherlist = from pa in attends
							join v in visitorlist on pa.a.PeopleId equals v.PeopleId into jv
							join m in memberlist on pa.a.PeopleId equals m.PeopleId into jm
							from v in jv.DefaultIfEmpty()
							from m in jm.DefaultIfEmpty()
						    let cid = pa != null ? pa.a.Commitment : null
							let mt = pa.a.MemberType == null ? "?" : pa.a.MemberType.Description + " ?"
							let at = pa.a.AttendType == null ? "?" : pa.a.AttendType.Description + " ?"
							where v == null && m == null
							select new AttendInfo
							{
								PeopleId = pa.a.PeopleId,
								Name = pa.Name2,
								Email = pa.EmailAddress,
								Attended = true,
								AttendCommitmentId = cid,
								Commitment = CmsData.Codes.AttendCommitmentCode.Lookup(cid ?? 99),
								Member = false,
								MemberType = mt,
								AttendType = at
							};

			// the final rollsheet
			var rollsheet = from p in memberlist.Union(visitorlist).Union(otherlist)
							select new AttendInfo
							{
								PeopleId = p.PeopleId,
								Name = p.Name,
								Email = p.Email,
								Attended = p.Attended,
								AttendCommitmentId = p.AttendCommitmentId,
								Commitment = p.Commitment,
								Member = p.Member,
								CurrMemberType = p.CurrMemberType,
								MemberType = p.MemberType,
								AttendType = p.AttendType,
								OtherAttend = p.OtherAttend
							};

			if (SortByName)
				rollsheet = rollsheet.OrderBy(pp => pp.Name);
			return rollsheet;
		}

	    public static IEnumerable<AttendInfo> RollList2(int? MeetingId, int OrgId, DateTime MeetingDate,
	        bool SortByName = false, bool CurrentMembers = false)
	    {
	        var q = from p in DbUtil.Db.RollList(MeetingId, MeetingDate, OrgId, CurrentMembers)
                    orderby p.Section, p.Last, p.FamilyId, p.First
    	            select new AttendInfo()
    	            {
                        PeopleId = p.PeopleId.Value,
                        Name = p.Name,
                        Email = p.Email,
                        Attended = p.Attended ?? false,
                        AttendCommitmentId = p.CommitmentId,
    					Commitment = AttendCommitmentCode.Lookup(p.CommitmentId ?? 99),
                        Member = p.Section == 1,
                        CurrMemberType = p.CurrMemberType,
                        MemberType = p.MemberType,
                        AttendType = p.AttendType,
                        OtherAttend = p.OtherAttends
    	            };
	        if (SortByName)
	            q = q.OrderBy(pp => pp.Name);
	        return q;
	    }

	    public class AttendInfo
		{
			public int PeopleId { get; set; }
			public string Name { get; set; }
			public string Age { get; set; }
			public string Email { get; set; }
			public bool Attended { get; set; }
			public int? AttendCommitmentId { get; set; }
			public string Commitment { get; set; }
			public bool CanAttend { get; set; }
			public bool Member { get; set; }
			public string CurrMemberType { get; set; }
			public string MemberType { get; set; }
			public string AttendType { get; set; }
			public int? OtherAttend { get; set; }
		}
	}
}