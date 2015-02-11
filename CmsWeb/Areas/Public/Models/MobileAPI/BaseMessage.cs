using Newtonsoft.Json;
using System.Web.Mvc;

namespace CmsWeb.MobileAPI
{
	public class BaseMessage : ActionResult
	{
		public int version = 0;
		public int device = API_DEVICE_UNKNOWN;

		public int error = 1;
		public int count = 0;

		public int id = 0;

		public int argInt = 0;
		public string argString = "";

		public string data = "";
		public string token = "";

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentType = "application/json";
			context.HttpContext.Response.Output.Write(JsonConvert.SerializeObject(this));
		}

		public static BaseMessage createErrorReturn(string sErrorMessage, int errorCode = 1)
		{
			BaseMessage br = new BaseMessage();
			br.data = sErrorMessage;
			br.error = errorCode;

			return br;
		}

		public static BaseMessage createTypeErrorReturn()
		{
			BaseMessage br = new BaseMessage();
			br.data = "ERROR: Type mis-match in API call.";

			return br;
		}

		public static BaseMessage createFromString(string sJSON)
		{
			BaseMessage br = JsonConvert.DeserializeObject<BaseMessage>(sJSON);
			return br;
		}

		public BaseMessage setData(string newData)
		{
			data = newData;
			return this;
		}

		// API Login Errors
		public const int API_ERROR_NONE = 0;
		public const int API_ERROR_PIN_INVALID = -1;
		public const int API_ERROR_PIN_EXPIRED = -2;
		public const int API_ERROR_SESSION_TOKEN_EXPIRED = -3;
		public const int API_ERROR_SESSION_TOKEN_NOT_FOUND = -4;
		public const int API_ERROR_IMPROPER_HEADER_STRUCTURE = -5;

		// API Device Numbers
		public const int API_DEVICE_UNKNOWN = 0;
		public const int API_DEVICE_IOS = 1;
		public const int API_DEVICE_ANDROID = 2;
	}
}