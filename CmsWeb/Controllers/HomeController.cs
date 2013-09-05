using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using AttributeRouting.Web.Mvc;
using CmsData;
using System.Diagnostics;
using CmsData.API;
using CmsData.Registration;
using CmsWeb.Areas.People.Models.Person;
using UtilityExtensions;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.SqlClient;
using System.Net.Mail;
using CmsWeb.Models;
using System.Configuration;
using System.Data;

namespace CmsWeb.Controllers
{
	public class HomeController : CmsStaffController
	{
		public ActionResult Index()
		{
			if (!Util2.OrgMembersOnly && User.IsInRole("OrgMembersOnly"))
			{
				Util2.OrgMembersOnly = true;
				DbUtil.Db.SetOrgMembersOnly();
			}
			else if (!Util2.OrgLeadersOnly && User.IsInRole("OrgLeadersOnly"))
			{
				Util2.OrgLeadersOnly = true;
				DbUtil.Db.SetOrgLeadersOnly();
			}
			var m = new HomeModel();
			return View(m);
		}
        [GET("Person/TinyImage/{id}")]
        [GET("Person2/TinyImage/{id}")]
        [GET("TinyImage/{id}")]
        public ActionResult TinyImage(int id)
        {
            return new PictureResult(id, portrait: true, tiny: true);
        }
        [GET("Person/Image/{id:int}/{w:int?}/{h:int?}")]
        [GET("Person2/Image/{id:int}/{w:int?}/{h:int?}")]
        [GET("Image/{id:int}/{w:int?}/{h:int?}")]
        public ActionResult Image(int id, int? w, int? h)
        {
            return new PictureResult(id);
        }
		public ActionResult About()
		{
			return View();
		}
		[ValidateInput(false)]
		public ActionResult ShowError(string error, string url)
		{
			ViewData["error"] = Server.UrlDecode(error);
			ViewData["url"] = url;
			return View();
		}
		public ActionResult NewQuery()
		{
			var qb = DbUtil.Db.QueryBuilderScratchPad();
			qb.CleanSlate(DbUtil.Db);
			return Redirect("/QueryBuilder/Main");
		}
		public ActionResult Test()
		{
			string test = null;
			var x = test.Replace('3', '4');
			return Content("done");
		}
		public ActionResult RecordTest(int id, string v)
		{
			var o = DbUtil.Db.LoadOrganizationById(id);
			o.AddEditExtra(DbUtil.Db, "tested", v);
			DbUtil.Db.SubmitChanges();
			return Content(v);
		}
		public ActionResult NthTimeAttenders(int id)
		{
			var name = "VisitNumber-" + id;
			var qb = DbUtil.Db.QueryBuilderClauses.FirstOrDefault(c => c.IsPublic && c.Description == name && c.SavedBy == "public");
			if (qb == null)
			{
				qb = DbUtil.Db.QueryBuilderScratchPad();
				qb.CleanSlate(DbUtil.Db);

				var comp = CompareType.Equal;
				QueryBuilderClause clause = null;
				switch (id)
				{
					case 1:
						clause = qb.AddNewClause(QueryType.RecentVisitNumber, comp, "1,T");
						clause.Quarters = "1";
						clause.Days = 7;
						break;
					case 2:
						clause = qb.AddNewClause(QueryType.RecentVisitNumber, comp, "1,T");
						clause.Quarters = "2";
						clause.Days = 7;
						clause = qb.AddNewClause(QueryType.RecentVisitNumber, comp, "0,F");
						clause.Quarters = "1";
						clause.Days = 7;
						break;
					case 3:
						clause = qb.AddNewClause(QueryType.RecentVisitNumber, comp, "1,T");
						clause.Quarters = "3";
						clause.Days = 7;
						clause = qb.AddNewClause(QueryType.RecentVisitNumber, comp, "0,F");
						clause.Quarters = "2";
						clause.Days = 7;
						break;
				}
				qb = qb.SaveTo(DbUtil.Db, name, "public", true);
			}
			TempData["autorun"] = true;
			return Redirect("/QueryBuilder/Main/{0}".Fmt(qb.QueryId));
		}
		[Authorize(Roles = "Admin")]
		public ActionResult ActiveRecords()
		{
			TempData["ActiveRecords"] = DbUtil.Db.ActiveRecords();
			return View("About");
		}
		public ActionResult TargetPerson(bool id)
		{
			DbUtil.Db.SetUserPreference("TargetLinkPeople", id ? "false" : "true");
			DbUtil.Db.SubmitChanges();
			if (Request.UrlReferrer != null)
				return Redirect(Request.UrlReferrer.OriginalString);
			return Redirect("/");
		}

		public ActionResult ToggleSupport()
		{
			var usesupport = DbUtil.Db.UserPreference("UseNewSupport", "false").ToBool();
			DbUtil.Db.SetUserPreference("UseNewSupport", usesupport ? "false" : "true");
			DbUtil.Db.SubmitChanges();
			if (Request.UrlReferrer != null)
				return Redirect(Request.UrlReferrer.OriginalString);
			return Redirect("/");
		}
		public ActionResult Names(string term)
		{
			var q = HomeModel.Names(term).ToList();
			return Json(q, JsonRequestBehavior.AllowGet);
		}
		public ActionResult Names2(string query)
		{
			var q = HomeModel.Names2(query).ToList();
			return Json(q, JsonRequestBehavior.AllowGet);
		}
		public ActionResult TestRegs()
		{
			foreach (var o in DbUtil.Db.Organizations)
			{
				try
				{
					var rs = new Settings(o.RegSetting, DbUtil.Db, o.OrganizationId);
				}
				catch (Exception ex)
				{
					return Content("bad org <a href=\"{0}{1}\">{2}</a>\n{3}".Fmt(Util.ServerLink("/RegSetting/Index/"), o.OrganizationId, o.OrganizationName, ex.Message));
				}
			}
			return Content("ok");
		}

		public static string[] SupportPeople = { "Unclaimed", "Bethany", "David", "Karen", "Kyle", "Steven" };
		public static string SQLSupportInsert = "INSERT INTO [dbo].[SupportRequests] ( Created, Who, Host, Urgency, Request, Subject ) OUTPUT INSERTED.ID VALUES ( @c, @w, @h, @u, @r, @s )";

		public ActionResult SendSupportRequest(string urgency, string request, string search)
		{
			var cs = ConfigurationManager.ConnectionStrings["CmsLogging"];
			if (cs == null) return Content("Database not available!");

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

			var cn = new SqlConnection(cs.ConnectionString);
			cn.Open();
			var cmd = new SqlCommand(SQLSupportInsert, cn);

			cmd.Parameters.AddWithValue("@c", DateTime.Now);
			cmd.Parameters.AddWithValue("@w", who);
			cmd.Parameters.AddWithValue("@h", Util.Host);
			cmd.Parameters.AddWithValue("@u", urgency);
			cmd.Parameters.AddWithValue("@r", request);
			cmd.Parameters.AddWithValue("@s", subject);

			int lastID = (int)cmd.ExecuteScalar();
			cn.Close();

			var body = "<b>Request ID:</b> " + lastID + "<br>" +
				 "<b>Request By:</b> " + Util.UserFullName + " (" + Util.UserEmail + ")<br>" +
				 "<b>Roles:</b> " + roles + "<br>" +
				 "<b>Host:</b> https://" + Util.Host + ".bvcms.com<br>" +
				 "<b>Urgency:</b> " + urgency + "<br>" +
				 "<b>Last Search:</b> " + search + "<br>" +
				 "<b>Claim:</b> " + CreateDibs(lastID) + "<br><br>" +
				 request;

			var smtp = Util.Smtp();
			var email = new MailMessage(from, to, subject, body);
			email.ReplyToList.Add(who);
			email.ReplyToList.Add("support@bvcms.com");
			email.IsBodyHtml = true;

			smtp.Send(email);

			var responseSubject = "Your BVCMS support request has been received";
			var responseBody = "Your support request has been received. We will respond to you as quickly as possible.<br><br>BVCMS Support Team";

			var response = new MailMessage("support@bvcms.com", Util.UserEmail, responseSubject, responseBody);
			response.IsBodyHtml = true;

			smtp.Send(response);

			if (DbUtil.AdminMail.Length > 0)
			{
				var toAdmin = new MailMessage("support@bvcms.com", DbUtil.AdminMail, subject, Util.UserFullName + " submitted a support request to BVCMS:<br><br>" + request);
				toAdmin.IsBodyHtml = true;

				smtp.Send(toAdmin);
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

