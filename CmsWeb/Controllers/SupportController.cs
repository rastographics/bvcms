using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using NPOI.SS.Formula.Functions;
using UtilityExtensions;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Configuration;

namespace CmsWeb.Controllers
{
	public class SupportController : CmsController
	{
		public static string[] SupportPeople = { "Unclaimed", "Bethany", "David", "Karen", "Kyle", "Steven" };
		public static string SQLSupportInsert = "INSERT INTO [dbo].[SupportRequests] ( Created, Who, Host, Urgency, Request, Subject ) OUTPUT INSERTED.ID VALUES ( @c, @w, @h, @u, @r, @s )";

	    public class SupportRequest
	    {
	        public string urgency { get; set; } 
            public string body { get; set; } 
            public string lastsearch { get; set; }
	        public string cc { get; set; }
	    }
        [POST("/Support/SendRequest")]
        [ValidateInput(false)]
		public ActionResult SendSupportRequest(SupportRequest req)
		{
			var cs = ConfigurationManager.ConnectionStrings["CmsLogging"];
			if (cs == null) return Content("Database not available!");

			List<String> ccAddrs = new List<string>();
			var p = (from e in DbUtil.Db.Users
						where e.UserId == Util.UserId
						select new
						{
							roles = string.Join(", ", e.Roles)
						}).SingleOrDefault();

			var who = Util.UserFullName + " <" + Util.UserEmail + ">";
			var from = "support-system@bvcms.com";
			var to = "support@bvcms.com";
			var subject = "Support Request: " + Util.UserFullName + " @ " + Util.Host + ".bvcms.com - " + DateTime.Now.ToString("g");
			var roles = ( p != null ? p.roles : "" );
			var ccto = req.cc != null && req.cc.Length > 0 ? "<b>CC:</b> " + req.cc + "<br>" : "";
			
			var cn = new SqlConnection(cs.ConnectionString);
			cn.Open();
			var cmd = new SqlCommand(SQLSupportInsert, cn);

			cmd.Parameters.AddWithValue("@c", DateTime.Now);
			cmd.Parameters.AddWithValue("@w", who);
			cmd.Parameters.AddWithValue("@h", Util.Host);
			cmd.Parameters.AddWithValue("@u", req.urgency);
			cmd.Parameters.AddWithValue("@r", req.body);
			cmd.Parameters.AddWithValue("@s", subject);

			int lastID = (int)cmd.ExecuteScalar();
			cn.Close();

			var body = "<b>Request ID:</b> " + lastID + "<br>" +
				 "<b>Request By:</b> " + Util.UserFullName + " (" + Util.UserEmail + ")<br>" +
				 "<b>Roles:</b> " + roles + "<br>" +
				 ccto +
				 "<b>Host:</b> https://" + Util.Host + ".bvcms.com<br>" +
				 "<b>Urgency:</b> " + req.urgency + "<br>" +
				 "<b>Last Search:</b> " + req.lastsearch + "<br>" +
				 "<b>Claim:</b> " + CreateDibs(lastID) + "<br><br>" +
				 req.body;

			var smtp = Util.Smtp();
			var email = new MailMessage(from, to, subject, body);
			email.ReplyToList.Add(who);
			email.ReplyToList.Add("support@bvcms.com");
			if (req.cc != null && req.cc.Length > 0)
			{
				var ccs = req.cc.Split(',');
				foreach (var addcc in ccs)
				{
					try
					{
						email.ReplyToList.Add(addcc);
						ccAddrs.Add(addcc);
					}
					catch (FormatException fe) {}
				}
			}
			email.IsBodyHtml = true;

			smtp.Send(email);

			var responseSubject = "Your BVCMS support request has been received";
			var responseBody = "Your support request has been received. We will respond to you as quickly as possible.<br><br>BVCMS Support Team";

			var response = new MailMessage("support@bvcms.com", Util.UserEmail, responseSubject, responseBody);
			response.IsBodyHtml = true;

			smtp.Send(response);

			if (DbUtil.AdminMail.Length > 0)
			{
				var toAdmin = new MailMessage("support@bvcms.com", DbUtil.AdminMail, subject, Util.UserFullName + " submitted a support request to BVCMS:<br><br>" + req.body);
				toAdmin.IsBodyHtml = true;

				smtp.Send(toAdmin);
			}

			foreach (var ccsend in ccAddrs)
			{
				var toCC = new MailMessage("support@bvcms.com", ccsend, subject, Util.UserFullName + " submitted a support request to BVCMS and CCed you:<br><br>" + req.body);
				toCC.IsBodyHtml = true;

				smtp.Send(toCC);
			}

			return Content("OK");
		}

#if DEBUG
		private static string DibClick = "<a href='http://test.bvcms.com/ExternalServices/BVCMSSupportLink?requestID={0}&supportPersonID={1}'>{2}</a>";
#else
		private static string DibClick = "<a href='https://bellevue.bvcms.com/ExternalServices/BVCMSSupportLink?requestID={0}&supportPersonID={1}'>{2}</a>";
#endif

		private string CreateDibs(int requestID)
		{
			List<string> dibLinks = new List<string>();

			for (int iX = 1; iX < SupportPeople.Length; iX++)
			{
				dibLinks.Add(DibClick.Fmt(requestID, iX, SupportPeople[iX]));
			}

			return String.Join(" - ", dibLinks);
		}
	}
}