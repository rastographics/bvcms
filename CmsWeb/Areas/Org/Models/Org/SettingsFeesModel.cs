using CmsData;
using CmsData.Registration;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Areas.Org.Models
{
    public class SettingsFeesModel
    {
        public Organization Org;

        public int Id
        {
            get { return Org != null ? Org.OrganizationId : 0; }
            set
            {
                if (Org == null)
                {
                    Org = DbUtil.Db.LoadOrganizationById(value);
                }
            }
        }

        public SettingsFeesModel()
        {
        }

        public SettingsFeesModel(int id)
        {
            Id = id;
            this.CopyPropertiesFrom(Org, typeof(OrgAttribute));
            this.CopyPropertiesFrom(RegSettings, typeof(RegAttribute));
        }
        public void Update()
        {
            this.CopyPropertiesTo(Org, typeof(OrgAttribute));
            RegSettings.OrgFees.Clear();
            this.CopyPropertiesTo(RegSettings, typeof(RegAttribute));
            var os = DbUtil.Db.CreateRegistrationSettings(RegSettings.ToString(), Id);
            Org.UpdateRegSetting(os);
            DbUtil.Db.SubmitChanges();
        }

        private Settings RegSettings => regsettings ?? (regsettings = DbUtil.Db.CreateRegistrationSettings(Id));
        private Settings regsettings;

        [Reg, Display(Description = FeeDescription)]
        public decimal? Fee { get; set; }

        [Reg, Display(Description = DepositDescription), DisplayName("Deposit Amount")]
        public decimal? Deposit { get; set; }

        [Reg, Display(Description = IncludeOtherFeesWithDepositDescription)]
        public bool IncludeOtherFeesWithDeposit { get; set; }

        [Reg, Display(Description = ExtraFeeDescription)]
        public decimal? ExtraFee { get; set; }

        [Org, Display(Description = LastDayBeforeExtraDescription)]
        public DateTime? LastDayBeforeExtra { get; set; }

        [Reg, Display(Description = MaximumFeeDescription)]
        public decimal? MaximumFee { get; set; }

        [Reg, Display(Description = ApplyMaxToOtherFeesDescription)]
        public bool ApplyMaxToOtherFees { get; set; }

        [Reg, Display(Description = AskDonationDescription)]
        public bool AskDonation { get; set; }

        [Reg, Display(Description = DonationFundIdDescription)]
        public int? DonationFundId { get; set; }

        [Reg, Display(Description = DonationLabelDescription)]
        public string DonationLabel { get; set; }

        [Reg, Display(Description = OrgFeesDescription), UIHint("OrgFees")]
        public List<Settings.OrgFee> OrgFees
        {
            get { return orgFees ?? new List<Settings.OrgFee>(); }
            set { orgFees = value; }
        }
        private List<Settings.OrgFee> orgFees;

        [Reg, Display(Description = OtherFeesAddedToOrgFeeDescription)]
        public bool OtherFeesAddedToOrgFee { get; set; }

        [Reg, Display(Description = AccountingCodeDescription)]
        public string AccountingCode { get; set; }

        [Reg, Display(Description = ExtraValueFeeNameDescription)]
        public string ExtraValueFeeName { get; set; }

        #region Descriptions

        private const string FeeDescription = "The base fee for the registration";
        private const string DepositDescription = @"
Allows the registrant to pay in full or pay a deposit.
If paying a deposit, they get a link to continue to pay on this account.
Must add {paylink} to the confirmation.
They can make as many additional payments as they want until paid in full.
Like an installment payment.";

        private const string IncludeOtherFeesWithDepositDescription =
            @"Indicate whether the Other Fees (Questions tab) are paid with the deposit.";

        private const string ExtraFeeDescription = @"A late registration fee.";
        private const string LastDayBeforeExtraDescription = @"
The date, after which, the extra fee goes into effect.
Good for when you want to discourage last minute registrations.";
        private const string MaximumFeeDescription = @"
The maximum fee for all registrants.
Good for family maximum fee.
Does not include shirt fees and other extra fees.";

        private const string ApplyMaxToOtherFeesDescription =
            @"Indicate whether the maximum applies to other fees too, or just to main fee.";

        private const string AskDonationDescription =
            @"Indicate whether you want to ask for an extra donation. Creates a contribution record for that amount too.";

        private const string DonationFundIdDescription = @"
Used to specify the Fund for a special donation.
Also used to specify the Pledge Fund for Online Pledges.";
        private const string DonationLabelDescription = @"HTML used to describe the 'featured' donation.";
        private const string OrgFeesDescription = @"
This will give registrants a special fee if they are members of a particular organization.
Note that this fee overrides all other fees and will not appear until the payment page.
If it is zero, the payment page will be skipped.";

        private const string OtherFeesAddedToOrgFeeDescription =
            @"Indicate whether the special fees for orgs includes other Fees on the Questions tab.";

        private const string AccountingCodeDescription =
            @"Used to add a (1234) to the end of the OrgName passed to the payment processor.";

        private const string ExtraValueFeeNameDescription = @"The fee will be taken from this Extra Value field.";

        #endregion

    }
}
