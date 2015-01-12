using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.MobileAPI;
using CmsWeb.Models;
using Newtonsoft.Json;

namespace CmsWeb.Areas.Public.Controllers
{
	public class MobileAPIListController : Controller
	{
		public ActionResult Countries()
		{
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

		public ActionResult HomeActions(string data)
		{
			// Check to see if type matches
			//BaseMessage dataIn = BaseMessage.createFromString(data);
			//if (dataIn.type != BaseMessage.API_TYPE_SYSTEM_HOME_ACTIONS)
			//				return BaseMessage.createTypeErrorReturn();

			var actions = from p in DbUtil.Db.MobileAppActions
							  join i in DbUtil.Db.MobileAppIcons on p.Id equals i.ActionID
							  join s in DbUtil.Db.MobileAppIconSets on i.SetID equals s.Id
							  where p.Enabled == true
							  where s.Active == true
							  orderby p.Order
							  select new MobileHomeAction
							  {
								  type = p.Type,
								  title = p.Title,
								  option = p.Option,
								  data = p.Data,
								  icon = i.Url,
								  loginType = p.LoginType
							  };

			BaseMessage br = new BaseMessage();
			br.error = 0;
			br.type = BaseMessage.API_TYPE_SYSTEM_HOME_ACTIONS;
			br.count = actions.Count();
			br.data = JsonConvert.SerializeObject(actions.ToList());

			return br;
		}

		public ActionResult MapInfo(string data)
		{
			var campuses = from p in DbUtil.Db.MobileAppBuildings
								where p.Enabled
								orderby p.Order
								select new MobileCampus
								{
									id = p.Id,
									name = p.Name
								};

			var campusList = campuses.ToList();

			foreach (MobileCampus campus in campusList)
			{
				var floors = from p in DbUtil.Db.MobileAppFloors
								 where p.Enabled
								 where p.Campus == campus.id
								 orderby p.Order
								 select new MobileFloor
								 {
									 id = p.Id,
									 name = p.Name,
									 image = p.Image,
								 };

				var floorList = floors.ToList();

				foreach (MobileFloor floor in floorList)
				{
					var rooms = from p in DbUtil.Db.MobileAppRooms
									where p.Enabled
									where p.Floor == floor.id
									select new MobileRoom
									{
										name = p.Name,
										room = p.Room,
										x = p.X,
										y = p.Y
									};

					floor.rooms = rooms.ToList();
				}

				campus.floors = floorList;
			}

			BaseMessage br = new BaseMessage();
			br.error = 0;
			br.type = BaseMessage.API_TYPE_MAP_INFO;
			br.count = campusList.Count();
			br.data = JsonConvert.SerializeObject(campusList);

			return br;
		}
	}
}
