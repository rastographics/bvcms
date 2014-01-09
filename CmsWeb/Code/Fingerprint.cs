using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Xml.Linq;
using UtilityExtensions;

public class Fingerprint
{
    private static HtmlString Include(string path)
    {
        if (HttpRuntime.Cache[path] == null)
        {
            string absolute = HostingEnvironment.MapPath(path) ?? "";
            var ext = Path.GetExtension(absolute);
            var fmt = ext == ".js"
                    ? "<script type=\"text/javascript\" src=\"{0}\"></script>\n"
                    : "<link href=\"{0}\" rel=\"stylesheet\" />\n";
            var result = new StringBuilder();
            var re = new Regex(@".*/bundle\.(?<name>.*)\.(?:js|css)", RegexOptions.Singleline | RegexOptions.Multiline);
            var m = re.Match(path);
            if (m.Success) // process bundle
            {
                const string bundleTargets = "/minify.xml";
                var xd = HttpRuntime.Cache[bundleTargets] as XDocument;
                if (xd == null)
                {
                    string fn = HostingEnvironment.MapPath(bundleTargets);
                    xd = XDocument.Load(fn);
                    HttpRuntime.Cache.Insert(bundleTargets, xd);
                }
                var ns = xd.Root.Name.Namespace;
                var node = m.Groups["name"].Value.ToLower();
#if DEBUG
                foreach (var p in xd.Descendants(ns + node).Select(i => i.Attribute("Include").Value))
                    result.AppendFormat(fmt, "/" + p);
//                {
//                    string fpath = HostingEnvironment.MapPath("/" + p) ?? "";
//                    var fdate = File.GetLastWriteTime(fpath);
//                    var f = Path.GetFileName(fpath);
//                    var d = p.Remove(p.LastIndexOf('/'));
//                    var t = "v-{0:yyMMddhhmmss}-".Fmt(fdate);
//                    var pp = "/{0}/{1}{2}".Fmt(d, t, f);
//                    result.AppendFormat(fmt, pp);
//                }
#else
                DateTime lastdate = DateTime.MinValue;
                foreach (var p in xd.Descendants(ns + node).Select(i => i.Attribute("Include").Value))
                {
                    string fpath = HostingEnvironment.MapPath("/" + p) ?? "";
                    var fdate = File.GetLastWriteTime(fpath);
                    if (fdate > lastdate)
                        lastdate = fdate;
                }
                var f = Path.GetFileName(absolute);
                var d = path.Remove(path.LastIndexOf('/'));
                var t = "v-{0:yyMMddhhmmss}-".Fmt(lastdate);
                var pp = "{0}/{1}{2}".Fmt(d, t, f);
                result.AppendFormat(fmt, pp);
#endif
            } // end bundle handling
            else
            {
#if DEBUG
                result.AppendFormat(fmt, path);
//                var f = Path.GetFileName(absolute);
//                var d = path.Remove(path.LastIndexOf('/'));
//                var t = "v-{0:yyMMddhhmmss}-".Fmt(File.GetLastWriteTime(absolute));
//                var pp = "{0}/{1}{2}".Fmt(d, t, f);
//                result.AppendFormat(fmt, pp);

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
            }
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