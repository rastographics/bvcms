using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using UtilityExtensions;

public class Fingerprint
{
    private static HtmlString Include(string path)
    {
#if DEBUG
        if (path.EndsWith("app.min.js"))
        {
            var s = File.ReadAllText(HttpContextFactory.Current.Server.MapPath("~/gulpfile.js"));
            var re = new Regex(@"//DebugFilesStart(.*)//DebugFilesEnd", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            var fs = re.Match(s).Groups[1].Value;
            var re2 = new Regex("'(.*?)'", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            var match = re2.Match(fs);
            var list = new List<string>();
            while (match.Success)
            {
                list.Add(match.Groups[1].Value);
                match = match.NextMatch();
            }
            var result = new StringBuilder();
            foreach (var file in list)
                result.AppendFormat($"<script type=\"text/javascript\" src=\"/{file}\"></script>\n");
            var paths = result.ToString();
            return new HtmlString(paths);
        }
        if (path.EndsWith("onlineregister.min.js"))
        {
            var s = File.ReadAllText(HttpContextFactory.Current.Server.MapPath("~/gulpfile.js"));
            var re = new Regex(@"//DebugOnlineRegFilesStart(.*)//DebugOnlineRegFilesEnd", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            var fs = re.Match(s).Groups[1].Value;
            var re2 = new Regex("'(.*?)'", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            var match = re2.Match(fs);
            var list = new List<string>();
            while (match.Success)
            {
                list.Add(match.Groups[1].Value);
                match = match.NextMatch();
            }
            var result = new StringBuilder();
            foreach (var file in list)
                result.AppendFormat($"<script type=\"text/javascript\" src=\"/{file}\"></script>\n");
            var paths = result.ToString();
            return new HtmlString(paths);
        }
#endif
        if (HttpRuntime.Cache[path] == null)
        {
            var absolute = HostingEnvironment.MapPath(path) ?? "";
            var ext = Path.GetExtension(absolute);
            var fmt = ext == ".js"
                ? "<script type=\"text/javascript\" src=\"{0}\"></script>\n"
                : "<link href=\"{0}\" rel=\"stylesheet\" />\n";
            var result = new StringBuilder();
#if DEBUG
            result.AppendFormat(fmt, path);
#else
            const string min = ".min";
            var dt = File.GetLastWriteTime(absolute);
            var f = Path.GetFileNameWithoutExtension(absolute);
            var d = path.Remove(path.LastIndexOf('/'));
            var t = $"v-{dt:yyMMddhhmmss}-";
            var minfile = $"{d}/{f}{min}{ext}";
            var p = File.Exists(HostingEnvironment.MapPath(minfile))
                ? $"{d}/{t}{f}{min}{ext}"
                : $"{d}/{t}{f}{ext}";
            result.AppendFormat(fmt, p);
#endif
            HttpRuntime.Cache.Insert(path, result.ToString(), new CacheDependency(absolute));
        }
        return new HtmlString(HttpRuntime.Cache[path] as string);
    }

    public static HtmlString Css(string path)
    {
        return Include(path);
    }
    public static HtmlString CssPrint(string path)
    {
        var absolute = HostingEnvironment.MapPath(path) ?? "";
        var ext = Path.GetExtension(absolute);
        var dt = File.GetLastWriteTime(absolute);
        var f = Path.GetFileNameWithoutExtension(absolute);
        var d = path.Remove(path.LastIndexOf('/'));
        var t = $"v-{dt:yyMMddhhmmss}-";
        var p = $"{d}/{t}{f}{ext}";
        return new HtmlString($"<link href=\"{p}\" rel=\"stylesheet\" media=\"print\" />\n");
    }

    public static HtmlString Script(string path)
    {
        return Include(path);
    }

    public static string ScriptStr(string path)
    {
        return Include(path).ToString();
    }
}
