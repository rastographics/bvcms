using Newtonsoft.Json;

namespace CmsCheckin
{
	public class BaseMessage
	{
		public int version = 0;

		public int error = 1;
		public int count = 0;

		public int id = 0;

		public int argInt = 0;
		public string argString = "";

		public string data = "";

		public static BaseMessage createErrorReturn(string sErrorMessage, int errorCode = API_ERROR_UNKNOWN)
		{
			BaseMessage br = new BaseMessage();
			br.data = sErrorMessage;
			br.error = errorCode;

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
		public const int API_ERROR_UNKNOWN = 1;
		public const int API_ERROR_NONE = 0;
	}
}