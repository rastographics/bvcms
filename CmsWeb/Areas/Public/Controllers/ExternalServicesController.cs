using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using CmsData;
using CmsWeb.Controllers;
using UtilityExtensions;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Configuration;

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

                DbUtil.Db.Email(DbUtil.AdminMail, check.User, "BVCMS Notification: Background Check Complete", "A scheduled background check has been completed for " + check.Person.Name);
            }

            //System.IO.File.WriteAllText(@"C:\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt", req);

            return Content("<?xml version=\"1.0\" encoding=\"utf-8\"?><OrderXML><Success>TRUE</Success></OrderXML>");
        }

        public static string SQLSupportQuery = "SELECT * FROM [dbo].[SupportRequests] WHERE ID = @id";
        public static string SQLSupportUpdate = "UPDATE [dbo].[SupportRequests] SET SupportPersonID = @pid, SupportPerson = @p, ClaimedOn = @c WHERE ID = @id";

        public ActionResult BVCMSSupportLink( int requestID, int supportPersonID )
        {
            var cs = ConfigurationManager.ConnectionStrings["CmsLogging"];
            if (cs == null) return Content("Database not available!");

            var subject = "";
            var host = "";
            var who = "";
            var claimedby = "";
            var claimedID = 0;
            var supportPerson = SupportController.SupportPeople[supportPersonID];

            var cn = new SqlConnection(cs.ConnectionString);
            cn.Open();
            var query = new SqlCommand(SQLSupportQuery, cn);

            query.Parameters.AddWithValue("@id", requestID);

            var reader = query.ExecuteReader();
            if (reader.Read())
            {
                subject = (string)reader["Subject"];
                host = (string)reader["Host"];
                who = (string)reader["Who"];
                claimedID = (int)reader["SupportPersonID"];
                claimedby = (string)reader["SupportPerson"];
            }
            reader.Close();

            if (claimedID == 0)
            {
                var update = new SqlCommand(SQLSupportUpdate, cn);

                update.Parameters.AddWithValue("@pid", supportPersonID);
                update.Parameters.AddWithValue("@p", supportPerson);
                update.Parameters.AddWithValue("@c", DateTime.Now);
                update.Parameters.AddWithValue("@id", requestID);

                update.ExecuteNonQuery();
            }

            cn.Close();

            if (claimedID == 0)
            {
                var from = "support-system@bvcms.com";
                var to = "support@bvcms.com";
                var body = supportPerson + " is going to respond to this support request.";

                var smtp = Util.Smtp();
                var email = new MailMessage(from, to, subject, body);
                email.ReplyToList.Add("support@bvcms.com");
                email.IsBodyHtml = true;
                smtp.Send(email);

                var requestOrigin = "https://" + host + ".bvcms.com";
                return Redirect(requestOrigin);
            }
            ViewBag.Message = "This support request has already been claimed by " + claimedby;
            return View();
        }

        [ValidateInput(false)]
        public ActionResult ct(string l)
        {
            var link = (from e in DbUtil.Db.EmailLinks
                        where e.Hash == l
                        select e).SingleOrDefault();

            if (link != null)
            {
                link.Count += 1;
                DbUtil.Db.SubmitChanges();

                if(link.Link.HasValue())
                    return Redirect( link.Link );
            }
            return Redirect( "http://www.bvcms.com" );
        }
    }
}
