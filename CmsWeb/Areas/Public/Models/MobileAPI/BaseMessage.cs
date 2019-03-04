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
		public bool argBool = false;

		public string data = "";
		public string token = "";
		public string key = "";

		public override void ExecuteResult( ControllerContext context )
		{
			context.HttpContext.Response.ContentType = "application/json";
			context.HttpContext.Response.Output.Write( JsonConvert.SerializeObject( this ) );
		}

		public static BaseMessage createErrorReturn( string sErrorMessage, int errorCode = 1 )
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

		public static BaseMessage createFromString( string json )
		{
			if( !string.IsNullOrEmpty( json ) ) {
				return JsonConvert.DeserializeObject<BaseMessage>( json );
			}

			return new BaseMessage();
		}

		public BaseMessage setData( string data )
		{
			this.data = data;
			return this;
		}

		public void setError( int error )
		{
			this.error = error;
		}

		public void setNoError()
		{
			this.error = API_ERROR_NONE;
		}

		public string getSourceOS()
		{
			switch( device ) {
				case API_DEVICE_ANDROID: {
					return "Android";
				}

				case API_DEVICE_IOS: {
					return "iOS";
				}

				default:
					return "Unknown";
			}
		}

		public string getSourceQueryString()
		{
			switch( device ) {
				case API_DEVICE_ANDROID: {
					return "source=Android";
				}

				case API_DEVICE_IOS: {
					return "source=iOS";
				}

				default:
					return "";
			}
		}

		// API Device Numbers
		public const int API_DEVICE_UNKNOWN = 0;
		public const int API_DEVICE_IOS = 1;
		public const int API_DEVICE_ANDROID = 2;

		// API Login Errors
		public const int API_ERROR_NONE = 0;
		public const int API_ERROR_PIN_INVALID = -1;
		public const int API_ERROR_PIN_EXPIRED = -2;
		public const int API_ERROR_SESSION_TOKEN_EXPIRED = -3;
		public const int API_ERROR_SESSION_TOKEN_NOT_FOUND = -4;
		public const int API_ERROR_IMPROPER_HEADER_STRUCTURE = -5;
		public const int API_ERROR_INVALID_CREDENTIALS = -6;

		// API Create Errors
		public const int API_ERROR_CREATE_FAILED = 50;

		// API People Errors
		public const int API_ERROR_PERSON_NOT_FOUND = 100;

		// API Push Notification Errors
		public const int API_PUSH_ID_DOESNT_EXISTS = 900;

		// API Version
		// public const int API_VERSION_UNKNOWN = 0;

		// Version 2 had issues with time zones being sent, remove after migrating to version 3
		public const int API_VERSION_2 = 2;

		// Version 3 has time zone corrections
		public const int API_VERSION_3 = 3;

		// Version 4 added rebrand flag
		// public const int API_VERSION_4 = 4;

		// Version 5 added Google Calendar integration
		public const int API_VERSION_5 = 5;

		// Version 6 added My Profile section
		public const int API_VERSION_6 = 6;

		// API Check-In Version
		// Version 1 is the initial release
		public const int API_CHECK_IN_VERSION_1 = 1;

		// Version 2 has the ability to remove birthday
		public const int API_CHECK_IN_VERSION_2 = 2;

		// Version 3 added father and mother name and alt names
		public const int API_CHECK_IN_VERSION_3 = 3;
	}
}