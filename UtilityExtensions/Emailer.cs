using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace UtilityExtensions
{
    public static partial class Util
    {
        public static void SendMsg(string sysFromEmail, string cmsHost, MailAddress fromAddress, string subject, string message, List<MailAddress> to, int id, int? pid, List<LinkedResource> attachments = null, List<MailAddress> cc = null)
        {
            if (ConfigurationManager.AppSettings["sendemail"] == "false")
                return;

            var senderrorsto = ConfigurationManager.AppSettings["senderrorsto"];
            var msg = new MailMessage();
            if (fromAddress == null)
                fromAddress = FirstAddress(senderrorsto);
            var problemDomains = (ConfigurationManager.AppSettings["ProblemDomainsForEmail"] ?? "").Split(',');
            if (problemDomains.Any(dd => fromAddress.Host.ToLower() == dd || to.Any(ee => ee.Host.ToLower() == dd)))
            {
                if (!sysFromEmail.HasValue())
                    sysFromEmail = "mailer@bvcms.com";
                var sysmail = new MailAddress(sysFromEmail, fromAddress.DisplayName);
                msg.From = sysmail;
                msg.ReplyToList.Add(fromAddress);
            }
            else
            {
                msg.From = fromAddress;
                if (sysFromEmail.HasValue())
                {
                    var sysmail = new MailAddress(sysFromEmail);
                    if (fromAddress.Host != sysmail.Host)
                        msg.Sender = sysmail;
                }
            }
            if (cc != null)
            {
                foreach (var a in cc)
                {
                    msg.ReplyToList.Add(a);
                }
                if (!msg.ReplyToList.Contains(msg.From) && msg.From.Address.NotEqual(sysFromEmail))
                {
                     msg.ReplyToList.Add(msg.From);
                }
            }

            msg.Headers.Add("X-SMTPAPI",
                $"{{\"unique_args\":{{\"host\":\"{cmsHost}\",\"mailid\":\"{id}\",\"pid\":\"{pid}\"}}}}");
            msg.Headers.Add("X-BVCMS", $"host:{cmsHost}, mailid:{id}, pid:{pid}");

            foreach (var ma in to)
            {
                if (ma.Host != "nowhere.name" || IsInRoleEmailTest)
                    msg.AddAddr(ma);
            }
            msg.Subject = subject;
            var addrs = string.Join(", ", to.Select(tt => tt.ToString()));
            var badEmailLink = "";
            if (msg.To.Count == 0 && to.Any(tt => tt.Host == "nowhere.name"))
                return;
            if (msg.To.Count == 0)
            {
                msg.AddAddr(msg.From);
                msg.AddAddr(FirstAddress(senderrorsto));
                msg.Subject += $"-- bad addr for {cmsHost}({pid})";
                badEmailLink = $"<p><a href='{cmsHost}/Person2/{pid}'>bad addr for</a></p>\n";
            }

            var regex = new Regex("</?([^>]*)>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var text = regex.Replace(message, string.Empty);
            var htmlView1 = AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Plain);
            htmlView1.TransferEncoding = TransferEncoding.Base64;
            msg.AlternateViews.Add(htmlView1);

 
            var html = badEmailLink + message;

            if (cc != null && cc.Count > 0)
            {
                string cclist = (string.Join(", ", cc));

                var ccstring = $"<p align='center'><small><i>This email was CC\'d to the email addresses below and they are included in the Reply-To Field.</br>" + cclist + "</i></small></p>";
                html = html + ccstring;
            }


            var result = PreMailer.Net.PreMailer.MoveCssInline(html);
            html = result.Html;
            var htmlView = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);
            htmlView.TransferEncoding = TransferEncoding.Base64;
            if (attachments != null)
                foreach (var a in attachments)
                    htmlView.LinkedResources.Add(a);
            msg.AlternateViews.Add(htmlView);

            try
            {
                var smtp = Smtp();
                smtp.Send(msg);
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
