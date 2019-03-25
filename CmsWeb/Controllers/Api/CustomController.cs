using CmsData;
using CmsWeb.Lifecycle;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Web.Http;
using UtilityExtensions;

namespace CmsWeb.Controllers.Api
{
    public class CustomController : ApiController
    {
        //todo: inheritance chain
        private readonly RequestManager _requestManager;
        private CMSDataContext CurrentDatabase => _requestManager.CurrentDatabase;

        public CustomController(RequestManager requestManager)
        {
            _requestManager = requestManager;
        }

        [HttpGet, Route("~/CustomAPI/{name}")]
        public IEnumerable<dynamic> Get(string name)
        {
            var content = CurrentDatabase.ContentOfTypeSql(name);
            if (content == null)
            {
                throw new Exception("no content");
            }

            if (!CanRunScript(content))
            {
                throw new Exception("Not Authorized to run this script");
            }

            var cs = User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            cn.Open();
            var d = Request.GetQueryNameValuePairs();
            var p = new DynamicParameters();
            foreach (var kv in d)
            {
                p.Add("@" + kv.Key, kv.Value);
            }

            return cn.Query(content, p);
        }
        private bool CanRunScript(string script)
        {
            return script.StartsWith("--API");
        }
    }
}
