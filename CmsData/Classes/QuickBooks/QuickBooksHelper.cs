using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using UtilityExtensions;
using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Intuit.Ipp.Services;
using Intuit.Ipp.Data;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;

namespace CmsData.Classes.QuickBooks
{
	public class QuickBooksHelper
	{
		// ---------------------- Start unified routines ----------------------

		public List<QBAccount> getAllAccounts() // Max per page is 100
		{
			var acctList = new List<QBAccount>();

			switch (getActiveConnection().DataSource)
			{
				case "QBO":
					{
						QBOHelper qbo = new QBOHelper();
						var accts = qbo.ListAllAccounts();

						foreach (var item in accts)
						{
							var newAcct = new QBAccount();
							newAcct.ID = item.Id.Value;
							newAcct.Name = item.Name;
							acctList.Add(newAcct);
						}
						break;
					}

				case "QBD":
					{
						QBDHelper qbd = new QBDHelper();
						var accts = qbd.ListAllAccounts();

						foreach (var item in accts)
						{
							var newAcct = new QBAccount();
							newAcct.ID = item.Id.Value;
							newAcct.Name = item.Name;
							acctList.Add(newAcct);
						}
						break;
					}

				default: break;
			}

			return acctList;
		}

		public int CommitJournalEntries(string sDescription, List<QBJournalEntryLine> jelEntries) // Max per page is 100
		{
			int iReturn = 0;

			switch (getActiveConnection().DataSource)
			{
				case "QBO":
					{
						QBOHelper qbo = new QBOHelper();
						iReturn = qbo.CommitJournalEntries(sDescription, jelEntries);
						break;
					}

				case "QBD":
					{
						QBDHelper qbd = new QBDHelper();
						iReturn = qbd.CommitJournalEntries(sDescription, jelEntries);
						break;
					}

				default: break;
			}

			return iReturn;
		}

		// ---------------------- Start common connection and access routines ----------------------
		protected IToken itToken;
		protected String sBaseURL;

		// Begin Common QuickBooks Routines
		public const string QB_EV_ID = "QBID";
		public const string QB_EV_SYNC_TOKEN = "QBSyncToken";
		public const string QB_EV_AMOUNT = "Amount";

		public const string QB_REQUEST_TOKEN = "https://oauth.intuit.com/oauth/v1/get_request_token";
		public const string QB_AUTHORIZE = "https://appcenter.intuit.com/Connect/Begin";
		public const string QB_ACCESS_TOKEN = "https://oauth.intuit.com/oauth/v1/get_access_token";
		public const string QB_DISCONNECT = "https://appcenter.intuit.com/api/v1/Connection/Disconnect";

		public const string QB_CALLBACK = "/Quickbooks/RequestAccessToken";

		// Account Creation Codes
		public const int QB_ACCOUNT_NONPROFITINCOME = 1;

		public QuickBooksHelper() { }

		// Request constructor is needed for connection and disconnection
		public QuickBooksHelper(HttpRequestBase request)
		{
			sBaseURL = request.Url.Scheme + "://" + request.Url.Authority;
		}

		public IToken GetCurrentToken()
		{
			return itToken;
		}

		public void SetCurrentToken(IToken itNew)
		{
			itToken = itNew;
		}

		public string RequestOAuthToken()
		{
			itToken = getOAuthSession().GetRequestToken();

			DbUtil.Db.ExecuteCommand("UPDATE dbo.QBConnections SET Active = 0");

			QBConnection qbc = new QBConnection();
			qbc.Creation = DateTime.Now;
			qbc.DataSource = "";
			qbc.Token = itToken.Token;
			qbc.UserID = Util.UserId;
			qbc.Active = 1;
			qbc.Secret = "";
			qbc.RealmID = "";

			DbUtil.Db.QBConnections.InsertOnSubmit(qbc);
			DbUtil.Db.SubmitChanges();

			// generate a user authorize url for this token (which you can use in a redirect from the current site)
			string authorizationLink = getOAuthSession().GetUserAuthorizationUrlForToken(itToken, getCallback());

			return authorizationLink;
		}

		public bool RequestAccessToken(string sRealmID, string sVerifier, string sDataSource)
		{
			IToken accessToken = getOAuthSession().ExchangeRequestTokenForAccessToken(itToken, sVerifier);

			QBConnection qbc = (from i in DbUtil.Db.QBConnections
									  where i.Active == 1
									  select i).FirstOrDefault();

			qbc.Token = accessToken.Token;
			qbc.Secret = accessToken.TokenSecret;
			qbc.RealmID = sRealmID;
			qbc.DataSource = sDataSource;
			DbUtil.Db.SubmitChanges();

			return true;
		}

		public QBConnection getActiveConnection()
		{
			return (from i in DbUtil.Db.QBConnections
					  where i.Active == 1
					  select i).FirstOrDefault();
		}

		public bool Disconnect()
		{
			bool complete = doDisconnect();

			if (complete) DbUtil.Db.ExecuteCommand("UPDATE dbo.QBConnections SET Active = 0 WHERE Active = 1");

			return complete;
		}

		public string getToken()
		{
			return System.Configuration.ConfigurationManager.AppSettings["QuickBooksToken"] ?? "";
		}

		public string getKey()
		{
			return System.Configuration.ConfigurationManager.AppSettings["QuickBooksKey"] ?? "";
		}

		public string getSecret()
		{
			return System.Configuration.ConfigurationManager.AppSettings["QuickBooksSecret"] ?? "";
		}

		public string getCallback()
		{
			return sBaseURL + QB_CALLBACK;
		}

		public string getBaseURL()
		{
			return sBaseURL;
		}

		public OAuthConsumerContext getOAuthConsumerContext()
		{
			return new OAuthConsumerContext
			{
				ConsumerKey = getKey(),
				SignatureMethod = SignatureMethod.HmacSha1,
				ConsumerSecret = getSecret()
			};
		}

		public OAuthSession getOAuthSession()
		{
			return new OAuthSession(getOAuthConsumerContext(), QB_REQUEST_TOKEN, QB_AUTHORIZE, QB_ACCESS_TOKEN, getCallback());
		}

		public ServiceContext getServiceContext()
		{
			var qbc = (from i in DbUtil.Db.QBConnections
						  where i.Active == 1
						  select i).SingleOrDefault();

			var orv = new OAuthRequestValidator(qbc.Token, qbc.Secret, getKey(), getSecret());

			if (qbc.DataSource == "QBO")
				return new ServiceContext(orv, getToken(), qbc.RealmID, IntuitServicesType.QBO);
			else
				return new ServiceContext(orv, getToken(), qbc.RealmID, IntuitServicesType.QBD);
		}

		public DataServices getDataService()
		{
			return new DataServices(getServiceContext());
		}

		public TokenBase getAccessToken()
		{
			var qbc = (from i in DbUtil.Db.QBConnections
						  where i.Active == 1
						  select i).SingleOrDefault();

			return new TokenBase { Token = qbc.Token, ConsumerKey = getKey(), TokenSecret = qbc.Secret };
		}

		public bool doDisconnect()
		{
			OAuthSession oas = getOAuthSession();
			oas.ConsumerContext.UseHeaderForOAuthParameters = true;
			oas.AccessToken = getAccessToken();

			IConsumerRequest icr = oas.Request();

			icr = icr.Get();
			icr = icr.ForUrl(QB_DISCONNECT);
			icr = icr.SignWithToken();
			var ret = icr.ToWebResponse();

			if (ret.StatusCode.ToInt() == 200) return true;
			else return false;
		}

		public bool hasActiveConnection()
		{
			return (from i in DbUtil.Db.QBConnections
					  where i.Active == 1
					  select i).Count() > 0;
		}
	}
}
