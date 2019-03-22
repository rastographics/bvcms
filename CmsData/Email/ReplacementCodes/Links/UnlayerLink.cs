using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        // match all links generated in the unlayer special links prompt, and handle from there
        private const string MatchUnlayerLinkRe = @"""https://(?:rsvplink|regretslink|registerlink|registerlink2|sendlink|sendlink2|supportlink|votelink)/\?.*?""";
        private static readonly Regex UnlayerLinkRe = new Regex(MatchUnlayerLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string UnlayerLinkReplacement(string code, EmailQueueTo emailqueueto)
        {
            var list = new Dictionary<string, OneTimeLink>();

            // remove the non url parts and reformat
            code = code.Replace("\"", string.Empty);
            code = code.Replace("&amp;", "&");
            code = HttpUtility.UrlDecode(code);

            // parse the special link url to get the component parts
            Uri SpecialLink = new Uri(code);
            string type = SpecialLink.Host;
            string orgId = HttpUtility.ParseQueryString(SpecialLink.Query).Get("org");
            string meetingId = HttpUtility.ParseQueryString(SpecialLink.Query).Get("meeting");
            string groupId = HttpUtility.ParseQueryString(SpecialLink.Query).Get("group");
            string confirm = HttpUtility.ParseQueryString(SpecialLink.Query).Get("confirm");
            string message = HttpUtility.ParseQueryString(SpecialLink.Query).Get("msg");

            // result variables
            string qs;      // the unique link combination for the db
            string url = "";

            // set some defaults for any missing properties
            bool showfamily = false;
            if (!message.HasValue())
            {
                message = "Thank you for responding.";
            }
            if (!confirm.HasValue())
            {
                confirm = "false";
            }

            // generate the one time link code and update any vars based on the link type
            switch(type)
            {
                case "rsvplink":
                case "regretslink":
                    qs = $"{meetingId},{emailqueueto.PeopleId},{emailqueueto.Id},{groupId}";
                    break;

                case "registerlink":
                case "registerlink2":
                    showfamily = (code == "registerlink2");
                    qs = $"{orgId},{emailqueueto.PeopleId},{emailqueueto.Id}";
                    break;

                case "sendlink":
                case "sendlink2":
                    showfamily = (code == "sendlink2");
                    qs = $"{orgId},{emailqueueto.PeopleId},{emailqueueto.Id},{(showfamily ? "registerlink2" : "registerlink")}";
                    break;
                case "supportlink":
                    qs = $"{orgId},{emailqueueto.PeopleId},{emailqueueto.Id},{"supportlink"}:{emailqueueto.GoerSupportId}";
                    break;
                case "votelink":
                    string pre = "";
                    var a = groupId.SplitStr(":");
                    if (a.Length > 1)
                    {
                        pre = a[0];
                    }
                    qs = $"{orgId},{emailqueueto.PeopleId},{emailqueueto.Id},{pre},{groupId}";
                    break;
                default:
                    return code;
            }

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

            // generate the one time link url based on the one time link id
            switch (type)
            {
                case "rsvplink":
                case "regretslink":
                    url = db.ServerLink($"/OnlineReg/RsvpLinkSg/{ot.Id.ToCode()}?confirm={confirm}&message={HttpUtility.UrlEncode(message)}");
                    if (code == "regretslink")
                    {
                        url += "&regrets=true";
                    }
                    break;

                case "registerlink":
                case "registerlink2":
                    url = db.ServerLink($"/OnlineReg/RegisterLink/{ot.Id.ToCode()}");
                    if (showfamily)
                    {
                        url += "?showfamily=true";
                    }
                    break;

                case "sendlink":
                case "sendlink2":
                case "supportlink":
                    url = db.ServerLink($"/OnlineReg/SendLink/{ot.Id.ToCode()}");
                    break;

                case "votelink":
                    url = db.ServerLink($"/OnlineReg/VoteLinkSg/{ot.Id.ToCode()}?confirm={confirm}&message={HttpUtility.UrlEncode(message)}");
                    break;
            }
            return url;
        }

    }
}
