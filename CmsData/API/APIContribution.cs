using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData.API
{
    public class APIContribution
    {
        private CMSDataContext Db;

        public APIContribution(CMSDataContext Db)
        {
            this.Db = Db;
        }

        public string PostContribution(int PeopleId, decimal Amount, int FundId, string desc, string date, int? type, string checkno)
        {
            try
            {
                var p = Db.LoadPersonById(PeopleId);
                if (p == null)
                    throw new Exception("no person");
                var c = p.PostUnattendedContribution(Db, Amount, FundId, desc);
                DateTime dt;
                if (date.DateTryParse(out dt))
                    c.ContributionDate = dt;
                if (type.HasValue)
                    c.ContributionTypeId = type.Value;
                if (checkno.HasValue())
                    c.CheckNo = (checkno ?? "").Trim().Truncate(20);
                Db.SubmitChanges();
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
                var p = Db.LoadPersonById(PeopleId);
                if (p == null)
                    throw new Exception("no person");
                if (p.PositionInFamilyId != PositionInFamily.PrimaryAdult)
                    throw new Exception("not a primary adult");
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
                Contributors = (from ci in contributors(Db, frdt, todt, 0, 0, p.FamilyId, noaddressok: true, useMinAmt: false)
                                select new Contributor
                                {
                                    Name = ci.Name,
                                    Type = ci.Joint ? "Joint" : "Individual",
                                    Contributions = (from c in contributions(Db, ci, frdt, todt)
                                                     select new Contribution
                                                     {
                                                         Amount = c.ContributionAmount,
                                                         Date = c.ContributionDate.ToShortDateString(),
                                                         Description = c.Description,
                                                         CheckNo = c.CheckNo,
                                                         Fund = c.Fund,
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
        public static IEnumerable<ContributorInfo> contributors(CMSDataContext Db,
            DateTime fromDate, DateTime toDate, int PeopleId, int? SpouseId, int FamilyId, bool noaddressok, bool useMinAmt,
            string startswith = null, string sort = null, bool singleStatement = false, int? tagid = null, bool excludeelectronic = false)
        {
            var MinAmt = Db.Setting("MinContributionAmount", "5").ToDecimal();
            if (!useMinAmt)
                MinAmt = 0;

            var endswith = "";
            if (startswith != null && startswith.Contains("-"))
            {
                var a = startswith.SplitStr("-", 2);
                startswith = a[0];
                endswith = a[1];
            }
            var q = from p in Db.Contributors(fromDate, toDate, PeopleId, SpouseId, FamilyId, noaddressok, tagid)
                    select p;

            if (startswith.HasValue() && !endswith.HasValue())
                q = from p in q
                    where p.LastName.StartsWith(startswith)
                    select p;
            else if (startswith.HasValue() && endswith.HasValue())
                q = from p in q
                    // ReSharper disable StringCompareToIsCultureSpecific
                    where (p.LastName.CompareTo(startswith) >= 0 && p.LastName.CompareTo(endswith) < 0) || SqlMethods.Like(p.LastName, endswith + "%")
                    select p;

            if (sort == "zip")
                q = from p in q
                    orderby p.PrimaryZip, p.FamilyId, p.PositionInFamilyId, p.HohFlag, p.Age
                    select p;
            else if (sort == "name")
                q = from p in q
                    orderby p.LastName, p.FamilyId, p.PositionInFamilyId, p.HohFlag, p.Age
                    select p;
            else
                q = from p in q
                    orderby p.FamilyId, p.PositionInFamilyId, p.HohFlag, p.Age
                    select p;

            if (singleStatement)
            {
                var familylist = q.ToList();
                if (familylist.Any(m => m.DeceasedDate != null && m.ContributionOptionsId == 2))
                    return GetInfo(familylist);
            }

            const int NOTSPEC = 0;
            const int NONE = StatementOptionCode.None;
            const int JOINT = StatementOptionCode.Joint;
            const int INDIV = StatementOptionCode.Individual;

            if (MinAmt > 0)
                q = from p in q
                    let option = (p.ContributionOptionsId ?? NOTSPEC) == NOTSPEC
                        ? (p.SpouseId > 0 && (p.SpouseContributionOptionsId ?? NOTSPEC) != INDIV ? JOINT : INDIV)
                        : p.ContributionOptionsId
                    where option != NONE || noaddressok
                    where (option == INDIV && (p.Amount >= MinAmt))
                          || (option == JOINT && p.HohFlag == 1 && ((p.Amount + p.SpouseAmount) >= MinAmt))
                    where p.ElectronicStatement == false || excludeelectronic == false
                    select p;
            else
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

            IEnumerable<ContributorInfo> q2 = null;
            if (Db.Setting("NoTitlesOnStatements", "false").ToBool())
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
                                     ? p.CoupleName ?? (p.Name + " and " + p.SpouseName)
                                     : p.CoupleName ?? (p.SpouseName + " and " + p.Name)))
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
            else
                q2 = GetInfo(q);

            return q2;
        }

        private static IEnumerable<ContributorInfo> GetInfo(IEnumerable<View.Contributor> q)
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

        private static string MailingAddress(View.Contributor c)
        {
            if (c.MailingAddress.HasValue())
                return c.MailingAddress;
            var sb = new StringBuilder();
            sb.AppendLine(c.PrimaryAddress);
            if (c.PrimaryAddress2.HasValue())
                sb.AppendLine(c.PrimaryAddress2);
            sb.AppendLine(Util.FormatCSZ4(c.PrimaryCity, c.PrimaryState, c.PrimaryZip));
            return sb.ToString();
        }

        public static IEnumerable<ContributionInfo> contributions(CMSDataContext Db, ContributorInfo ci, DateTime fromDate, DateTime toDate)
        {
            var q = from c in Db.Contributions
                    where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                    where c.ContributionTypeId != ContributionTypeCode.GiftInKind
                    where c.ContributionTypeId != ContributionTypeCode.Stock
                    where c.ContributionStatusId == ContributionStatusCode.Recorded
                    where c.ContributionDate >= fromDate
                    where c.ContributionDate.Value.Date <= toDate.Date
                    where c.PeopleId == ci.PeopleId || (ci.Joint && c.PeopleId == ci.SpouseID)
                    where !(c.ContributionFund.NonTaxDeductible ?? false)
                    where !ContributionTypeCode.NonTaxTypes.Contains(c.ContributionTypeId)
                    orderby c.ContributionDate
                    select new ContributionInfo
                    {
                        ContributionId = c.ContributionId,
                        ContributionAmount = c.ContributionAmount ?? 0,
                        ContributionDate = c.ContributionDate ?? SqlDateTime.MinValue.Value,
                        Fund = c.ContributionFund.FundName,
                        CheckNo = c.CheckNo,
                        Name = c.Person.Name,
                        Description = c.ContributionDesc
                    };

            return q;
        }

        public static IEnumerable<ContributionInfo> NonTaxItems(CMSDataContext Db, ContributorInfo ci, DateTime fromDate, DateTime toDate)
        {
            var q = from c in Db.Contributions
                    where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                    where c.ContributionTypeId != ContributionTypeCode.Stock
                    where c.ContributionTypeId != ContributionTypeCode.GiftInKind
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where c.ContributionStatusId == ContributionStatusCode.Recorded
                    where c.ContributionDate >= fromDate
                    where c.ContributionDate.Value.Date <= toDate.Date
                    where c.PeopleId == ci.PeopleId || (ci.Joint && c.PeopleId == ci.SpouseID)
                    where c.ContributionFund.NonTaxDeductible == true || ContributionTypeCode.NonTaxTypes.Contains(c.ContributionTypeId)
                    where (c.PledgeFlag ?? false) == false
                    orderby c.ContributionDate
                    select c;
            return from c in q
                   select new ContributionInfo
                   {
                       ContributionId = c.ContributionId,
                       ContributionAmount = c.ContributionAmount ?? 0,
                       ContributionDate = c.ContributionDate ?? SqlDateTime.MinValue.Value,
                       Fund = c.ContributionFund.FundName,
                       CheckNo = c.CheckNo,
                       Name = c.Person.Name,
                       Description = c.ContributionDesc,
                       ContributionTypeId = c.ContributionTypeId,
                   };
        }

        public static IEnumerable<ContributionInfo> StockGifts(CMSDataContext db, ContributorInfo ci, DateTime fromDate, DateTime toDate)
        {
            var q = from c in db.Contributions
                    where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                    where c.ContributionTypeId == ContributionTypeCode.Stock
                    where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                    where c.ContributionStatusId == ContributionStatusCode.Recorded
                    where c.ContributionDate >= fromDate
                    where c.ContributionDate.Value.Date <= toDate.Date
                    where c.PeopleId == ci.PeopleId || (ci.Joint && c.PeopleId == ci.SpouseID)
                    orderby c.ContributionDate
                    select new ContributionInfo
                    {
                        ContributionId = c.ContributionId,
                        ContributionAmount = c.ContributionAmount ?? 0,
                        ContributionDate = c.ContributionDate ?? SqlDateTime.MinValue.Value,
                        Fund = c.ContributionFund.FundName,
                        CheckNo = c.CheckNo,
                        Name = c.Person.Name,
                        Description = c.ContributionDesc
                    };
            return q;
        }

        public static IEnumerable<PledgeSummaryInfo> pledges(CMSDataContext db, ContributorInfo ci, DateTime toDate)
        {
            var PledgeExcludes = new int[]
            {
                ContributionTypeCode.Reversed,
            };

            var showPledgeIfMet = db.Setting("ShowPledgeIfMet", "true").ToBool();

            var qp = from p in db.Contributions
                     where p.PeopleId == ci.PeopleId || (ci.Joint && p.PeopleId == ci.SpouseID)
                     where p.ContributionTypeId == ContributionTypeCode.Pledge
                     where p.ContributionStatusId.Value != ContributionStatusCode.Reversed
                     where p.ContributionFund.FundStatusId == 1 // active
                     where p.ContributionDate.Value.Date <= toDate.Date
                     group p by p.FundId into g
                     select new {FundId = g.Key, Fund = g.First().ContributionFund.FundName, Total = g.Sum(p => p.ContributionAmount)};
            var qc = from c in db.Contributions
                     where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                     where c.ContributionTypeId != ContributionTypeCode.GiftInKind
                     where !Codes.ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                     where c.ContributionStatusId == ContributionStatusCode.Recorded
                     where c.PeopleId == ci.PeopleId || (ci.Joint && c.PeopleId == ci.SpouseID)
                     where c.ContributionTypeId != ContributionTypeCode.Pledge
                     where c.ContributionStatusId != ContributionStatusCode.Reversed
                     where c.ContributionDate.Value.Date <= toDate.Date
                     group c by c.FundId into g
                     select new {FundId = g.Key, Total = g.Sum(c => c.ContributionAmount)};
            var q = from p in qp
                    join c in qc on p.FundId equals c.FundId into items
                    from c in items.DefaultIfEmpty()
                    where (p.Total ?? 0) > (c == null ? 0 : c.Total ?? 0) || showPledgeIfMet
                    orderby p.FundId descending
                    select new PledgeSummaryInfo
                    {
                        Fund = p.Fund,
                        ContributionAmount = c.Total,
                        PledgeAmount = p.Total
                    };
            return q;
        }

        public static IEnumerable<ContributionInfo> quarterlySummary(CMSDataContext Db, ContributorInfo ci, DateTime fromDate, DateTime toDate)
        {
            int[] excludetypes = new int[]
            {
                ContributionTypeCode.ReturnedCheck,
                ContributionTypeCode.Reversed,
                ContributionTypeCode.GiftInKind,
                ContributionTypeCode.Stock,
                ContributionTypeCode.NonTaxDed,
                ContributionTypeCode.Pledge,
            };
            var q = from c in Db.Contributions
                    where !excludetypes.Contains(c.ContributionTypeId)
                    where c.ContributionStatusId == ContributionStatusCode.Recorded
                    where c.ContributionDate >= fromDate
                    where c.ContributionDate.Value.Date <= toDate.Date
                    where c.PeopleId == ci.PeopleId || (ci.Joint && c.PeopleId == ci.SpouseID)
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where (c.ContributionFund.NonTaxDeductible ?? false) == false
                    group c by c.ContributionFund.FundName into g
                    orderby g.Key
                    select new ContributionInfo
                    {
                        ContributionAmount = g.Sum(z => z.ContributionAmount ?? 0),
                        Fund = g.Key,
                    };

            return q;
        }

        public static IEnumerable<ContributionInfo> GiftsInKind(CMSDataContext Db, ContributorInfo ci, DateTime fromDate, DateTime toDate)
        {
            var q = from c in Db.Contributions
                    where c.PeopleId == ci.PeopleId || (ci.Joint && c.PeopleId == ci.SpouseID)
                    where c.ContributionTypeId == ContributionTypeCode.GiftInKind || c.ContributionTypeId == ContributionTypeCode.Stock
                    where c.ContributionStatusId.Value != ContributionStatusCode.Reversed
                    where c.ContributionDate >= fromDate
                    where c.ContributionDate.Value.Date <= toDate.Date
                    orderby c.ContributionDate
                    select new ContributionInfo
                    {
                        ContributionId = c.ContributionId,
                        ContributionDate = c.ContributionDate ?? SqlDateTime.MinValue.Value,
                        Fund = c.ContributionFund.FundName,
                        Description = c.ContributionDesc
                    };
            return q;
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

    public class PledgeSummaryInfo
    {
        public string Fund { get; set; }
        public decimal? ContributionAmount { get; set; }
        public decimal? PledgeAmount { get; set; }
        public string Description { get; set; }
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
                    return true;
                return StatusId != (int) ContributionStatusCode.Recorded
                       || ContributionTypeCode.ReturnedReversedTypes.Contains(ContributionTypeId);
            }
        }
    }
}