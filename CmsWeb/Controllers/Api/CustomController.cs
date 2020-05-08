using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            using (var cn = CurrentDatabase.ReadonlyConnection())
            {
                cn.Open();
                var d = Request.GetQueryNameValuePairs();
                var p = new DynamicParameters();
                foreach (var kv in d)
                {
                    p.Add("@" + kv.Key, kv.Value);
                }
                return cn.Query(content, p);
            }

        }

        [HttpGet, HttpPost, Route("~/PythonAPI/{name}")]
        public object PythonAPI(string name)
        {
            var model = new PythonScriptModel(CurrentDatabase);
            var script = model.FetchScript(name);
            if (!CanRunScript(script))
            {
                throw new Exception("Not Authorized to run this script");
            }
            model.PrepareHttpPost();
            var query = Request.RequestUri.ParseQueryString();
            foreach (var key in query.AllKeys)
            {
                model.pythonModel.DictionaryAdd(key, query.Get(key));
            }

            var ret = model.RunPythonScript(script);

            return new { output = ret, data = model.pythonModel.Data };
        }

        private bool CanRunScript(string script)
        {
            return script.StartsWith("--API") || script.StartsWith("#API");
        }
    }
}
