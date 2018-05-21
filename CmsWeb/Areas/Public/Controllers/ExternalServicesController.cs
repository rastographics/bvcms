using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;
using CmsData;
using UtilityExtensions;
using CmsWeb.Models;
using MoreLinq;


namespace CmsWeb.Areas.Public.Controllers
{
    public class ExternalServicesController : Controller
    {
        public ActionResult Index()
        {
            return Content( "Success!" );
        }

        [ValidateInput(false)]
        public ActionResult PMMResults()
        {
            string req = Request["request"];

            int iBillingReference = 0;
            int iReportID = 0;
            
            string sReportLink = "";
            string sOrderID = "";

            bool bHasAlerts = false;

            XDocument xd = XDocument.Parse(req, LoadOptions.None);

            iReportID = Int32.Parse( xd.Root.Element("ReportID").Value );
            iBillingReference = Int32.Parse(xd.Root.Element("Order").Element("BillingReferenceCode").Value);

            if (xd.Root.Element("Order").Element("Alerts") != null) bHasAlerts = true;

            sReportLink = xd.Root.Element("Order").Element("ReportLink").Value;
            sOrderID = xd.Root.Element("Order").Element("OrderDetail").Attribute("OrderId").Value;

            var check = (from e in DbUtil.Db.BackgroundChecks
                         where e.Id == iBillingReference
                         select e).Single();

            if (check != null)
            {
                check.Updated = DateTime.Now;
                check.ReportID = iReportID;
                check.ReportLink = sReportLink;
                check.StatusID = 3;
                if (bHasAlerts) check.IssueCount = 1;

                DbUtil.Db.SubmitChanges();

                DbUtil.Db.Email(DbUtil.AdminMail, check.User, "TouchPoint Notification: Background Check Complete", "A scheduled background check has been completed for " + check.Person.Name);
            }

            //System.IO.File.WriteAllText(@"C:\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt", req);

            return Content("<?xml version=\"1.0\" encoding=\"utf-8\"?><OrderXML><Success>TRUE</Success></OrderXML>");
        }

        [ValidateInput(false)]
        public ActionResult ct(string l) // Click Tracking
        {
            string link = null;
            DbUtil.Db.TrackClick(l, ref link);
            if(link.HasValue())
                return Redirect( Server.HtmlDecode(link) );
            return Redirect("/");
        }

        enum ErrorMessage
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
            var header = Request.Headers;

            //Check incoming values are valid        
            var apiKeyValue = header.GetValues("ApiKey");

            // ApiKey header is missing
            if (apiKeyValue == null)
            {
                return Json(new
                {
                    error = ErrorMessage.ApiKeyHeaderMissing
                }, JsonRequestBehavior.AllowGet);                
            }

            // ApiKey header has no values 
            string apiKey = apiKeyValue.First();
            if (apiKey.IsNull())
            {
                return Json(new
                {
                    error = ErrorMessage.ApiKeyHeaderValueNull
                }, JsonRequestBehavior.AllowGet);
            }

            // Authorization header is missing
            var authorizationValue = header.GetValues("Authorization");
            if (authorizationValue == null)
            {
                return Json(new
                {
                    error = ErrorMessage.AuthHeaderMissing
                }, JsonRequestBehavior.AllowGet);
            }

            // Authorization header has no values/bad data
            string getAuthInfo;
            try
            {
                if (!authorizationValue.First().Substring(0,6).Trim().Equals("Basic"))
                {
                    return Json(new
                    {
                        error = ErrorMessage.AuthHeaderValueInvalid
                    }, JsonRequestBehavior.AllowGet);
                }
                getAuthInfo=Encoding.ASCII.GetString(Convert.FromBase64String(authorizationValue.First().Substring(6)));
            }
            catch (Exception)
            {
                return Json(new
                {
                    error = ErrorMessage.MalformedBase64
                }, JsonRequestBehavior.AllowGet);
            }

            var authorization = getAuthInfo.Split(':');
            if (authorization.Count() != 2)
            {
                return Json(new
                {
                    error = ErrorMessage.AuthorizationHeaderValueInvalid
                }, JsonRequestBehavior.AllowGet);
            }

            string userName = authorization[0];
            string password = authorization[1];

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Json(new
                {
                    error = ErrorMessage.InvalidCredentials
                }, JsonRequestBehavior.AllowGet);
            }

            //Check Api Key setting exists 
            var getApiUserInfoKey = DbUtil.Db.Setting("ApiUserInfoKey", "");
            if (getApiUserInfoKey == "")
            {
                return Json(new
                {
                    error = ErrorMessage.ApiKeySettingNotFound
                }, JsonRequestBehavior.AllowGet);
            }

            // Check Api Key is valid
            if (getApiUserInfoKey.trim() == apiKey)
            {
                var getIp = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                //Check Ip setting is valid
                var getIpWhiteList = DbUtil.Db.Setting("ApiUserInfoIPList", "");
                if (getIpWhiteList == "")
                {
                    return Json(new
                    {
                        error = ErrorMessage.IpSettingNotFound
                    }, JsonRequestBehavior.AllowGet);
                }

                //Check Ip is valid
                var ipCheck = getIpWhiteList.Split(',').Contains(getIp);
                if (!ipCheck)
                {
                    return Json(new
                    {
                        error = ErrorMessage.IpNotFoundInKey
                    }, JsonRequestBehavior.AllowGet);
                }
                
                //Authenticate user
                var retUser = AccountModel.AuthenticateLogon(userName, password, "");
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
                    error = ErrorMessage.FailedAuthentication
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                error = ErrorMessage.ApiKeyNotValid
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
