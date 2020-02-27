using NHttp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace SharedTestFixtures.Network
{
    public class NetworkFixture : IDisposable
    {
        public NetworkFixture()
        {
            MockHttpServer.Start(ProxyPort, HttpServer_Request);
        }

        public static int ProxyPort => (ConfigurationManager.AppSettings["mockHttpServerPort"] ?? "8099").ToInt();
        public static string ProxyUrl => $"http://localhost:{ProxyPort}/";

        /// <summary>
        /// The history of requests that were mocked
        /// </summary>
        public static Dictionary<string, string> Requests = new Dictionary<string, string>();

        private static Dictionary<string, MockHttpResponse> mockResponses;
        public static NameValueCollection JsonHeaders => new NameValueCollection { { "Content-Type", "application/json; charset=utf-8" } };

        private void HttpServer_Request(object sender, HttpRequestEventArgs e)
        {
            MockHttpResponse mockresponse;
            string url = e.Request.RawUrl;
            Console.WriteLine(url);
            if (HasMockResponse(url, out mockresponse))
            {
                if (e.Request.ContentLength > 0)
                {
                    byte[] bytes = new byte[e.Request.ContentLength];
                    try
                    {
                        e.Request.InputStream.Read(bytes, 0, e.Request.ContentLength);
                    }
                    catch { }
                    Requests[url] = Encoding.ASCII.GetString(bytes);
                }
                else
                {
                    Requests[url] = url;
                }
                e.Finish(mockresponse.ResponseBody, mockresponse.Headers ?? new MockHttpHeaders(), mockresponse.Encoding);
            }
            else
            {
                throw new Exception($"Unexpected request received for {url}");
            }
        }

        /// <summary>
        /// Mocks the response for a specific <paramref name="url"/>
        /// </summary>
        /// <param name="url">The address to mock</param>
        /// <param name="response">The response to be returned when any url matching <paramref name="url"/> is requested</param>
        public static void MockResponse(string url, MockHttpResponse response)
        {
            if (mockResponses == null)
            {
                mockResponses = new Dictionary<string, MockHttpResponse>();
            }
            mockResponses[url] = response;
        }

        private bool HasMockResponse(string url, out MockHttpResponse response)
        {
            response = null;
            if (mockResponses != null)
            {
                foreach (string key in mockResponses.Keys)
                {
                    if (key.Matches(url, RegexOptions.IgnoreCase))
                    {
                        response = mockResponses[key];
                        return true;
                    }
                }
            }
            return false;
        }

        public static void DisableSSLCertValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                new RemoteCertificateValidationCallback(delegate { return true; });
        }

        public void Dispose()
        {
            MockHttpServer.Stop();
            mockResponses = null;
        }
    }
}
