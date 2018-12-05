using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog.Models
{
    public class OrgMemberTransactionModel
    {
        private int? orgId;
        private int? peopleId;
        private OrganizationMember om;
        private bool isMissionTrip;
        public CmsData.View.TransactionSummary TransactionSummary;
        public decimal Due;
        public OrgMemberTransactionModel() { }
        private void Populate()
        {
            var q = from mm in DbUtil.Db.OrganizationMembers
                    let ts = DbUtil.Db.ViewTransactionSummaries.SingleOrDefault(tt => tt.RegId == mm.TranId && tt.PeopleId == mm.PeopleId)
                    where mm.OrganizationId == OrgId && mm.PeopleId == PeopleId
                    select new
                    {
                        mm.Person.Name,
                        mm.Organization.OrganizationName,
                        om = mm,
                        mt = mm.Organization.IsMissionTrip ?? false,
                        ts
                    };
            var i = q.SingleOrDefault();
            if (i == null)
            {
                return;
            }

            Name = i.Name;
            OrgName = i.OrganizationName;
            om = i.om;
            isMissionTrip = i.mt;
            TransactionSummary = i.ts;
            Due = isMissionTrip
                ? MissionTripFundingModel.TotalDue(peopleId, orgId)
                : i.ts != null ? i.ts.TotDue ?? 0 : 0;
        }

        public string Group { get; set; }

        public int? OrgId
        {
            get { return orgId; }
            set
            {
                orgId = value;
                if (peopleId.HasValue)
                {
                    Populate();
                }
            }
        }
        public int? PeopleId
        {
            get { return peopleId; }
            set
            {
                peopleId = value;
                if (orgId.HasValue)
                {
                    Populate();
                }
            }
        }
        public string Name { get; set; }
        public string OrgName { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Payment { get; set; }
        public bool AdjustFee { get; set; }
        [StringLength(100)]
        public string Description { get; set; }

        internal void PostTransaction(ModelStateDictionary modelState)
        {
            if (TransactionSummary != null && (Payment ?? 0) == 0)
            {
                modelState.AddModelError("Payment", "must have non zero value");
            }

            if (TransactionSummary == null && (Amount ?? 0) == 0)
            {
                modelState.AddModelError("Amount", "Initial Fee Must be > 0");
            }

            if (!modelState.IsValid)
            {
                return;
            }

            var reason = TransactionSummary == null
                ? "Initial Tran"
                : AdjustFee
                    ? "AdjustFee"
                    : "Adjustment";
            if (isMissionTrip)
            {
                if (TransactionSummary == null)
                {
                    om.AddToGroup(DbUtil.Db, "Goer");
                    om.Amount = Amount;
                }
                if (AdjustFee == false && (Payment ?? 0) != 0)
                {
                    var gs = new GoerSenderAmount
                    {
                        GoerId = om.PeopleId,
                        SupporterId = om.PeopleId,
                        Amount = Payment,
                        OrgId = om.OrganizationId,
                        Created = DateTime.Now,
                    };
                    DbUtil.Db.GoerSenderAmounts.InsertOnSubmit(gs);
                }
            }
            var descriptionForPayment = OnlineRegModel.GetDescriptionForPayment(OrgId);
            om.AddTransaction(DbUtil.Db, reason, Payment ?? 0, Description, Amount, AdjustFee, descriptionForPayment);
            var showcount = "";
            if (TransactionSummary != null && TransactionSummary.NumPeople > 1)
            {
                showcount = $"({TransactionSummary.NumPeople}) ";
            }

            DbUtil.LogActivity($"OrgMem{showcount} {reason}", OrgId, PeopleId);
        }
    }
}
