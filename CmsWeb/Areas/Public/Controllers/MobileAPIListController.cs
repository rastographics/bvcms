using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.MobileAPI;
using Newtonsoft.Json;

namespace CmsWeb.Areas.Public.Controllers
{
	public class MobileAPIListController : Controller
	{
		public ActionResult Authenticate()
		{
			if (CmsWeb.Models.AccountModel.AuthenticateMobile()) return null;
			else
			{
				return BaseMessage.createErrorReturn("You are not authorized!");
			}
		}

		public ActionResult Countries()
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			var countries = from e in DbUtil.Db.Countries
								 orderby e.Id
								 select new MobileCountry
								 {
									 id = e.Id,
									 code = e.Code,
									 description = e.Description
								 };

			BaseMessage br = new BaseMessage();
			br.error = 0;
			br.type = BaseMessage.API_TYPE_SYSTEM_COUNTRIES;
			br.count = countries.Count();
			br.data = JsonConvert.SerializeObject(countries.ToList());

			return br;
		}

		public ActionResult States()
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			var states = from e in DbUtil.Db.StateLookups
							 orderby e.StateCode
							 select new MobileState
							 {
								 code = e.StateCode,
								 name = e.StateName
							 };

			BaseMessage br = new BaseMessage();
			br.error = 0;
			br.type = BaseMessage.API_TYPE_SYSTEM_STATES;
			br.count = states.Count();
			br.data = JsonConvert.SerializeObject(states.ToList());

			return br;
		}

		public ActionResult MaritalStatuses()
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			var statuses = from e in DbUtil.Db.MaritalStatuses
								orderby e.Id
								select new MobileMaritalStatus
								{
									id = e.Id,
									code = e.Code,
									description = e.Description
								};

			BaseMessage br = new BaseMessage();
			br.error = 0;
			br.type = BaseMessage.API_TYPE_SYSTEM_MARITAL_STATUSES;
			br.count = statuses.Count();
			br.data = JsonConvert.SerializeObject(statuses.ToList());

			return br;
		}

		public ActionResult GivingFunds()
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			var funds = from f in DbUtil.Db.ContributionFunds
							where f.FundStatusId == 1
							where f.OnlineSort > 0
							orderby f.OnlineSort
							select new MobileFund
							{
								id = f.FundId,
								name = f.FundName
							};

			BaseMessage br = new BaseMessage();
			br.error = 0;
			br.type = BaseMessage.API_TYPE_SYSTEM_GIVING_FUNDS;
			br.count = funds.Count();
			br.data = JsonConvert.SerializeObject(funds.ToList());

			return br;
		}

		public ActionResult Playlists()
		{
			// Authenticate first
			var authError = Authenticate();
			if (authError != null) return authError;

			var playlists = from p in DbUtil.Db.MobileAppPlaylists
								 select new MobilePlaylist
								 {
									 id = p.Id,
									 name = p.Name
								 };

			BaseMessage br = new BaseMessage();
			br.error = 0;
			br.type = BaseMessage.API_TYPE_MEDIA_PLAYLIST;
			br.count = playlists.Count();
			br.data = JsonConvert.SerializeObject(playlists.ToList());

			return br;
		}
	}
}
