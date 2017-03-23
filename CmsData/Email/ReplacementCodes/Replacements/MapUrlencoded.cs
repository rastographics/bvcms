using System.Collections.Generic;
using System.Net;

namespace CmsData
{
    public partial class EmailReplacements
    {
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
                if (urlEncoded != null && text.Contains(urlEncoded))
                    text = text.Replace(urlEncoded, codeToReplace);
            }
            return text;
        }

    }
}