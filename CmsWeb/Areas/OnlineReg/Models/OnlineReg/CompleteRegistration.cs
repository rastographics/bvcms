using System.Linq;
using CmsData;
using System.Web.Routing;
using CmsData.Codes;
using CmsWeb.Areas.OnlineReg.Controllers;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public partial class OnlineRegModel
    {

        public enum RouteType
        {
            Error,
            Action,
            Redirect,
            Terms,
            Payment,
        }
        public class RouteModel
        {
            public RouteType RouteType;
            public string Message;
            public string View;
            public PaymentForm PaymentForm;
            public RouteValueDictionary RouteData;

            public static RouteModel ViewAction(string view)
            {
                return new RouteModel()
                {
                    RouteType = RouteType.Action, 
                    View = view
                };
            }
            public static RouteModel ViewTerms(string view)
            {
                return new RouteModel()
                {
                    RouteType = RouteType.Terms, 
                    View = view,
                };
            }
            public static RouteModel ViewPayment(string view, PaymentForm pf)
            {
                return new RouteModel()
                {
                    RouteType = RouteType.Payment, 
                    View = view,
                    PaymentForm = pf, 
                };
            }
            public static RouteModel ErrorMessage(string message)
            {
                return new RouteModel()
                {
                    RouteType = RouteType.Error, 
                    Message = message
                };
            }
            public static RouteModel Redirect(string where, object d)
            {
                return new RouteModel()
                {
                    RouteType = RouteType.Redirect,
                    View = where, 
                    RouteData = new RouteValueDictionary(d),
                };
            }
        }
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