using CmsData;
using CmsData.Registration;
using CmsData.View;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class OrgPrevMemberModel
    {
        public Organization Organization;
        public List<string> OrgMemMemTags;
        public TransactionSummary TransactionSummary;
        public string NewGroup { get; set; }
        private int? orgId;
        private int? peopleId;

        public OrgPrevMemberModel()
        {
        }

        public OrgPrevMemberModel(int oid, int pid)
        {
            OrgId = oid;
            PeopleId = pid;
        }

        public EnrollmentTransaction PrevMember;

        private void Populate()
        {
            if (dopopulate)
            {
                dopopulate = false;
                if (!OrgId.HasValue || !PeopleId.HasValue)
                {
                    return;
                }
            }
            var i = (from et in DbUtil.Db.EnrollmentTransactions
                     where et.OrganizationId == OrgId && et.PeopleId == PeopleId
                     where et.TransactionTypeId == 5
                     orderby et.TransactionDate descending
                     select new
                     {
                         et,
                         et.Person.Name,
                         et.Organization.OrganizationName,
                         et.Organization,
                         et.SmallGroups,
                         ts = DbUtil.Db.ViewTransactionSummaries.SingleOrDefault(
                             ts => ts.RegId == et.TranId && ts.PeopleId == PeopleId && ts.OrganizationId == OrgId)
                     }
            ).FirstOrDefault();

            if (i == null)
            {
                throw new Exception($"missing PrevOrgMember at oid={OrgId}, pid={PeopleId}");
            }

            PrevMember = i.et;
            TransactionSummary = i.ts;
            this.CopyPropertiesFrom(PrevMember);
            Name = i.Name;

            AmtFee = i.ts?.IndPaid + i.ts?.IndDue;
            AmtDonation = i.ts?.IndAmt - AmtFee;
            AmtCoupon = i.ts?.TotCoupon;

            OrgName = i.OrganizationName;
            Organization = i.Organization;
            OrgMemMemTags = i.SmallGroups.SplitStr("\n").ToList();
            Setting = DbUtil.Db.CreateRegistrationSettings(OrgId ?? 0);
        }

        private bool dopopulate = false;

        public int? OrgId
        {
            get { return orgId; }
            set
            {
                if (orgId != value)
                {
                    dopopulate = true;
                }

                orgId = value;
                Populate();
            }
        }

        [SkipFieldOnCopyProperties]
        public int? PeopleId
        {
            get { return peopleId; }
            set
            {
                if (peopleId != value)
                {
                    dopopulate = true;
                }

                peopleId = value;
                Populate();
            }
        }

        public string Name { get; set; }
        public string OrgName { get; set; }

        public string AttendStr { get; set; }

        public Settings Setting { get; set; }

        [TrackChanges]
        public CodeInfo MemberType { get; set; }

        [TrackChanges]
        public DateTime? InactiveDate { get; set; }

        [TrackChanges]
        //        [RegularExpression(@"\d{1,2}/\d{1,2}/(\d\d){1,2}( \d{1,2}:\d\d [AP]M){0,1}")]
        [DisplayName("Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }

        [TrackChanges]
        public bool Pending { get; set; }

        [TrackChanges]
        public string RegisterEmail { get; set; }

        [TrackChanges]
        public string Request { get; set; }

        [TrackChanges]
        public int? Grade { get; set; }

        [TrackChanges]
        public int? TranId { get; set; }

        [TrackChanges]
        public int? Tickets { get; set; }

        [TrackChanges, StringLength(50)]
        public string ShirtSize { get; set; }

        [TrackChanges]
        [DisplayName("Extra Member Info")]
        public string UserData { get; set; }

        [TrackChanges]
        public DateTime? DropDate { get; set; }


        //        [DisplayName("Total Amount")]
        //        public decimal? Amount { get; set; }

        [DisplayName("Fee")]
        public decimal? AmtFee { get; set; }

        [DisplayName("Paid")]
        public decimal? AmtPaid { get; set; }

        [DisplayName("Due")]
        public decimal? AmtDue { get; set; }

        [DisplayName("Donation")]
        public decimal? AmtDonation { get; set; }

        [DisplayName("Coupon")]
        public decimal? AmtCoupon { get; set; }

        private string transactionsLink;

        public string TransactionsLink
        {
            get
            {
                if (transactionsLink.HasValue())
                {
                    return transactionsLink;
                }

                if (PrevMember == null)
                {
                    return "";
                }

                return transactionsLink = PrevMember.TranId.HasValue ? $"/Transactions/{PrevMember.TranId}" : null;
            }
        }

        public IEnumerable<PrevOrgMemberExtra> ExtraValues()
        {
            var q = from ev in DbUtil.Db.PrevOrgMemberExtras
                    where ev.EnrollmentTranId == PrevMember.TransactionId
                    select ev;
            return q;
        }
        public IEnumerable<OrgMemberQuestion> RegQuestions()
        {
            var q = DbUtil.Db.OrgMemberQuestions(OrgId, PeopleId);
            return q;
        }
    }
}
