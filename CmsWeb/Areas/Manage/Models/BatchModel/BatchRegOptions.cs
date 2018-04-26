using System.IO;
using CmsData;
using CmsData.Registration;
using CmsWeb.Code;
using LumenWorks.Framework.IO.Csv;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Models.BatchModel
{
    public class BatchRegOptions
    {
        private CsvReader csv;
        private Settings rs;

        public static void Update(string text)
        {
            new BatchRegOptions().DoUpdate(text);
        }
        private void DoUpdate(string text)
        {
            csv = new CsvReader(new StringReader(text), true, '\t');
            if(csv.GetFieldIndex("OrganizationId") == -1)
                throw new UserInputException("Missing required OrganizationId column");
            while (csv.ReadNextRecord())
            {
                var oid = csv["OrganizationId"].ToInt();
                var o = DbUtil.Db.LoadOrganizationById(oid);
                rs = DbUtil.Db.CreateRegistrationSettings(oid);

                var name = FindColumn("OrganizationName");
                if (name.HasValue())
                    o.OrganizationName = name;

                UpdateAsk("AskAllergies");
                UpdateAsk("AnswersNotRequired");
                UpdateAsk("AskChurch");
                UpdateAsk("AskCoaching");
                UpdateAsk("AskDoctor");
                UpdateAsk("AnswersNotRequired");
                UpdateAsk("AskChurch");
                UpdateAsk("AskCoaching");
                UpdateAsk("AskDoctor");
                UpdateAsk("AskEmContact");
                UpdateAsk("AskInsurance");
                UpdateAsk("AskParents");
                UpdateAsk("AskSMS");
                UpdateAsk("AskTylenolEtc");
                UpdateAsk("AskSuggestedFee");
                UpdateAskLabel("AskRequest");
                UpdateAskLabel("AskTickets");

                var b = FindColumn("NoReqBirthYear").ToBool2();
                if (b.HasValue)
                    rs.NoReqBirthYear = b.Value;
                b = FindColumn("NotReqDOB").ToBool2();
                if (b.HasValue)
                    rs.NotReqDOB = b.Value;
                b = FindColumn("NotReqAddr").ToBool2();
                if (b.HasValue)
                    rs.NotReqAddr = b.Value;
                b = FindColumn("NotReqGender").ToBool2();
                if (b.HasValue)
                    rs.NotReqGender = b.Value;
                b = FindColumn("NotReqMarital").ToBool2();
                if (b.HasValue)
                    rs.NotReqMarital = b.Value;
                b = FindColumn("NotReqCampus").ToBool2();
                if (b.HasValue)
                    rs.NotReqCampus = b.Value;
                b = FindColumn("NotReqPhone").ToBool2();
                if (b.HasValue)
                    rs.NotReqPhone = b.Value;
                b = FindColumn("NotReqZip").ToBool2();
                if (b.HasValue)
                    rs.NotReqZip = b.Value;

                b = FindColumn("AllowOnlyOne").ToBool2();
                if (b.HasValue)
                    rs.AllowOnlyOne = b.Value;
                b = FindColumn("MemberOnly").ToBool2();
                if (b.HasValue)
                    rs.MemberOnly = b.Value;
                b = FindColumn("AddAsProspect").ToBool2();
                if (b.HasValue)
                    rs.AddAsProspect = b.Value;
                b = FindColumn("TargetExtraValues").ToBool2();
                if (b.HasValue)
                    rs.TargetExtraValues = b.Value;
                b = FindColumn("AllowReRegister").ToBool2();
                if (b.HasValue)
                    rs.AllowReRegister = b.Value;
                b = FindColumn("AllowSaveProgress").ToBool2();
                if (b.HasValue)
                    rs.AllowSaveProgress = b.Value;
                b = FindColumn("DisallowAnonymous").ToBool2();
                if (b.HasValue)
                    rs.DisallowAnonymous = b.Value;
                b = FindColumn("ApplyMaxToOtheFees").ToBool2();
                if (b.HasValue)
                    rs.ApplyMaxToOtherFees = b.Value;
                b = FindColumn("IncludeOtherFeesWithDeposit").ToBool2();
                if (b.HasValue)
                    rs.IncludeOtherFeesWithDeposit = b.Value;
                b = FindColumn("OtherFeesAddedToOrgFee").ToBool2();
                if (b.HasValue)
                    rs.OtherFeesAddedToOrgFee = b.Value;
                b = FindColumn("AskDonation").ToBool2();
                if (b.HasValue)
                    rs.AskDonation = b.Value;

                var s = FindColumn("ConfirmationTrackingCode");
                rs.ConfirmationTrackingCode = s;

                s = FindColumn("ValidateOrgs");
                rs.ValidateOrgs = s;

                s = FindColumn("Shell");
                rs.Shell = s;

                s = FindColumn("ShellBs");
                rs.ShellBs = s;

                s = FindColumn("FinishRegistrationButton");
                rs.FinishRegistrationButton = s;

                s = FindColumn("SpecialScript");
                rs.SpecialScript = s;

                s = FindColumn("GroupToJoin");
                rs.GroupToJoin = s;

                s = FindColumn("TimeOut");
                rs.TimeOut = s.ToInt2();

                s = FindColumn("Fee");
                rs.Fee = s.ToDecimal();

                s = FindColumn("MaximumFee");
                rs.MaximumFee = s.ToDecimal();

                s = FindColumn("ExtraFee");
                rs.ExtraFee = s.ToDecimal();

                s = FindColumn("Deposit");
                rs.Deposit = s.ToDecimal();

                s = FindColumn("AccountingCode");
                rs.AccountingCode = s;

                s = FindColumn("ExtraValueFeeName");
                rs.ExtraValueFeeName = s;

                s = FindColumn("DonationLabel");
                rs.DonationLabel = s;

                s = FindColumn("DonationFundId");
                rs.DonationFundId = s.ToInt();

                o.UpdateRegSetting(rs);
                DbUtil.Db.SubmitChanges();
            }
        }

        private string FindColumn(string col)
        {
            var i = csv.GetFieldIndex(col);
            if (i >= 0)
                return csv[i];
            return null;
        }
        private void UpdateAskLabel(string name)
        {
            var val = FindColumn(name);
            if (val.HasValue())
            {
                var askrequest = rs.AskItem(name) as AskRequest;
                if (askrequest == null)
                    rs.AskItems.Add(new AskRequest { Label = val });
                else
                    askrequest.Label = val;
            }
        }

        private void UpdateAsk(string name)
        {
            var b = FindColumn(name).ToBool2();
            var a = rs.AskItem(name);
            if (a == null && b == true)
                rs.AskItems.Add(new Ask(name));
            else if (a != null && b == false)
                rs.AskItems.Remove(a);
        }
    }
}
