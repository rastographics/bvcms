using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using UtilityExtensions;

namespace CmsData
{
    public partial class TextReplacements
    {
        // match all links generated in the unlayer special links prompt, and handle from there
        private const string MatchUnlayerLinkRe = @"https{0,1}://(?:rsvplink|regretslink|registerlink|registerlink2|sendlink|sendlink2|votelink)/\?[^\s,.;]*";
        private static readonly Regex UnlayerLinkRe = new Regex(MatchUnlayerLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly Dictionary<string, OneTimeLink> oneTimeLinkList = new Dictionary<string, OneTimeLink>();

        private string UnlayerLinkReplacement(string code, SMSItem item)
        {
            // remove the non url parts and reformat
            code = code.Replace("\"", string.Empty);
            code = code.Replace("&amp;", "&");
            code = HttpUtility.UrlDecode(code);

            // parse the special link url to get the component parts
            Uri specialLink = new Uri(code);
            string type = specialLink.Host;
            var querystring = HttpUtility.ParseQueryString(specialLink.Query);
            string orgId = querystring.Get("org");
            string meetingId = querystring.Get("meeting");
            string groupId = querystring.Get("group");
            string confirm = querystring.Get("confirm");
            string message = querystring.Get("msg");

            // result variables
            string qs;      // the unique link combination for the db

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
            switch (type)
            {
                case "rsvplink":
                case "regretslink":
                    qs = $"{meetingId},{item.PeopleID},{item.Id},{groupId}";
                    break;

                case "registerlink":
                case "registerlink2":
                    showfamily = (type == "registerlink2");
                    qs = $"{orgId},{item.PeopleID},{item.Id}";
                    break;

                case "sendlink":
                case "sendlink2":
                    showfamily = (type == "sendlink2");
                    qs = $"{orgId},{item.PeopleID},{item.Id},{(showfamily ? "registerlink2" : "registerlink")}";
                    break;
                case "votelink":
                    string pre = "";
                    var a = groupId.SplitStr(":");
                    if (a.Length > 1)
                    {
                        pre = a[0];
                    }
                    qs = $"{orgId},{item.PeopleID},{item.Id},{pre},{groupId}";
                    break;
                default:
                    return code;
            }

            var ot = CreateOrFetchOneTimeLink(qs, oneTimeLinkList);
            var url = CreateUrlForLink(type, ot, confirm, message, showfamily);

            return PythonModel.CreateTinyUrl(url);
        }

        private OneTimeLink CreateOrFetchOneTimeLink(string qs, Dictionary<string, OneTimeLink> list)
        {
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
                CurrentDatabase.OneTimeLinks.InsertOnSubmit(ot);
                CurrentDatabase.SubmitChanges();
                list.Add(qs, ot);
            }
            return ot;
        }

        private string CreateUrlForLink(string type, OneTimeLink ot, string confirm, string message, bool showfamily)
        {
            string url = "";
            switch (type)
            {
                case "rsvplink":
                case "regretslink":
                    url = CurrentDatabase.ServerLink(
                        $"/OnlineReg/RsvpLinkSg/{ot.Id.ToCode()}?confirm={confirm}&message={HttpUtility.UrlEncode(message)}");
                    if (type == "regretslink")
                    {
                        url += "&regrets=true";
                    }
                    break;

                case "registerlink":
                case "registerlink2":
                    url = CurrentDatabase.ServerLink($"/OnlineReg/RegisterLink/{ot.Id.ToCode()}");
                    if (showfamily)
                    {
                        url += "?showfamily=true";
                    }
                    break;

                case "sendlink":
                case "sendlink2":
                    url = CurrentDatabase.ServerLink($"/OnlineReg/SendLink/{ot.Id.ToCode()}");
                    break;

                case "votelink":
                    url = CurrentDatabase.ServerLink(
                        $"/OnlineReg/VoteLinkSg/{ot.Id.ToCode()}?confirm={confirm}&message={HttpUtility.UrlEncode(message)}");
                    break;
            }
            return url;
        }
    }
}
