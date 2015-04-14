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

		public ActionResult Authenticate(string data)
		{
			if (!Auth())
				return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

			var br = new BaseMessage();
			br.error = 0;
			br.data = JsonConvert.SerializeObject(new CheckInInformation(getSettings(), getCampuses()));
			return br;
		}

		private List<CheckInSettingsEntry> getSettings()
		{
			return (from s in DbUtil.Db.CheckInSettings
					  select new CheckInSettingsEntry {
						  name = s.Name,
						  settings = s.Settings
					  }).ToList();
		}

		private List<CheckInCampus> getCampuses()
		{
			return (from c in DbUtil.Db.Campus
					  where c.Organizations.Any(o => o.CanSelfCheckin == true)
					  orderby c.Id
					  select new CheckInCampus {
						  id = c.Id,
						  name = c.Description
					  }).ToList();
		}

		[HttpPost]
		public ActionResult SaveSettings(string data)
		{
			if (!Auth())
				return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

			BaseMessage dataIn = BaseMessage.createFromString(data);
			CheckInSettingsEntry entry = JsonConvert.DeserializeObject<CheckInSettingsEntry>(dataIn.data);

			var setting = (from e in DbUtil.Db.CheckInSettings
								where e.Name == entry.name
								select e).SingleOrDefault();

			BaseMessage br = new BaseMessage();

			if (setting == null) {
				setting = new CheckInSetting();
				setting.Name = entry.name;
				setting.Settings = entry.settings;

				DbUtil.Db.CheckInSettings.InsertOnSubmit(setting);

				br.data = "Settings saved";
			} else {
				setting.Settings = entry.settings;

				br.data = "Settings updated";
			}

			DbUtil.Db.SubmitChanges();

			br.error = 0;
			br.id = setting.Id;
			br.count = 1;

			return br;
		}
	}
}