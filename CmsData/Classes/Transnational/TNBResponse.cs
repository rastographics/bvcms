using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsData.Classes.Transnational
{
	class TNBResponse
	{
		public int response = 0;
		public int response_code = ERROR_NONE;
		public int customer_vault_id = 0;

		public string responsetext = "";
		public string avsresponse = "";
		public string cvvresponse = "";

		public string type = "";
		public string orderid = "";
		public string transactionid = "";
		public string authcode = "";

		public string raw = "";

		public TNBResponse populate(NameValueCollection nvc, string rawResponse)
		{
			Int32.TryParse(nvc.Get(FIELD_RESPONSE), out response);
			Int32.TryParse(nvc.Get(FIELD_RESPONSE_CODE), out response_code);
			Int32.TryParse(nvc.Get(FIELD_CUSTOMER_VAULT_ID), out customer_vault_id);

			responsetext = nvc.Get(FIELD_RESPONSE_TEXT);
			avsresponse = nvc.Get(FIELD_AVS_RESPONSE);
			cvvresponse = nvc.Get(FIELD_CVV_RESPONSE);

			type = nvc.Get(FIELD_TYPE);
			orderid = nvc.Get(FIELD_ORDER_ID);
			transactionid = nvc.Get(FIELD_TRANSACTION_ID);
			authcode = nvc.Get(FIELD_AUTH_CODE);

			raw = rawResponse;

			return this;
		}

		public string getErrorMessage()
		{
			if( errorMessages.ContainsKey(response_code))
			{
				return errorMessages[response_code];
			}
			else
			{
				return "Unknown error";
			}
		}


		// --- FIELDS ---
		public const string FIELD_RESPONSE = "response";
		public const string FIELD_RESPONSE_CODE = "response_code";
		public const string FIELD_CUSTOMER_VAULT_ID = "customer_vault_id";
		public const string FIELD_RESPONSE_TEXT = "responsetext";
		public const string FIELD_AVS_RESPONSE = "avsresponse";
		public const string FIELD_CVV_RESPONSE = "cvvresponse";
		public const string FIELD_TYPE = "type";
		public const string FIELD_ORDER_ID = "orderid";
		public const string FIELD_TRANSACTION_ID = "transactionid";
		public const string FIELD_AUTH_CODE = "authcode";
		// --- END FIELDS ---

		// --- RESPONSES ---
		public const int RESPONSE_APPROVED = 1;
		public const int RESPONSE_DECLINED = 2;
		public const int RESPONSE_ERROR = 3;
		// --- END RESPONSES ---

		// --- RESPONSE_CODES ---
		public const int ERROR_NONE = 100;

		public const int ERROR_DECLINED = 200;
		public const int ERROR_DO_NOT_HONOR = 201;
		public const int ERROR_INSUFFICENT_FUNDS = 202;
		public const int ERROR_OVER_LIMIT = 203;
		public const int ERROR_NOT_ALLOWED = 204;

		public const int ERROR_INCORRECT_DATA = 220;
		public const int ERROR_NO_SUCH_CARD_ISSUER = 221;
		public const int ERROR_NO_SUCH_CARD_NUMBER = 222;
		public const int ERROR_EXPIRED = 223;
		public const int ERROR_INVALID_EXPIRATION = 224;
		public const int ERROR_INVALID_CVV = 225;

		public const int ERROR_CALL_ISSUER = 240;

		public const int ERROR_PICK_UP_CARD = 250;
		public const int ERROR_LOST_CARD = 251;
		public const int ERROR_STOLEN_CARD = 252;
		public const int ERROR_FRAUDULANT_CARD = 253;

		public const int ERROR_DECLINED_WITH_INSTRUCTIONS = 260;
		public const int ERROR_DECLINED_STOP_ALL_RECURRING = 261;
		public const int ERROR_DECLINED_STOP_THIS_RECURRING = 262;
		public const int ERROR_DECLINED_UPDATE_DATA_AVAILABLE = 263;
		public const int ERROR_DECLINED_RETRY_LATER = 264;

		public const int ERROR_REJECTED_BY_GATEWAY = 300;

		public const int ERROR_RETURNED_BY_PROCESSOR = 400;

		public const int ERROR_INVALID_MERCHANT_CONFIG = 410;
		public const int ERROR_INACTIVE_MERCHANT_ACCOUNT = 411;

		public const int ERROR_COM_ERROR = 420;
		public const int ERROR_COM_ERROR_WITH_ISSUER = 421;

		public const int ERROR_DUPLICATE = 430;

		public const int ERROR_FORMAT_ERROR = 440;
		public const int ERROR_INVALID_TRANSACTION_INFO = 441;

		public const int ERROR_FEATURE_NOT_AVAILABLE = 460;
		public const int ERROR_UNSUPPORTED_CARD_TYPE = 461;
		// --- END RESPONSE_CODES ---


		// --- AVS_RESPOSE_CODES ---
		public const string AVS_EXACT_MATCH_9 = "X";

		public const string AVS_EXACT_MATCH_5 = "Y";
		public const string AVS_EXACT_MATCH_5_D = "D";
		public const string AVS_EXACT_MATCH_5_M = "M";

		public const string AVS_ADDRESS_MATCH_ONLY = "A";
		public const string AVS_ADDRESS_MATCH_ONLY_B = "B";

		public const string AVS_ZIP_MATCH_ONLY_9 = "W";
		public const string AVS_ZIP_MATCH_ONLY_5 = "Z";
		public const string AVS_ZIP_MATCH_ONLY_5_P = "P";
		public const string AVS_ZIP_MATCH_ONLY_5_L = "L";

		public const string AVS_NO_MATCH = "N";
		public const string AVS_NO_MATCH_C = "C";

		public const string AVS_ADDRESS_UNAVAILABLE = "U";

		public const string AVS_NON_US_DOESNT_PARTICIPATE = "G";
		public const string AVS_NON_US_DOESNT_PARTICIPATE_I = "I";

		public const string AVS_ISSUER_SYSTEM_UNAVAILABLE = "R";

		public const string AVS_NOT_A_MAILPHONE_ORDER = "E";

		public const string AVS_SERVICE_NOT_SUPPORTED = "S";

		public const string AVS_NOT_AVAILABLE = "0";
		public const string AVS_NOT_AVAILABLE_O = "O";
		public const string AVS_NOT_AVAILABLE_B = "B";
		// --- END AVS_RESPOSE_CODES ---

		// --- CVV_RESPOSE_CODES ---
		public const string CVV_MATCH = "M";
		public const string CVV_NOT_MATCHED = "N";
		public const string CVV_NOT_PROCESSED = "P";
		public const string CVV_NOT_PRESENT = "S";
		public const string CVV_NOT_CERTIFIED = "U";
		// --- END CVV_RESPOSE_CODES ---

		public const Dictionary<int, string> errorMessages = new Dictionary<int, string>() {
			{ ERROR_NONE, "Transaction was approved" },
			{ ERROR_DECLINED, "Transaction was declined" },
			{ ERROR_DO_NOT_HONOR, "Do not honor" },
			{ ERROR_INSUFFICENT_FUNDS, "Insufficient funds" },
			{ ERROR_OVER_LIMIT, "Over limit" },
			{ ERROR_NOT_ALLOWED, "Transaction not allowed" },
			{ ERROR_INCORRECT_DATA, "Incorrect payment data" },
			{ ERROR_NO_SUCH_CARD_ISSUER, "No such card issuer" },
			{ ERROR_NO_SUCH_CARD_NUMBER, "Card number not on file with issuer" },
			{ ERROR_EXPIRED, "Card is expired" },
			{ ERROR_INVALID_EXPIRATION, "Invalid card expiration date" },
			{ ERROR_INVALID_CVV, "Invalid card security code" },
			{ ERROR_CALL_ISSUER, "Call card issuer for more information" },
			{ ERROR_PICK_UP_CARD, "Pick up card" },
			{ ERROR_LOST_CARD, "Lost card" },
			{ ERROR_STOLEN_CARD, "Stolen card" },
			{ ERROR_FRAUDULANT_CARD, "Fraudulant card" },
			{ ERROR_DECLINED_WITH_INSTRUCTIONS, "Declined with further instructions" },
			{ ERROR_DECLINED_STOP_ALL_RECURRING, "Declined: Stop all recurring payments" },
			{ ERROR_DECLINED_STOP_THIS_RECURRING, "Declined: Stop this recurring payment" },
			{ ERROR_DECLINED_UPDATE_DATA_AVAILABLE, "Declined: Updated cardholder data available" },
			{ ERROR_DECLINED_RETRY_LATER, "Declined: Retry again later" },
			{ ERROR_REJECTED_BY_GATEWAY, "Transaction was rejected by the gateway" },
			{ ERROR_RETURNED_BY_PROCESSOR, "Transaction error return from processor" },
			{ ERROR_INVALID_MERCHANT_CONFIG, "Invalid merchant configuration" },
			{ ERROR_INACTIVE_MERCHANT_ACCOUNT, "Merchant account is inactive" },
			{ ERROR_COM_ERROR, "Communications error" },
			{ ERROR_COM_ERROR_WITH_ISSUER, "Communications error with issuer" },
			{ ERROR_DUPLICATE, "Duplicate transaction" },
			{ ERROR_FORMAT_ERROR, "Processor format error" },
			{ ERROR_INVALID_TRANSACTION_INFO, "Invalid transaction information" },
			{ ERROR_FEATURE_NOT_AVAILABLE, "Feature not available" },
			{ ERROR_UNSUPPORTED_CARD_TYPE, "Unsupported card type" }
		};
	}
}
