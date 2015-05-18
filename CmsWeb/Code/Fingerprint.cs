using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using UtilityExtensions;

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
            var t = "v-{0:yyMMddhhmmss}-".Fmt(dt);
            var minfile = "{0}/{1}{2}{3}".Fmt(d, f, min, ext);
            string p = File.Exists(HostingEnvironment.MapPath(minfile))
                ? "{0}/{1}{2}{3}{4}".Fmt(d, t, f, min, ext)
                : "{0}/{1}{2}{3}".Fmt(d, t, f, ext);
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
}