/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using CmsData;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class MailingController
    {
        public bool UseTitles { get; set; }
        public bool UseMailFlags { get; set; }

        public IEnumerable<MailingInfo> FetchIndividualList(string sortExpression, Guid queryId)
        {
            var q = DbUtil.Db.PeopleQuery(queryId);
            if (UseMailFlags)
            {
                q = FilterMailFlags(q);
            }

            q = ApplySort(q, sortExpression);
            var q2 = from p in q
                     let altaddr = p.AddressTypeId == 10
                        ? p.PeopleExtras.SingleOrDefault(ee => ee.PeopleId == p.PeopleId && ee.Field == "MailingAddress").Data
                        : p.Family.FamilyExtras.SingleOrDefault(ee => ee.FamilyId == p.FamilyId && ee.Field == "MailingAddress").Data
                     where p.DeceasedDate == null
                     select new MailingInfo
                     {
                         MailingAddress = altaddr,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         LabelName = UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name,
                         Name2 = p.Name2,
                         LastName = p.LastName,
                         PeopleId = p.PeopleId,
                         CellPhone = p.CellPhone,
                         HomePhone = p.HomePhone,
                     };
            return q2;
        }
        public IEnumerable<MailingInfo> GroupByAddress(string sortExpression, Guid queryId)
        {
            var q = DbUtil.Db.PeopleQuery(queryId);
            if (UseMailFlags)
            {
                q = FilterMailFlags(q);
            }

            var q2 = from p in q
                     where p.DeceasedDate == null
                     group p by p.FamilyId into g
                     let one = g.First().Family.HeadOfHousehold
                     let altaddr = one.Family.FamilyExtras.SingleOrDefault(ee => ee.FamilyId == one.FamilyId && ee.Field == "MailingAddress").Data
                     let last = one.LastName
                     orderby one.ZipCode
                     select new MailingInfo
                     {
                         MailingAddress = altaddr,
                         Address = one.PrimaryAddress,
                         Address2 = one.PrimaryAddress2,
                         City = one.PrimaryCity,
                         State = one.PrimaryState,
                         Zip = one.PrimaryZip,
                         LabelName = Regex.Replace(string.Join(", ", g.Select(vv => vv.PreferredName)), "(.*)(,)([^,]*$)", "$1 &$3", RegexOptions.IgnoreCase),
                         LastName = one.LastName,
                         Name2 = one.Name2,
                         CellPhone = "",
                         HomePhone = one.HomePhone,
                     };
            q2 = ApplySort(q2, sortExpression);
            return q2;
        }

        public IEnumerable<MailingInfo> FetchFamilyList(int startRowIndex, int maximumRows, string sortExpression, Guid queryId)
        {
            var q1 = DbUtil.Db.PeopleQuery(queryId);
            var q = from f in DbUtil.Db.Families
                    where q1.Any(p => p.FamilyId == f.FamilyId)
                    select f.People.Single(fm => fm.PeopleId == f.HeadOfHouseholdId);

            if (UseMailFlags)
            {
                q = FilterMailFlags(q);
            }

            var q2 = from h in q
                     let spouse = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == h.SpouseId)
                     let altaddr = h.Family.FamilyExtras.SingleOrDefault(ee => ee.FamilyId == h.FamilyId && ee.Field == "MailingAddress").Data
                     where h.DeceasedDate == null
                     select new MailingInfo
                     {
                         MailingAddress = altaddr,
                         Address = h.PrimaryAddress,
                         Address2 = h.PrimaryAddress2,
                         City = h.PrimaryCity,
                         State = h.PrimaryState,
                         Zip = h.PrimaryZip,
                         LabelName = (h.Family.CoupleFlag == 1 ? (UseTitles ? (h.TitleCode != null ? h.TitleCode + " and Mrs. " + h.Name
                                                                                                    : "Mr. and Mrs. " + h.Name)
                                                                             : h.PreferredName + " and " + spouse.PreferredName + " " + h.LastName + (h.SuffixCode.Length > 0 ? ", " + h.SuffixCode : "")) :
                                      h.Family.CoupleFlag == 2 ? ("The " + h.Name + " Family") :
                                      h.Family.CoupleFlag == 3 ? ("The " + h.Name + " Family") :
                                      h.Family.CoupleFlag == 4 ? (h.Name + " & Family") :
                                      UseTitles ? (h.TitleCode != null ? h.TitleCode + " " + h.Name : h.Name) : h.Name),
                         LastName = h.LastName,
                         Name2 = h.Name2,
                         CellPhone = h.CellPhone,
                         HomePhone = h.HomePhone,
                         PeopleId = h.PeopleId
                     };
            q2 = ApplySort(q2, sortExpression);
            if (maximumRows == 0)
            {
                return q2;
            }

            return q2.Skip(startRowIndex).Take(maximumRows);
        }
        public IEnumerable<MailingInfo> FetchFamilyMembers(string sortExpression, Guid queryId)
        {
            var q = DbUtil.Db.PeopleQuery(queryId);
            var q2 = from pp in q
                     group pp by pp.FamilyId into g
                     from p in g.First().Family.People
                     where p.DeceasedDate == null
                     let famname = g.First().Family.People.Single(hh => hh.PeopleId == hh.Family.HeadOfHouseholdId).LastName
                     let altaddr = p.Family.FamilyExtras.SingleOrDefault(ee => ee.FamilyId == p.FamilyId && ee.Field == "MailingAddress").Data
                     orderby famname, p.FamilyId, p.PositionInFamilyId, p.GenderId
                     select new MailingInfo
                     {
                         MailingAddress = altaddr,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         LabelName = (UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name),
                         LastName = p.LastName,
                         Name2 = p.Name2,
                         CellPhone = p.CellPhone,
                         HomePhone = p.HomePhone,
                         PeopleId = p.PeopleId
                     };
            q2 = ApplySort(q2, sortExpression);
            return q2;
        }
        public IEnumerable<MailingInfo> FetchFamilyList(string sortExpression, Guid queryId)
        {
            int startRowIndex = 0;
            int maximumRows = 0;
            return FetchFamilyList(startRowIndex, maximumRows, sortExpression, queryId);
        }

        public IEnumerable<MailingInfo> FetchParentsOfList(string sortExpression, Guid queryId)
        {
            var q = DbUtil.Db.PeopleQuery(queryId);
            if (UseMailFlags)
            {
                q = FilterMailFlags(q);
            }

            var q2 = from p in q
                     where p.DeceasedDate == null
                     let altaddr = p.Family.FamilyExtras.SingleOrDefault(ee => ee.FamilyId == p.FamilyId && ee.Field == "MailingAddress").Data
                     select new MailingInfo
                     {
                         MailingAddress = altaddr,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         LabelName = (p.PositionInFamilyId == 30 ? ("Parents of " + p.Name) : UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name),
                         LastName = p.LastName,
                         Name2 = p.Name2,
                         CellPhone = p.CellPhone,
                         HomePhone = p.HomePhone,
                         PeopleId = p.PeopleId
                     };
            q2 = ApplySort(q2, sortExpression);
            return q2;
        }

        private const string STR_Couples = "couples";

        private IQueryable<Person> EliminateCoupleDoublets(IQueryable<Person> q)
        {
            IQueryable<Person> q1 = q;
            if (UseMailFlags)
            {
                q1 = from p in q
                         // exclude person who has a partner who is already in the list and with different PeopleID.
                     where !(p.SpouseId != null && q.Any(pp => pp.PeopleId == p.SpouseId && pp.PeopleId < p.PeopleId)
                        )
                     where (p.PrimaryBadAddrFlag ?? 0) == 0
                     where p.DoNotMailFlag == false
                     select p;
            }
            else
            {
                q1 = from p in q
                         // exclude person who has a partner who is already in the list and with different PeopleID.
                     where !(p.SpouseId != null && q.Any(pp => pp.PeopleId == p.SpouseId && pp.PeopleId < p.PeopleId))
                     select p;
            }

            return q1;
        }
        public static IQueryable<Person> FilterMailFlags(IQueryable<Person> q)
        {
            var q1 = from p in q
                     where (p.PrimaryBadAddrFlag ?? 0) == 0
                     where p.DoNotMailFlag == false
                     select p;
            return q1;
        }

        public IEnumerable<MailingInfo> FetchCouplesEitherList(string sortExpression, Guid queryId)
        {
            var q = DbUtil.Db.PopulateSpecialTag(queryId, DbUtil.TagTypeId_CouplesHelper).People(DbUtil.Db);
            var q1 = EliminateCoupleDoublets(q);
            var q2 = from p in q1
                     let spouse = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == p.SpouseId)
                     let altaddr = p.Family.FamilyExtras.SingleOrDefault(ee => ee.FamilyId == p.FamilyId && ee.Field == "MailingAddress").Data
                     let altcouple = p.Family.FamilyExtras.SingleOrDefault(ee => (ee.FamilyId == p.FamilyId) && ee.Field == "CoupleName" && p.SpouseId != null).Data
                     select new MailingInfo
                     {
                         MailingAddress = altaddr,
                         CoupleName = altcouple,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         LabelName = (spouse == null ? (UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name) :
                             (p.Family.HeadOfHouseholdId == p.PeopleId ?
                                 (UseTitles ? (p.TitleCode != null ? p.TitleCode + " and Mrs. " + p.Name : "Mr. and Mrs. " + p.Name)
                                             : (p.PreferredName + " and " + spouse.PreferredName + " " + p.LastName + (p.SuffixCode.Length > 0 ? ", " + p.SuffixCode : ""))) :
                                 (UseTitles ? (spouse.TitleCode != null ? spouse.TitleCode + " and Mrs. " + spouse.Name : "Mr. and Mrs. " + spouse.Name)
                                             : (spouse.PreferredName + " and " + p.PreferredName + " " + spouse.LastName + (spouse.SuffixCode.Length > 0 ? ", " + spouse.SuffixCode : ""))))),
                         LastName = p.LastName,
                         Name2 = p.Name2,
                         CellPhone = p.CellPhone,
                         HomePhone = p.HomePhone,
                         PeopleId = p.PeopleId
                     };
            q2 = ApplySort(q2, sortExpression);
            return q2;
        }

        public IEnumerable<MailingInfo> FetchCouplesBothList(string sortExpression, Guid queryId)
        {
            var q = DbUtil.Db.PopulateSpecialTag(queryId, DbUtil.TagTypeId_CouplesHelper).People(DbUtil.Db);
            var q1 = EliminateCoupleDoublets(q);
            var q2 = from p in q1
                         // get spouse if in the query
                     let altaddr = p.Family.FamilyExtras.SingleOrDefault(ee => ee.FamilyId == p.FamilyId && ee.Field == "MailingAddress").Data
                     let altcouple = p.Family.FamilyExtras.SingleOrDefault(ee => (ee.FamilyId == p.FamilyId) && ee.Field == "CoupleName" && p.SpouseId != null).Data
                     let spouse = q.SingleOrDefault(sp => sp.PeopleId == p.SpouseId)
                     select new MailingInfo
                     {
                         MailingAddress = altaddr,
                         CoupleName = altcouple,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         LabelName = (spouse == null ? (UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name) :
                             (p.Family.HeadOfHouseholdId == p.PeopleId ?
                                 (UseTitles ? (p.TitleCode != null ? p.TitleCode + " and Mrs. " + p.Name : "Mr. and Mrs. " + p.Name)
                                             : (p.PreferredName + " and " + spouse.PreferredName + " " + p.LastName + (p.SuffixCode.Length > 0 ? ", " + p.SuffixCode : ""))) :
                                 (UseTitles ? (spouse.TitleCode != null ? spouse.TitleCode + " and Mrs. " + spouse.Name : "Mr. and Mrs. " + spouse.Name)
                                             : (spouse.PreferredName + " and " + p.PreferredName + " " + spouse.LastName + (spouse.SuffixCode.Length > 0 ? ", " + spouse.SuffixCode : ""))))),
                         LastName = p.LastName,
                         Name2 = p.Name2,
                         CellPhone = p.CellPhone,
                         HomePhone = p.HomePhone,
                         PeopleId = p.PeopleId
                     };
            q2 = ApplySort(q2, sortExpression);
            return q2;
        }

        public IQueryable<MailingInfo> ApplySort(IQueryable<MailingInfo> query, string sortExpression)
        {
            switch (sortExpression)
            {
                case "Name":
                    return query.OrderBy(mi => mi.Name2);
                case "Zip":
                    return query.OrderBy(mi => mi.Zip);
                //break;
                default:
                    break;
            }
            return query;
        }
        public IQueryable<Person> ApplySort(IQueryable<Person> query, string sortExpression)
        {
            switch (sortExpression)
            {
                case "Name":
                    return query.OrderBy(mi => mi.Name2);
                case "Zip":
                    return query.OrderBy(mi => mi.PrimaryZip);
                //break;
                default:
                    break;
            }
            return query;
        }

        public EpplusResult FetchExcelCouplesBoth(Guid queryId, int maximumRows)
        {
            var q = DbUtil.Db.PopulateSpecialTag(queryId, DbUtil.TagTypeId_CouplesHelper).People(DbUtil.Db);
            var q1 = EliminateCoupleDoublets(q);
            var q2 = from p in q1
                         // get spouse if in the query
                     let altaddr = p.Family.FamilyExtras.SingleOrDefault(ee => ee.FamilyId == p.FamilyId && ee.Field == "MailingAddress").Data
                     let altcouple = p.Family.FamilyExtras.FirstOrDefault(ee => ee.FamilyId == p.PeopleId && ee.Field == "CoupleName" && p.SpouseId != null).Data
                     let spouse = q.SingleOrDefault(sp => sp.PeopleId == p.SpouseId)
                     select new
                     {
                         PeopleId = p.PeopleId,
                         LabelName = (spouse == null ? (UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name) :
                             (p.Family.HeadOfHouseholdId == p.PeopleId ?
                                 (UseTitles ? (p.TitleCode != null ? p.TitleCode + " and Mrs. " + p.Name : "Mr. and Mrs. " + p.Name)
                                             : (p.PreferredName + " and " + spouse.PreferredName + " " + p.LastName + (p.SuffixCode.Length > 0 ? ", " + p.SuffixCode : ""))) :
                                 (UseTitles ? (spouse.TitleCode != null ? spouse.TitleCode + " and Mrs. " + spouse.Name : "Mr. and Mrs. " + spouse.Name)
                                             : (spouse.PreferredName + " and " + p.PreferredName + " " + spouse.LastName + (spouse.SuffixCode.Length > 0 ? ", " + spouse.SuffixCode : ""))))),
                         FirstName = p.PreferredName,
                         FirstNameSpouse = spouse != null ? spouse.PreferredName : "",
                         LastName = p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip.FmtZip(),
                         Email = p.EmailAddress,
                         EmailSpouse = spouse != null ? spouse.EmailAddress : "",
                         HomePhone = p.Family.HomePhone.FmtFone(),
                         MemberStatus = p.MemberStatus.Description,
                         Employer = p.EmployerOther,
                         MailingAddress = altaddr,
                         CoupleName = altcouple,
                     };
            return q2.Take(maximumRows).ToDataTable().ToExcel("CouplesBoth.xlsx");
        }

        public EpplusResult FetchExcelCouplesEither(Guid queryId, int maximumRows)
        {
            var q = DbUtil.Db.PopulateSpecialTag(queryId, DbUtil.TagTypeId_CouplesHelper).People(DbUtil.Db);
            var q1 = EliminateCoupleDoublets(q);
            var q2 = DbUtil.Db.Setting("CouplesEitherHusbandFirst", "false").ToBool()
                ? from p in q1
                  let spouse = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == p.SpouseId)
                  let him = p.GenderId == 1 || spouse == null ? p : spouse
                  let her = p.GenderId == 1 || spouse == null ? spouse : p
                  select new
                  {
                      p.PeopleId,
                      LabelName = (spouse == null ? (UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name) :
                          (p.Family.HeadOfHouseholdId == p.PeopleId ?
                              (UseTitles ? (p.TitleCode != null ? p.TitleCode + " and Mrs. " + p.Name : "Mr. and Mrs. " + p.Name)
                                          : (p.PreferredName + " and " + spouse.PreferredName + " " + p.LastName + (p.SuffixCode.Length > 0 ? ", " + p.SuffixCode : ""))) :
                              (UseTitles ? (spouse.TitleCode != null ? spouse.TitleCode + " and Mrs. " + spouse.Name : "Mr. and Mrs. " + spouse.Name)
                                          : (spouse.PreferredName + " and " + p.PreferredName + " " + spouse.LastName + (spouse.SuffixCode.Length > 0 ? ", " + spouse.SuffixCode : ""))))),
                      FirstName = him.PreferredName,
                      FirstNameSpouse = her.PreferredName,
                      p.LastName,
                      Address = p.PrimaryAddress,
                      Address2 = p.PrimaryAddress2,
                      City = p.PrimaryCity,
                      State = p.PrimaryState,
                      Zip = p.PrimaryZip.FmtZip(),
                      Email = p.EmailAddress,
                      EmailSpouse = spouse.EmailAddress,
                      MemberStatus = p.MemberStatus.Description,
                      Employer = p.EmployerOther,
                      HomePhone = p.HomePhone.FmtFone(),
                      CellPhone = p.CellPhone.FmtFone(),
                      WorkPhone = p.WorkPhone.FmtFone(),
                  }
                : from p in q1
                  let spouse = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == p.SpouseId)
                  select new
                  {
                      p.PeopleId,
                      LabelName = (spouse == null ? (UseTitles ? (p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name) : p.Name) :
                          (p.Family.HeadOfHouseholdId == p.PeopleId ?
                              (UseTitles ? (p.TitleCode != null ? p.TitleCode + " and Mrs. " + p.Name : "Mr. and Mrs. " + p.Name)
                                          : (p.PreferredName + " and " + spouse.PreferredName + " " + p.LastName + (p.SuffixCode.Length > 0 ? ", " + p.SuffixCode : ""))) :
                              (UseTitles ? (spouse.TitleCode != null ? spouse.TitleCode + " and Mrs. " + spouse.Name : "Mr. and Mrs. " + spouse.Name)
                                          : (spouse.PreferredName + " and " + p.PreferredName + " " + spouse.LastName + (spouse.SuffixCode.Length > 0 ? ", " + spouse.SuffixCode : ""))))),
                      FirstName = p.PreferredName,
                      FirstNameSpouse = spouse.PreferredName,
                      p.LastName,
                      Address = p.PrimaryAddress,
                      Address2 = p.PrimaryAddress2,
                      City = p.PrimaryCity,
                      State = p.PrimaryState,
                      Zip = p.PrimaryZip.FmtZip(),
                      Email = p.EmailAddress,
                      EmailSpouse = spouse.EmailAddress,
                      MemberStatus = p.MemberStatus.Description,
                      Employer = p.EmployerOther,
                      HomePhone = p.HomePhone.FmtFone(),
                      CellPhone = p.CellPhone.FmtFone(),
                      WorkPhone = p.WorkPhone.FmtFone(),
                  };
            return q2.Take(maximumRows).ToDataTable().ToExcel("CouplesEither.xlsx");
        }

        public EpplusResult FetchExcelFamily(Guid queryId, int maximumRows)
        {
            var qp = DbUtil.Db.PeopleQuery(queryId);

            var q = from f in DbUtil.Db.Families
                    where qp.Any(p => p.FamilyId == f.FamilyId)
                    select f.People.Single(fm => fm.PeopleId == f.HeadOfHouseholdId);
            if (UseMailFlags)
            {
                q = FilterMailFlags(q);
            }

            return (from h in q
                    let spouse = DbUtil.Db.People.SingleOrDefault(sp => sp.PeopleId == h.SpouseId)
                    where h.DeceasedDate == null
                    let altaddr = h.Family.FamilyExtras.SingleOrDefault(ee => ee.FamilyId == h.FamilyId && ee.Field == "MailingAddress").Data
                    let altcouple = h.Family.FamilyExtras.SingleOrDefault(ee => (ee.FamilyId == h.FamilyId) && ee.Field == "CoupleName" && h.SpouseId != null).Data
                    select new
                    {
                        LabelName = (h.Family.CoupleFlag == 1 ? (UseTitles ? (h.TitleCode != null ? h.TitleCode + " and Mrs. " + h.Name
                                                                                                   : "Mr. and Mrs. " + h.Name)
                                                                            : h.PreferredName + " and " + spouse.PreferredName + " " + h.LastName + (h.SuffixCode.Length > 0 ? ", " + h.SuffixCode : "")) :
                                     h.Family.CoupleFlag == 2 ? ("The " + h.Name + " Family") :
                                     h.Family.CoupleFlag == 3 ? ("The " + h.Name + " Family") :
                                     h.Family.CoupleFlag == 4 ? (h.Name + " & Family") :
                                     UseTitles ? (h.TitleCode != null ? h.TitleCode + " " + h.Name : h.Name) : h.Name),
                        Name = h.Name,
                        LastName = h.LastName,
                        Address = h.PrimaryAddress,
                        Address2 = h.PrimaryAddress2,
                        CityStateZip = Util.FormatCSZ4(h.PrimaryCity, h.PrimaryState, h.PrimaryZip),
                        City = h.PrimaryCity,
                        State = h.PrimaryState,
                        Zip = h.PrimaryZip,
                        Email = h.EmailAddress,
                        SpouseEmail = spouse.EmailAddress,
                        CellPhone = h.CellPhone.FmtFone(),
                        SpouseCell = spouse.CellPhone.FmtFone(),
                        MailingAddress = altaddr,
                        CoupleName = altcouple,
                    }).Take(maximumRows).ToDataTable().ToExcel("Families.xlsx");
        }

        public EpplusResult FetchExcelParents(Guid queryId, int maximumRows)
        {
            var q = DbUtil.Db.PeopleQuery(queryId);
            if (UseMailFlags)
            {
                q = FilterMailFlags(q);
            }

            var q2 = from p in q
                     where p.DeceasedDate == null
                     let hohemail = p.Family.HeadOfHousehold.EmailAddress
                     select new
                     {
                         LabelName = (p.PositionInFamilyId == 30 ? ("Parents of " + p.Name) : p.TitleCode != null ? p.TitleCode + " " + p.Name : p.Name),
                         Name = p.Name,
                         LastName = p.LastName,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         CityStateZip = Util.FormatCSZ4(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                         City = p.PrimaryCity,
                         State = p.PrimaryState,
                         Zip = p.PrimaryZip,
                         ParentEmail = (p.PositionInFamilyId == 30 ?
                            (hohemail != null && hohemail != "" ?
                                hohemail
                                : p.Family.HeadOfHouseholdSpouse.EmailAddress)
                            : p.EmailAddress)
                     };
            return q2.Take(maximumRows).ToDataTable().ToExcel("Parents.xlsx");
        }
        public class TaggedPersonInfo : PersonInfo
        {
            public bool HasTag { get; set; }
        }

        public class MailingInfo
        {
            public int PeopleId { get; set; }
            public string LabelName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Name2 { get; set; }
            public string Address { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public string CSZ
            {
                get { return Util.FormatCSZ4(City, State, Zip); }
            }
            public string CellPhone { get; set; }
            public string HomePhone { get; set; }
            public string CoupleName { get; set; }
            public string MailingAddress { get; set; }
        }
        public class PersonInfo
        {
            private enum PhoneType
            {
                Home, Cell, Work
            }
            public int PeopleId { get; set; }
            public string MemberStatus { get; set; }
            public string Name { get; set; }
            public string Name2 { get; set; }
            //public DateTime? JoinDate { get; set; }
            public int? BirthYear { get; set; }
            public int? BirthMon { get; set; }
            public int? BirthDay { get; set; }
            public string BirthDate => Person.FormatBirthday(BirthYear, BirthMon, BirthDay, PeopleId);
            public string Address { get; set; }
            public string Address2 { get; set; }
            public string CityStateZip { get; set; }
            public string Email { get; set; }
            public string Age { get; set; }
            private int _PhonePref;
            public int PhonePref { set { _PhonePref = value; } }
            private string PhoneFmt(string prefix, PhoneType type, string number)
            {
                var s = number.FmtFone(type + " ");
                if ((type == PhoneType.Home && _PhonePref == 10)
                    || (type == PhoneType.Cell && _PhonePref == 20)
                    || (type == PhoneType.Work && _PhonePref == 30))
                {
                    return number.FmtFone("*" + prefix + " ");
                }

                return number.FmtFone(prefix + " ");
            }
            private List<string> _Phones = new List<string>();
            public List<string> Phones
            {
                get { return _Phones; }
            }
            private string _CellPhone;
            public string CellPhone
            {
                set
                {
                    if (value.HasValue())
                    {
                        _CellPhone = PhoneFmt(string.Empty, PhoneType.Cell, value);
                        _Phones.Add(PhoneFmt("C", PhoneType.Cell, value));
                    }
                }
                get { return _CellPhone; }
            }

            private string _HomePhone;
            public string HomePhone
            {
                set
                {
                    if (value.HasValue())
                    {
                        _HomePhone = PhoneFmt(string.Empty, PhoneType.Home, value);
                        _Phones.Add(PhoneFmt("H", PhoneType.Home, value));
                    }
                }
                get { return _HomePhone; }
            }
            private string _WorkPhone;
            public string WorkPhone
            {
                set
                {
                    if (value.HasValue())
                    {
                        _WorkPhone = PhoneFmt(string.Empty, PhoneType.Work, value);
                        _Phones.Add(PhoneFmt("W", PhoneType.Work, value));
                    }
                }
                get { return _WorkPhone; }
            }
            public string BFTeacher { get; set; }
            public int? BFTeacherId { get; set; }
            public bool Deceased { get; set; }

        }
    }
}
