using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CmsData;
using CmsWeb.Models;
using CmsWeb.Models.Api;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Controllers.Api
{
    public class CustomController : ApiController
    {
        [HttpGet, Route("~/CustomAPI/{name}/{parameter?}")]
        public IEnumerable<dynamic> Get(string name, string parameter)
        {
            var content = DbUtil.Db.ContentOfTypeSql(name);
            if (content == null)
                throw new Exception("no content");
            var cs = User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            cn.Open();
            var d = Request.Properties;
            var p = new DynamicParameters();
            foreach (var kv in d)
                p.Add("@" + kv.Key, kv.Value);
            var script = RunScriptSql(parameter, content.Body, p);
            return cn.Query(script);
        }
        private string RunScriptSql(string parameter, string body, DynamicParameters p)
        {
            if (!CanRunScript(body))
                throw new Exception("Not Authorized to run this script");
            p.Add("@p1", parameter ?? "");
            return body;
        }
        private bool CanRunScript(string script)
        {
            return script.StartsWith("--API");
        }
    }
}
