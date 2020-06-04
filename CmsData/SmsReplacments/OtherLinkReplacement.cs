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
        private const string MatchOtherLinkRe = @"https{0,1}://[^\s]*";
        private static readonly Regex OtherLinkRe = new Regex(MatchOtherLinkRe, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private string OtherLinkReplacement(string code)
        {
            return PythonModel.CreateTinyUrl(code);
        }
    }
}
