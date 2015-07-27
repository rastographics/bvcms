using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using CmsData;
using CmsData.API;
using CmsWeb.Areas.Setup.Controllers;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
    public class APIMetaController : CmsController
    {
        [HttpGet]
        public ActionResult Lookups(string id)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content($"<Lookups error=\"{ret.Substring(1)}\" />");
            if (!id.HasValue())
                return Content("Lookups error=\"not found\">");
            var q = DbUtil.Db.ExecuteQuery<LookupController.Row>("select * from lookup." + id);
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
                return Content("supports cookies<br>" + s);
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
                return Content($"<SQLView error=\"{ret.Substring(1)}\" />");
            if (!id.HasValue())
                return Content("<SQLView error\"no view name\" />");
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
                        w.Attr(rdr.GetName(i), rdr[i].ToString());
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
    }
}
