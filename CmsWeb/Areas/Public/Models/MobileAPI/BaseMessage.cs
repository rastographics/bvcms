using Newtonsoft.Json;
using System.Web.Mvc;

namespace CmsWeb.MobileAPI
{
	public class BaseMessage : ActionResult
	{
		public int version = 0;
		public int device = API_DEVICE_UNKNOWN;

		public int type = 0;
		public int error = 1;
		public int count = 0;

		public int id = 0;

		public string data = "";

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentType = "application/json";
			context.HttpContext.Response.Output.Write(JsonConvert.SerializeObject(this));
		}

		public static BaseMessage createErrorReturn(string sErrorMessage, int errorCode = 1)
		{
			var br = new BaseMessage();
			br.data = sErrorMessage;
		    br.error = errorCode;

			return br;
		}

		public static BaseMessage createTypeErrorReturn()
		{
			var br = new BaseMessage();
			br.data = "ERROR: Type mis-match in API call.";

			return br;
		}

		public static BaseMessage createFromString(string sJSON)
		{
			var br = JsonConvert.DeserializeObject<BaseMessage>(sJSON);
			return br;
		}

		public BaseMessage setData(string newData)
		{
			data = newData;
			return this;
		}


	    public const int API_ERROR_SUCCESS = 0;
	    public const int API_ERROR_PIN_INVALID = 1;
	    public const int API_ERROR_PIN_EXPIRED = 2;
	    public const int API_ERROR_SESSION_TOKEN_EXPIRED = 3;
	    public const int API_ERROR_SESSION_TOKEN_NOT_FOUND = 4;


		// API Device Numbers
		public const int API_DEVICE_UNKNOWN = 0;
		public const int API_DEVICE_IOS = 1;
		public const int API_DEVICE_ANDROID = 2;


		// API Type Numbers
		// 9000's - Handshake
		public const int API_TYPE_LOGIN = 9001;

		// 10000's - People = 11000's - Read / 12000's Write
		// People Read
		public const int API_TYPE_PEOPLE_SEARCH = 11001;
		public const int API_TYPE_PERSON_REFRESH = 11002;
		public const int API_TYPE_PERSON_IMAGE_REFRESH = 11003;
		public const int API_TYPE_PERSON_PAYMENT_INFO = 11004;
		// People Write
		public const int API_TYPE_PERSON_ADD = 12001;
		public const int API_TYPE_PERSON_IMAGE_ADD = 12002;

		// 20000's - Orgs = 21000's - Read / 22000's Write
		// Org Read
		public const int API_TYPE_ORG_REFRESH = 21001;
		public const int API_TYPE_ORG_ROLL_REFRESH = 21002;
		// Org Write
		public const int API_TYPE_ORG_RECORD_ATTEND = 22001;
		public const int API_TYPE_ORG_JOIN = 22002;

		// 30000's - Giving = 31000's - Read / 32000's Write
		// Giving Read
		public const int API_TYPE_GIVING = 31001;
		public const int API_TYPE_GIVING_ONE_TIME_LINK_GIVING = 31001;
		public const int API_TYPE_GIVING_ONE_TIME_LINK_MANAGED_GIVING = 31002;
		// Giving Write

		// 40000's - Registration = 41000's - Read / 42000's Write
		public const int API_TYPE_REGISTRATION_ONE_TIME_LINK = 41001;

		// 80000's - Giving = 81000's - Read / 82000's Write
		// Media Read
		public const int API_TYPE_MEDIA_PLAYLIST = 81001;
		// Media Write
		
		// 90000's - System = 91000's - Read / 92000's Write
		// System Read
		public const int API_TYPE_SYSTEM_MARITAL_STATUSES = 91001;
		public const int API_TYPE_SYSTEM_STATES = 91002;
		public const int API_TYPE_SYSTEM_COUNTRIES = 91003;
		public const int API_TYPE_SYSTEM_GIVING_FUNDS = 91004;
		// System Write
	}
}