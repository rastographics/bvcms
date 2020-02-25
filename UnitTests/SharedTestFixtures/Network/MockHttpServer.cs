using NHttp;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace SharedTestFixtures.Network
{
    public class MockHttpServer
    {
        private static HttpServer server;
        public static void Start(int listenPort, HttpRequestEventHandler handler)
        {
            if (server != null)
            {
                Stop();
            }
            server = new HttpServer();
            server.RequestReceived += (s, e) =>
            {
                handler(s, e);
            };
            server.EndPoint = new IPEndPoint(IPAddress.Loopback, listenPort);
            server.Start();
        }

        public static void Stop()
        {
            try
            {
                if (server != null)
                {
                    server.Stop();
                    server = null;
                }
            }
            catch { }
        }
    }

    public static class HttpServerExtensions
    {
        public static void Finish(this HttpRequestEventArgs e, string responseBody, NameValueCollection headers, Encoding encoding)
        {
            e.Response.HeadersEncoding = encoding;
            foreach(var header in headers.AllKeys)
            {
                e.Response.Headers[header] = headers[header];
            }
            if (headers.AllKeys.Contains("Content-Type"))
            {
                e.Response.ContentType = headers["Content-Type"];
            }
            var bytes = encoding.GetBytes(responseBody);
            e.Response.OutputStream.Write(bytes, 0, bytes.Length);
            e.Response.OutputStream.Flush();
        }
    }
}
