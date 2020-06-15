using CmsWeb;
using CmsWeb.Common;
using Newtonsoft.Json;
using System;
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
        if (Util.IsDebug())
        { 
            if (path.EndsWith("app.min.js"))
            {
                return GulpFilesFor("");
            }
            if (path.EndsWith("onlineregister.min.js"))
            {
                return GulpFilesFor("OnlineReg");
            }
        }

        if (HttpRuntime.Cache[path] == null)
        {
            var absolute = HostingEnvironment.MapPath(path) ?? "";
            var ext = Path.GetExtension(absolute);
            var fmt = ext == ".js"
                ? "<script type=\"text/javascript\" src=\"{0}\"></script>\n"
                : "<link href=\"{0}\" rel=\"stylesheet\" />\n";
            string url;
            if (Util.IsDebug())
            {
                url = path;
            }
            else if (ViewExtensions2.CurrentDatabase.Setting("UseCDN") || Configuration.Current.UseCDN)
            {
                url = GetFingerprintCDNUrl(path, absolute, ext);
            }
            else
            {
                url = GetFingerprintUrl(path, absolute, ext);
            }
            var result = string.Format(fmt, url);
            HttpRuntime.Cache.Insert(path, result, new CacheDependency(absolute));
        }

        return new HtmlString(HttpRuntime.Cache[path] as string);
    }

    private static HtmlString GulpFilesFor(string section)
    {
        var gulpfile = File.ReadAllText(HttpContextFactory.Current.Server.MapPath("~/gulpfile.js"));
        var beginEnd = new Regex($@"//{section}FilesStart(.*)//{section}FilesEnd", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        var files = beginEnd.Match(gulpfile).Groups[1].Value;
        var fileMatcher = new Regex("'(.*?)'", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        var match = fileMatcher.Match(files);
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

    private static string GetFingerprintCDNUrl(string path, string absolute, string ext)
    {
        var fingerprints = HttpRuntime.Cache["fingerprints"] as Dictionary<string, string>;
        if (fingerprints == null)
        {
            var file = HostingEnvironment.MapPath("/Content/fingerprints.json");
            var json = File.ReadAllText(file);
            fingerprints = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            HttpRuntime.Cache.Insert("fingerprints", fingerprints, new CacheDependency(file));
        }
        return (fingerprints?.ContainsKey(path) == true && fingerprints[path].HasValue())
            ? fingerprints[path]
            : GetFingerprintUrl(path, absolute, ext);
    }

    private static string GetFingerprintUrl(string path, string absolute, string ext)
    {
        const string min = ".min";
        var date = File.GetLastWriteTime(absolute);
        var filename = Path.GetFileNameWithoutExtension(absolute);
        var directory = path.Remove(path.LastIndexOf('/'));
        var timestamp = $"v-{date:yyMMddhhmmss}-";
        var minfile = $"{directory}/{filename}{min}{ext}";
        return File.Exists(HostingEnvironment.MapPath(minfile))
            ? $"{directory}/{timestamp}{filename}{min}{ext}"
            : $"{directory}/{timestamp}{filename}{ext}";
    }

    public static HtmlString Css(string path)
    {
        return Include(path);
    }

    public static HtmlString CssPrint(string path)
    {
        var absolute = HostingEnvironment.MapPath(path) ?? "";
        var ext = Path.GetExtension(absolute);
        var url = GetFingerprintUrl(path, absolute, ext);
        return new HtmlString($"<link href=\"{url}\" rel=\"stylesheet\" media=\"print\" />\n");
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
