using CmsData.Codes;
using CmsData.View;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UtilityExtensions;

namespace CmsData.API
{
    public class APIContribution
    {
        private readonly CMSDataContext _db;

        public APIContribution(CMSDataContext db)
        {
            _db = db;
        }

        public string PostContribution(int PeopleId, decimal Amount, int FundId, string desc, string date, int? type, string checkno)
        {
            try
            {
                var p = _db.LoadPersonById(PeopleId);
                if (p == null)
                {
                    throw new Exception("no person");
                }

                var c = p.PostUnattendedContribution(_db, Amount, FundId, desc);
                DateTime dt;
                if (date.DateTryParse(out dt))
                {
                    c.ContributionDate = dt;
                }

                if (type.HasValue)
                {
                    c.ContributionTypeId = type.Value;
                }

                if (checkno.HasValue())
                {
                    c.CheckNo = (checkno ?? "").Trim().Truncate(20);
                }

                _db.SubmitChanges();
                return $@"<PostContribution status=""ok"" id=""{c.ContributionId}"" />";
            }
            catch (Exception ex)
            {
                return $@"<PostContribution status=""error"">{ex.Message}</PostContribution>";
            }
        }

        public string Contributions(int PeopleId, int Year)
        {
            try
            {
                var p = _db.LoadPersonById(PeopleId);
                if (p == null)
                {
                    throw new Exception("no person");
                }

                if (p.PositionInFamilyId != PositionInFamily.PrimaryAdult)
                {
                    throw new Exception("not a primary adult");
                }

                var frdt = new DateTime(Year, 1, 1);
                var todt = new DateTime(Year, 12, 31);
                var f = GetFamilyContributions(frdt, todt, p);
                return SerializeContributions(f);
            }
            catch (Exception ex)
            {
                return $@"<PostContribution status=""error"">{ex.Message}</PostContribution>";
            }
        }

        private FamilyContributions GetFamilyContributions(DateTime frdt, DateTime todt, Person p)
        {
            var f = new FamilyContributions
            {
                status = "ok",
                Contributors = (from ci in Contributors(_db, frdt, todt, 0, 0, p.FamilyId, funds: null, noaddressok: true, useMinAmt: false)
                                select new Contributor
                                {
                                    Name = ci.Name,
                                    Type = ci.Joint ? "Joint" : "Individual",
                                    Contributions = (from c in Contributions(_db, ci, frdt, todt, null)
                                                     select new Contribution
                                                     {
                                                         Amount = c.ContributionAmount ?? 0,
                                                         Date = c.ContributionDate.ToString2("d"),
                                                         Description = c.Description,
                                                         CheckNo = c.CheckNo,
                                                         Fund = c.FundName,
                                                         Name = c.Name,
                                                     }).ToList()
                                }).ToList()
            };
            return f;
        }

        private static string SerializeContributions(FamilyContributions f)
        {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(FamilyContributions));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            xs.Serialize(sw, f, ns);
            return sw.ToString();
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static IEnumerable<ContributorInfo> Contributors(CMSDataContext db,
            DateTime fromDate, DateTime toDate, int PeopleId, int? SpouseId, int FamilyId, List<int> funds, bool noaddressok, bool useMinAmt,
            string startswith = null, string sort = null, bool singleStatement = false, int? tagid = null, bool excludeelectronic = false)
        {
            var MinAmt = db.Setting("MinContributionAmount", "5").ToDecimal();
            if (!useMinAmt)
            {
                MinAmt = 0;
            }

            var endswith = "";
            if (startswith != null && startswith.Contains("-"))
            {
                var a = startswith.SplitStr("-", 2);
                startswith = a[0];
                endswith = a[1];
            }
            var q = from p in db.Donors(fromDate, toDate, PeopleId, SpouseId, FamilyId, noaddressok, tagid, funds.JoinInts(","))
                    select p;

            if (startswith.HasValue() && !endswith.HasValue())
            {
                q = from p in q
                    where p.LastName.StartsWith(startswith)
                    select p;
            }
            else if (startswith.HasValue() && endswith.HasValue())
            {
                q = from p in q
                        // ReSharper disable StringCompareToIsCultureSpecific
                    where (p.LastName.CompareTo(startswith) >= 0 && p.LastName.CompareTo(endswith) < 0) || SqlMethods.Like(p.LastName, endswith + "%")
                    select p;
            }

            if (sort == "zip")
            {
                q = from p in q
                    orderby p.PrimaryZip, p.FamilyId, p.PositionInFamilyId, p.HohFlag, p.Age
                    select p;
            }
            else if (sort == "name")
            {
                q = from p in q
                    orderby p.LastName, p.FamilyId, p.PositionInFamilyId, p.HohFlag, p.Age
                    select p;
            }
            else
            {
                q = from p in q
                    orderby p.FamilyId, p.PositionInFamilyId, p.HohFlag, p.Age
                    select p;
            }

            if (singleStatement)
            {
                var familylist = q.ToList();
                if (familylist.Any(m => m.DeceasedDate != null && m.ContributionOptionsId == 2))
                {
                    return GetInfo(familylist);
                }
            }

            const int NOTSPEC = 0;
            const int NONE = StatementOptionCode.None;
            const int JOINT = StatementOptionCode.Joint;
            const int INDIV = StatementOptionCode.Individual;

            if (MinAmt > 0)
            {
                q = from p in q
                    let option = (p.ContributionOptionsId ?? NOTSPEC) == NOTSPEC
                        ? (p.SpouseId > 0 && (p.SpouseContributionOptionsId ?? NOTSPEC) != INDIV ? JOINT : INDIV)
                        : p.ContributionOptionsId
                    where option != NONE || noaddressok
                    where (option == INDIV && (p.Amount >= MinAmt))
                          || (option == JOINT && p.HohFlag == 1 && ((p.Amount + p.SpouseAmount) >= MinAmt))
                    where p.ElectronicStatement == false || excludeelectronic == false
                    select p;
            }
            else
            {
                q = from p in q
                    let option =
                        (p.ContributionOptionsId ?? NOTSPEC) == NOTSPEC
                            ? (p.SpouseId > 0 && (p.SpouseContributionOptionsId ?? 0) != INDIV ? JOINT : INDIV)
                            : p.ContributionOptionsId
                    where option != NONE || noaddressok
                    where p.ElectronicStatement == false || excludeelectronic == false
                    where
                        (option == INDIV && (p.Amount > 0 || p.GiftInKind == true)) // GiftInKind = NonTaxDeductible Fund or Pledge OR GiftInkind
                        || (option == JOINT && p.HohFlag == 1 && ((p.Amount + p.SpouseAmount) > 0 || p.GiftInKind == true))
                    select p;
            }

            IEnumerable<ContributorInfo> q2;
            if (db.Setting("NoTitlesOnStatements", "false").ToBool())
            {
                q2 = from p in q
                     let option = (p.ContributionOptionsId ?? 0) == 0
                         ? (p.SpouseId > 0 && (p.SpouseContributionOptionsId ?? 0) != 1 ? 2 : 1)
                         : p.ContributionOptionsId
                     let name =
                         option == 1
                             ? p.Name
                             : (p.SpouseId == null
                                 ? p.Name
                                 : (p.HohFlag == 1
                                     ? p.CoupleName ?? (p.Name + " and " + p.SpouseNameWithoutNickname)
                                     : p.CoupleName ?? (p.SpouseNameWithoutNickname + " and " + p.Name)))
                     select new ContributorInfo
                     {
                         Name = name,
                         MailingAddress = MailingAddress(p),
                         PeopleId = p.PeopleId,
                         SpouseID = p.SpouseId,
                         DeacesedDate = p.DeceasedDate,
                         FamilyId = p.FamilyId,
                         Age = p.Age,
                         FamilyPositionId = p.PositionInFamilyId,
                         hohInd = p.HohFlag,
                         Joint = option == 2,
                         CampusId = p.CampusId,
                     };
            }
            else
            {
                q2 = GetInfo(q);
            }

            return q2;
        }

        private static IEnumerable<ContributorInfo> GetInfo(IEnumerable<Donor> q)
        {
            var q2 = from p in q
                     let option = (p.ContributionOptionsId ?? 0) == 0
                         ? (p.SpouseId > 0 && (p.SpouseContributionOptionsId ?? 0) != 1 ? 2 : 1)
                         : p.ContributionOptionsId
                     let name =
                         (option == 1
                             ? (p.Title != null ? p.Title + " " + p.Name : p.Name)
                             : (p.SpouseId == null
                                 ? (p.Title != null ? p.Title + " " + p.Name : p.Name)
                                 : p.CoupleName ?? (p.HohFlag == 1
                                     ? ((p.Title ?? "") != ""
                                         ? p.Title + " and Mrs. " + p.Name
                                         : "Mr. and Mrs. " + p.Name)
                                     : ((p.SpouseTitle ?? "") != ""
                                         ? p.SpouseTitle + " and Mrs. " + p.SpouseName
                                         : "Mr. and Mrs. " + p.SpouseName))))
                         + ((p.Suffix ?? "") == "" ? "" : ", " + p.Suffix)
                     select new ContributorInfo
                     {
                         Name = name,
                         MailingAddress = MailingAddress(p),
                         PeopleId = p.PeopleId,
                         SpouseID = p.SpouseId,
                         DeacesedDate = p.DeceasedDate,
                         FamilyId = p.FamilyId,
                         Age = p.Age,
                         FamilyPositionId = p.PositionInFamilyId,
                         hohInd = p.HohFlag,
                         Joint = option == 2,
                         CampusId = p.CampusId,
                     };
            return q2;
        }

        private static string MailingAddress(Donor c)
        {
            if (c.MailingAddress.HasValue())
            {
                return c.MailingAddress;
            }

            var sb = new StringBuilder();
            sb.AppendLine(c.PrimaryAddress);
            if (c.PrimaryAddress2.HasValue())
            {
                sb.AppendLine(c.PrimaryAddress2);
            }

            sb.AppendLine(Util.FormatCSZ4(c.PrimaryCity, c.PrimaryState, c.PrimaryZip));
            return sb.ToString();
        }

        public static IEnumerable<NormalContribution> Contributions(CMSDataContext db, ContributorInfo ci, DateTime fromDate, DateTime toDate, List<int> funds)
        {
            var q = from c in
                db.NormalContributions(ci.PeopleId, ci.SpouseID, ci.Joint, fromDate, toDate, funds?.JoinInts(","))
                    orderby c.ContributionDate
                    select c;
            return q;
        }

        public static IEnumerable<NonTaxContribution> NonTaxItems(CMSDataContext db, ContributorInfo ci, DateTime fromDate, DateTime toDate, List<int> funds)
        {
            var q = from c in
                db.NonTaxContributions(ci.PeopleId, ci.SpouseID, ci.Joint, fromDate, toDate, funds.JoinInts(","))
                    orderby c.ContributionDate
                    select c;
            return q;
        }

        public static IEnumerable<StockGift> StockGifts(CMSDataContext db, ContributorInfo ci, DateTime fromDate, DateTime toDate, List<int> funds)
        {
            var q = from c in
                db.StockGifts(ci.PeopleId, ci.SpouseID, ci.Joint, fromDate, toDate, funds.JoinInts(","))
                    orderby c.ContributionDate
                    select c;
            return q;
        }

        public static IEnumerable<UnitPledgeSummary> Pledges(CMSDataContext db, ContributorInfo ci, DateTime toDate, List<int> funds)
        {
            var q = from c in
                db.UnitPledgeSummary(ci.PeopleId, ci.SpouseID, ci.Joint, toDate, funds.JoinInts(","))
                    orderby c.FundName
                    select c;
            return q;
        }

        public static IEnumerable<GiftSummary> GiftSummary(CMSDataContext db, ContributorInfo ci, DateTime fromDate, DateTime toDate, List<int> funds)
        {
            var q = from c in
                db.GiftSummary(ci.PeopleId, ci.SpouseID, ci.Joint, fromDate, toDate, funds.JoinInts(","))
                    orderby c.FundName
                    select c;
            return q;
        }

        public static IEnumerable<GiftsInKind> GiftsInKind(CMSDataContext db, ContributorInfo ci, DateTime fromDate, DateTime toDate, List<int> funds)
        {
            var q = from c in
                db.GiftsInKind(ci.PeopleId, ci.SpouseID, ci.Joint, fromDate, toDate, funds.JoinInts(","))
                    orderby c.ContributionDate
                    select c;
            return q;
        }
        public static int? OneTimeGiftOrgId(CMSDataContext db)
        {
            var sql = @"
               select coalesce((
                   select convert(int, Setting)
                   from dbo.Setting
                   where Id = 'OneTimeGiftOrgId'
               ) , (
                   select top 1 OrganizationId
                   from dbo.Organizations
                   where RegistrationTypeId = 8
                         and RegSettingXml.value('(/Settings/Fees/DonationFundId)[1]', 'int') is null
				   order by OrganizationId))
";
            var oid = db.Connection.ExecuteScalar(sql) as int?;
            return oid;
        }

        [Serializable]
        public class FamilyContributions
        {
            [XmlAttribute]
            public string status { get; set; }

            public List<Contributor> Contributors { get; set; }
        }

        [Serializable]
        public class Contributor
        {
            [XmlAttribute]
            public string Type { get; set; }

            public string Name { get; set; }
            public List<Contribution> Contributions { get; set; }
        }

        [Serializable]
        public class Contribution
        {
            public int ContributionId { get; set; }
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public string Date { get; set; }
            public decimal Amount { get; set; }
            public string Fund { get; set; }

            [DefaultValue("")]
            public string Description { get; set; }

            [DefaultValue("")]
            public string CheckNo { get; set; }
        }
    }

    public class ContributorInfo
    {
        public string Name { get; set; }
        public string MailingAddress { get; set; }
        public int PeopleId { get; set; }
        public int? SpouseID { get; set; }
        public int FamilyId { get; set; }
        public DateTime? DeacesedDate { get; set; }
        public int hohInd { get; set; }
        public int FamilyPositionId { get; set; }
        public int? Age { get; set; }
        public bool Joint { get; set; }
        public int? CampusId { get; set; }
    }

    [Serializable]
    public class ContributionInfo
    {
        public int PeopleId { get; set; }
        public string Name { get; set; }
        public DateTime ContributionDate { get; set; }
        public decimal ContributionAmount { get; set; }
        public string BundleType { get; set; }
        public string Fund { get; set; }
        public string Description { get; set; }
        public string CheckNo { get; set; }
        public int BundleId { get; set; }
        public int ContributionId { get; set; }
        public string ContributionType { get; set; }
        public int ContributionTypeId { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public bool Pledge { get; set; }
        public bool NonTaxDed { get; set; }
        public int? FamilyId { get; set; }
        public string MemberStatus { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public bool ReturnedReversed => ContributionTypeCode.ReturnedReversedTypes.Contains(ContributionTypeId);
        public bool Recorded => StatusId == ContributionStatusCode.Recorded;
        public bool CanReturnReverse => Recorded && !ReturnedReversed;

        public bool NotIncluded
        {
            get
            {
                if (StatusId < 0)
                {
                    return true;
                }

                return StatusId != ContributionStatusCode.Recorded
                       || ContributionTypeCode.ReturnedReversedTypes.Contains(ContributionTypeId);
            }
        }
    }
}
