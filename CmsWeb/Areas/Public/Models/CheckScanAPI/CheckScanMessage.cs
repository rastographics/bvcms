using System.Web.Mvc;
using CmsWeb.Areas.Public.Models.MobileAPIv2;
using Newtonsoft.Json;

namespace CmsWeb.Areas.Public.Models.CheckScanAPI
{
	public class CheckScanMessage : ActionResult
	{
		public int version = (int) Version.NONE;

		public int error = 1;

		public int id = 0;
		public bool flag = false;
		public string data = "";

		public override void ExecuteResult( ControllerContext context )
		{
			context.HttpContext.Response.ContentType = "application/json";
			context.HttpContext.Response.Output.Write( JsonConvert.SerializeObject( this ) );
		}

		public static CheckScanMessage createLoginErrorReturn( CheckScanAuthentication authentication )
		{
			CheckScanMessage br = new CheckScanMessage
			{
				error = authentication.getError(),
				data = authentication.getErrorMessage()
			};

			return br;
		}

		public static CheckScanMessage createErrorReturn( string sErrorMessage, int errorCode = 1 )
		{
			CheckScanMessage br = new CheckScanMessage
			{
				data = sErrorMessage,
				error = errorCode
			};

			return br;
		}

		public static CheckScanMessage createFromString( string json )
		{
			if( !string.IsNullOrEmpty( json ) ) {
				return JsonConvert.DeserializeObject<CheckScanMessage>( json );
			}

			return new CheckScanMessage();
		}

		public void setError( int error, string data )
		{
			this.error = error;
			this.data = data;
		}

		public void setSuccess()
		{
			this.error = (int) Error.NONE;
		}

		public enum Version
		{
			NONE = 0,
			ONE = 1, // Initial release version
		}

		public enum Error
		{
			// Common Errors
			NONE = 0,
			INVALID_INSTANCE_ID = 1,
			INVALID_PIN = 2,

			// Create Errors	
			CREATE_FAILED = 50,

			// People Errors
			PERSON_NOT_FOUND = 100,
		}
	}
}