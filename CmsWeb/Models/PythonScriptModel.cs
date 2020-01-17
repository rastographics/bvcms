using CmsData;
using CmsData.Codes;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using CmsData.API;
using ICSharpCode.SharpZipLib.Zip;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class PythonScriptModel
    {
        private CMSDataContext Db;
        internal PythonModel pythonModel { get; set; }

        public PythonScriptModel(CMSDataContext db)
        {
            Db = db;
            pythonModel = new PythonModel(db);
        }

        internal static bool CanRunScript(string script)
        {
            if (!script.StartsWith("#Roles=", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var re = new Regex("#Roles=(?<roles>.*)", RegexOptions.IgnoreCase);
            var roles = re.Match(script).Groups["roles"].Value.Split(',').Select(aa => aa.Trim()).ToArray();
            if (roles.Length > 0)
            {
                return roles.Any(rr => HttpContextFactory.Current.User.IsInRole(rr));
            }

            return true;
        }

        public static Dictionary<string, string> CustomDepositImportMenuItems(CMSDataContext db)
        {
            var q = from c in db.Contents
                    where c.TypeID == ContentTypeCode.TypePythonScript
                    where c.Body.Contains("#class=UploadContributionsMenu")
                    select c;
            var list = new Dictionary<string, string>();
            foreach (var c in q)
            {
                list.Add(c.Name, ContributionsMenuTitle(c.Body));
            }
            return list;
        }

        public static string ContributionsMenuTitle(string body)
        {
            var re = new Regex(@"#class=UploadContributionsMenu,title=(?<title>.*)\r");
            return re.Match(body).Groups["title"].Value;
        }

        internal void PrepareHttpPost()
        {
            var request = HttpContextFactory.Current.Request;
            var files = request.Files;
            var a = files.AllKeys;
            for (var i = 0; i < a.Length; i++)
            {
                var file = files[i];
                if (file == null)
                    continue;
                if (file.FileName.EndsWith(".zip"))
                {
                    UnpackZipIntoDynamicData(a[i], file);
                }
                else
                {
                    AddTextFileToDynamicData(a[i], file);
                }
            }
            foreach (var key in request.Form.AllKeys)
            {
                pythonModel.DictionaryAdd(key, request.Form[key]);
            }
            pythonModel.HttpMethod = "post";
        }

        private void AddTextFileToDynamicData(string key, HttpPostedFileBase file)
        {
            var buffer = new byte[file.ContentLength];
            file.InputStream.Read(buffer, 0, file.ContentLength);
            System.Text.Encoding enc;
            string s = null;
            if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
            {
                enc = new System.Text.ASCIIEncoding();
                s = enc.GetString(buffer, 3, buffer.Length - 3);
            }
            else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
            {
                enc = new System.Text.UnicodeEncoding();
                s = enc.GetString(buffer, 2, buffer.Length - 2);
            }
            else
            {
                enc = new System.Text.ASCIIEncoding();
                s = enc.GetString(buffer);
            }

            pythonModel.DictionaryAdd(key, s);
        }

        private void UnpackZipIntoDynamicData(string key, HttpPostedFileBase file)
        {
            var dd = new DynamicData();
            pythonModel.DictionaryAdd(key, dd);
            // The name of the zip file will be the Special Content keyword filter for all the files
            dd.AddValue("keyword", Path.GetFileNameWithoutExtension(file.FileName));

            var data = new byte[4096];
            using (var zs = new ZipInputStream(file.InputStream))
            {
                ZipEntry ze;
                while ((ze = zs.GetNextEntry()) != null)
                {
                    if (!ze.IsFile)
                        continue;
                    if (ze.Name.Contains("/."))
                        continue;
                    if (ze.Name.Contains(@"\\."))
                        continue;
                    var sb = new StringBuilder();
                    var size = zs.Read(data, 0, data.Length);
                    while (size > 0)
                    {
                        sb.Append(Encoding.ASCII.GetString(data, 0, size));
                        size = zs.Read(data, 0, data.Length);
                    }

                    var filename = Path.GetFileName(ze.Name);
                    dd.AddValue(filename, sb.ToString());
                }
            }
        }

#if DEBUG
        private string runFromPath;
#endif
        public string FetchScript(string name)
        {
            pythonModel.Data.pyscript = name;
#if DEBUG
            runFromPath = DebugScriptsHelper.LocateLocalFileInPath(Db, name, ".py");
#endif
            var script = Db.ContentOfTypePythonScript(name);
            return script;
        }
        public string ReplaceParametersInScript(string script, string p1, string p2, string v1, string v2)
        {
            script = script.Replace("@P1", p1 ?? "NULL")
                .Replace("@P2", p2 ?? "NULL")
                .Replace("V1", v1 ?? "None")
                .Replace("V2", v2 ?? "None");
            if (pythonModel.Dictionary("p1") != null)
            {
                script = script.Replace("@P1", pythonModel.Dictionary("p1") ?? "NULL");
            }
            if (script.Contains("@qtagid"))
            {
                var id = Db.FetchLastQuery().Id;
                var tag = Db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);
                script = script.Replace("@qtagid", tag.Id.ToString());
            }
            return script;
        }
        public string RunPythonScript(string script, string p1 = null, string p2 = null)
        {
            if (script.Contains("@BlueToolbarTagId"))
            {
                var id = Db.FetchLastQuery().Id;
                pythonModel.DictionaryAdd("BlueToolbarGuid", id.ToCode());
            }
            var request = HttpContextFactory.Current.Request;
            foreach (var key in request.QueryString.AllKeys)
            {
                pythonModel.DictionaryAdd(key, request.QueryString[key]);
            }
            if (p1.HasValue())
                pythonModel.DictionaryAdd("p1", p1);
            if (p2.HasValue())
                pythonModel.DictionaryAdd("p2", p2);
            pythonModel.Data.Title = ContributionsMenuTitle(script);
#if DEBUG
            if (runFromPath.HasValue())
            {
                return PythonModel.ExecutePython(runFromPath, pythonModel, fromFile: true);
            }
#endif
            return pythonModel.RunScript(script);
        }

        public IEnumerable<NamesInfo> PythonSearch(string term, int limit)
        {
            var qp = FindNames(term);
            var showaltname = Db.Setting("ShowAltNameOnSearchResults");

            var rp = from p in qp
                     let spouse = Db.People.SingleOrDefault(ss =>
                         ss.PeopleId == p.SpouseId)
                     orderby p.Name2
                     select new NamesInfo
                     {
                         showaltname = showaltname,
                         Pid = p.PeopleId,
                         name = p.Name2,
                         age = p.Age,
                         spouse = spouse.Name,
                         addr = p.PrimaryAddress ?? "",
                         altname = p.AltName,
                     };
            return rp.Take(limit);
        }
        private IQueryable<Person> FindNames(string q)
        {
            if (q.AllDigits())
                return AllDigitsFind(q);
            if (Db.Setting("UseAltnameContains"))
                return AltNameContainsFind(q);
            return NormalFind(q);
        }

        private IQueryable<Person> NormalFind(string q)
        {
            Util.NameSplit(q, out var First, out var Last);
            var hasFirst = First.HasValue();
            var qp = Db.People.AsQueryable();
            qp = from p in qp
                 where
                     (p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last)
                                                  || p.LastName.StartsWith(q) || p.MaidenName.StartsWith(q))
                     &&
                     (!hasFirst || p.FirstName.StartsWith(First) || p.NickName.StartsWith(First) ||
                      p.MiddleName.StartsWith(First)
                      || p.LastName.StartsWith(q) || p.MaidenName.StartsWith(q))
                 select p;
            return qp;
        }

        private IQueryable<Person> AltNameContainsFind(string q)
        {
            Util.NameSplit(q, out var First, out var Last);
            var hasFirst = First.HasValue();
            var qp = from p in Db.People
                 where
                     (p.LastName.StartsWith(Last) || p.MaidenName.StartsWith(Last) || p.AltName.Contains(Last)
                      || p.LastName.StartsWith(q) || p.MaidenName.StartsWith(q))
                     &&
                     (!hasFirst || p.FirstName.StartsWith(First) || p.NickName.StartsWith(First) ||
                      p.MiddleName.StartsWith(First)
                      || p.LastName.StartsWith(q) || p.MaidenName.StartsWith(q))
                 select p;
            return qp;
        }

        private IQueryable<Person> AllDigitsFind(string q)
        {
            var id = q.ToInt();
            if (q.HasValue() && q.Length == 7) // do phone search too 
            {
                return from p in Db.People
                     where
                         p.PeopleId == id
                         || p.CellPhone.Contains(q)
                         || p.Family.HomePhone.Contains(q)
                         || p.WorkPhone.Contains(q)
                     select p;
            }
            return from p in Db.People
                 where p.PeopleId == id
                 select p;
        }

        public class NamesInfo
        {
            public string Name => displayname + (age.HasValue ? $" ({Person.AgeDisplay(age, Pid)})" : "");

            internal bool showaltname;
            internal string name;
            internal string altname;
            internal int? age;

            public int Pid { get; set; }

            internal string addr { get; set; }
            public string Addr => addr.HasValue() ? $"<br>{addr}" : "";

            internal string spouse { get; set; }
            public string Spouse => spouse.HasValue() ? $"<br>Spouse: {spouse}" : "";

            internal string email { get; set; }
            public string Email => email.HasValue() ? $"<br>{email}" : "";
            internal string displayname => (showaltname ? $"{name} {altname}" : name);
        }
    }
}
