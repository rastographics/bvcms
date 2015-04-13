using System.Linq;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.OnlineReg.Controllers;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public partial class OnlineRegModel
    {
        public RouteModel CompleteRegistration(OnlineRegController ctl)
        {
            HistoryAdd("CompleteRegistration");

            var ret = CheckSpecialJavascript();
            if (ret != null) return ret;

            ret = CheckAskDonation(ctl);
            if (ret != null) return ret;

            if (List.Count == 0)
                return RouteModel.ErrorMessage("Can't find any registrants");

            RemoveLastRegistrantIfEmpty();

            UpdateDatum();
            DbUtil.LogActivity("Online Registration: {0} ({1})".Fmt(Header, DatumId));

            ret = CheckNoFeesDue();
            if (ret != null) return ret;

            var terms = Util.PickFirst(Terms, "");
            if (terms.HasValue())
                ctl.ViewBag.Terms = terms;

            ret = CheckTermsNoFee(ctl);
            if (ret != null) return ret;

            ret = CheckAlreadyRegistered();
            if (ret != null) return ret;

            var pf = PaymentForm.CreatePaymentForm(this);
            ctl.ModelState.Clear();
            return RouteModel.ViewPayment("Payment/Process", pf);
        }

        private RouteModel CheckAlreadyRegistered()
        {
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(
                mm => mm.OrganizationId == Orgid && mm.PeopleId == List[0].PeopleId);
            ParseSettings();

            if (om != null && settings[om.OrganizationId].AllowReRegister == false && !SupportMissionTrip)
                return RouteModel.ErrorMessage("You are already registered it appears");
            return null;
        }

        private RouteModel CheckTermsNoFee(OnlineRegController ctl)
        {
            ctl.SetHeaders(this);
            if (PayAmount() == 0 && Terms.HasValue())
                return RouteModel.ViewTerms("Terms");
            return null;
        }

        private RouteModel CheckNoFeesDue()
        {
            if (PayAmount() == 0 && (donation ?? 0) == 0 && !Terms.HasValue())
                return RouteModel.Redirect("Confirm",
                    new
                    {
                        id = DatumId,
                        TransactionID = "zero due",
                    });
            return null;
        }

        private RouteModel CheckAskDonation(OnlineRegController ctl)
        {
            if (AskDonation() && !donor.HasValue && donation > 0)
            {
                ctl.SetHeaders(this);
                ctl.ModelState.AddModelError("donation",
                    "Please indicate a donor or clear the donation amount");
                return RouteModel.ViewAction("AskDonation");
            }
            return null;
        }

        private RouteModel CheckSpecialJavascript()
        {
            if (org != null && org.RegistrationTypeId == RegistrationTypeCode.SpecialJavascript)
            {
                var p = List[0];
                if (p.IsNew)
                    p.AddPerson(null, p.org.EntryPointId ?? 0);
                SpecialRegModel.SaveResults(Orgid ?? 0, List[0].PeopleId ?? 0, List[0].SpecialTest);
                return RouteModel.ViewAction("SpecialRegistrationResults");
            }
            return null;
        }

        public void RemoveLastRegistrantIfEmpty()
        {
            if (!last.IsNew && !last.Found == true)
                List.Remove(last);
            if (!(last.IsValidForNew || last.IsValidForExisting))
                List.Remove(last);
        }
    }

}