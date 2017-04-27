using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using CmsData;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class SupportRequestModel
    {
        private readonly List<string> ccAddrs = new List<string>();
        private readonly ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["CmsLogging"];
        private readonly string DibLink = ConfigurationManager.AppSettings["DibLink"];
        private readonly string ManageLink = ConfigurationManager.AppSettings["SupportManageLink"];
        private readonly string SupportInsert = ConfigurationManager.AppSettings["SupportInsert"];

        private readonly List<DbUtil.SupportPerson> supportPeople
            = DbUtil.Supporters(ConfigurationManager.AppSettings["SupportPeople"]);

        private readonly string SupportRead = ConfigurationManager.AppSettings["SupportRead"];
        private readonly string SupportUpdate = ConfigurationManager.AppSettings["SupportUpdate"];
        private readonly string SupportUpdate2 = ConfigurationManager.AppSettings["SupportUpdate2"];
        public string urgency { get; set; }
        public string body { get; set; }
        public string lastsearch { get; set; }
        public string cc { get; set; }
        public string subj { get; set; }

        public static bool CanSupport => Util.IsHosted;

        public void SendSupportRequest()
        {
            const string fromsupport = "Touchpoint Support <mailer@bvcms.com>";
            const string to = "support@touchpointsoftware.com";

            var msg = CreateRequest(subj, to);

            var smtp = DbUtil.Db.Smtp();
            smtp.Send(msg);

            const string responseSubject = "Your TouchPoint support request has been received";
            const string responseBody = "Your support request has been received. We will respond to you as quickly as possible.<br><br>TouchPoint Support Team";

            var response = new MailMessage(fromsupport, Util.UserEmail, responseSubject, responseBody)
            { IsBodyHtml = true };

            smtp.Send(response);

            if (DbUtil.AdminMail.Length > 0)
            {
                var toAdmin = new MailMessage(fromsupport, DbUtil.AdminMail, msg.Subject, Util.UserFullName + " submitted a support request to TouchPoint:<br><br>" + body)
                { IsBodyHtml = true };
                smtp.Send(toAdmin);
            }

//            foreach (var ccsend in ccAddrs)
//            {
//                var toCC = new MailMessage(fromsupport, ccsend, msg.Subject, Util.UserFullName + " submitted a support request to TouchPoint and CCed you:<br><br>" + body)
//                { IsBodyHtml = true };
//                smtp.Send(toCC);
//            }
        }

        public void MyDataSendSupportRequest()
        {
            var to = DbUtil.AdminMail;
            var msg = CreateRequest("TouchPoint MyData Request", to);
            var smtp = DbUtil.Db.Smtp();
            smtp.Send(msg);
        }

        private MailMessage CreateRequest(string prefix, string toaddress)
        {
            var who = Util.UserFullName + " <" + Util.UserEmail + ">";
            var id = 0;
            var subject = $"{prefix} --{urgency}: {Util.UserFullName} @ {DbUtil.Db.Host}";
            if (cs != null)
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
                    whoid = Util.UserPeopleId
                }).Single();
                subject += $" [{id}]";

                cn.Execute(SupportUpdate, new { subject, id });
                cn.Close();
            }
            const string fromsupport = "Touchpoint Support <mailer@bvcms.com>";

            var sb = new StringBuilder();
            sb.AppendFormat(@"<b>Request ID: {0}</b><br>
                            <b>Request By:</b> {1} ({2})<br>
                            <b>Host:</b> https://{3}.tpsdb.com<br>
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
                var ccto = !string.IsNullOrEmpty(cc) ? $@"<b>CC:</b> {cc}<br>" : "";
                sb.AppendFormat(@"<b>Roles:</b> {0}<br>
                                {1}<a href='{2}'>Manage Support</a>
                                ", roles, ccto, ManageLink);
            }
            sb.Append(body);

            var msg = new MailMessage(fromsupport, toaddress, subject, sb.ToString());
            msg.ReplyToList.Add(who);
            if (!string.IsNullOrEmpty(cc))
            {
                var ccs = cc.Split(',');
                foreach (var addcc in ccs)
                {
                    try
                    {
                        ccAddrs.Add(addcc);
                        msg.CC.Add(addcc);
                    }
                    catch (FormatException)
                    {
                    }
                }
            }
            if (prefix.Contains("MyData"))
                msg.To.Add("support@touchpointsoftware.com");
            msg.IsBodyHtml = true;
            msg.Headers.Add("X-BVCMS-SUPPORT", "request");

            return msg;
        }
    }
}
