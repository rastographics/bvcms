using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using MarkdownDeep;
using RestSharp;
using UtilityExtensions;
using System.Linq;
using System.Net.Mime;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using CmsData.API;
using CmsData.Codes;
using Dapper;
using IronPython.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RestSharp.Authenticators;
using Method = RestSharp.Method;

namespace CmsData
{
    public partial class PythonModel
    {
        public string CmsHost => db.ServerLink().TrimEnd('/');
        public bool FromMorningBatch { get; set; }
        public int? QueryTagLimit { get; set; }
        public string UserName => Util.UserName;
        public dynamic Data { get; }

        public string CallScript(string scriptname)
        {
            var script = db.ContentOfTypePythonScript(scriptname);
            var model = new PythonModel(db, dictionary);
            model.FromMorningBatch = FromMorningBatch;
            return ExecutePython(script, model);
        }

        public string Content(string name)
        {
            var c = db.Content(name);
#if DEBUG
            if (c == null)
            {
                var txt = File.ReadAllText(name);
                if (!txt.HasValue())
                    return txt;
                var nam = Path.GetFileNameWithoutExtension(name);
                var ext = Path.GetExtension(name);
                int typ = ContentTypeCode.TypeText;
                switch (ext)
                {
                    case ".sql":
                        typ = ContentTypeCode.TypeSqlScript;
                        break;
                    case ".text":
                        typ = ContentTypeCode.TypeText;
                        break;
                    case ".html":
                        typ = ContentTypeCode.TypeHtml;
                        break;
                }
                c = db.Content(nam, typ);
                if (c == null)
                {
                    c = new Content
                    {
                        Name = nam,
                        TypeID = typ
                    };
                    db.Contents.InsertOnSubmit(c);
                }
                c.Body = txt;
                db.SubmitChanges();
            }
#endif
            return c.Body;
        }

        public bool DataHas(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public string Dictionary(string s)
        {
            if (dictionary != null && dictionary.ContainsKey(s))
                return dictionary[s].ToString();
            return "";
        }

        public DynamicData DynamicData()
        {
            return new DynamicData();
        }
        public DynamicData DynamicData(PythonDictionary dict)
        {
            return new DynamicData(dict);
        }
        /// <summary>
        /// Creates a new DynamicData instance populated with a previous instance
        /// </summary>
        public DynamicData DynamicData(DynamicData dd)
        {
            return new DynamicData(dd);
        }

        public void DictionaryAdd(string key, string value)
        {
            dictionary.Add(key, value);
        }
        public void DictionaryAdd(string key, object value)
        {
            dictionary.Add(key, value);
        }

        public string FmtPhone(string s, string prefix = null)
        {
            return s.FmtFone(prefix);
        }

        public string FmtZip(string s)
        {
            return s.FmtZip();
        }

        public string HtmlContent(string name)
        {
            var c = db.ContentOfTypeHtml(name);
            return c.Body;
        }
        public string SqlContent(string name)
        {
            var sql = db.ContentOfTypeSql(name);
            return sql;
        }
        public string TextContent(string name)
        {
            return db.ContentOfTypeText(name);
        }
        public string TitleContent(string name)
        {
            var c = db.ContentOfTypeHtml(name);
            return c.Title;
        }
        public string Draft(string name)
        {
            var c = db.ContentOfTypeSavedDraft(name);
            return c.Body;
        }
        public string DraftTitle(string name)
        {
            var c = db.ContentOfTypeSavedDraft(name);
            return c.Title;
        }

        public string Replace(string text, string pattern, string replacement)
        {
            return Regex.Replace(text, pattern, replacement, RegexOptions.Singleline);
        }
        public static string Markdown(string text)
        {
            if (text == null)
                return "";
            var md = new Markdown();
            return md.Transform(text.Trim());
        }

        public string RegexMatch(string s, string regex)
        {
            return Regex.Match(s, regex, RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;
        }
        public string UrlEncode(string s)
        {
            return HttpUtility.UrlEncode(s);
        }
        public string RestGet(string url, PythonDictionary headers, string user = null, string password = null)
        {
#if DEBUG2
            var ttt = System.IO.File.ReadAllText(@"C:\dev\bvcms\ttt.json");
            return ttt;
#else
            var client = new RestClient(url);
            if (user?.Length > 0 && password?.Length > 0)
                client.Authenticator = new HttpBasicAuthenticator(user, password);

            var request = new RestRequest(Method.GET);
            foreach (var kv in headers)
                request.AddHeader((string)kv.Key, (string)kv.Value);
            var response = client.Execute(request);
            return response.Content;
#endif
        }
        public string RestPost(string url, PythonDictionary headers, object body, string user = null, string password = null)
        {
            var client = new RestClient(url);
            if (user?.Length > 0 && password?.Length > 0)
                client.Authenticator = new HttpBasicAuthenticator(user, password);

            var request = new RestRequest(Method.POST);
            foreach (var kv in headers)
                request.AddHeader((string)kv.Key, (string)kv.Value);
            request.AddBody(body);
            var response = client.Execute(request);
            return response.Content;
        }
        public string RestPostJson(string url, PythonDictionary headers, object obj, string user = null, string password = null)
        {
            var client = new RestClient(url);
            if (user?.Length > 0 && password?.Length > 0)
                client.Authenticator = new HttpBasicAuthenticator(user, password);
            var request = new RestRequest(Method.POST);
            request.JsonSerializer = new RestSharp.Serializers.Shared.JsonSerializer();
            foreach (var kv in headers)
                request.AddHeader((string)kv.Key, (string)kv.Value);
            request.AddJsonBody(obj);
            var response = client.Execute(request);
            return response.Content;
        }
        public string RestDelete(string url, PythonDictionary headers, string user = null, string password = null)
        {
            var client = new RestClient(url);
            if (user?.Length > 0 && password?.Length > 0)
                client.Authenticator = new HttpBasicAuthenticator(user, password);
            var request = new RestRequest(Method.DELETE);
            foreach (var kv in headers)
                request.AddHeader((string)kv.Key, (string)kv.Value);
            var response = client.Execute(request);
            return response.Content;
        }
        public static dynamic JsonDeserialize(string s)
        {
            dynamic d = JObject.Parse(s);
            return d;
        }
        public static IEnumerable<dynamic> JsonDeserialize2(string s)
        {
            var  list =  JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(s);
            var list2 = list.Select(vv => new DynamicData(vv));
            return list2;
        }
        public string JsonSerialize(object o)
        {
            return JsonConvert.SerializeObject(o);
        }

        public string Setting(string name, string def = "")
        {
            return db.Setting(name, def);
        }
        public void SetSetting(string name, object value)
        {
            db.SetSetting(name, value.ToString());
            db.SubmitChanges();
        }

        public string FormatJson(string json)
        {
            var d = JsonConvert.DeserializeObject(json);
            var s = JsonConvert.SerializeObject(d, Formatting.Indented);
            return s.Replace("\r\n", "\n");
        }
        public string FormatJson(DynamicData data)
        {
            var json = data.ToString();
            return FormatJson(json);
        }
        public string FormatJson(Dictionary<string, object> data)
        {
            var s = JsonConvert.SerializeObject(data, Formatting.Indented);
            return s.Replace("\r\n", "\n");
        }

        public string Md5Hash(string s)
        {
            return s.Md5Hash();
        }

        public string ReplaceCodeStr(string text, string codes)
        {
            codes = Regex.Replace(codes, @"//\w*", ","); // replace comments
            codes = Regex.Replace(codes, @"\s*", ""); // remove spaces
            foreach (var pair in codes.SplitStr(","))
            {
                var a = pair.SplitStr("=", 2);
                text = text.Replace(a[0], a[1]);
            }
            return text;
        }
        public void ReplaceQueryFromCode(string encodedguid, string code)
        {
            var queryid = encodedguid.ToGuid();
            var query = db.LoadQueryById2(queryid);
            var c = Condition.Parse(code, queryid);
            query.Text = c.ToXml();
            db.SubmitChanges();
        }

        public class StatusFlag
        {
            public string Id { get; set; }
            public string Flag { get; set; }
            public string Desc { get; set; }
            public string Code { get; set; }
        }

        public List<StatusFlag> StatusFlagList()
        {
            var q = from c in db.Queries
                    where c.Name.StartsWith("F") && c.Name.Contains(":")
                    orderby c.Name
                    select new { c.Name, c.QueryId, c.Text };

            const string findPrefix = @"^F\d+:.*";
            var re = new Regex(findPrefix, RegexOptions.Singleline | RegexOptions.Multiline);
            var q2 = from s in q.ToList()
                     where re.Match(s.Name).Success
                     let a = s.Name.SplitStr(":", 2)
                     let c = Condition.Import(s.Text)
                     orderby a[0]
                     select new StatusFlag()
                     {
                         Flag = a[0],
                         Id = s.QueryId.ToCode(),
                         Desc = a[1],
                         Code = Regex.Replace(c.ToCode(), "^", "\t", RegexOptions.Multiline),
                     };
            return q2.ToList();
        }
        public Dictionary<string, StatusFlag> StatusFlagDictionary(string flags = null)
        {
            var filter = flags?.Split(',');

            var q = from c in db.Queries
                    where c.Name.StartsWith("F") && c.Name.Contains(":")
                    orderby c.Name
                    select new { c.Name, c.QueryId, c.Text };

            const string findPrefix = @"^F\d+:.*";
            var re = new Regex(findPrefix, RegexOptions.Singleline | RegexOptions.Multiline);
            var q2 = from s in q.ToList()
                     where re.Match(s.Name).Success
                     let a = s.Name.SplitStr(":", 2)
                     let c = Condition.Import(s.Text)
                     where (filter == null || filter.Contains(a[0]))
                     orderby a[0]
                     select new StatusFlag()
                     {
                         Flag = a[0],
                         Id = s.QueryId.ToCode(),
                         Desc = a[1],
                         Code = Regex.Replace(c.ToCode(), "^", "\t", RegexOptions.Multiline),
                     };
            return q2.ToDictionary(vv => vv.Flag, vv => vv);
        }

        public void UpdateStatusFlags()
        {
            db.UpdateStatusFlags();
        }
        public void UpdateStatusFlag(string flagid, string encodedguid)
        {
            var temptag = db.PopulateTempTag(new List<int>());
            var queryid = encodedguid.ToGuid();
            var qq = db.PeopleQuery(queryid ?? Guid.Empty);
            db.TagAll2(qq, temptag);
            db.ExecuteCommand("dbo.UpdateStatusFlag {0}, {1}", flagid, temptag.Id);
        }

        public int CreateQueryTag(string name, string code)
        {
            var qq = db.PeopleQuery2(code);
            if (QueryTagLimit > 0)
                qq = qq.Take(QueryTagLimit.Value);
            int tid = db.PopulateSpecialTag(qq, name, DbUtil.TagTypeId_QueryTags);
            return db.TagPeople.Count(v => v.Id == tid);
        }
        public void DeleteQueryTags(string namelike)
        {
            db.Connection.Execute(@"
DELETE dbo.TagPerson FROM dbo.TagPerson tp JOIN dbo.Tag t ON t.Id = tp.Id WHERE t.TypeId = 101 AND t.Name LIKE @namelike
DELETE dbo.Tag WHERE TypeId = 101 AND Name LIKE @namelike
", new {namelike});
            Util2.CurrentTag = "UnNamed";
        }

        public void WriteContentSql(string name, string sql)
        {
            var c = db.Content(name, ContentTypeCode.TypeSqlScript);
            if (c == null)
            {
                c = new Content()
                {
                    Name = name,
                    TypeID = ContentTypeCode.TypeSqlScript
                };
                db.Contents.InsertOnSubmit(c);
            }
            c.Body = sql;
            db.SubmitChanges();
        }
        public void WriteContentText(string name, string text)
        {
            var c = db.Content(name, ContentTypeCode.TypeText);
            if (c == null)
            {
                c = new Content()
                {
                    Name = name,
                    TypeID = ContentTypeCode.TypeText
                };
                db.Contents.InsertOnSubmit(c);
            }
            c.Body = text;
            db.SubmitChanges();
        }
        public int TagLastQuery(string defaultcode)
        {
            Tag tag = null;
            if (FromMorningBatch)
            {
                var qq = db.PeopleQuery2(defaultcode);
                tag = db.PopulateSpecialTag(qq, DbUtil.TagTypeId_Query);
            }
            else
            {
                var guid = db.FetchLastQuery().Id;
                tag = db.PopulateSpecialTag(guid, DbUtil.TagTypeId_Query);
            }
            return tag.Id;
        }
        public CsvHelper.CsvReader CsvReader(string text)
        {
            var csv = new CsvHelper.CsvReader(new StringReader(text));
            csv.Read();
            csv.ReadHeader();
            return csv;
        }

        public CsvHelper.CsvReader CsvReaderNoHeader(string text)
        {
            var csv = new CsvHelper.CsvReader(new StringReader(text));
            csv.Configuration.HasHeaderRecord = false;
            return csv;
        }

        public string AppendIfBoth(string s1, string join, string s2)
        {
            if (s1.HasValue() && s2.HasValue())
                return s1 + join + s2;
            if(s1.HasValue())
                return s1;
            return s2;
        }
        [Obsolete]
        public DynamicData FromJson(string json)
        {
            var dd =  JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return new DynamicData(dd);
        }

        public DynamicData DynamicDataFromJson(string json)
        {
            return JsonConvert.DeserializeObject<DynamicData>(json);
        }
        /// <summary>
        /// This returns a csv string of the fundids when a church is using Custom Statements and FundSets for different statements
        /// The csv string can be used in SQL using dbo.SplitInts in a query to match a set of fundids.
        /// </summary>
        public string CustomStatementsFundIdList(string name)
        {
            return string.Join(",", APIContributionSearchModel.GetCustomStatementsList(db, name));
        }
        
        public string SpaceCamelCase(string s)
        {
            return s.SpaceCamelCase();
        }

        public string Trim(string s)
        {
            return s.Trim();
        }

        public bool UserIsInRole(string role)
        {
            return HttpContextFactory.Current?.User.IsInRole(role) ?? false;
        }
    }
}
