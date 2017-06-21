using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        public static bool IsSpecialLink(string link)
        {
            return new List<string>()
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
            }.Contains(link.ToLower());
        }

    }
}