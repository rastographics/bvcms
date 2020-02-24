using eSpace.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

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
            if (data != null && data.IsSuccessStatusCode)
            {
                list = data.Data;
            }
            else
            {
                throw new Exception(data.Message ?? "An unknown error occurred while processing the request");
            }
        }

        protected RestClient restClient => new RestClient(Client.BaseUrl ?? BASE_URL)
        {
            Authenticator = new HttpBasicAuthenticator(Client.Username, Client.Password)
        };
    }
}
