using System.Text.RegularExpressions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string CodeRe = "{[^}]*?}";
        //private const string CodeNoHandlebarsRe = "(?<!{){(?!{).*?}";

        private const string RegisterLinkRe = "<a[^>]*?href=\"https{0,1}://registerlink2{0,1}/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex registerLinkRe = new Regex(RegisterLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string RegisterTagRe = "(?:<|&lt;)registertag[^>]*(?:>|&gt;).+?(?:<|&lt;)/registertag(?:>|&gt;)";
        private readonly Regex registerTagRe = new Regex(RegisterTagRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string RegisterHrefRe = "href=\"https{0,1}://registerlink2{0,1}/\\d+\"";
        private readonly Regex registerHrefRe = new Regex(RegisterHrefRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string RsvpLinkRe = "<a[^>]*?href=\"https{0,1}://(?:rsvplink|regretslink)/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex rsvpLinkRe = new Regex(RsvpLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string SendLinkRe = "<a[^>]*?href=\"https{0,1}://sendlink2{0,1}/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex sendLinkRe = new Regex(SendLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string SupportLinkRe = "<a[^>]*?href=\"https{0,1}://supportlink/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex supportLinkRe = new Regex(SupportLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string MasterLinkRe = "<a[^>]*?href=\"https{0,1}://masterlink/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex masterLinkRe = new Regex(MasterLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string VolReqLinkRe = "<a[^>]*?href=\"https{0,1}://volreqlink\"[^>]*>.*?</a>";
        private readonly Regex volReqLinkRe = new Regex(VolReqLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string VolSubLinkRe = "<a[^>]*?href=\"https{0,1}://volsublink\"[^>]*>.*?</a>";
        private readonly Regex volSubLinkRe = new Regex(VolSubLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private const string VoteLinkRe = "<a[^>]*?href=\"https{0,1}://votelink/{0,1}\"[^>]*>.*?</a>";
        private readonly Regex voteLinkRe = new Regex(VoteLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);


        private static readonly Regex InsertDraftRe = new Regex(@"\{insertdraft:(?<draft>.*?)}", RegexOptions.Singleline);


    }
}