using System;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;
using CmsData;
using UtilityExtensions;
using CmsWeb.Models;

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

        public ActionResult ApiUserInfo(string apiKey, string userName, string password)
        {
            //check if Key and Ip are valid
            var check = (from e in DbUtil.Db.ApiUserInfos
                where e.ApiKey == apiKey 
                select e).FirstOrDefault();
            if (check != null)
            {
                var getIp = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                var ipCheck = check.IpAddress.Split(',').Contains(getIp);
                if (ipCheck)
                {
                    //authenticate user
                    var retUser = AccountModel.AuthenticateLogon(userName, password, "");
                    if (retUser.IsValid)
                    {
                        return Json(new
                        {
                            PeopleId = retUser.User.PeopleId,
                            Name = retUser.User.Name,
                            EmailAddress = retUser.User.EmailAddress,
                            AltEmail = retUser.User.Person.EmailAddress2,
                            roles = retUser.User.Roles,
                            error = 0
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new
            {
                error = 1
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
