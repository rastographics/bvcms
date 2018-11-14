using CmsData;
using CmsData.API;
using CmsWeb.Areas.Setup.Controllers;
using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
    public class APIMetaController : CmsController
    {
        public APIMetaController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        public ActionResult Lookups(string id)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($"<Lookups error=\"{ret.Substring(1)}\" />");
            }

            if (!id.HasValue())
            {
                return Content("Lookups error=\"not found\">");
            }

            var q = CurrentDatabase.ExecuteQuery<LookupController.Row>("select * from lookup." + id);
            var w = new APIWriter();
            w.Start("Lookups");
            w.Attr("name", id);
            foreach (var i in q)
            {
                w.Start("Lookup");
                w.Attr("Id", i.Id);
                w.AddText(i.Description);
                w.End();
            }
            w.End();
            DbUtil.LogActivity("APIMeta Lookups");
            return Content(w.ToString(), "text/xml");
        }

        public ActionResult Cookies()
        {
            var s = Request.UserAgent;
            if (Request.Browser.Cookies)
            {
                return Content("supports cookies<br>" + s);
            }

            return Content("does not support cookies<br>" + s);
        }

        //		public ActionResult TestCors()
        //		{
        //            var ret = AuthenticateDeveloper();
        ////            if (ret.StartsWith("!"))
        ////                return Content(ret.Substring(1));
        //			return Content("This is from a CORS request " + DateTime.Now);
        //		}
        [HttpGet]
        public ActionResult SQLView(string id)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($"<SQLView error=\"{ret.Substring(1)}\" />");
            }

            if (!id.HasValue())
            {
                return Content("<SQLView error\"no view name\" />");
            }

            try
            {
                var cmd = new SqlCommand("select * from guest." + id.Replace(" ", ""));
                cmd.Connection = new SqlConnection(Util.ConnectionString);
                cmd.Connection.Open();
                var rdr = cmd.ExecuteReader();
                DbUtil.LogActivity("APIMeta SQLView " + id);
                var w = new APIWriter();
                w.Start("SQLView");
                w.Attr("name", id);

                var read = rdr.Read();
                while (read)
                {
                    w.Start("row");
                    for (var i = 0; i < rdr.FieldCount; i++)
                    {
                        w.Attr(rdr.GetName(i), rdr[i].ToString());
                    }

                    w.End();
                    read = rdr.Read();
                }
                w.End();
                return Content(w.ToString(), "text/xml");
            }
            catch (Exception)
            {
                return Content($"<SQLView error=\"cannot find view guest.{id}\" />");
            }
        }
        [HttpGet, Route("~/APIMeta/SqlScriptXml/{id}/{p1}")]
        [Route("~/APIMeta/SqlScriptXml/{id}")]
        public ActionResult SqlScriptXml(string id, string p1 = null)
        {
            var f = new APIFunctions(CurrentDatabase);
            var e = $"<SqlScriptXml id={id}><Error>{{0}}</Error></SqlScriptXml>";
            return Content(SqlScript(id, p1, f.SqlScriptXml, e), "application/xml");
        }
        [HttpGet, Route("~/APIMeta/SqlScriptJson/{id}/{p1}")]
        [Route("~/APIMeta/SqlScriptJson/{id}")]
        public ActionResult SqlScriptJson(string id, string p1 = null)
        {
            var f = new APIFunctions(CurrentDatabase);
            var e = $@"{{ ""id"": ""{id}"", ""Error"": ""{{0}}"" }}";
            return Content(SqlScript(id, p1, f.SqlScriptJson, e), "application/json");
        }
        private string SqlScript(string id, string p1, Func<string, string, Dictionary<string, string>, string> sqlscript, string e)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return string.Format(e, ret.Substring(1));
            }

            if (!id.HasValue())
            {
                return string.Format(e, $"no view named {id}");
            }

            try
            {
                var cs = User.IsInRole("Finance")
                    ? Util.ConnectionStringReadOnlyFinance
                    : Util.ConnectionStringReadOnly;
                var cn = new SqlConnection(cs);
                cn.Open();
                var d = Request.QueryString.AllKeys.ToDictionary(key => key, key => Request.QueryString[key]);
                return sqlscript(id, p1, d);
            }
            catch (Exception ex)
            {
                return string.Format(e, ex.Message);
            }
        }
    }
}
