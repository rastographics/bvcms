using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Xml.Linq;
using CmsData;
using UtilityExtensions;

public class Fingerprint
{
    public static HtmlString Script(string path)
    {
        if (HttpRuntime.Cache[path] == null)
        {
            string absolute = HostingEnvironment.MapPath("~" + path);
            var result = new StringBuilder();
#if DEBUG
            var bundle = absolute + ".bundle";
            if (File.Exists(bundle))
            {
                var xd = XDocument.Load(bundle);
                foreach (var i in xd.Descendants("file"))
                {
                    string a = HostingEnvironment.MapPath("~" + i.Value);
                    var fd = File.GetLastWriteTime(a);
                    string t = i.Value + "?v=" + fd.Ticks;
                    result.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>\n", t);
                }
            }
            else
            {
                Debug.Assert(absolute != null, "absolute != null");
                var fd = File.GetLastWriteTime(absolute);
                string t = path + "?v=" + fd.Ticks;
                result.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>\n", t);
            }
#else
            const string min = ".min";
            var ext = Path.GetExtension(absolute);
            var f = Path.GetFileNameWithoutExtension(absolute);
            var d = path.Remove(path.LastIndexOf('/'));
            DateTime dt = File.GetLastWriteTime(absolute);
            string tag = "?v=" + dt.Ticks;
            result.AppendFormat("<script type=\"text/javascript\" src=\"{0}/{1}{2}{3}{4}\"></script>\n", d, f, min, ext, tag);
#endif
            HttpRuntime.Cache.Insert(path, result.ToString(), new CacheDependency(absolute));
        }
        return new HtmlString(HttpRuntime.Cache[path] as string);
    }
    public static HtmlString Css(string path)
    {
        if (HttpRuntime.Cache[path] == null)
        {
            string absolute = HostingEnvironment.MapPath("~" + path);
            var result = new StringBuilder();
#if DEBUG
            var bundle = absolute + ".bundle";
            if (File.Exists(bundle))
            {
                var xd = XDocument.Load(bundle);
                foreach (var i in xd.Descendants("file"))
                {
                    string a = HostingEnvironment.MapPath("~" + i.Value);
                    var fd = File.GetLastWriteTime(a);
                    string t = i.Value + "?v=" + fd.Ticks;
                    result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" />\n", t);
                }
            }
            else
            {
                Debug.Assert(absolute != null, "absolute != null");
                var fd = File.GetLastWriteTime(absolute);
                string t = path + "?v=" + fd.Ticks;
                result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" />\n", t);
            }
#else
            const string min = ".min";
            var ext = Path.GetExtension(absolute);
            var f = Path.GetFileNameWithoutExtension(absolute);
            var d = path.Remove(path.LastIndexOf('/'));
            DateTime dt = File.GetLastWriteTime(absolute);
            string tag = "?v=" + dt.Ticks;
            result.AppendFormat("<link href=\"{0}/{1}{2}{3}{4}\" rel=\"stylesheet\" />\n", d, f, min, ext, tag);
#endif
            HttpRuntime.Cache.Insert(path, result.ToString(), new CacheDependency(absolute));
        }
        return new HtmlString(HttpRuntime.Cache[path] as string);
    }
    public static string Layout()
    {
        return (UseNewLook())
            ? "~/Views/Shared/SiteLayout2c.cshtml"
            : "~/Views/Shared/SiteLayout.cshtml";

    }
    public static bool UseNewLook()
    {
        return DbUtil.Db.UserPreference("UseNewLook", "false").ToBool();
    }
    public static bool TestSb2()
    {
        return DbUtil.Db.UserPreference("TestSb2", "false").ToBool();
    }
}