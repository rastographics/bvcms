using Newtonsoft.Json;
using System.Web.Mvc;

namespace CmsWeb.CheckInAPI
{
    public class CheckInMessage : ActionResult
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
        public string key = "";

        public override void ExecuteResult( ControllerContext context )
        {
            context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
            context.HttpContext.Response.Output.Write( JsonConvert.SerializeObject( this ) );
        }

        public static CheckInMessage createErrorReturn( string sErrorMessage, int errorCode = 1 )
        {
            return new CheckInMessage
            {
                data = sErrorMessage,
                error = errorCode
            };
        }

        public static CheckInMessage createTypeErrorReturn()
        {
            return new CheckInMessage {data = "ERROR: Type mis-match in API call."};
        }

        public static CheckInMessage createFromString( string json )
        {
            if( !string.IsNullOrEmpty( json ) ) {
                return JsonConvert.DeserializeObject<CheckInMessage>( json );
            }

            return new CheckInMessage();
        }

        public CheckInMessage setData( string data )
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
                case API_DEVICE_ANDROID:
                {
                    return "Android";
                }

                case API_DEVICE_IOS:
                {
                    return "iOS";
                }

                default:
                    return "Unknown";
            }
        }

        public string getSourceQueryString()
        {
            switch( device ) {
                case API_DEVICE_ANDROID:
                {
                    return "source=Android";
                }

                case API_DEVICE_IOS:
                {
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

        // API Errors
        public const int API_ERROR_NONE = 0;
        public const int API_ERROR_INVALID_CREDENTIALS = -6;

        // API People Errors
        public const int API_ERROR_PERSON_NOT_FOUND = 100;

        // API Check-In Version
        // Version 1 is the initial release
        public const int API_V1 = 1;

        // Version 2 has the ability to remove birthday
        public const int API_V2 = 2;

        // Version 3 added father and mother name and alt names
        public const int API_V3 = 3;
    }
}