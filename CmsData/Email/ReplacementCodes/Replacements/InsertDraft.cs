using System.Text.RegularExpressions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private static readonly Regex InsertDraftRe = new Regex(@"\{insertdraft:(?<draft>.*?)}", RegexOptions.Singleline);

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

        private string InsertDraft(string code)
        {
            var match = InsertDraftRe.Match(code);
            if (!match.Success)
                return code;

            var draft = match.Groups["draft"].Value;

            var c = db.ContentOfTypeSavedDraft(draft);
            return c?.Body ?? "Draft could not be found";
        }
    }
}
