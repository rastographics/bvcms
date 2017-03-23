using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using CmsData.Codes;
using CmsData.View;
using HtmlAgilityPack;
using Novacode;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private string CreateUserTag(EmailQueueTo emailqueueto)
        {
            User user = (from u in db.Users
                where u.PeopleId == emailqueueto.PeopleId
                select u).FirstOrDefault();
            if (user != null)
            {
                user.ResetPasswordCode = Guid.NewGuid();
                user.ResetPasswordExpires = Util.Now.AddHours(db.Setting("ResetPasswordExpiresHours", "24").ToInt());
                string link = db.ServerLink("/Account/SetPassword/" + user.ResetPasswordCode.ToString());
                db.SubmitChanges();
                return $@"<a href=""{link}"">Set password for {user.Username}</a>";
            }
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = emailqueueto.PeopleId.ToString()
            };
            db.OneTimeLinks.InsertOnSubmit(ot);
            db.SubmitChanges();
            var url = db.ServerLink($"/Account/CreateAccount/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">Create Account</a>";
        }

        private string RegisterLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var id = GetId(d, "RegisterLink");

            var showfamily = code.Contains("registerlink2", ignoreCase: true);
            var qs = $"{id},{emailqueueto.PeopleId},{emailqueueto.Id}";
            OneTimeLink ot;
            if (list.ContainsKey(qs))
                ot = list[qs];
            else
            {
                ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }
            var url = db.ServerLink($"/OnlineReg/RegisterLink/{ot.Id.ToCode()}");
            if (showfamily)
                url += "?showfamily=true";
            if (d.ContainsKey("style"))
                return $@"<a href=""{url}"" style=""{d["style"]}"">{inside}</a>";
            return $@"<a href=""{url}"">{inside}</a>";
        }

        private const string RegisterHrefReId = "href=\"https{0,1}://registerlink2{0,1}/(?<id>\\d+)\"";
        readonly Regex registerHrefReId = new Regex(RegisterHrefReId, RegexOptions.Singleline);

        private string RegisterLinkHref(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var match = registerHrefReId.Match(code);
            if (!match.Success)
                return code;
            var id = match.Groups["id"].Value.ToInt();

            var showfamily = code.Contains("registerlink2", ignoreCase: true);
            string qs = $"{id},{emailqueueto.PeopleId},{emailqueueto.Id}";
            OneTimeLink ot;
            if (list.ContainsKey(qs))
                ot = list[qs];
            else
            {
                ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }
            string url = db.ServerLink($"/OnlineReg/RegisterLink/{ot.Id.ToCode()}");
            if (showfamily)
                url += "?showfamily=true";
            return $"href=\"{url}\"";
        }

        private string RegisterTag(string code, EmailQueueTo emailqueueto)
        {
            var doc = new HtmlDocument();
            if (code.Contains("&lt;"))
                code = HttpUtility.HtmlDecode(code);
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.FirstChild;
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var id = ele.Id.ToInt();
            var url = RegisterLinkUrl(db, id, emailqueueto.PeopleId, emailqueueto.Id, "registerlink");
            return $@"<a href=""{url}"">{inside}</a>";
        }

        private string RsvpLink(string code, EmailQueueTo emailqueueto)
        {
            //<a dir="ltr" href="http://rsvplink" id="798" rel="meetingid" title="This is a message">test</a>
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var msg = "Thank you for responding.";
            if (d.ContainsKey("title"))
                msg = d["title"];

            string confirm = "false";
            if (d.ContainsKey("dir") && d["dir"] == "ltr")
                confirm = "true";

            string smallgroup = null;
            if (d.ContainsKey("rel"))
                smallgroup = d["rel"];

            var id = GetId(d, "RsvpLink");

            string qs = $"{id},{emailqueueto.PeopleId},{emailqueueto.Id},{smallgroup}";
            OneTimeLink ot;
            if (list.ContainsKey(qs))
                ot = list[qs];
            else
            {
                ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }
            var url = db.ServerLink(
                $"/OnlineReg/RsvpLinkSg/{ot.Id.ToCode()}?confirm={confirm}&message={HttpUtility.UrlEncode(msg)}");

            var href = d["href"];
            if (href.Contains("regretslink", ignoreCase: true))
                url = url + "&regrets=true";

            return $@"<a href=""{url}"">{inside}</a>";
        }

        private string SendLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var id = GetId(d, "SendLink");

            var showfamily = code.Contains("sendlink2", ignoreCase: true);
            var qs = $"{id},{emailqueueto.PeopleId},{emailqueueto.Id},{(showfamily ? "registerlink2" : "registerlink")}";

            OneTimeLink ot;
            if (list.ContainsKey(qs))
                ot = list[qs];
            else
            {
                ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }
            var url = db.ServerLink($"/OnlineReg/SendLink/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">{inside}</a>";
        }

        private string SupportLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var oid = GetId(d, "SupportLink");
            var qs = $"{oid},{emailqueueto.PeopleId},{emailqueueto.Id},{"supportlink"}:{emailqueueto.GoerSupportId}";

            OneTimeLink ot;
            if (list.ContainsKey(qs))
                ot = list[qs];
            else
            {
                ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }
            var url = db.ServerLink($"/OnlineReg/SendLink/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">{inside}</a>";
        }

        private string MasterLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var oid = GetId(d, "MasterLink");
            var qs = $"{oid},{emailqueueto.PeopleId},{emailqueueto.Id},{"masterlink"}";

            OneTimeLink ot;
            if (list.ContainsKey(qs))
                ot = list[qs];
            else
            {
                ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }
            var url = db.ServerLink($"/OnlineReg/SendLink/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">{inside}</a>";
        }

        private string UnSubscribeLink(EmailQueueTo emailqueueto)
        {
            var qs = "OptOut/UnSubscribe/?enc=" + Util.EncryptForUrl($"{emailqueueto.PeopleId}|{@from.Address}");
            var url = db.ServerLink(qs);
            return $@"<a href=""{url}"">Unsubscribe</a>";
        }

        private string VolReqLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var qs = $"{d["mid"]},{d["pid"]},{d["ticks"]},{emailqueueto.PeopleId}";
            OneTimeLink ot = null;
            if (list.ContainsKey(qs))
                ot = list[qs];
            else
            {
                ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }

            var url = db.ServerLink($"/OnlineReg/VolRequestResponse/{d["ans"]}/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">{inside}</a>";
        }

        private string VolSubLink(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var qs = $"{d["aid"]},{d["pid"]},{d["ticks"]},{emailqueueto.PeopleId}";
            OneTimeLink ot = null;
            if (list.ContainsKey(qs))
                ot = list[qs];
            else
            {
                ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs,
                    Expires = Util.Now.AddHours(72)
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }

            var url = db.ServerLink($"/OnlineReg/ClaimVolSub/{d["ans"]}/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">{inside}</a>";
        }

        private string VoteLink(string code, EmailQueueTo emailqueueto)
        {
            //<a dir="ltr" href="http://votelink" id="798" rel="smallgroup" title="This is a message">test</a>
            var list = new Dictionary<string, OneTimeLink>();

            var doc = new HtmlDocument();
            doc.LoadHtml(code);
            var ele = doc.DocumentNode.Element("a");
            var inside = ele.InnerHtml.Replace("{last}", person.LastName);
            var d = ele.Attributes.ToDictionary(aa => aa.Name.ToString(), aa => aa.Value);

            var msg = "Thank you for responding.";
            if (d.ContainsKey("title"))
                msg = d["title"];

            var confirm = "false";
            if (d.ContainsKey("dir") && d["dir"] == "ltr")
                confirm = "true";

            if (!d.ContainsKey("rel"))
                throw new Exception("Votelink: no smallgroup attribute");
            var smallgroup = d["rel"];
            var pre = "";
            var a = smallgroup.SplitStr(":");
            if (a.Length > 1)
                pre = a[0];

            var id = GetId(d, "VoteLink");

            var qs = $"{id},{emailqueueto.PeopleId},{emailqueueto.Id},{pre},{smallgroup}";
            OneTimeLink ot;
            if (list.ContainsKey(qs))
                ot = list[qs];
            else
            {
                ot = new OneTimeLink
                {
                    Id = Guid.NewGuid(),
                    Querystring = qs
                };
                db.OneTimeLinks.InsertOnSubmit(ot);
                db.SubmitChanges();
                list.Add(qs, ot);
            }
            var url = db.ServerLink(
                $"/OnlineReg/VoteLinkSg/{ot.Id.ToCode()}?confirm={confirm}&message={HttpUtility.UrlEncode(msg)}");
            return $@"<a href=""{url}"">{inside}</a>";
        }

        private static string GetId(IReadOnlyDictionary<string, string> d, string from)
        {
            string id = null;
            if (d.ContainsKey("lang"))
                id = d["lang"];
            else if (d.ContainsKey("id"))
                id = d["id"];
            if (id == null)
                throw new Exception($"No \"Organization Id\" attribute found on \"{@from}\"");
            return id;
        }

        private static List<string> SPECIAL_FORMATS = new List<string>()
        {
            "http://votelink",
            "https://votelink",
            "http://registerlink",
            "https://registerlink",
            "http://registerlink2",
            "https://registerlink2",
            "http://supportlink",
            "https://supportlink",
            "http://masterlink",
            "https://masterlink",
            "http://rsvplink",
            "https://rsvplink",
            "http://regretslink",
            "https://regretslink",
            "http://volsublink",
            "https://volsublink",
            "http://volreqlink",
            "https://volreqlink",
            "http://sendlink",
            "https://sendlink",
            "http://sendlink2",
            "https://sendlink2",
            "{emailhref}",
            "mailto"
        };

        public static bool IsSpecialLink(string link)
        {
            return SPECIAL_FORMATS.Contains(link.ToLower());
        }

        public static string RegisterLinkUrl(CMSDataContext db, int orgid, int pid, int queueid, string linktype, DateTime? expires = null)
        {
            var showfamily = linktype == "registerlink2";
            var qs = $"{orgid},{pid},{queueid},{linktype}";
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = qs,
            };
            if (expires.HasValue)
                ot.Expires = expires.Value;
            db.OneTimeLinks.InsertOnSubmit(ot);
            db.SubmitChanges();
            var url = db.ServerLink($"/OnlineReg/RegisterLink/{ot.Id.ToCode()}");
            if (showfamily)
                url += "?showfamily=true";
            return url;
        }

        public static string CreateRegisterLink(int? orgid, string text)
        {
            if (!orgid.HasValue)
                throw new ArgumentException("null not allowed on GetRegisterLink", nameof(orgid));
            return $"<a href=\"http://registerlink\" lang=\"{orgid}\">{text}</a>";
        }

    }
}