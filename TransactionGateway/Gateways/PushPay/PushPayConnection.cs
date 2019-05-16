using CmsData;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TransactionGateway.ApiModels;
using TransactionGateway.Entities;
using TransactionGateway.Enums;
using UtilityExtensions;
using Organization = TransactionGateway.ApiModels.Organization;

namespace TransactionGateway
{
    /// <summary>
    ///     Centralized logic for communicating with the Pushpay payment server
    /// </summary>
    public class PushpayConnection
    {
        private ApiClient _client;
        private string accessToken;
        private string refreshToken;
        private string _pushpayClientID;
        private string _pushpayClientSecret;
        private string _oAuth2TokenEndpoint;
        private string _pushpayAPIBaseUrl;
        private string _touchpointAuthServer;
        private CMSDataContext db;

        public PushpayConnection(string access_token, string refresh_token, CMSDataContext db_context,
            string pushpayAPIBaseUrl, string pushpayClientID, string pushpayClientSecret, string oAuth2TokenEndpoint,
            string touchpointAuthServer, string OAuth2TokenEndpoint
            )
        {
            accessToken = access_token;
            refreshToken = refresh_token;
            _pushpayClientID = pushpayClientID;
            _pushpayClientSecret = pushpayClientSecret;
            _oAuth2TokenEndpoint = oAuth2TokenEndpoint;
            _pushpayAPIBaseUrl = pushpayAPIBaseUrl;
            _touchpointAuthServer = touchpointAuthServer;
            db = db_context;
        }

        public PushpayConnection(string access_token, string refresh_token, CMSDataContext db_context)
        {
            accessToken = access_token;
            refreshToken = refresh_token;
            db = db_context;
        }

        /// <summary>
        ///     Helper method to create a client connection
        /// </summary>
        /// <returns></returns>
        private async Task<ApiClient> CreateClient()
        {
            if (_client == null)
            {
                string baseUrl = _pushpayAPIBaseUrl;
                if (string.IsNullOrWhiteSpace(baseUrl)) RaiseError(new Exception("Please provide a PushpayAPIBaseUrl in your configuration AppSettings"));
                _client = new ApiClient(baseUrl);

                // Authenticate
                string token = await AuthenticateClient();

                _client.SetBearerToken(token);
            }
            return _client;
        }

        /// <summary>
        ///     Helper method to authenticate the client. Run this on client creation and if the bearer token expires
        /// </summary>
        /// <returns></returns>
        public async Task<string> AuthenticateClient()
        {
            //_pushpayClientID, _pushpayClientSecret, _oAuth2TokenEndpoint
            if (string.IsNullOrWhiteSpace(_pushpayClientID) || string.IsNullOrWhiteSpace(_pushpayClientSecret))
            {
                RaiseError(new Exception("Please provide Pushpay client ID and secret tokens in your web.config"));
            }

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                RaiseError(new Exception("No refresh token set on this DB. Please authenticate with PushPay first"));
            }

            // Update the access token required by Pushpay
            string newAccessToken = null;
            string newRefreshToken = null;

            // exchange old refresh token for a new access and refresh token
            Dictionary<string, string> post = null;
            post = new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token"}
                    ,{"refresh_token", refreshToken}
                };

            var client = new HttpClient();

            // Setting a "basic auth" header
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", _pushpayClientID, _pushpayClientSecret)
                        )));

            var postContent = new FormUrlEncodedContent(post);
            var response = await client.PostAsync(_oAuth2TokenEndpoint, postContent);
            var content = await response.Content.ReadAsStringAsync();

            // received tokens from authorization server
            var json = JObject.Parse(content);
            newAccessToken = json["access_token"].ToString();
            newRefreshToken = json["refresh_token"].ToString();

            if (json["refresh_token"] == null || json["access_token"] == null)
            {
                RaiseError(new Exception("Failed to retrieve access token, error was: " + response.ReasonPhrase));
            }
            db.SetSetting("PushPayAccessToken", newAccessToken);
            db.SetSetting("PushPayRefreshToken", newRefreshToken);
            Console.WriteLine("PushPay authenticated!");
            return newAccessToken;
        }

        public async Task<AccessToken> AuthorizationCodeCallback(string _authCode)
        {
            // exchange authorization code at authorization server for an access and refresh token
            Dictionary<string, string> post = null;
            post = new Dictionary<string, string>
            {
                { "client_id", _pushpayClientID}
                ,{"client_secret", _pushpayClientSecret}
                ,{"grant_type", "authorization_code"}
                ,{"code", _authCode}
                ,{"redirect_uri", _touchpointAuthServer}
            };

            var client = new HttpClient();
            //Setting a "basic auth" header
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", _pushpayClientID, _pushpayClientSecret)
                        )));
            var postContent = new FormUrlEncodedContent(post);
            var response = await client.PostAsync(_oAuth2TokenEndpoint, postContent);
            var content = await response.Content.ReadAsStringAsync();
            var _accessToken = new AccessToken();
            // exchange code for tokens from authorization server
            try
            {
                var json = JObject.Parse(content);
                _accessToken.access_token = json["access_token"].ToString();
                _accessToken.token_type = json["token_type"].ToString();
                _accessToken.expires_in = Convert.ToInt64(json["expires_in"].ToString());
                if (json["refresh_token"] != null)
                {
                    _accessToken.refresh_token = json["refresh_token"].ToString();
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("form", ex.Message);
                throw ex;
            }
            if (_accessToken != null)
            {
                return _accessToken;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///     Centralized error handling
        /// </summary>
        /// <param name="ex"></param>
        private void RaiseError(Exception ex)
        {
            throw ex;
        }

        public async Task<IEnumerable<Merchant>> GetMerchants()
        {
            var result = await GET<MerchantList>("my/merchants");
            return result.Items;
        }        

        public async Task<Merchant> GetMerchant(string merchantKey)
        {
            return await GET<Merchant>($"merchant/{merchantKey}");
        }

        public async Task<Batch> GetBatch(string merchantKey, string batchKey)
        {
            return await GET<Batch>($"merchant/{merchantKey}/batch/{batchKey}");
        }

        public async Task<BatchList> GetBatchesForMerchantSince(string merchantKey, DateTime startDate, int page = 0)
        {
            return await GET<BatchList>($"merchant/{merchantKey}/batches?from={format(startDate)}&page={page}");
        }

        public async Task<PaymentList> GetPaymentsForMerchantSince(string merchantKey, DateTime startDate, int page = 0)
        {
            // todo: add payment types filter to query
            return await GET<PaymentList>($"merchant/{merchantKey}/payments?updatedFrom={format(startDate)}&page={page}");
        }

        public async Task<FundList> GetFundsForMerchant(string merchantKey)
        {
            return await GET<FundList>($"merchant/{merchantKey}/funds");
        }

        public async Task<FundList> GetFundsForOrganization(string organizationKey)
        {
            //GET /v1/organization/{organizationKey}/funds
            return await GET<FundList>($"organization/{organizationKey}/funds");
        }

        public async Task<PaymentList> GetPaymentsForBatch(string merchantKey, string batchKey, int page = 0)
        {
            // GET /v1/merchant/{merchantKey}/batch/{batchKey}/payments
            return await GET<PaymentList>($"merchant/{merchantKey}/batch/{batchKey}/payments?page={page}");
        }

        public async Task<IEnumerable<Merchant>> SearchMerchants(string handle, int page = 0, int pageSize = 25)
        {
            var result = await GET<MerchantList>($"merchants?handle={handle}&page={page}&pageSize={pageSize}");
            return result.Items;
        }

        public async Task<IEnumerable<Organization>> GetOrganizations()
        {
            var result = await GET<OrganizationList>("organizations/in-scope?status=Active");
            return result.Items;
        }

        public async Task<Organization> GetOrganization(string orgKey)
        {
            return await GET<Organization>($"organization/{orgKey}");
        }

        public async Task<PaymentList> GetPaymentsForOrganizationSince(string orgKey, DateTime startdate, int page = 0)
        {
            var list = await GET<PaymentList>($"organization/{orgKey}/payments?status=Success&updatedFrom={format(startdate)}&page={page}");
            var splitPayments = new List<Payment>();
            foreach (var payment in list.Items)
            {
                if (payment.SplitPaymentKey.HasValue())
                {
                    splitPayments.AddRange((await GetSplitPayment(payment.Recipient.Key, payment.SplitPaymentKey)).Items);
                }
            }
            list.Items.AddRange(splitPayments);
            list.Items = list.Items.OrderBy(p => p.UpdatedOn).Distinct(Payment.Comparer).ToList();
            return list;
        }

        private string format(DateTime value)
        {
            return value.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z";
        }

        public async Task<Settlement> GetSettlement(string settlementKey)
        {
            return await GET<Settlement>($"settlement/{settlementKey}");
        }

        public async Task<PaymentList> GetSplitPayment(string merchantKey, string paymentKey)
        {
            return await GET<PaymentList>($"merchant/{merchantKey}/splitpayment/{paymentKey}");
        }

        public async Task<Payment> GetPayment(string merchantKey, string paymentToken)
        {
            return await GET<Payment>($"merchant/{merchantKey}/payment/{paymentToken}");
        }
        public async Task<RecurringPayment> GetRecurringPayment(string merchantKey, string token)
        {
            return await GET<RecurringPayment>($"merchant/{merchantKey}/recurringpayment/{token}");
        }
        public async Task<IEnumerable<RecurringPayment>> GetRecurringPaymentsForAPayer(string personKey)
        {
            var result = await GET<RecurringPaymentList>($"person/{personKey}/recurringpayments");
            return result.Items;
        }
        private async Task<T> GET<T>(string url) where T : BaseResponse
        {
            if (Util.IsDebug())
            {
                Console.WriteLine($"GET {url}");
            }
            ApiClient client = await CreateClient();
            T result = await client.Init(url).SetMethod(RequestMethodTypes.GET).Execute<T>();
            return result;
        }
    }
}
