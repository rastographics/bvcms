using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CmsData.API;

namespace CmsData
{
    public partial class TextReplacements
    {
        private readonly string[] stringlist;
        private readonly string connStr;
        private readonly string host;
        private int? currentOrgId;
        private CMSDataContext db;
        private Person person;
        private const string MatchCodeRe = "(?<!{){(?!{)[^}]*?}";

        private const string MatchRes = MatchUnlayerLinkRe;

        private const string Pattern = "(" + MatchCodeRe + "|" + MatchRes + ")";

        public TextReplacements(CMSDataContext callingContext)
        {
            currentOrgId = Util2.CurrentOrgId;
            connStr = callingContext.ConnectionString;
            host = callingContext.Host;
            db = callingContext;
        }

        public TextReplacements(CMSDataContext callingContext, string text)
            : this(callingContext)
        {
            stringlist = Regex.Split(text, Pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
        }

        public string DoReplacements(SMSItem item)
        {
            using (db = CMSDataContext.Create(connStr, host))
            {
                if (currentOrgId.HasValue)
                    db.SetCurrentOrgId(currentOrgId);
                person = db.LoadPersonById(item.PeopleID ?? 0);

                var texta = new List<string>(stringlist);
                for (var i = 1; i < texta.Count; i++)
                {
                    var part = texta[i];
                    if(part.StartsWith("{") || part.StartsWith("http"))
                        texta[i] = DoReplaceCode(part, item);
                }
                return string.Join("", texta);
            }
        }
    }
}
