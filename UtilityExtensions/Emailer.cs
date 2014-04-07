using System;
using System.Configuration;
using System.Text;
using System.Linq;
using System.Net.Mail;
using System.Web.Configuration;
using System.IO;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web;
using System.Threading;

namespace UtilityExtensions
{
	public static partial class Util
	{
	    public static void SendMsg(string SysFromEmail, string CmsHost, MailAddress From, string subject, string Message, List<MailAddress> to, int id, int? pid)
		{
			if (ConfigurationManager.AppSettings["sendemail"] == "false")
				return;
			sendmsg(SysFromEmail, CmsHost, From, subject, Message, to, id, pid);
			return;
		}
		public static void sendmsg(string SysFromEmail, string CmsHost, MailAddress From, string subject, string Message, List<MailAddress> to, int id, int? pid, List<LinkedResource> attachments = null)
		{
			var senderrorsto = ConfigurationManager.AppSettings["senderrorsto"];
			var msg = new MailMessage();
			if (From == null)
				From = Util.FirstAddress(senderrorsto);

		    if (From.Host == "yahoo.com" || to.Any(ee => ee.Host == "yahoo.com"))
		    {
		        if (!SysFromEmail.HasValue())
		            SysFromEmail = "mailer@bvcms.com";
				var sysmail = new MailAddress(SysFromEmail, From.DisplayName);
    			msg.From = sysmail;
                msg.ReplyToList.Add(From);
		    }
		    else
		    {
    			msg.From = From;
    			if (SysFromEmail.HasValue())
    			{
    				var sysmail = new MailAddress(SysFromEmail);
    				if (From.Host != sysmail.Host)
    					msg.Sender = sysmail;
    			}
		    }

			msg.Headers.Add("X-SMTPAPI",
				"{{\"unique_args\":{{\"host\":\"{0}\",\"mailid\":\"{1}\",\"pid\":\"{2}\"}}}}"
				.Fmt(CmsHost, id, pid));
			msg.Headers.Add("X-BVCMS", "host:{0}, mailid:{1}, pid:{2}".Fmt(CmsHost, id, pid));

			foreach (var ma in to)
			{
				if (ma.Host != "nowhere.name" || IsInRoleEmailTest)
					msg.AddAddr(ma);
			}
			msg.Subject = subject;
			var addrs = string.Join(", ", to.Select(tt => tt.ToString()));
			var BadEmailLink = "";
			if (msg.To.Count == 0)
			{
				msg.AddAddr(msg.From);
				msg.AddAddr(Util.FirstAddress(senderrorsto));
				msg.Subject += "-- bad addr for {0}({1})".Fmt(CmsHost, pid);
				BadEmailLink = "<p><a href='{0}Person2/{1}'>bad addr for</a></p>\n".Fmt(CmsHost, pid);
			}

			var regex = new Regex("</?([^>]*)>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			var text = regex.Replace(Message, string.Empty);
			var htmlView1 = AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Plain);
			htmlView1.TransferEncoding = TransferEncoding.Base64;
			msg.AlternateViews.Add(htmlView1);

			var html = BadEmailLink + Message;
			var htmlView = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);
			htmlView.TransferEncoding = TransferEncoding.Base64;
			if (attachments != null)
				foreach (var a in attachments)
					htmlView.LinkedResources.Add(a);
			msg.AlternateViews.Add(htmlView);

			try
			{
				var smtp = Util.Smtp();
				smtp.Send(msg);
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				htmlView.Dispose();
			}
		}
		private static void AddAddr(this MailMessage msg, MailAddress a)
		{
			if (IsInRoleEmailTest)
				a = new MailAddress(UserEmail, a.DisplayName + " (test)");
			msg.To.Add(a);
		}
		public static bool IsInRoleEmailTest
		{
			get
			{
			    if (HttpContext.Current != null)
			        return HttpContext.Current.User.IsInRole("EmailTest") || ((bool?)HttpContext.Current.Session["IsInRoleEmailTest"] ?? false);
				return (bool?)Thread.GetData(Thread.GetNamedDataSlot("IsInRoleEmailTest")) ?? false;
			}
			set
			{
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current.Session != null)
						HttpContext.Current.Session["IsInRoleEmailTest"] = value;
				}
				else
					Thread.SetData(Thread.GetNamedDataSlot("IsInRoleEmailTest"), value);
			}
		}
		private const string STR_UserEmail = "UserEmail";
		public static string UserEmail
		{
			get
			{
				string email = null;
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current.Session != null)
						if (HttpContext.Current.Session[STR_UserEmail] != null)
							email = HttpContext.Current.Session[STR_UserEmail] as String;
				}
				else
					email = (string)Thread.GetData(Thread.GetNamedDataSlot("UserEmail"));
				return email;
			}
			set
			{
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current.Session != null)
						HttpContext.Current.Session[STR_UserEmail] = value;
				}
				else
					Thread.SetData(Thread.GetNamedDataSlot(STR_UserEmail), value);
			}
		}

	}
}