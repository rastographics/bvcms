using CmsData;
using Dapper;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class SupportRequestModel
    {
        private readonly CMSDataContext _db;
        private readonly List<string> _ccAddrs = new List<string>();
        private readonly ConnectionStringSettings _cs = ConfigurationManager.ConnectionStrings["CmsLogging"];
        private readonly string _supportInsert = ConfigurationManager.AppSettings["SupportInsert"];
        private readonly string _supportUpdate = ConfigurationManager.AppSettings["SupportUpdate"];
        private readonly bool _useZenDeskApi = ConfigurationManager.AppSettings["UseZenDeskApi"] == "true";
        private readonly string _mydataRequest = "MyData Request";
        private const string SupportEmail = "support@touchpointsoftware.zendesk.com";

        public string Urgency { get; set; }
        private string Priority => Urgency == "1" ? "Critical" : Urgency == "2" ? "Normal" : "Low";
        public string Body { get; set; }
        public string LastSearch { get; set; }
        public string Cc { get; set; }
        public string Subj { get; set; }

        public SupportRequestModel()
        {
            _db = DbUtil.Db;
        }

        public static bool CanSupport => Util.IsHosted;

        public void SendSupportRequest()
        {
            if (_useZenDeskApi)
            {
                CreateZenDeskApiRequest();
                return;
            }
            var msg = CreateRequest();
            var smtp = _db.Smtp();
            smtp.Send(msg);
        }

        public void MyDataSendSupportRequest()
        {
            Subj = _mydataRequest;
            if (_useZenDeskApi)
            {
                CreateZenDeskApiRequest();
                return;
            }
            var msg = CreateRequest();
            var smtp = _db.Smtp();
            smtp.Send(msg);
        }

        private MailMessage CreateRequest()
        {
            var who = Util.UserFullName + " <" + Util.UserEmail + ">";
            var id = 0;
            var subject = $"{Urgency} {Subj}: {Util.UserFullName} @ {_db.Host}";
            if (_cs != null)
            {
                var cn = new SqlConnection(_cs.ConnectionString);
                cn.Open();

                id = cn.Query<int>(_supportInsert, new
                {
                    c = DateTime.Now,
                    w = who,
                    h = Util.Host,
                    u = Urgency,
                    r = Body,
                    whoid = Util.UserPeopleId
                }).Single();
                subject += $" [{id}]";

                cn.Execute(_supportUpdate, new { subject, id });
                cn.Close();
            }
            const string fromsupport = "Touchpoint Support <mailer@tpsdb.com>";

            var sb = new StringBuilder();
            sb.Append(
$@"<b>Request ID: {id}</b><br>
<b>Request By:</b> {Util.UserFullName} ({Util.UserEmail})<br>
<b>Priority: {Priority}</b><br>
<b>Host:</b> https://{Util.Host}.tpsdb.com<br>");

            if (Subj != _mydataRequest)
            {
                var roles = (from e in _db.Users
                             where e.UserId == Util.UserId
                             select string.Join(", ", e.Roles)).SingleOrDefault();

                var ccto = !string.IsNullOrEmpty(Cc) ? $@"<b>CC:</b> {Cc}<br>" : "";
                sb.Append($"<b>Roles:</b> {roles}<br>\n{ccto}<hr>");
            }
            sb.Append(Body);

            var msg = new MailMessage(fromsupport, SupportEmail, subject, sb.ToString());
            msg.ReplyToList.Add(who);
            if (!string.IsNullOrEmpty(Cc))
            {
                var ccs = Cc.Split(',');
                foreach (var addcc in ccs)
                {
                    var email = addcc.Trim();
                    if (Util.ValidEmail(email))
                    {
                        msg.CC.Add(email);
                    }
                }
            }
            if (DbUtil.AdminMail.Length > 0)
            {
                msg.CC.Add(DbUtil.AdminMail);
            }

            msg.IsBodyHtml = true;
            msg.Headers.Add("X-BVCMS-SUPPORT", "request");

            return msg;
        }

        private void CreateZenDeskApiRequest()
        {
            var who = Util.UserFullName + " <" + Util.UserEmail + ">";
            var id = 0;
            var subject = $"{Subj}";
            if (_cs != null)
            {
                var cn = new SqlConnection(_cs.ConnectionString);
                cn.Open();

                id = cn.Query<int>(_supportInsert, new
                {
                    c = DateTime.Now,
                    w = who,
                    h = Util.Host,
                    u = Urgency,
                    r = Body,
                    whoid = Util.UserPeopleId
                }).Single();
                subject += $" [{id}]";

                cn.Execute(_supportUpdate, new { subject, id });
                cn.Close();
            }
            var reqbody = new StringBuilder();
            reqbody.AppendFormat(@"<b>Request ID: {0}</b><br>
                            <b>Request By:</b> {1} ({2})<br>
                            <b>Host:</b> https://{3}.tpsdb.com<br>
                            ", id, Util.UserFullName, Util.UserEmail, Util.Host);

            if (Subj != _mydataRequest)
            {
                var p = (from e in _db.Users
                         where e.UserId == Util.UserId
                         select new
                         {
                             roles = string.Join(", ", e.Roles)
                         }).SingleOrDefault();

                var roles = (p != null ? p.roles : "");
                var ccto = !string.IsNullOrEmpty(Cc) ? $@"<b>CC:</b> {Cc}<br>" : "";
                reqbody.AppendFormat($@"<b>Roles:</b> {roles}<br>\n{ccto}");
            }
            reqbody.Append(Body);

            var client = new RestClient("https://touchpointsoftware.zendesk.com/api/v2/tickets.json");
            var request = new RestRequest(Method.POST);
            var priority = Urgency == "1" ? "urgent" : Urgency == "2" ? "normal" : "low";
            var collaborators = "";
            if (!string.IsNullOrEmpty(Cc))
            {
                var ccs = Cc.Split(',');
                foreach (var addcc in ccs)
                {
                    try
                    {
                        if (Util.ValidEmail(addcc))
                        {
                            _ccAddrs.Add($"\"{addcc.Trim()}\"");
                        }
                    }
                    catch (FormatException)
                    {
                    }
                }
                if (_ccAddrs.Count > 0)
                {
                    collaborators = $"\n\t\t\"collaborators\": [ {string.Join(",", _ccAddrs)} ],";
                }
            }
            var escapedbody = HttpUtility.JavaScriptStringEncode(reqbody.ToString());

            var data =
$@"{{
    ""ticket"": {{
        ""requester"": {{
            ""name"": ""{Util.UserFullName}"",
            ""email"": ""{Util.UserEmail}""
        }},
        ""requester_id"": {Util.UserPeopleId},
        ""subject"": ""{subject}"",
        ""comment"": {{ ""body"": ""{escapedbody}"" }},
        ""external_id"": {id},
        ""priority"": ""{priority}"",
        ""created_at"": ""{DateTime.Now:o}"",{collaborators}
        ""status"": ""open""
    }}
}}";
            request.AddParameter("application/json", data, ParameterType.RequestBody);

            var apitoken = ConfigurationManager.AppSettings["ZenDeskApiToken"];
            var user = ConfigurationManager.AppSettings["ZenDeskApiUser"];
            var authorization = $"{user}/token:{apitoken}";
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization));

            request.AddHeader("authorization", $"Basic {encoded}");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");

            client.Execute(request);
        }
    }
}
