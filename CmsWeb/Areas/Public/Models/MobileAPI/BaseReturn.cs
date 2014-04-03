using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsWeb.MobileAPI
{
	public class BaseReturn : ActionResult
	{
		public int device = API_DEVICE_UNKNOWN;
		public int error = 1;
		public int type = 0;
		public int count = 0;
		public int id = 0;
		public string data = "";

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentType = "application/json";
			context.HttpContext.Response.Output.Write(JsonConvert.SerializeObject(this));
		}

		public static BaseReturn createErrorReturn(string sErrorMessage)
		{
			BaseReturn br = new BaseReturn();
			br.data = sErrorMessage;

			return br;
		}

		public static BaseReturn createTypeErrorReturn()
		{
			BaseReturn br = new BaseReturn();
			br.data = "ERROR: Type mis-match in API call.";

			return br;
		}

		public static BaseReturn createFromString(string sJSON)
		{
			BaseReturn br = JsonConvert.DeserializeObject<BaseReturn>(sJSON);
			return br;
		}

		public BaseReturn setData(string newData)
		{
			data = newData;
			return this;
		}

		/*
		public BaseReturn getData()
		{
			return data;
		}
		*/

		// API Device Numbers
		public const int API_DEVICE_UNKNOWN = 0;
		public const int API_DEVICE_IOS = 1;
		public const int API_DEVICE_ANDROID = 2;

		// API Type Numbers
		// 10000's - People = 11000's - Read / 12000's Write
		// People Read
		public const int API_TYPE_PEOPLE_SEARCH = 11001;
		public const int API_TYPE_PERSON_REFRESH = 11002;
		public const int API_TYPE_PERSON_IMAGE_REFRESH = 11003;
		// People Write
		public const int API_TYPE_PERSON_ADD = 12001;
		public const int API_TYPE_PERSON_IMAGE_ADD = 12002;

		// 20000's - Orgs = 21000's - Read / 22000's Write
		// Org Read
		public const int API_TYPE_ORG_REFRESH = 21001;
		public const int API_TYPE_ORG_ROLL_REFRESH = 21002;
		// Org Write
		public const int API_TYPE_ORG_RECORD_ATTEND = 22001;
		
		// 90000's - System = 91000's - Read / 92000's Write
		// System Read
		public const int API_TYPE_SYSTEM_MARITAL_STATUSES = 91001;
		public const int API_TYPE_SYSTEM_STATES = 91002;
		public const int API_TYPE_SYSTEM_COUNTRIES = 91003;
		// System Write
	}
}