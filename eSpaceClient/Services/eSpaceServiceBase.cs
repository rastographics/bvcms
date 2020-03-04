using eSpace.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace eSpace.Services
{
    public class eSpaceServiceBase
    {
        internal const string BASE_URL = "https://api.espace.cool/api/v1/";

        internal eSpaceClient Client { get; set; }

        protected void ExecuteGet<T>(RestRequest request, NameValueCollection filters, out List<T> list)
        {
            foreach (var key in filters.AllKeys)
            {
                request.AddParameter(key, filters[key]);
            }
            var response = restClient.Execute<eSpaceResponse<T>>(request);
            var data = response.Data;
            if (response.IsSuccessful && data != null && data.IsSuccessStatusCode)
            {
                list = data.Data;
            }
            else
            {
                if (new[] { HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden }.Contains(response.StatusCode))
                {
                    throw new Exception($"Invalid eSPACE credentials or access denied ({response.StatusCode})");
                }
                throw new Exception(data?.Message ?? $"eSPACE returned an error: {response.StatusCode} {response.StatusDescription}");
            }
        }

        protected RestClient restClient => new RestClient(Client.BaseUrl ?? BASE_URL)
        {
            Authenticator = new HttpBasicAuthenticator(Client.Username, Client.Password)
        };
    }
}
