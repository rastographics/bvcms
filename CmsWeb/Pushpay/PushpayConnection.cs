using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Client;
using CmsWeb.Common;
using CmsWeb.Pushpay.ApiModels;
using CmsWeb.Pushpay.Enums;

namespace CmsWeb.Pushpay    
{
    /// <summary>
    ///     Centralized logic for communicating with the Pushpay payment server
    /// </summary>
    public class PushpayConnection
    {
        private ApiClient _client;
        private string clientID;
        private string clientSecret;

        public PushpayConnection(string client_id, string client_secret)
        {
            clientID = client_id;
            clientSecret = client_secret;
        }

        /// <summary>
        ///     Helper method to create a client connection
        /// </summary>
        /// <returns></returns>
        private async Task<ApiClient> CreateClient()
        {
            if (_client == null)
            {
                string baseUrl = Configuration.Current.PushpayAPIBaseUrl;
                string OrgBaseRedirect = Configuration.Current.OrgBaseRedirect;

                if (string.IsNullOrWhiteSpace(baseUrl)) RaiseError(new Exception("Please provide a PushpayAPIBaseUrl in your configuration AppSettings"));
                _client = new ApiClient(baseUrl);

                // Authenticate
                if (string.IsNullOrWhiteSpace(clientID) || string.IsNullOrWhiteSpace(clientSecret)) RaiseError(new Exception("Please provide Pushpay client ID and secret tokens in your db settings"));

                // Create an OAuth client to get the token required by Pushpay
                var oauthClient = new OAuth2Client(new Uri(Configuration.Current.OAuth2TokenEndpoint), clientID, clientSecret);
                TokenResponse response = await oauthClient.RequestClientCredentialsAsync("create_anticipated_payment read");
                if (response.AccessToken == null) RaiseError(new Exception("Failed to retrieve access token, error was: " + response.Raw));
                _client.SetBearerToken(response.AccessToken);
            }
            return _client;
        }

        /// <summary>
        ///     Centralized error handling
        /// </summary>
        /// <param name="ex"></param>
        private void RaiseError(Exception ex)
        {
            throw ex;
        }

        

        
    }
}
