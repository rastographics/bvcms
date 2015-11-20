using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CmsData;
using CmsData.View;
using MoreLinq;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class ExportInvolvements
    {
        public static EpplusResult InvolvementList(Guid queryid)
        {
            var db = DbUtil.Db;
            var nocheckrole = db.Setting("AllowLimitToRoleForInvolvementExport", "false").ToBool();
            var q = db.PeopleQuery(queryid);
            var q2 = from p in q
                     orderby p.LastName, p.FirstName
                     let spouse = db.People.SingleOrDefault(w => p.SpouseId == w.PeopleId)
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
                         Age = p.Age ?? 0,
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
                         CampusDate = db.LastChanged(p.PeopleId, "CampusId").FormatDate()
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
                         BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                         JoinDate = p.JoinDate.FormatDate(),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         MemberStatus = p.MemberStatus.Description,
                         FellowshipLeader = p.BFClass.LeaderName,
                         Age = p.Age.ToString(),
                         School = p.SchoolOther,
                         Grade = p.Grade.ToString(),
                         EmContact = rr.Emcontact,
                         EmPhone = rr.Emphone,
                         Medical = rr.MedicalDescription
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
                         BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         FellowshipLeader = p.BFClass.LeaderName,
                         Age = p.Age.ToString(),
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
                         BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                         JoinDate = p.JoinDate.FormatDate(),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         MemberStatus = p.MemberStatus.Description,
                         FellowshipLeader = p.BFClass.LeaderName,
                         Age = p.Age.ToString(),
                         School = p.SchoolOther,
                         Grade = p.Grade.ToString(),
                         LastAttend = bfm.LastAttended.ToString(),
                         AttendPct = bfm.AttendPct.ToString(),
                         bfm.AttendStr
                     };
            return q2.Take(maximumRows).ToDataTable().ToExcel("AttendList.xlsx");
        }

        //        public static IEnumerable OrgMemberList2(int qid)
        //        {
        //            var q = DbUtil.Db.PeopleQuery(qid);
        //            var q2 = q.Select(p => new
        //            {
        //                om = DbUtil.Db.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == Util2.CurrentOrgId && om.PeopleId == p.PeopleId),
        //                rr = p.RecRegs.FirstOrDefault(),
        //                p = p,
        //                test = p.PeopleExtras.SingleOrDefault(vv => vv.Field == "test")
        //            });
        //            var q3 = q2.Select("new(p.PreferredName,p.LastName,om.AttendStr,om.AmountPaid)");
        //            return q3;
        //        }
        public static EpplusResult OrgMemberListGroups()
        {
            var cmd = new SqlCommand(
                $"dbo.OrgMembers {DbUtil.Db.CurrentOrg.Id}, '{DbUtil.Db.CurrentOrg.SgFilter}'");
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();
            return cmd.ExecuteReader().ToExcel("OrgMemberGroups.xlsx");
        }

        public static IEnumerable<CurrOrgMember> OrgMemberList(int orgid)
        {
            var Db = DbUtil.Db;
            return Db.CurrOrgMembers(orgid.ToString());
        }

        public static EpplusResult PromoList(Guid queryid, int maximumRows)
        {
            var Db = DbUtil.Db;
            var q = Db.PeopleQuery(queryid);
            var q2 = from p in q
                     let bfm = Db.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == DbUtil.Db.CurrentOrg.Id && om.PeopleId == p.PeopleId)
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
                        sb.Append(MemberInfoRaw.Replace("\n", "<br/>"));
                    if (Fname.HasValue())
                        sb.AppendFormat("Father: {0}<br/>", Fname);
                    if (Mname.HasValue())
                        sb.AppendFormat("Mother: {0}<br/>", Mname);
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
