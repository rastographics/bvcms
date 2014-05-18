using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;
using UtilityExtensions;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace CmsData
{
	public class SagePayments
	{
		string login;
		string key;
		CMSDataContext Db;
		bool testing;

		public SagePayments(CMSDataContext Db, bool testing)
		{
#if DEBUG2
            testing = true;
#endif
			this.Db = Db;
			if (testing)
			{
				login = "287793447481";
				key = "S4S3N4D2W1D9";
			}
			else
			{
				login = Db.Setting("M_id", "");
				key = Db.Setting("M_key", "");
			}
			this.testing = testing;
		}
		private XElement getResponse(string xml)
		{
			var x = XDocument.Parse(xml);
			var t = x.Descendants("Table1").First();
			var success = t.Element("SUCCESS");
			if (success != null && success.Value.ToLower() == "false")
			{
				var message = t.Element("MESSAGE").Value;
				throw new Exception(message);
			}
			return t;
		}
		public void storeVault(int PeopleId, 
			string type, string cardnumber, string expires, string cardcode,
			string routing, string account, bool giving)
		{
			var p = Db.LoadPersonById(PeopleId);
			var pi = p.PaymentInfo();
			if (pi == null)
			{
				pi = new PaymentInfo();
				p.PaymentInfos.Add(pi);
			}
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/wsVault/wsVault.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = login;
			coll["M_KEY"] = key;

			XElement resp = null;
			if (type == "C")
			{
				coll["CARDNUMBER"] = cardnumber;
				coll["EXPIRATION_DATE"] = expires;

				if (pi.SageCardGuid == null)
				{
					var b = wc.UploadValues("INSERT_CREDIT_CARD_DATA", "POST", coll);
					var ret = Encoding.ASCII.GetString(b);
					resp = getResponse(ret);
					pi.SageCardGuid = Guid.Parse(resp.Element("GUID").Value);
				}
				else
				{
					coll["GUID"] = pi.SageCardGuid.ToString().Replace("-", "");
					if (!cardnumber.StartsWith("X"))
					{
						var b = wc.UploadValues("UPDATE_CREDIT_CARD_DATA", "POST", coll);
						var ret = Encoding.ASCII.GetString(b);
						resp = getResponse(ret);
					}
					else
					{
						var b = wc.UploadValues("UPDATE_CREDIT_CARD_EXPIRATION_DATE", "POST", coll);
						var ret = Encoding.ASCII.GetString(b);
						resp = getResponse(ret);
					}
				}
			}
			else
			{
				coll["ROUTING_NUMBER"] = routing; // 064000020
				coll["ACCOUNT_NUMBER"] = account; // my account number
				coll["C_ACCT_TYPE"] = "DDA";

				if (pi.SageBankGuid == null)
				{
					var b = wc.UploadValues("INSERT_VIRTUAL_CHECK_DATA", "POST", coll);
					var ret = Encoding.ASCII.GetString(b);
					resp = getResponse(ret);
					pi.SageBankGuid = Guid.Parse(resp.Element("GUID").Value);
				}
				else
				{
					if (!account.StartsWith("X"))
					{
						coll["GUID"] = pi.SageBankGuid.ToString().Replace("-", "");
						var b = wc.UploadValues("UPDATE_VIRTUAL_CHECK_DATA", "POST", coll);
						var ret = Encoding.ASCII.GetString(b);
						resp = getResponse(ret);
					}
				}
			}
			pi.MaskedAccount = Util.MaskAccount(account);
			pi.Routing = Util.Mask(new StringBuilder(routing), 2);
			pi.MaskedCard = Util.MaskCC(cardnumber);
			pi.Ccv = cardcode;
			pi.Expires = expires;
			pi.Testing = testing;
			if (giving)
				pi.PreferredGivingType = type;
			else
				pi.PreferredPaymentType = type;
			Db.SubmitChanges();
			//var sw =new StringWriter();
			//ObjectDumper.Write(pi, 0, sw);

			//Util.SendMsg(DbUtil.AdminMail, Db.CmsHost, Util.TryGetMailAddress("david@bvcms.com"), "Sage Vault",
			//	"<a href='{0}{1}'>{2}</a><br>{3},{4}<br><pre>{5}</pre>".Fmt(
			//	Db.CmsHost, p.PeopleId, p.Name, type, giving ? "giving" : "regular", sw.ToString()),
			//	Util.ToMailAddressList("david@bvcms.com"), 0, null);
		}
		public void deleteVaultData(int PeopleId)
		{
			var p = Db.LoadPersonById(PeopleId);
			var pi = p.PaymentInfo();
			if (pi == null)
				return;
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/wsVault/wsVault.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = login; // 779966396962
			coll["M_KEY"] = key; // T7N8I1M1F7L9

			if (pi.SageCardGuid.HasValue)
			{
				coll["GUID"] = pi.SageCardGuid.ToString().Replace("-", "");
				var b = wc.UploadValues("DELETE_DATA", "POST", coll);
				var ret = Encoding.ASCII.GetString(b);
			}
			if (pi.SageBankGuid.HasValue)
			{
				coll["GUID"] = pi.SageBankGuid.ToString().Replace("-", "");
				var b = wc.UploadValues("DELETE_DATA", "POST", coll);
				var ret = Encoding.ASCII.GetString(b);
			}

			pi.SageCardGuid = null;
			pi.SageBankGuid = null;
			pi.MaskedCard = null;
			pi.MaskedAccount = null;
			pi.Ccv = null;
			Db.SubmitChanges();
		}

		public TransactionResponse voidTransactionRequest(string reference)
		{
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/transaction_processing.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = login;
			coll["M_KEY"] = key;
			coll["T_REFERENCE"] = reference;
			var b = wc.UploadValues("BANKCARD_VOID", "POST", coll);
			var ret = Encoding.ASCII.GetString(b);
			var resp = getResponse(ret);
			var tr = new TransactionResponse
			{
				Approved = resp.Element("APPROVAL_INDICATOR").Value == "A",
				AuthCode = resp.Element("CODE").Value,
				Message = resp.Element("MESSAGE").Value,
				TransactionId = resp.Element("REFERENCE").Value
			};
			return tr;
		}
		public TransactionResponse voidCheckRequest(string reference)
		{
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/transaction_processing.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = login;
			coll["M_KEY"] = key;
			coll["T_REFERENCE"] = reference;
            var b = wc.UploadValues("VIRTUAL_CHECK_VOID", "POST", coll);
			var ret = Encoding.ASCII.GetString(b);
			var resp = getResponse(ret);
			var tr = new TransactionResponse
			{
				Approved = resp.Element("APPROVAL_INDICATOR").Value == "A",
				AuthCode = resp.Element("CODE").Value,
				Message = resp.Element("MESSAGE").Value,
				TransactionId = resp.Element("REFERENCE").Value
			};
			return tr;
		}

		public TransactionResponse creditTransactionRequest(string reference, Decimal amt)
		{
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/transaction_processing.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = login;
			coll["M_KEY"] = key;
			coll["T_REFERENCE"] = reference;
			coll["T_AMT"] = amt.ToString("n2");

			var b = wc.UploadValues("BANKCARD_CREDIT", "POST", coll);
			var ret = Encoding.ASCII.GetString(b);
			var resp = getResponse(ret);
			var tr = new TransactionResponse
			{
				Approved = resp.Element("APPROVAL_INDICATOR").Value == "A",
				AuthCode = resp.Element("CODE").Value,
				Message = resp.Element("MESSAGE").Value,
				TransactionId = resp.Element("REFERENCE").Value
			};
			return tr;
		}
		public TransactionResponse creditCheckTransactionRequest(string reference, Decimal amt)
		{
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/transaction_processing.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = login;
			coll["M_KEY"] = key;
			coll["T_REFERENCE"] = reference;
			coll["T_AMT"] = amt.ToString("n2");

			var b = wc.UploadValues("VIRTUAL_CHECK_CREDIT_BY_REFERENCE", "POST", coll);
			var ret = Encoding.ASCII.GetString(b);
			var resp = getResponse(ret);
			var tr = new TransactionResponse
			{
				Approved = resp.Element("APPROVAL_INDICATOR").Value == "A",
				AuthCode = resp.Element("CODE").Value,
				Message = resp.Element("MESSAGE").Value,
				TransactionId = resp.Element("REFERENCE").Value
			};
			return tr;
		}

		public TransactionResponse createTransactionRequest(int PeopleId, decimal amt,
			string cardnumber, string expires, string description, int tranid, string cardcode,
			string email, string first, string last,
			string addr, string city, string state, string zip, string phone)
		{
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/transaction_processing.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = login;
			coll["M_KEY"] = key;
			coll["T_AMT"] = amt.ToString("n2");
			coll["C_NAME"] = first + " " + last;
			coll["C_ADDRESS"] = addr;
			coll["C_CITY"] = city;
			coll["C_STATE"] = state;
			coll["C_ZIP"] = zip;
			coll["C_COUNTRY"] = "";
			coll["C_EMAIL"] = email;
			coll["C_CARDNUMBER"] = cardnumber;
			coll["C_EXP"] = expires;
			coll["C_CVV"] = cardcode;
			coll["T_CUSTOMER_NUMBER"] = PeopleId.ToString();
			coll["T_ORDERNUM"] = tranid.ToString();
			coll["C_TELEPHONE"] = phone;
			AddShipping(coll);

			var b = wc.UploadValues("BANKCARD_SALE", "POST", coll);
			var ret = Encoding.ASCII.GetString(b);
			var resp = getResponse(ret);
			var tr = new TransactionResponse
			{
				Approved = resp.Element("APPROVAL_INDICATOR").Value == "A",
				AuthCode = resp.Element("CODE").Value,
				Message = resp.Element("MESSAGE").Value,
				TransactionId = resp.Element("REFERENCE").Value
			};
			return tr;
		}
		public TransactionResponse createCheckTransactionRequest(int PeopleId, decimal amt,
			string routing, string acct, string description, int tranid,
			string email, string first, string middle, string last, string suffix,
			string addr, string city, string state, string zip, string phone)
		{
			try
			{

				var wc = new WebClient();
				wc.BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/transaction_processing.asmx/";
				var coll = new NameValueCollection();
				coll["M_ID"] = login;
				coll["M_KEY"] = key;
				coll["C_ORIGINATOR_ID"] = Db.Setting("SageOriginatorId", ""); // 1031360711, 1031412710
				coll["C_FIRST_NAME"] = first;
			    coll["C_MIDDLE_INITIAL"] = middle.Truncate(1) ?? "";
				coll["C_LAST_NAME"] = last;
				coll["C_SUFFIX"] = suffix;
				coll["C_ADDRESS"] = addr;
				coll["C_CITY"] = city;
				coll["C_STATE"] = state;
				coll["C_ZIP"] = zip;
				coll["C_COUNTRY"] = "";
				coll["C_EMAIL"] = email;
				coll["C_RTE"] = routing;
				coll["C_ACCT"] = acct;
				coll["C_ACCT_TYPE"] = "DDA";
				coll["T_AMT"] = amt.ToString("n2");
				coll["T_ORDERNUM"] = tranid.ToString();
				coll["C_TELEPHONE"] = phone;
				AddShipping(coll);

				var b = wc.UploadValues("VIRTUAL_CHECK_PPD_SALE", "POST", coll);
				var ret = Encoding.ASCII.GetString(b);
				var resp = getResponse(ret);
				var tr = new TransactionResponse
				{
					Approved = resp.Element("APPROVAL_INDICATOR").Value == "A",
					AuthCode = resp.Element("CODE").Value,
					Message = resp.Element("MESSAGE").Value,
					TransactionId = resp.Element("REFERENCE").Value
				};
				return tr;
			}
			catch (Exception ex)
			{
				return new TransactionResponse { Approved = false, Message = ex.Message, };
			}
		}
		public TransactionResponse createVaultTransactionRequest(int PeopleId, decimal amt, string description, int tranid, string type)
		{
			var p = Db.LoadPersonById(PeopleId);
			var pi = p.PaymentInfo();
			if (pi == null)
				return new TransactionResponse 
				{ 
					Approved = false,
					Message = "missing payment info",
				};

			XElement resp = null;
			if ((type ?? "B") == "B" && pi.SageBankGuid.HasValue) // Bank Account (check)
			{
				var wc = new WebClient();
				wc.BaseAddress = "https://www.sagepayments.net/web_services/wsVault/wsVaultVirtualCheck.asmx/";
				var coll = new NameValueCollection();

				coll["M_ID"] = login;
				coll["M_KEY"] = key;
				var guid = pi.SageBankGuid.ToString().Replace("-", "");
				coll["GUID"] = guid;
				coll["C_ORIGINATOR_ID"] = Db.Setting("SageOriginatorId", "");
				coll["C_FIRST_NAME"] = pi.FirstName ?? p.FirstName;
			    coll["C_MIDDLE_INITIAL"] = (pi.MiddleInitial ?? p.MiddleName).Truncate(1) ?? "";
				coll["C_LAST_NAME"] = pi.LastName ?? p.LastName;
				coll["C_SUFFIX"] = pi.Suffix ?? p.SuffixCode;
				coll["C_ADDRESS"] = pi.Address ?? p.PrimaryAddress;
				coll["C_CITY"] = pi.City ?? p.PrimaryCity;
				coll["C_STATE"] = pi.State ?? p.PrimaryState;
				coll["C_ZIP"] = pi.Zip ?? p.PrimaryZip;
				coll["C_COUNTRY"] = p.PrimaryCountry;
				coll["C_EMAIL"] = p.EmailAddress;
				coll["T_AMT"] = amt.ToString("n2");
				coll["T_ORDERNUM"] = tranid.ToString();
				coll["C_TELEPHONE"] = pi.Phone;
				AddShipping(coll);

				var b = wc.UploadValues("VIRTUAL_CHECK_PPD_SALE", "POST", coll);
				var ret = Encoding.ASCII.GetString(b);
				resp = getResponse(ret);
			}
			else
			{
				var wc = new WebClient();
				wc.BaseAddress = "https://www.sagepayments.net/web_services/wsVault/wsVaultBankcard.asmx/";
				var coll = new NameValueCollection();

				coll["M_ID"] = login;
				coll["M_KEY"] = key;
				var guid = pi.SageCardGuid.ToString().Replace("-", "");
				coll["GUID"] = guid;
                coll["C_NAME"] = (pi.FirstName ?? p.FirstName) + (pi.MiddleInitial ?? p.MiddleName).Truncate(1) + (p.LastName ?? pi.LastName);
				coll["C_ADDRESS"] = pi.Address ?? p.PrimaryAddress;
				coll["C_CITY"] = pi.City ?? p.PrimaryCity;
				coll["C_STATE"] = pi.State ?? p.PrimaryState;
				coll["C_ZIP"] = pi.Zip ?? p.PrimaryZip;
				coll["C_COUNTRY"] = p.PrimaryCountry;
				coll["C_EMAIL"] = p.EmailAddress;
				coll["T_AMT"] = amt.ToString("n2");
				coll["T_ORDERNUM"] = tranid.ToString();
				coll["C_TELEPHONE"] = p.HomePhone;
			    coll["T_CUSTOMER_NUMBER"] = p.PeopleId.ToString();
				coll["C_CVV"] = pi.Ccv;
				AddShipping(coll);

				var b = wc.UploadValues("VAULT_BANKCARD_SALE_CVV", "POST", coll);
				var ret = Encoding.ASCII.GetString(b);
				resp = getResponse(ret);
			}
			var tr = new TransactionResponse
			{
				Approved = resp.Element("APPROVAL_INDICATOR").Value == "A",
				AuthCode = resp.Element("CODE").Value,
				Message = resp.Element("MESSAGE").Value,
				TransactionId = resp.Element("REFERENCE").Value
			};
			return tr;
		}
		private void AddShipping(NameValueCollection coll)
		{
			// these not needed
			coll["T_SHIPPING"] = "";
			coll["T_TAX"] = "";
			coll["C_FAX"] = "";
			coll["C_SHIP_NAME"] = "";
			coll["C_SHIP_CITY"] = "";
			coll["C_SHIP_ADDRESS"] = "";
			coll["C_SHIP_ZIP"] = "";
			coll["C_SHIP_STATE"] = "";
			coll["C_SHIP_COUNTRY"] = "";
		}
		public DataSet SettledBatchSummary(DateTime start, DateTime end, bool IncludeCreditCard, bool IncludeVirtualCheck)
		{
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/reporting.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = login;
			coll["M_KEY"] = key;
			coll["START_DATE"] = start.ToShortDateString();
			coll["END_DATE"] = end.ToShortDateString();
			coll["INCLUDE_BANKCARD"] = IncludeCreditCard.ToString();
			coll["INCLUDE_VIRTUAL_CHECK"] = IncludeVirtualCheck.ToString();

			var b = wc.UploadValues("VIEW_SETTLED_BATCH_SUMMARY", "POST", coll);
			var ret = Encoding.ASCII.GetString(b);
			var ds = new DataSet();
			ds.ReadXml(new StringReader(ret));
			return ds;
		}
		public DataSet SettledBatchListing(string batchref, string type)
		{
			var wc = new WebClient();
			wc.BaseAddress = "https://www.sagepayments.net/web_services/vterm_extensions/reporting.asmx/";
			var coll = new NameValueCollection();
			coll["M_ID"] = login;
			coll["M_KEY"] = key;
			coll["BATCH_REFERENCE"] = batchref;

			string method = null;
			switch (type)
			{
				case "eft":
					method = "VIEW_VIRTUAL_CHECK_SETTLED_BATCH_LISTING";
					break;
				case "bankcard":
					method = "VIEW_BANKCARD_SETTLED_BATCH_LISTING";
					break;
			}
			var b = wc.UploadValues(method, "POST", coll);
			var ret = Encoding.ASCII.GetString(b);
			var ds = new DataSet();
			ds.ReadXml(new StringReader(ret));
			return ds;
		}
	}
}