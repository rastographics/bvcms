using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Xml.Linq;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;
using System.Data.SqlClient;

namespace CmsWeb.Areas.Search
{
    public class ExportPeople
    {
        public static IEnumerable FetchExcelLibraryList(Guid queryid)
        {
            var Db = DbUtil.Db;
            var query = Db.PeopleQuery(queryid);
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
                        BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                        HomePhone = p.HomePhone.FmtFone(),
                        CellPhone = p.CellPhone.FmtFone(),
                        WorkPhone = p.WorkPhone.FmtFone(),
                        MemberStatus = p.MemberStatus.Description,
                        Married = p.MaritalStatus.Description,
                    };
            return q;
        }
        public static IEnumerable FetchExcelList(Guid queryid, int maximumRows)
        {
            var Db = DbUtil.Db;
            var query = Db.PeopleQuery(queryid);
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
                        BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                        BirthDay = Util.FormatBirthday(null, p.BirthMonth, p.BirthDay),
                        JoinDate = p.JoinDate.FormatDate(),
                        HomePhone = p.HomePhone.FmtFone(),
                        CellPhone = p.CellPhone.FmtFone(),
                        WorkPhone = p.WorkPhone.FmtFone(),
                        MemberStatus = p.MemberStatus.Description,
                        Age = p.Age.ToString(),
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
            return q.Take(maximumRows);
        }

        public static IEnumerable<ExcelFamilyMember> FetchExcelListFamilyMembers(Guid qid)
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
                         BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                         BirthDay = Util.FormatBirthday(null, p.BirthMonth, p.BirthDay),
                         JoinDate = p.JoinDate.FormatDate(),
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         MemberStatus = p.MemberStatus.Description,
                         Age = p.Age.ToString(),
                         School = p.SchoolOther,
                         Married = p.MaritalStatus.Description,
                         FamilyName = famname,
                         FamilyId = p.FamilyId,
                         FamilyPosition = pos.ToString(),
                         Grade = p.Grade.ToString(),
                         FellowshipLeader = p.BFClass.LeaderName,
                         AttendPctBF = (om == null ? 0 : om.AttendPct == null ? 0 : om.AttendPct.Value),
                         FellowshipClass = (om == null ? "" : om.Organization.OrganizationName),
                         AltName = p.AltName,
                     };
            return q2;
        }
        public static IEnumerable FetchExcelListFamily(Guid queryid)
        {
            var Db = DbUtil.Db;
            var query = Db.PeopleQuery(queryid);

            var q = from f in Db.Families
                    where query.Any(ff => ff.FamilyId == f.FamilyId)
                    let p = Db.People.Single(pp => pp.PeopleId == f.HeadOfHouseholdId)
                    let spouse = Db.People.SingleOrDefault(sp => sp.PeopleId == f.HeadOfHouseholdSpouseId)
                    let children = from pp in f.People
                                   where pp.PeopleId != f.HeadOfHouseholdId
                                   where pp.DeceasedDate == null
                                   where pp.PeopleId != (f.HeadOfHouseholdSpouseId ?? 0)
                                   where pp.PositionInFamilyId == 30
                                   orderby pp.LastName == p.LastName ? 1 : 2, pp.Age descending
                                   select pp.LastName == p.LastName ? pp.PreferredName : pp.Name
                    select new
                    {
                        FamilyId = p.FamilyId,
                        LastName = p.LastName,
                        LabelName = (spouse == null ? p.PreferredName : p.PreferredName + " & " + spouse.PreferredName),
                        Children = string.Join(", ", children),
                        Address = p.AddrCityStateZip,
                        HomePhone = p.HomePhone.FmtFone(),
                        Email = p.EmailAddress,
                        SpouseEmail = spouse.EmailAddress
                    };
            return q;
        }
        public static IEnumerable FetchExcelListFamily2(Guid queryid)
        {
            var Db = DbUtil.Db;
            var query = Db.PeopleQuery(queryid);

            var q = from p in Db.People
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
                        BirthDate = Util.FormatBirthday(p.BirthYear, p.BirthMonth, p.BirthDay),
                        BirthDay = Util.FormatBirthday(null, p.BirthMonth, p.BirthDay),
                        JoinDate = p.JoinDate.FormatDate(),
                        HomePhone = p.HomePhone.FmtFone(),
                        CellPhone = p.CellPhone.FmtFone(),
                        WorkPhone = p.WorkPhone.FmtFone(),
                        MemberStatus = p.MemberStatus.Description,
                        FellowshipLeader = p.BFClass.LeaderName,
                        Age = p.Age.ToString(),
                        School = p.SchoolOther,
                        Grade = p.Grade.ToString(),
                        AttendPctBF = (om == null ? 0 : om.AttendPct == null ? 0 : om.AttendPct.Value),
                    };
            return q;
        }

        public static IEnumerable<ExcelPic> FetchExcelListPics(Guid queryid, int maximumRows)
        {
            var Db = DbUtil.Db;
            var query = Db.PeopleQuery(queryid);
            var q = from p in query
                    let om = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == p.BibleFellowshipClassId)
                    let spouse = Db.People.Where(pp => pp.PeopleId == p.SpouseId).Select(pp => pp.PreferredName).SingleOrDefault()
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
                        BirthDate = p.BirthMonth + "/" + p.BirthDay + "/" + p.BirthYear,
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
                        Age = p.Age.ToString(),
                        School = p.SchoolOther,
                        Grade = p.Grade.ToString(),
                        AttendPctBF = (om == null ? 0 : om.AttendPct == null ? 0 : om.AttendPct.Value),
                        Married = p.MaritalStatus.Description,
                        FamilyId = p.FamilyId,
                        Image = p.PictureId == null ? Util.ServerLink("/images/unknown.jpg") :
                            Util.ServerLink("/Portrait/{0}/160/200".Fmt(p.Picture.LargeId))
                    };
            return q.Take(maximumRows);
        }
    }
}