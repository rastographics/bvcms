using CmsData;
using CmsWeb.CheckInAPI;
using CmsWeb.MobileAPI;
using CmsWeb.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CmsWeb.Areas.Public.Controllers
{
	public class CheckInAPIController : Controller
	{
		public ActionResult Exists()
		{
			return Content("1");
		}

		private static bool Auth()
		{
			return AccountModel.AuthenticateMobile("Checkin").IsValid;
		}

		[HttpPost]
		public ActionResult Authenticate(string data)
		{
			var result = Auth();

			if (!result)
				return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

			var br = new BaseMessage();
			br.error = 0;
			br.data = JsonConvert.SerializeObject(getSettings());
			return br;
		}

		private List<CmsWeb.CheckInAPI.CheckInSetting> getSettings()
		{
			return (from s in DbUtil.Db.CheckInSettings
					  select new CmsWeb.CheckInAPI.CheckInSetting {
						  id = s.Id,
						  name = s.Name,
						  settings = s.Settings
					  }).ToList();
		}
	}
}