using System;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using UtilityExtensions;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.Manage.Controllers
{
	public partial class BatchController 
	{
		public ActionResult RunPythonScriptInBackgrond(string file)
		{
			string host = Util.Host;
            HostingEnvironment.QueueBackgroundWorkItem(ct => 
			{
                var pe = new PythonModel(host);
                foreach (var key in Request.QueryString.AllKeys)
                    pe.DictionaryAdd(key, Request.QueryString[key]);
                pe.DictionaryAdd("Logfile", $"RunPythonScriptInBackground.{DateTime.Now:g}");
                ViewBag.Text = PythonModel.ExecutePythonFile(file, pe);
			});
		    return View();
		}
	}
}

