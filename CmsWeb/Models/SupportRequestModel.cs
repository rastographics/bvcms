using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CmsData;
using CmsData.View;
using Dapper;
using UtilityExtensions;
using System.Net.Mail;

namespace CmsWeb.Models
{
    public class SupportRequestModel
    {
        public string urgency { get; set; }
        public string body { get; set; }
        public string lastsearch { get; set; }
        public string cc { get; set; }

        private readonly string SupportInsert = ConfigurationManager.AppSettings["SupportInsert"];
        private readonly string SupportUpdate = ConfigurationManager.AppSettings["SupportUpdate"];
        private readonly string SupportUpdate2 = ConfigurationManager.AppSettings["SupportUpdate2"];
        private readonly string SupportRead = ConfigurationManager.AppSettings["SupportRead"];
        private readonly string DibLink = ConfigurationManager.AppSettings["DibLink"];
        private readonly string ManageLink = ConfigurationManager.AppSettings["SupportManageLink"];
        private readonly ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["CmsLogging"];
        private readonly List<DbUtil.SupportPerson> supportPeople
                = DbUtil.Supporters(ConfigurationManager.AppSettings["SupportPeople"]);

        public static bool CanSupport
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["CmsLogging"];
                var supporters = ConfigurationManager.AppSettings["SupportPeople"];
                return cs != null && supporters != null;
            }
        }

        private readonly List<string> ccAddrs = new List<string>();
        public void SendSupportRequest()
        {
            const string to = "support@bvcms.com";

            var msg = CreateRequest("Support Request", to);

            if (Util.UserPeopleId.HasValue)
            {
                var c = Contact.AddContact(DbUtil.Db, Util.UserPeopleId.Value, DateTime.Now, "<p>{0}</p>{1}".Fmt(msg.Subject, body));
                c.LimitToRole = "Admin";
                c.MinistryId = Contact.FetchOrCreateMinistry(DbUtil.Db, "BVCMS Support").MinistryId;
                DbUtil.Db.SubmitChanges();
            }

            var smtp = Util.Smtp();
            smtp.Send(msg);

            const string responseSubject = "Your BVCMS support request has been received";
            const string responseBody = "Your support request has been received. We will respond to you as quickly as possible.<br><br>BVCMS Support Team";

            var response = new MailMessage("support@bvcms.com", Util.UserEmail, responseSubject, responseBody)
                { IsBodyHtml = true };

            smtp.Send(response);

            if (DbUtil.AdminMail.Length > 0)
            {
                var toAdmin = new MailMessage("support@bvcms.com", DbUtil.AdminMail, msg.Subject, Util.UserFullName + " submitted a support request to BVCMS:<br><br>" + body)
                    { IsBodyHtml = true };
                smtp.Send(toAdmin);
            }

            foreach (var ccsend in ccAddrs)
            {
                var toCC = new MailMessage("support@bvcms.com", ccsend, msg.Subject, Util.UserFullName + " submitted a support request to BVCMS and CCed you:<br><br>" + body)
                    { IsBodyHtml = true };
                smtp.Send(toCC);
            }
        }

        public void MyDataSendSupportRequest()
        {
            var to = DbUtil.AdminMail;
            var msg = CreateRequest("BVCMS MyData Request", to);
            var smtp = Util.Smtp();
            smtp.Send(msg);
        }

        private MailMessage CreateRequest(string prefix, string toaddress)
        {
            var who = Util.UserFullName + " <" + Util.UserEmail + ">";
            int id = 0;
            var subject = prefix + (urgency.HasValue() ? " {0}: ".Fmt(urgency) : ": ")
                          + "{0} @ {1}".Fmt(Util.UserFullName, DbUtil.Db.Host);
            if(cs != null)
            {
                var cn = new SqlConnection(cs.ConnectionString);
                cn.Open();

                id = cn.Query<int>(SupportInsert, new
                {
                    c = DateTime.Now,
                    w = who,
                    h = Util.Host,
                    u = urgency,
                    r = body,
                    whoid = Util.UserPeopleId,
                }).Single();
                subject += "{0} [{1}]".Fmt(subject, id);

                cn.Execute(SupportUpdate, new { subject, id });
                cn.Close();
            }
            const string @from = "support-system@bvcms.com";

            var sb = new StringBuilder();
            sb.AppendFormat(@"<b>Request ID: {0}</b><br>
                            <b>Request By:</b> {1} ({2})<br>
                            <b>Host:</b> https://{3}.bvcms.com<br>
                            ", id, Util.UserFullName, Util.UserEmail, Util.Host);

            if (!prefix.Contains("MyData"))
            {
                var p = (from e in DbUtil.Db.Users
                         where e.UserId == Util.UserId
                         select new
                         {
                             roles = string.Join(", ", e.Roles)
                         }).SingleOrDefault();

                var roles = (p != null ? p.roles : "");
                var ccto = !string.IsNullOrEmpty(cc) ? "<b>CC:</b> " + cc + "<br>" : "";
                sb.AppendFormat(@"<b>Roles:</b> {0}<br>
                                <b>CC:</b> {1}<br>
                                <b>Last Search:</b> {2}<br>
                                <b>Claim:</b> <a href='{3}'>Manage Support</a>
                                {4}
                                ", roles, ccto, lastsearch, ManageLink, CreateDibs(id));
            }
            sb.Append(body);

            var msg = new MailMessage(from, toaddress, subject, sb.ToString());
            if (!string.IsNullOrEmpty(cc))
            {
                var ccs = cc.Split(',');
                foreach (var addcc in ccs)
                {
                    try
                    {
                        msg.ReplyToList.Add(addcc);
                        ccAddrs.Add(addcc);
                    }
                    catch (FormatException) { }
                }
            }
            msg.To.Add("support@bvcms.com");
            msg.ReplyToList.Add(who);
            msg.ReplyToList.Add("support@bvcms.com");
            msg.IsBodyHtml = true;
            msg.Headers.Add("X-BVCMS-SUPPORT", "request");

            return msg;
        }
        private string CreateDibs(int requestID)
        {
            string diblink = DibLink;
            var dibLinks = supportPeople.Select(s =>
                "<a href='{0}'>{1}</a>".Fmt(diblink.Fmt(requestID, s.id), s.name)
                ).ToList();
            var sb = new StringBuilder("<br><table cellpadding=5>\n");
            var closetr = "";
            for(var i = 0;i<dibLinks.Count;i++)
            {
                var a = dibLinks[i];
                if (i%4 == 0)
                {
                    sb.Append("{0}<tr>".Fmt(closetr));
                    closetr = "</tr>";
                }
                sb.AppendFormat("<td>{0}</td>", a);
            }
            sb.Append("</tr></table>");
            return sb.ToString();
        }
    }
}
