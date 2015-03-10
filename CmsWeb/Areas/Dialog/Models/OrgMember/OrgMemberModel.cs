using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;
using CmsData.Registration;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Models
{
    public class OrgMemberModel
    {
        private OrganizationMember om;
        public Organization Organization;
        public List<OrgMemMemTag> OrgMemMemTags;
        public bool IsMissionTrip;
        public CmsData.View.TransactionSummary TransactionSummary;
        private int? orgId;
        private int? peopleId;

        public OrgMemberModel()
        {
        }

        public OrgMemberModel(int oid, int pid)
        {
            OrgId = oid;
            PeopleId = pid;
        }

        private void Populate()
        {
            if (dopopulate)
            {
                dopopulate = false;
                if (!OrgId.HasValue || !PeopleId.HasValue)
                    return;
            }
            var i = (from mm in DbUtil.Db.OrganizationMembers
                where mm.OrganizationId == OrgId && mm.PeopleId == PeopleId
                select new
                {
                    mm,
                    mm.Person.Name,
                    mm.Organization.OrganizationName,
                    mm.Organization.RegSetting,
                    mm.Organization,
                    mm.OrgMemMemTags,
                    mm.Organization.IsMissionTrip,
                    ts = DbUtil.Db.ViewTransactionSummaries.SingleOrDefault(tt => tt.RegId == mm.TranId && tt.PeopleId == PeopleId)
                }).SingleOrDefault();
            if (i == null)
                throw new Exception("missing OrgMember at oid={0}, pid={1}".Fmt(OrgId, PeopleId));
            om = i.mm;
            TransactionSummary = i.ts;
            this.CopyPropertiesFrom(om);
            Name = i.Name;

            IsMissionTrip = i.IsMissionTrip ?? false;
            if (TransactionSummary != null)
            {
                AmountPaidTransactions = IsMissionTrip
                    ? om.TotalPaid(DbUtil.Db)
                    : TransactionSummary.IndPaid;
                AmountDue = IsMissionTrip
                    ? om.AmountDue(DbUtil.Db)
                    : TransactionSummary.IndDue;
            }

            OrgName = i.OrganizationName;
            Organization = i.Organization;
            OrgMemMemTags = i.OrgMemMemTags.ToList();
            Setting = new Settings(i.RegSetting, DbUtil.Db, OrgId ?? 0);
        }


        private bool dopopulate = false;
        public int? OrgId
        {
            get { return orgId; }
            set
            {
                if (orgId != value)
                    dopopulate = true;
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
                    dopopulate = true;
                peopleId = value;
                Populate();
            }
        }

        public string Name { get; set; }
        public string OrgName { get; set; }

        public string AttendStr { get; set; }

        public Settings Setting { get; set; }

        public CodeInfo MemberType { get; set; }

        public DateTime? InactiveDate { get; set; }

        [DisplayName("Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }

        public bool Pending { get; set; }

        public string RegisterEmail { get; set; }

        public string Request { get; set; }

        public int? Grade { get; set; }

        public int? Tickets { get; set; }

        [DisplayName("Total Amount")]
        public decimal? Amount { get; set; }

//        [DisplayName("Amount Paid Manually")]
//        public decimal? AmountPaid { get; set; }

        [DisplayName("Amount Paid")]
        public decimal? AmountPaidTransactions { get; set; }

        [DisplayName("Amount Due")]
        public decimal? AmountDue { get; set; }

        public string TransactionsLink
        {
            get
            {
                if (!IsMissionTrip) 
                    return om.TranId.HasValue ? "/Transactions/{0}".Fmt(om.TranId) : null;

                if(om.IsInGroup("Goer") && om.IsInGroup("Sender"))
                    return om.TranId.HasValue ? "/Transactions/{0}?goerid={1}&senderid={1}".Fmt(om.TranId, om.PeopleId) : null;

                if(om.IsInGroup("Goer"))
                    return om.TranId.HasValue ? "/Transactions/{0}?goerid={1}".Fmt(om.TranId, om.PeopleId) : null;

                if (om.IsInGroup("Sender"))
                    return "/Transactions/{0}?senderid={1}".Fmt(0, om.PeopleId);

                return om.TranId.HasValue ? "/Transactions/{0}".Fmt(om.TranId) : null;
            }
        }

        public string ShirtSize { get; set; }

        [DisplayName("Extra Member Info")]
        public string UserData { get; set; }

        public void UpdateModel()
        {
            om = DbUtil.Db.OrganizationMembers.Single(mm => mm.OrganizationId == OrgId && mm.PeopleId == PeopleId);
            this.CopyPropertiesTo(om);
            DbUtil.Db.SubmitChanges();
            Populate();
        }

        public string PayLink
        {
            get { return om.PayLink2(DbUtil.Db); }
        }

        public DateTime? DropDate { get; set; }
        public bool RemoveFromEnrollmentHistory { get; set; }

        public void Drop()
        {
            var om = DbUtil.Db.OrganizationMembers.Single(mm => mm.PeopleId == PeopleId && mm.OrganizationId == OrgId);
            if(DropDate.HasValue)
    			om.Drop(DbUtil.Db, DropDate.Value);
            else
    			om.Drop(DbUtil.Db);
            DbUtil.Db.SubmitChanges();
            if (RemoveFromEnrollmentHistory)
            {
                DbUtil.DbDispose();
                DbUtil.Db = new CMSDataContext(Util.ConnectionString);
                var q = DbUtil.Db.EnrollmentTransactions.Where(tt => tt.OrganizationId == OrgId && tt.PeopleId == PeopleId);
                DbUtil.Db.EnrollmentTransactions.DeleteAllOnSubmit(q);
                DbUtil.Db.SubmitChanges();
            }
        }
    }
}
