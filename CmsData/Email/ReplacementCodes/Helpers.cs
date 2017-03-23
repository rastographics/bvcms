using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using CmsData.Codes;
using CmsData.View;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private readonly Dictionary<int, string> fundnames = new Dictionary<int, string>();


        private string DoInsertDrafts(string text)
        {
            var a = Regex.Split(text, "({insertdraft:.*?})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (a.Length <= 2)
                return text;
            for (var i = 1; i < a.Length; i += 2)
                if (a[i].StartsWith("{insertdraft:"))
                    a[i] = InsertDraft(a[i]);
            text = string.Join("", a);
            return text;
        }

        private class PayInfo
        {
            public string PayLink { get; set; }
            public decimal? Amount { get; set; }
            public decimal? AmountPaid { get; set; }
            public decimal? AmountDue { get; set; }
            public string RegisterMail { get; set; }
        }

        private string InsertDraft(string code)
        {
            var match = InsertDraftRe.Match(code);
            if (!match.Success)
                return code;

            var draft = match.Groups["draft"].Value;

            var c = db.ContentOfTypeSavedDraft(draft);
            return c?.Body ?? "Draft could not be found";
        }



        private string NextMeetingDate(int? orgid, int pid)
        {
            if (!orgid.HasValue)
                return null;

            var mt = (from aa in db.Attends
                where aa.OrganizationId == orgid
                where aa.PeopleId == pid
                where AttendCommitmentCode.committed.Contains(aa.Commitment ?? 0)
                where aa.MeetingDate > Util.Now
                orderby aa.MeetingDate
                select aa.MeetingDate).FirstOrDefault();
            return mt == DateTime.MinValue ? "none" : mt.ToString("g");
        }

        private string NextMeetingDate0(int? orgid)
        {
            if (!orgid.HasValue)
                return null;

            var mt = (from mm in db.Meetings
                         where mm.OrganizationId == orgid
                         where mm.MeetingDate > Util.Now
                         orderby mm.MeetingDate
                         select mm.MeetingDate).FirstOrDefault() ?? DateTime.MinValue;
            return mt == DateTime.MinValue ? "none" : mt.ToString("g");
        }





        /// <summary>
        /// Depending on the WYSIWYG editor being used, the URLs (where replacement codes are set) might end up getting URL encoded.
        /// This method will replace the URL-encoded version with the normal version so that the actual replacement logic can be relatively
        /// consistent.
        /// </summary>
        private static string MapUrlEncodedReplacementCodes(string text, IEnumerable<string> codesToReplace)
        {
            foreach (var code in codesToReplace)
            {
                var codeToReplace = $"{{{code}}}";
                var urlEncoded = WebUtility.UrlEncode(codeToReplace);
                if (urlEncoded != null)
                    text = text.Replace(urlEncoded, codeToReplace);
            }
            return text;
        }

        private PayInfo GetPayInfo(int? orgid, int pid)
        {
            if (orgid == null)
                return null;
            return (
                from m in db.OrganizationMembers
                let ts = db.ViewTransactionSummaries.SingleOrDefault(
                    tt => tt.RegId == m.TranId && tt.PeopleId == m.PeopleId)
                where m.PeopleId == pid && m.OrganizationId == orgid
                select new PayInfo
                {
                    PayLink = m.PayLink2(db),
                    Amount = ts.IndAmt,
                    AmountPaid = ts.IndPaid,
                    AmountDue = ts.IndDue,
                    RegisterMail = m.RegisterEmail
                }
            ).SingleOrDefault();
        }
    }
}