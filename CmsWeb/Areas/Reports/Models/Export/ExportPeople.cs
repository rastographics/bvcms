using CmsData;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class ExportPeople
    {
        public static EpplusResult FetchExcelLibraryList(Guid queryid)
        {
            //var Db = Db;
            var query = DbUtil.Db.PeopleQuery(queryid);
            var q = from p in query
                    let om = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == p.BibleFellowshipClassId)
                    select new
                    {
                        PeopleId = p.PeopleId,
                        FirstName = p.FirstName,
                        GoesBy = p.NickName,
                        LastName = p.LastName,
                        Address = p.PrimaryAddress,
                        City = p.PrimaryCity,
                        State = p.PrimaryState,
                        Zip = p.PrimaryZip.FmtZip(),
                        Email = p.EmailAddress,
                        BirthDate = Person.FormatBirthday(p.BirthYr, p.BirthMonth, p.BirthDay, p.PeopleId),
                        HomePhone = p.HomePhone.FmtFone(),
                        CellPhone = p.CellPhone.FmtFone(),
                        WorkPhone = p.WorkPhone.FmtFone(),
                        MemberStatus = p.MemberStatus.Description,
                        Married = p.MaritalStatus.Description,
                    };
            return q.ToDataTable().ToExcel("LibraryList.xlsx");
        }
        public static DataTable FetchExcelList(Guid queryid, int maximumRows, bool useMailFlags)
        {
            //var Db = Db;
            var query = DbUtil.Db.PeopleQuery(queryid);
            if (useMailFlags)
            {
                query = MailingController.FilterMailFlags(query);
            }

            var q = from p in query
                    let om = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == p.BibleFellowshipClassId)
                    let oid = p.PeopleExtras.FirstOrDefault(pe => pe.Field == "OtherId").Data
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
                        Country = p.PrimaryCountry,
                        Zip = p.PrimaryZip.FmtZip(),
                        Email = p.EmailAddress,
                        BirthDate = Person.FormatBirthday(p.BirthYr, p.BirthMonth, p.BirthDay, p.PeopleId),
                        BirthDay = Person.FormatBirthday(null, p.BirthMonth, p.BirthDay, p.PeopleId),
                        JoinDate = p.JoinDate.FormatDate(),
                        HomePhone = p.HomePhone.FmtFone(),
                        CellPhone = p.CellPhone.FmtFone(),
                        WorkPhone = p.WorkPhone.FmtFone(),
                        MemberStatus = p.MemberStatus.Description,
                        Age = Person.AgeDisplay(p.Age, p.PeopleId).ToString(),
                        Married = p.MaritalStatus.Description,
                        Wedding = p.WeddingDate.FormatDate(),
                        p.FamilyId,
                        FamilyPosition = p.FamilyPosition.Description,
                        Gender = p.Gender.Description,
                        School = p.SchoolOther,
                        Grade = p.Grade.ToString(),
                        FellowshipLeader = p.BFClass.LeaderName,
                        AttendPctBF = (om == null ? 0 : om.AttendPct == null ? 0 : om.AttendPct.Value),
                        FellowshipClass = (om == null ? "" : om.Organization.OrganizationName),
                        p.AltName,
                        Employer = p.EmployerOther,
                        OtherId = oid ?? "",
                        Campus = p.Campu == null ? "" : p.Campu.Description,
                        DecisionDate = p.DecisionDate.FormatDate()
                    };
            return q.Take(maximumRows).ToDataTable();
        }
        public static DataTable DonorDetails(DateTime startdt, DateTime enddt,
            int fundid, int campusid, bool pledges, bool? nontaxdeductible, bool includeUnclosed, int? tagid, string fundids)
        {
            var UseTitles = !DbUtil.Db.Setting("NoTitlesOnStatements");

            if (DbUtil.Db.Setting("UseLabelNameForDonorDetails"))
            {
                var q = from c in DbUtil.Db.GetContributionsDetails(startdt, enddt, campusid, pledges, nontaxdeductible, includeUnclosed, tagid, fundids)
                        join p in DbUtil.Db.People on c.CreditGiverId equals p.PeopleId
                        let mainFellowship = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == p.BibleFellowshipClassId).OrganizationName
                        let head1 = DbUtil.Db.People.Single(hh => hh.PeopleId == p.Family.HeadOfHouseholdId)
                        let head2 = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == p.Family.HeadOfHouseholdSpouseId)
                        let altcouple = p.Family.FamilyExtras.SingleOrDefault(ee => (ee.FamilyId == p.FamilyId) && ee.Field == "CoupleName" && p.SpouseId != null).Data
                        select new
                        {
                            c.FamilyId,
                            Date = c.DateX.Value.ToShortDateString(),
                            GiverId = c.PeopleId,
                            c.CreditGiverId,
                            c.HeadName,
                            c.SpouseName,
                            MainFellowship = mainFellowship,
                            MemberStatus = p.MemberStatus.Description,
                            p.JoinDate,
                            Amount = c.Amount ?? 0m,
                            Pledge = c.PledgeAmount ?? 0m,
                            c.CheckNo,
                            c.ContributionDesc,
                            c.FundId,
                            c.FundName,
                            BundleHeaderId = c.BundleHeaderId ?? 0,
                            c.BundleType,
                            c.BundleStatus,
                            Addr = p.PrimaryAddress,
                            Addr2 = p.PrimaryAddress2,
                            City = p.PrimaryCity,
                            ST = p.PrimaryState,
                            Zip = p.PrimaryZip,
                            FirstName = p.PreferredName,
                            p.LastName,
                            FamilyName = altcouple.Length > 0 ? altcouple : head2 == null
                                ? (UseTitles ? (head1.TitleCode != null ? head1.TitleCode + " " + head1.Name : head1.Name) : head1.Name)
                                : (UseTitles
                                        ? (head1.TitleCode != null
                                            ? head1.TitleCode + " and Mrs. " + head1.Name
                                            : "Mr. and Mrs. " + head1.Name)
                                        : head1.PreferredName + " and " + head2.PreferredName + " " + head1.LastName +
                                           (head1.SuffixCode.Length > 0 ? ", " + head1.SuffixCode : "")),
                            p.EmailAddress
                        };
                return q.ToDataTable();

            }
            else
            {
                var q = from c in DbUtil.Db.GetContributionsDetails(startdt, enddt, campusid, pledges, nontaxdeductible, includeUnclosed, tagid, fundids)
                        join p in DbUtil.Db.People on c.CreditGiverId equals p.PeopleId
                        let mainFellowship = DbUtil.Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == p.BibleFellowshipClassId).OrganizationName
                        let spouse = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == p.SpouseId)
                        let altcouple = p.Family.FamilyExtras.SingleOrDefault(ee => (ee.FamilyId == p.FamilyId) && ee.Field == "CoupleName" && p.SpouseId != null).Data
                        select new
                        {
                            c.FamilyId,
                            Date = c.DateX.Value.ToShortDateString(),
                            GiverId = c.PeopleId,
                            CreditGiverId = c.CreditGiverId.Value,
                            c.HeadName,
                            c.SpouseName,
                            MainFellowship = mainFellowship,
                            MemberStatus = p.MemberStatus.Description,
                            p.JoinDate,
                            Amount = c.Amount ?? 0m,
                            Pledge = c.PledgeAmount ?? 0m,
                            c.CheckNo,
                            c.ContributionDesc,
                            c.FundId,
                            c.FundName,
                            BundleHeaderId = c.BundleHeaderId ?? 0,
                            c.BundleType,
                            c.BundleStatus,
                            p.FullAddress,
                            p.EmailAddress
                        };
                return q.ToDataTable();
            }
        }
        public static DataTable ExcelDonorTotals(DateTime startdt, DateTime enddt,
            int campusid, bool pledges, bool? nontaxdeductible, bool includeUnclosed, int? tagid, string fundids)
        {
#if DEBUG2
            // for reconciliation by developer
            var v = from c in DbUtil.Db.GetContributionsDetails(startdt, enddt, campusid, pledges, nontaxdeductible, includeUnclosed, tagid, fundids)
                orderby c.ContributionId
                select c.ContributionId;
            using(var tw = new StreamWriter("D:\\exportdonors.txt"))
               foreach (var s in v)
                  tw.WriteLine(s);
#endif


            var q2 = from r in DbUtil.Db.GetTotalContributionsDonor(startdt, enddt, campusid, nontaxdeductible, includeUnclosed, tagid, fundids)
                     select new
                     {
                         GiverId = r.CreditGiverId,
                         Count = r.Count ?? 0,
                         Amount = r.Amount ?? 0m,
                         Pledged = r.PledgeAmount ?? 0m,
                         r.Email,
                         FirstName = r.Head_FirstName,
                         LastName = r.Head_LastName,
                         Spouse = r.SpouseName ?? "",
                         MainFellowship = r.MainFellowship ?? "",
                         MemberStatus = r.MemberStatus ?? "",
                         r.JoinDate,
                         r.Addr,
                         r.Addr2,
                         r.City,
                         r.St,
                         r.Zip
                     };
            return q2.ToDataTable();
        }
        public static DataTable ExcelDonorFundTotals(DateTime startdt, DateTime enddt,
            int fundid, int campusid, bool pledges, bool? nontaxdeductible, bool includeUnclosed, int? tagid, string fundids)
        {
            var q2 = from r in DbUtil.Db.GetTotalContributionsDonorFund(startdt, enddt, campusid, nontaxdeductible, includeUnclosed, tagid, fundids)
                     select new
                     {
                         GiverId = r.CreditGiverId,
                         Count = r.Count ?? 0,
                         Amount = r.Amount ?? 0m,
                         Pledged = r.PledgeAmount ?? 0m,
                         Name = r.HeadName,
                         SpouseName = r.SpouseName ?? "",
                         r.FundName,
                         r.FundId,
                         r.MainFellowship,
                         r.MemberStatus,
                         r.JoinDate,
                     };
            return q2.ToDataTable();
        }

        public static EpplusResult FetchExcelListFamilyMembers(Guid qid)
        {
            var q = DbUtil.Db.PeopleQuery(qid);
            var q2 = from pp in q
                     group pp by pp.FamilyId into g
                     from p in g.First().Family.People
                     where p.DeceasedDate == null
                     let pos = p.PositionInFamilyId * 1000 + (p.PositionInFamilyId == 10 ? p.GenderId : 1000 - (p.Age ?? 0))
                     let om = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == p.BibleFellowshipClassId)
                     let famname = g.First().Family.People.Single(hh => hh.PeopleId == hh.Family.HeadOfHouseholdId).Name2
                     orderby famname, p.FamilyId, pos
                     select new ExcelFamilyMember
                     {
                         PeopleId = p.PeopleId,
                         Title = p.TitleCode,
                         FirstName = p.PreferredName,
                         LastName = p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         BirthDate = Person.FormatBirthday(p.BirthYr, p.BirthMonth, p.BirthDay, p.PeopleId),
                         BirthDay = Person.FormatBirthday(null, p.BirthMonth, p.BirthDay, p.PeopleId),
                         JoinDate = p.JoinDate.FormatDate(),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         MemberStatus = p.MemberStatus.Description,
                         Age = Person.AgeDisplay(p.Age, p.PeopleId).ToString(),
                         School = p.SchoolOther,
                         Married = p.MaritalStatus.Description,
                         Gender = p.Gender.Description,
                         FamilyName = famname,
                         FamilyId = p.FamilyId,
                         FamilyPosition = pos.ToString(),
                         Grade = p.Grade.ToString(),
                         FellowshipLeader = p.BFClass.LeaderName,
                         AttendPctBF = (om == null ? 0 : om.AttendPct == null ? 0 : om.AttendPct.Value),
                         FellowshipClass = (om == null ? "" : om.Organization.OrganizationName),
                         AltName = p.AltName,
                     };
            return q2.ToDataTable().ToExcel("ListFamilyMembers.xlsx");
        }
        public static EpplusResult FetchExcelListFamily(Guid queryid)
        {
            //var Db = Db;
            var query = DbUtil.Db.PeopleQuery(queryid);

            var q = from f in DbUtil.Db.Families
                    where query.Any(ff => ff.FamilyId == f.FamilyId)
                    let p = DbUtil.Db.People.Single(pp => pp.PeopleId == f.HeadOfHouseholdId)
                    let spouse = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == f.HeadOfHouseholdSpouseId)
                    let children = from pp in f.People
                                   where pp.PeopleId != f.HeadOfHouseholdId
                                   where pp.DeceasedDate == null
                                   where pp.PeopleId != (f.HeadOfHouseholdSpouseId ?? 0)
                                   where pp.PositionInFamilyId == 30
                                   orderby pp.LastName == p.LastName ? 1 : 2, pp.Age descending
                                   select pp.LastName == p.LastName ? pp.PreferredName : pp.Name
                    let altaddr = p.Family.FamilyExtras.SingleOrDefault(ee => ee.FamilyId == p.FamilyId && ee.Field == "MailingAddress").Data
                    let altcouple = p.Family.FamilyExtras.SingleOrDefault(ee => (ee.FamilyId == p.FamilyId) && ee.Field == "CoupleName" && p.SpouseId != null).Data
                    select new
                    {
                        FamilyId = p.FamilyId,
                        LastName = p.LastName,
                        LabelName = (spouse == null ? p.PreferredName : p.PreferredName + " & " + spouse.PreferredName),
                        Children = string.Join(", ", children),
                        Address = p.AddrCityStateZip,
                        HomePhone = p.HomePhone.FmtFone(),
                        Email = p.EmailAddress,
                        SpouseEmail = spouse.EmailAddress,
                        CellPhone = p.CellPhone.FmtFone(),
                        SpouseCell = spouse.CellPhone.FmtFone(),
                        MailingAddress = altaddr,
                        CoupleName = altcouple,
                        AltNames = (spouse == null ? p.AltName : p.AltName + " & " + spouse.AltName),
                    };
            return q.ToDataTable().ToExcel("FamilyList.xlsx");
        }
        public static EpplusResult FetchExcelListFamily2(Guid queryid)
        {
            //var Db = Db;
            var query = DbUtil.Db.PeopleQuery(queryid);

            var q = from p in DbUtil.Db.People
                    where query.Any(ff => ff.FamilyId == p.FamilyId)
                    orderby p.LastName, p.FamilyId, p.FirstName
                    where p.DeceasedDate == null
                    let om = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == p.BibleFellowshipClassId)
                    select new
                    {
                        FamilyId = p.FamilyId,
                        PeopleId = p.PeopleId,
                        LastName = p.LastName,
                        FirstName = p.PreferredName,
                        Position = p.FamilyPosition.Description,
                        Married = p.MaritalStatus.Description,
                        Title = p.TitleCode,
                        Address = p.PrimaryAddress,
                        Address2 = p.PrimaryAddress2,
                        City = p.PrimaryCity,
                        State = p.PrimaryState,
                        Zip = p.PrimaryZip.FmtZip(),
                        Email = p.EmailAddress,
                        BirthDate = Person.FormatBirthday(p.BirthYr, p.BirthMonth, p.BirthDay, p.PeopleId),
                        BirthDay = Person.FormatBirthday(null, p.BirthMonth, p.BirthDay, p.PeopleId),
                        JoinDate = p.JoinDate.FormatDate(),
                        HomePhone = p.HomePhone.FmtFone(),
                        CellPhone = p.CellPhone.FmtFone(),
                        WorkPhone = p.WorkPhone.FmtFone(),
                        MemberStatus = p.MemberStatus.Description,
                        FellowshipLeader = p.BFClass.LeaderName,
                        Age = Person.AgeDisplay(p.Age, p.PeopleId).ToString(),
                        School = p.SchoolOther,
                        Grade = p.Grade.ToString(),
                        AttendPctBF = (om == null ? 0 : om.AttendPct == null ? 0 : om.AttendPct.Value),
                        p.AltName,
                    };
            return q.ToDataTable().ToExcel("ListFamily2.xlsx");
        }

        public static IEnumerable<ExcelPic> FetchExcelListPics(Guid queryid, int maximumRows)
        {
            //var Db = Db;
            var query = DbUtil.Db.PeopleQuery(queryid);
            var q = from p in query
                    let om = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == p.BibleFellowshipClassId)
                    let spouse = DbUtil.Db.People.Where(pp => pp.PeopleId == p.SpouseId).Select(pp => pp.PreferredName).SingleOrDefault()
                    select new ExcelPic
                    {
                        PeopleId = p.PeopleId,
                        Title = p.TitleCode,
                        FirstName = p.PreferredName,
                        LastName = p.LastName,
                        Address = p.PrimaryAddress,
                        Address2 = p.PrimaryAddress2,
                        City = p.PrimaryCity,
                        State = p.PrimaryState,
                        Zip = p.PrimaryZip.FmtZip(),
                        Email = p.EmailAddress,
                        BYear = p.BirthYear,
                        BMon = p.BirthMonth,
                        BDay = p.BirthDay,
                        BirthDay = " " + p.BirthMonth + "/" + p.BirthDay,
                        Anniversary = " " + p.WeddingDate.Value.Month + "/" + p.WeddingDate.Value.Day,
                        JoinDate = p.JoinDate.FormatDate(),
                        JoinType = p.JoinType.Description,
                        HomePhone = p.HomePhone.FmtFone(),
                        CellPhone = p.CellPhone.FmtFone(),
                        WorkPhone = p.WorkPhone.FmtFone(),
                        MemberStatus = p.MemberStatus.Description,
                        FellowshipLeader = p.BFClass.LeaderName,
                        Spouse = spouse,
                        Age = Person.AgeDisplay(p.Age, p.PeopleId).ToString(),
                        School = p.SchoolOther,
                        Grade = p.Grade.ToString(),
                        AttendPctBF = (om == null ? 0 : om.AttendPct == null ? 0 : om.AttendPct.Value),
                        Married = p.MaritalStatus.Description,
                        FamilyId = p.FamilyId,
                    };
            return q.Take(maximumRows);
        }
        public static EpplusResult ExportExtraValues(Guid qid)
        {
            var roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);
            var xml = XDocument.Parse(DbUtil.Db.Content("StandardExtraValues2", "<Fields/>"));
            var fields = (from ff in xml.Root.Descendants("Value")
                          let vroles = ff.Attribute("VisibilityRoles")
                          where vroles != null && (vroles.Value.Split(',').All(rr => !roles.Contains(rr)))
                          select ff.Attribute("Name").Value);
            var nodisplaycols = string.Join("|", fields);

            var tag = DbUtil.Db.PopulateSpecialTag(qid, DbUtil.TagTypeId_ExtraValues);

            var cmd = new SqlCommand("dbo.ExtraValues @p1, @p2, @p3");
            cmd.Parameters.AddWithValue("@p1", tag.Id);
            cmd.Parameters.AddWithValue("@p2", "");
            cmd.Parameters.AddWithValue("@p3", nodisplaycols);
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();
            return cmd.ExecuteReader().ToExcel("ExtraValues.xlsx");
        }
    }
}
