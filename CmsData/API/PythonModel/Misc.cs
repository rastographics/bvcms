using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public string CmsHost => db.ServerLink().TrimEnd('/');
        public bool FromMorningBatch { get; set; }
        public string UserName => Util.UserName;
        public dynamic Data { get; }

        public string CallScript(string scriptname)
        {
            var script = db.ContentOfTypePythonScript(scriptname);
            return ExecutePython(script, new PythonModel(db.Host, dictionary));
        }

        public string Content(string name)
        {
            var c = db.Content(name);
            return c.Body;
        }

        public bool DataHas(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public string Dictionary(string s)
        {
            if (dictionary != null && dictionary.ContainsKey(s))
                return dictionary[s].ToString();
            return "";
        }

        public void DictionaryAdd(string key, string value)
        {
            dictionary.Add(key, value);
        }

        public string FmtPhone(string s, string prefix = null)
        {
            return s.FmtFone(prefix);
        }

        public string FmtZip(string s)
        {
            return s.FmtZip();
        }

        public string HtmlContent(string name)
        {
            var c = db.ContentOfTypeHtml(name);
            return c.Body;
        }
        public string TitleContent(string name)
        {
            var c = db.ContentOfTypeHtml(name);
            return c.Title;
        }

        public string Replace(string text, string pattern, string replacement)
        {
            return Regex.Replace(text, pattern, replacement);
        }
    }
}