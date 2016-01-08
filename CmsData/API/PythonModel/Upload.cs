using System;
using System.Configuration;
using System.IO;
using System.Net;
using Dapper;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public void UploadExcelFromSqlToDropBox(string savedQuery, string sqlscript, string targetpath, string filename)
        {
            var accesstoken = DbUtil.Db.Setting("DropBoxAccessToken", ConfigurationManager.AppSettings["DropBoxAccessToken"]);
            var script = db.Content(sqlscript, "");
            if (!script.HasValue())
                throw new Exception("no sql script found");

            var p = new DynamicParameters();
            foreach (var kv in dictionary)
                p.Add("@" + kv.Key, kv.Value);
            if (script.Contains("@qtagid"))
            {
                int? qtagid = null;
                if (savedQuery.HasValue())
                {
                    var q = db.PeopleQuery2(savedQuery);
                    var tag = db.PopulateSpecialTag(q, DbUtil.TagTypeId_Query);
                    qtagid = tag.Id;
                }
                p.Add("@qtagid", qtagid);
            }
            var bytes = db.Connection.ExecuteReader(script, p).ToExcelBytes(filename);

            var wc = new WebClient();
            wc.Headers.Add($"Authorization: Bearer {accesstoken}");
            wc.Headers.Add("Content-Type: application/octet-stream");
            wc.Headers.Add($@"Dropbox-API-Arg: {{""path"":""{targetpath}/{filename}"",""mode"":""overwrite""}}");
            wc.UploadData("https://content.dropboxapi.com/2-beta-2/files/upload", bytes);
        }

        public void UploadExcelFromSqlToDropBox(string sqlscript, string targetpath, string filename)
        {
            UploadExcelFromSqlToDropBox(null, sqlscript, targetpath, filename);
        }

        public string UploadExcelFromSqlToFtp(string sqlscript, string username, string password, string targetpath, string filename)
        {
            var script = db.Content(sqlscript, "");
            if (!script.HasValue())
                throw new Exception("no sql script found");
            var bytes = db.Connection.ExecuteReader(sqlscript).ToExcelBytes(filename);
            var url = Path.Combine(targetpath, filename);
            using (var webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential(username, password);
                webClient.UploadData(url, bytes);
            }
            return url;
        }
    }
}