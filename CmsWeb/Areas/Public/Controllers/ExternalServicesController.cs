using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
    public class ExternalServicesController : CMSBaseController
    {
        public ExternalServicesController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index()
        {
            return Content("Success!");
        }

        [ValidateInput(false)]
        public ActionResult PMMResults()
        {
            string req = Request["request"];

            int reportID = 0;

            string reportLink = "";
            string orderID = "";
            string billingReference = "";

            bool hasAlerts = false;

            XDocument xmldoc = XDocument.Parse(req, LoadOptions.None);

            reportID = int.Parse(xmldoc.Root.Element("ReportID").Value);
            billingReference = xmldoc.Root.Element("Order").Element("BillingReferenceCode").Value;

            if (xmldoc.Root.Element("Order").Element("Alerts") != null)
            {
                hasAlerts = true;
            }

            reportLink = xmldoc.Root.Element("Order").Element("ReportLink").Value;
            orderID = xmldoc.Root.Element("Order").Element("OrderDetail").Attribute("OrderId").Value;

            var check = (from e in CurrentDatabase.BackgroundChecks
                         where e.ReportID == reportID
                         select e).SingleOrDefault();

            if (check != null)
            {
                check.Updated = DateTime.Now;
                check.ReportID = reportID;
                check.ReportLink = reportLink;
                check.StatusID = 3;
                if (hasAlerts)
                {
                    check.IssueCount = 1;
                }

                CurrentDatabase.SubmitChanges();

                CurrentDatabase.Email(DbUtil.AdminMail, check.User, "TouchPoint Notification: Background Check Complete", "A scheduled background check has been completed for " + check.Person.Name);
            }

            return Content("<?xml version=\"1.0\" encoding=\"utf-8\"?><OrderXML><Success>TRUE</Success></OrderXML>");
        }

        [ValidateInput(false)]
        public ActionResult ct(string l) // Click Tracking
        {
            string link = null;
            CurrentDatabase.TrackClick(l, ref link);
            if (link.HasValue())
            {
                return Redirect(Server.HtmlDecode(link));
            }

            return Redirect("/");
        }

        private enum ErrorMessage
        {
            NoErrors = 0,
            ApiKeyHeaderMissing = 1,
            ApiKeyHeaderValueNull = 2,
            AuthHeaderMissing = 3,
            AuthHeaderValueInvalid = 4,
            AuthorizationHeaderValueInvalid = 5,
            MalformedBase64 = 6,
            InvalidCredentials = 7,
            ApiKeySettingNotFound = 8,
            ApiKeyNotValid = 9,
            IpSettingNotFound = 10,
            IpNotFoundInKey = 11,
            FailedAuthentication = 12
        }

        public ActionResult ApiUserInfo()
        {
            var header = HttpContextFactory.Current.Request.Headers;

            //Check incoming values are valid        
            var apiKeyValue = header.GetValues("ApiKey");
            var ip = HttpContextFactory.Current.Request.ServerVariables["REMOTE_ADDR"];

            // ApiKey header is missing
            if (apiKeyValue == null)
            {
                return Json(new
                {
                    error = ErrorMessage.ApiKeyHeaderMissing,
                    ip,
                }, JsonRequestBehavior.AllowGet);
            }

            // ApiKey header has no values 
            string apiKey = apiKeyValue.First();
            if (apiKey.IsNull())
            {
                return Json(new
                {
                    error = ErrorMessage.ApiKeyHeaderValueNull,
                    ip,
                }, JsonRequestBehavior.AllowGet);
            }

            // Authorization header is missing
            var authorizationValue = header.GetValues("Authorization");
            if (authorizationValue == null)
            {
                return Json(new
                {
                    error = ErrorMessage.AuthHeaderMissing,
                    ip,
                }, JsonRequestBehavior.AllowGet);
            }

            // Authorization header has no values/bad data
            string getAuthInfo;
            try
            {
                if (!authorizationValue.First().Substring(0, 6).Trim().Equals("Basic"))
                {
                    return Json(new
                    {
                        error = ErrorMessage.AuthHeaderValueInvalid,
                        ip,
                    }, JsonRequestBehavior.AllowGet);
                }
                getAuthInfo = Encoding.ASCII.GetString(Convert.FromBase64String(authorizationValue.First().Substring(6)));
            }
            catch (Exception)
            {
                return Json(new
                {
                    error = ErrorMessage.MalformedBase64,
                    ip,
                }, JsonRequestBehavior.AllowGet);
            }

            var authorization = getAuthInfo.Split(new[] { ':' }, 2);
            if (authorization.Count() != 2)
            {
                return Json(new
                {
                    error = ErrorMessage.AuthorizationHeaderValueInvalid,
                    ip,
                }, JsonRequestBehavior.AllowGet);
            }

            string userName = authorization[0];
            string password = authorization[1];

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Json(new
                {
                    error = ErrorMessage.InvalidCredentials,
                    ip,
                }, JsonRequestBehavior.AllowGet);
            }

            //Check Api Key setting exists 
            var getApiUserInfoKey = CurrentDatabase.Setting("ApiUserInfoKey", "");
            if (getApiUserInfoKey == "")
            {
                return Json(new
                {
                    error = ErrorMessage.ApiKeySettingNotFound,
                    ip,
                }, JsonRequestBehavior.AllowGet);
            }

            // Check Api Key is valid
            if (getApiUserInfoKey?.Trim() == apiKey)
            {
                //Check Ip setting is valid
                var getIpWhiteList = CurrentDatabase.Setting("ApiUserInfoIPList", "");
                if (getIpWhiteList == "")
                {
                    return Json(new
                    {
                        error = ErrorMessage.IpSettingNotFound,
                        ip,
                    }, JsonRequestBehavior.AllowGet);
                }

                //Check Ip is valid
                var ipCheck = getIpWhiteList.Split(',', ';', ' ').Contains(ip);
                if (!ipCheck)
                {
                    return Json(new
                    {
                        error = ErrorMessage.IpNotFoundInKey,
                        ip,
                    }, JsonRequestBehavior.AllowGet);
                }

                //Authenticate user
                var retUser = AccountModel.AuthenticateLogon(userName, password, "", CurrentDatabase);
                if (retUser.IsValid)
                {
                    return Json(new
                    {
                        id = retUser.User.PeopleId,
                        name = retUser.User.Name,
                        emailAddress = retUser.User.EmailAddress,
                        altEmail = retUser.User.Person.EmailAddress2,
                        roles = retUser.User.Roles,
                        error = ErrorMessage.NoErrors
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    error = ErrorMessage.FailedAuthentication,
                    ip,
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                error = ErrorMessage.ApiKeyNotValid,
                ip,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
