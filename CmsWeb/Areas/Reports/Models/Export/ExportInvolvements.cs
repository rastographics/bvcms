using CmsData;
using CmsData.View;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class ExportInvolvements
    {
        public static EpplusResult InvolvementList(Guid queryid)
        {
            //var db = Db;
            var nocheckrole = DbUtil.Db.Setting("AllowLimitToRoleForInvolvementExport", "false").ToBool();
            var q = DbUtil.Db.PeopleQuery(queryid);
            var q2 = from p in q
                     orderby p.LastName, p.FirstName
                     let spouse = DbUtil.Db.People.SingleOrDefault(w => p.SpouseId == w.PeopleId)
                     let om = p.OrganizationMembers.SingleOrDefault(m => m.OrganizationId == p.BibleFellowshipClassId)
                     select new InvolvementInfo
                     {
                         PeopleId = p.PeopleId,
                         Addr = p.PrimaryAddress,
                         Addr2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         Name = p.Name,
                         HomePhone = p.HomePhone,
                         WorkPhone = p.WorkPhone,
                         CellPhone = p.CellPhone,
                         Email = p.EmailAddress,
                         DivName = om.Organization.Division.Name,
                         OrgName = om.Organization.OrganizationName,
                         Teacher = p.BFClass.LeaderName,
                         MemberType = om.MemberType.Description,
                         AttendPct = om.AttendPct,
                         AgeDb = p.Age,
                         Spouse = spouse != null ? spouse.FirstName : "",
                         activities = (from m in p.OrganizationMembers
                                       where nocheckrole || (m.Organization.LimitToRole ?? "") == ""
                                       select new ActivityInfo
                                       {
                                           Name = m.Organization.OrganizationName,
                                           Pct = m.AttendPct,
                                           Leader = m.Organization.LeaderName
                                       }).ToList(),
                         JoinInfo = p.JoinType.Description + " , " + p.JoinDate.ToString().Substring(0, 11),
                         Notes = "",
                         OfficeUseOnly = "",
                         LastName = p.LastName,
                         FirstName = p.PreferredName,
                         Campus = p.Campu.Description,
                         CampusDate = DbUtil.Db.LastChanged(p.PeopleId, "CampusId").FormatDate()
                     };
            var list = q2.ToList();
            return ExcelExportModel.ToDataTable(list).ToExcel("Involvements.xlsx");
        }

        public static EpplusResult ChildrenList(Guid queryid, int maximumRows)
        {
            var q = DbUtil.Db.PeopleQuery(queryid);
            var q2 = from p in q
                     let rr = p.RecRegs.FirstOrDefault()
                     select new
                     {
                         p.PeopleId,
                         Title = p.TitleCode,
                         FirstName = p.PreferredName,
                         p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         BirthDate = Person.FormatBirthday(p.BirthYr, p.BirthMonth, p.BirthDay, p.PeopleId),
                         JoinDate = p.JoinDate.FormatDate(),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         MemberStatus = p.MemberStatus.Description,
                         FellowshipLeader = p.BFClass.LeaderName,
                         Age = Person.AgeDisplay(p.Age, p.PeopleId).ToString(),
                         School = p.SchoolOther,
                         Grade = p.Grade.ToString(),
                         EmContact = rr.Emcontact,
                         EmPhone = rr.Emphone,
                         Allergies = rr.MedicalDescription
                     };
            return q2.Take(maximumRows).ToDataTable().ToExcel("ChildrenList.xlsx");
        }

        public static EpplusResult ChurchList(Guid queryid, int maximumRows)
        {
            var q = DbUtil.Db.PeopleQuery(queryid);
            var q2 = from p in q
                     let rescode = DbUtil.Db.ResidentCodes.SingleOrDefault(r => r.Id == p.PrimaryResCode).Description
                     select new
                     {
                         p.PeopleId,
                         Title = p.TitleCode,
                         FirstName = p.PreferredName,
                         p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         BirthDate = Person.FormatBirthday(p.BirthYr, p.BirthMonth, p.BirthDay, p.PeopleId),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         FellowshipLeader = p.BFClass.LeaderName,
                         Age = Person.AgeDisplay(p.Age, p.PeopleId).ToString(),
                         MemberStatus = p.MemberStatus.Description,
                         DropType = p.DropType.Description,
                         DropDate = p.DropDate.FormatDate(),
                         NewChurch = p.OtherNewChurch,
                         JoinType = p.JoinType.Description,
                         JoinDate = p.JoinDate.FormatDate(),
                         BaptismDate = p.BaptismDate.FormatDate(),
                         PrevChurch = p.OtherPreviousChurch,
                         Resident = rescode
                     };
            return q2.Take(maximumRows).ToDataTable().ToExcel("ChurchList.xlsx");
        }

        public static EpplusResult AttendList(Guid queryid, int maximumRows)
        {
            var q = DbUtil.Db.PeopleQuery(queryid);
            var q2 = from p in q
                     let bfm = DbUtil.Db.OrganizationMembers.FirstOrDefault(om =>
                         om.Organization.IsBibleFellowshipOrg == true
                         && om.PeopleId == p.PeopleId
                         && om.AttendPct > 0)
                     select new
                     {
                         p.PeopleId,
                         Title = p.TitleCode,
                         FirstName = p.PreferredName,
                         p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         BirthDate = Person.FormatBirthday(p.BirthYr, p.BirthMonth, p.BirthDay, p.PeopleId),
                         JoinDate = p.JoinDate.FormatDate(),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         MemberStatus = p.MemberStatus.Description,
                         FellowshipLeader = p.BFClass.LeaderName,
                         Age = Person.AgeDisplay(p.Age, p.PeopleId).ToString(),
                         School = p.SchoolOther,
                         Grade = p.Grade.ToString(),
                         LastAttend = bfm.LastAttended.ToString(),
                         AttendPct = bfm.AttendPct.ToString(),
                         bfm.AttendStr
                     };
            return q2.Take(maximumRows).ToDataTable().ToExcel("AttendList.xlsx");
        }

        public static EpplusResult OrgMemberListGroups(Guid queryid)
        {
            var filter = DbUtil.Db.OrgFilter(queryid);
            var cmd = new SqlCommand(
                $"dbo.OrgMembers {filter.Id}, '{filter.SgFilter}'");
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();
            return cmd.ExecuteReader().ToExcel("OrgMemberGroups.xlsx");
        }

        public static IEnumerable<CurrOrgMember> OrgMemberList(int orgid)
        {
            //var Db = Db;
            return DbUtil.Db.CurrOrgMembers(orgid.ToString());
        }

        public static EpplusResult PromoList(Guid queryid, int maximumRows)
        {
            //var db = Db;
            var filter = DbUtil.Db.OrgFilter(queryid);
            var q = DbUtil.Db.PeopleQuery(queryid);
            var q2 = from p in q
                     let bfm = DbUtil.Db.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == filter.Id && om.PeopleId == p.PeopleId)
                     let sc = bfm.Organization.OrgSchedules.FirstOrDefault() // SCHED
                     let tm = sc.SchedTime.Value
                     select new
                     {
                         p.PeopleId,
                         Title = p.TitleCode,
                         FirstName = p.PreferredName,
                         p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         MemberType = bfm.MemberType.Description,
                         bfm.Organization.Location,
                         bfm.Organization.PendingLoc,
                         Leader = bfm.Organization.LeaderName,
                         OrgName = bfm.Organization.OrganizationName,
                         Schedule = tm.Hour + ":" + tm.Minute.ToString().PadLeft(2, '0'),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone1 = p.Family.HeadOfHousehold.CellPhone.FmtFone(),
                         CellPhone2 = p.Family.HeadOfHouseholdSpouse.CellPhone.FmtFone()
                     };
            return q2.Take(maximumRows).ToDataTable().ToExcel("PromotionList.xlsx");
        }

        public class ActivityInfo
        {
            public string Name { get; set; }
            public decimal? Pct { get; set; }
            public string Leader { get; set; }
        }

        public class MemberInfoClass
        {
            public string Fname;
            public string MemberInfoRaw;
            public string Mname;
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public string Grade { get; set; }
            public string ShirtSize { get; set; }
            public string Request { get; set; }
            public decimal Amount { get; set; }
            public decimal AmountPaid { get; set; }
            public bool HasBalance { get; set; }
            public string Email { get; set; }
            public string HomePhone { get; set; }
            public string CellPhone { get; set; }
            public string WorkPhone { get; set; }
            public string Age { get; set; }
            public string BirthDate { get; set; }
            public string JoinDate { get; set; }
            public string MemberStatus { get; set; }
            public string School { get; set; }
            public string LastAttend { get; set; }
            public string AttendPct { get; set; }
            public string AttendStr { get; set; }
            public string MemberType { get; set; }

            public string MemberInfo
            {
                get
                {
                    var sb = new StringBuilder();
                    if (MemberInfoRaw.HasValue())
                    {
                        sb.Append(MemberInfoRaw.Replace("\n", "<br/>"));
                    }

                    if (Fname.HasValue())
                    {
                        sb.AppendFormat("Father: {0}<br/>", Fname);
                    }

                    if (Mname.HasValue())
                    {
                        sb.AppendFormat("Mother: {0}<br/>", Mname);
                    }

                    return sb.ToString();
                }
            }

            public string InactiveDate { get; set; }
            public string Medical { get; set; }
            public int PeopleId { get; set; }
            public string EnrollDate { get; set; }
            public string Groups { get; set; }
            public string Tickets { get; set; }
        }
    }
}
