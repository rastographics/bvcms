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
        /// <summary>
        /// LocateLocalFileInPath
        /// 
        /// This method facilitates developing and debugging Python Projects involving multiple file types.
        /// It is intended to be run within Visual Studio against a local database.
        /// However, while not recommended, it will also work with a connectionstring to a production database.
        /// It is designed to allow you to edit every file locally with your favorite code editor.
        /// I prefer Visual Studio Code rather than mixing in with the normal Visual Studio.
        /// To debug a Python script:
        ///     You must uncheck the 'Enable Just My Code' setting
        ///         in Tools>Options>Debugging/General area (requires restart I think).
        ///     You must open the file being debugged in the Visual Studio running bvcms.
        ///         You do not need to copy the file to the bvcms project however.
        ///         Just open it from Visual Studio.
        ///         This way, every change made in Visual Studio Code,
        ///         will automatically ask to propagate changes to the file open in Visual Studio.
        ///     You must be in Debug mode.
        ///     You will need to add two settings in your AppSettings:
        ///         <add key="LocalScriptsPath" value="c:/dev/MyProject"/>
        ///         <add key="LocalScriptsContentKeyword" value="MyProject"/>
        ///         MyProject would be the name of your project, which should not conflict with any other project
        ///         LocalScriptsPath is the place you add all of your project files in any Directory structure you want.
        ///         LocalScriptsContentKey is the keyword used in Special Content to filter just your project.
        ///         I use a secrets.config file so as to not accidently commit that setting to the GitHub repo via the Web.Config file.
        /// Having these requirements satisfied, you can set breakpoints and examine variables.
        /// Unfortunately, global variables outside of a function def are not able to be inspected.
        /// To get around this, I pass any global variable I need to inspect as a function argument and inspect the parameter there.
        /// Of course, you can always use Python's print statement.
        ///
        /// LocalLocalFileInPath serves two purposes:
        ///     1)  It finds the full path of the file and writes the file's text
        ///         into the appropriate Special Content folder
        ///         using the extensionless name of the file with the keyword attached for filtering.
        ///     2)  Additionally, if the file is a Python script,
        ///         it will allow the file run the file in debug mode assuming all of the above requirements are set.
        ///
        /// Basically, this function works from three locations:
        ///     CmsWeb/Models/PythonScriptModel.cs -- FetchScript(name) method
        ///     CmsWeb/Models/SqlScriptModel.cs -- FetchScript(name) method
        ///     CMSData/API/PythonModel/PythonModel.Misc.cs -- Content(name) method
        /// 
        /// </summary>
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
            var files = dirinfo.GetFiles();

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
#endif
    }
}
