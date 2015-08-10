using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

public class Fingerprint
{
    private static HtmlString Include(string path)
    {
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

    public static HtmlString Script(string path)
    {
        return Include(path);
    }

    public static string ScriptStr(string path)
    {
        return Include(path).ToString();
    }
}
