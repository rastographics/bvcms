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
using CmsData.View;
using System.Data;

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
                groups = new int[] {0};
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

        //Accept subgroup to limit query by those who are in this subgroup  --JoelS
        public static List<AttendInfo> RollListFilteredBySubgroup(int? meetingId, int orgId, DateTime meetingDate,
            bool sortByName = false, bool currentMembers = false, bool fromMobile = false)
        {
            string subgroupIds = "";
            bool includeLeaderless = false;
            var userPersonId = Util.UserPeopleId.GetValueOrDefault();
            //get array of subgroupIds for the groups that this user is a leader of
            var subgroups = (from mt in DbUtil.Db.OrgMemMemTags
                             where mt.PeopleId == userPersonId
                             where mt.OrgId == orgId
                             where mt.IsLeader == true
                             select mt.MemberTagId).ToArray();
            if (subgroups.Any())
            {
                subgroupIds = string.Join(",", subgroups);
                //if current user is the OrgLeader, include all members NOT in a subgroup
                var o = DbUtil.Db.LoadOrganizationById(orgId);
                includeLeaderless = userPersonId == o.LeaderId;
            }

            var q = DbUtil.Db.RollListFilteredBySubgroups(meetingId, meetingDate, orgId, currentMembers, fromMobile, subgroupIds, includeLeaderless);

            if (sortByName)
                q = from p in q
                    orderby p.Name
                    select p;
            else
                q = from p in q
                    orderby p.Section, p.Last, p.FamilyId, p.First
                    select p;

            var q2 = from p in q
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
                         OtherAttend = p.OtherAttends,
                         CurrMember = p.CurrMember == true,
                         Conflict = p.Conflict == true,
                         ChurchMemberStatus = p.ChurchMemberStatus
                     };
            return q2.ToList();
        }

        public static List<AttendInfo> RollList(int? meetingId, int orgId, DateTime meetingDate,
            bool sortByName = false, bool currentMembers = false, bool fromMobile = false, bool registeredOnly = false)
        {
            var q = DbUtil.Db.RollList(meetingId, meetingDate, orgId, currentMembers, fromMobile);

            if (sortByName)
                q = from p in q
                    orderby p.Name
                    select p;
            else
                q = from p in q
                    orderby p.Section, p.Last, p.FamilyId, p.First
                    select p;
            if (registeredOnly)
                q = from p in q
                    where AttendCommitmentCode.committed.Contains(p.CommitmentId ?? 0)
                    select p;

            var q2 = from p in q
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
                         OtherAttend = p.OtherAttends,
                         CurrMember = p.CurrMember == true,
                         Conflict = p.Conflict == true,
                         ChurchMemberStatus = p.ChurchMemberStatus
                     };
            return q2.ToList();
        }

        public static List<AttendInfo> RollListHighlight(int? meetingId, int orgId, DateTime meetingDate,
            bool sortByName = false, bool currentMembers = false, string highlight = null, bool registeredOnly = false)
        {
            var q = from p in DbUtil.Db.RollListHighlight(meetingId, meetingDate, orgId, currentMembers, highlight)
                    select p;

            if (sortByName)
                q = from p in q
                    orderby p.Name
                    select p;
            else
                q = from p in q
                    orderby p.Section, p.Last, p.FamilyId, p.First
                    select p;

            if (registeredOnly)
                q = from p in q
                    where AttendCommitmentCode.committed.Contains(p.CommitmentId ?? 0)
                    select p;

            var q2 = from p in q
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
                         OtherAttend = p.OtherAttends,
                         CurrMember = p.CurrMember ?? false,
                         Highlight = p.Highlight ?? false,
                         ChurchMemberStatus = p.ChurchMemberStatus
                     };
            return q2.ToList();
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
            public bool CurrMember { get; set; }
            public string CurrMemberType { get; set; }
            public string MemberType { get; set; }
            public string AttendType { get; set; }
            public string ChurchMemberStatus { get; set; }
            public int? OtherAttend { get; set; }
            public bool Highlight { get; set; }
            public bool Conflict { get; set; }
        }
    }
}