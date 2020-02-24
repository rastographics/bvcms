using System;

namespace CmsWeb.Models
{
    public class DonorDetail
    {
        public int? FamilyId { get; set; }
        public string Date { get; set; }
        public int? GiverId { get; set; }
        public int? CreditGiverId { get; set; }
        public string HeadName { get; set; }
        public string SpouseName { get; set; }
        public string MainFellowship { get; set; }
        public string MemberStatus { get; set; }
        public DateTime? JoinDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Pledge { get; set; }
        public string CheckNo { get; set; }
        public string ContributionDesc { get; set; }
        public int FundId { get; set; }
        public string FundName { get; set; }
        public int BundleHeaderId { get; set; }
        public string BundleType { get; set; }
        public string BundleStatus { get; set; }
        public string Addr { get; set; }
        public string Addr2 { get; set; }
        public string City { get; set; }
        public string ST { get; set; }
        public string Zip { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FamilyName { get; set; }
        public string EmailAddress { get; set; }
        public string FullAddress { get; internal set; }
        public int ContributionId { get; internal set; }
        public int? PeopleId { get; internal set; }
        public int ContributionTypeId { get; internal set; }
        public object DateX { get; internal set; }
        public decimal? PledgeAmount { get; internal set; }
        public int ContributionStatusId { get; internal set; }
        public int? CreditGiverId2 { get; internal set; }
        public int OpenPledgeFund { get; internal set; }
        public int? SpouseId { get; internal set; }
    }
}
