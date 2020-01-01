using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using UtilityExtensions;

namespace CmsData
{
    public class DebugScriptsHelper
    {
#if DEBUG
        public static string LocateLocalFileInPath(CMSDataContext db, string name, string ext)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            var path = WebConfigurationManager.AppSettings["LocalScriptsPath"];
            var keyword = WebConfigurationManager.AppSettings["LocalScriptsContentKeyword"];
            if (!path.HasValue())
                return null;
            var dirinfo = new DirectoryInfo(path);
            var list = new Dictionary<string, string>();
            WalkTree(dirinfo, ext, list);
            if (list.ContainsKey(name))
            {
                var fullpath = list[name];
                var script = File.ReadAllText(fullpath);

                var nam = Path.GetFileNameWithoutExtension(fullpath);
                ext = Path.GetExtension(fullpath);
                if (fullpath.EndsWith(".text.html"))
                {
                    ext = Path.GetExtension(nam);
                    nam = Path.GetFileNameWithoutExtension(nam);
                }
                switch (ext)
                {
                    case ".py":
                        db.WriteContentPython(nam, script, keyword);
                        break;
                    case ".sql":
                        db.WriteContentSql(nam, script, keyword);
                        break;
                    case ".text":
                    case ".json":
                        db.WriteContentText(nam, script, keyword);
                        break;
                    case ".html":
                        db.WriteContentHtml(nam, script, keyword);
                        break;
                }
                return fullpath;
            }
            return null;
        }
        private static void WalkTree(DirectoryInfo dirinfo, string ext, Dictionary<string, string> list)
        {
            var files = dirinfo.GetFiles($"*{ext}");

            if (files != null)
            {
                foreach (var fi in files)
                {
                    var fn = Path.GetFileNameWithoutExtension(fi.Name);
                    if (fn.EndsWith(".text"))
                        fn = Path.GetFileNameWithoutExtension(fn);
                    if (fn.EndsWith(".view"))
                        fn = Path.GetFileNameWithoutExtension(fn);
                    list[fn] = fi.FullName;
                }
                foreach (var subdir in dirinfo.GetDirectories())
                {
                    if (subdir.Name.StartsWith("."))
                        continue;
                    WalkTree(subdir, ext, list);
                }
            }
        }
#endif
    }
}
