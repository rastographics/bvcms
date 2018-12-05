using System;
using System.Web.Routing;
using CmsData;
using Elmah;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public class RouteModel
    {
        public RouteType Route;
        public string Message;
        public string View;
        public PaymentForm PaymentForm;
        public RouteValueDictionary RouteData;
        public OnlineRegModel Model;
        public string AmtDue;
        public Transaction Transaction;

        public RouteModel() { }
        public static RouteModel ViewAction(string view)
        {
            return new RouteModel()
            {
                Route = RouteType.Action,
                View = view
            };
        }
        public static RouteModel ViewAction(string view, OnlineRegModel m)
        {
            return new RouteModel()
            {
                Route = RouteType.ModelAction,
                Model = m,
                View = view
            };
        }
        public static RouteModel ViewTerms(string view)
        {
            return new RouteModel()
            {
                Route = RouteType.Terms,
                View = view,
            };
        }
        public static RouteModel ViewPayment(string view, PaymentForm pf)
        {
            return new RouteModel()
            {
                Route = RouteType.Payment,
                View = view,
                PaymentForm = pf,
            };
        }
        public static RouteModel ErrorMessage(string message)
        {
            return new RouteModel()
            {
                Route = RouteType.Error,
                Message = message
            };
        }
        public static RouteModel Redirect(string where, object d)
        {
            return new RouteModel()
            {
                Route = RouteType.Redirect,
                View = @where,
                RouteData = new RouteValueDictionary(d),
            };
        }

        public static RouteModel Invalid(string where, string message)
        {
            return new RouteModel()
            {
                Route = RouteType.ValidationError,
                View = @where,
                Message = message
            };
        }

        public static RouteModel ProcessPayment()
        {
            return new RouteModel()
            {
                Route = RouteType.ValidationError,
                View = "Payment/Process"
            };
        }

        public static RouteModel AmountDue(decimal amt, Transaction ti)
        {
            return new RouteModel()
            {
                Route = RouteType.AmtDue,
                AmtDue = amt.ToString("C"),
                Transaction = ti,
                View = "PayAmtDue/Confirm",
            };
        }
    }
    public enum RouteType
    {
        Error,
        Action,
        ModelAction,
        ValidationError,
        Redirect,
        Terms,
        Payment,
        AmtDue,
    }
}
