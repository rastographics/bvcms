using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using UtilityExtensions;
using CmsWeb.Models;
using System.Xml;
using System.IO;
using System.Net.Mail;
using CmsData.Codes;
using CmsData.API;
using System.Text;
using System.Net;
using CmsData;
using CmsWeb.Areas.Manage.Controllers;

namespace CmsWeb.Areas.Public.Controllers
{
    public class APIMetaController : CmsController
    {
        [HttpGet]
        public ActionResult Lookups(string id)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("<Lookups error=\"{0}\" />".Fmt(ret.Substring(1)));
            if (!id.HasValue())
                return Content("Lookups error=\"not found\">");
            var q = DbUtil.Db.ExecuteQuery<Setup.Controllers.LookupController.Row>("select * from lookup." + id);
            var w = new CmsData.API.APIWriter();
            w.Start("Lookups");
            w.Attr("name", id);
            foreach(var i in q)
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
			if (Request.Browser.Cookies == true)
				return Content("supports cookies<br>" + s);
			return Content("does not support cookies<br>" + s);
		}
        [HttpGet]
		public ActionResult SQLView(string id)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
                return Content("<SQLView error=\"{0}\" />".Fmt(ret.Substring(1)));
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
                return Content("<SQLView error=\"cannot find view guest.{0}\" />".Fmt(id));
            }
		}
    }
}