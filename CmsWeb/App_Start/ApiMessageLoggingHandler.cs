using CmsData;
using System;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CmsWeb
{
    internal class ApiMessageLoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestInfo = $"{request.Method} {request.RequestUri}";
            var clientIp = GetClientIp(request);

            var response = await base.SendAsync(request, cancellationToken);

            var userDetails = GetUserDetails(request);

            DbUtil.LogActivity(
                activity: $"API: {requestInfo}",
                userId: userDetails.Item1,
                pageUrl: request.RequestUri.ToString(),
                clientIp: clientIp);

            return response;
        }

        private static string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }

            return null;
        }

        private static Tuple<int?, int?> GetUserDetails(HttpRequestMessage request)
        {
            var userName = GetUserName(request);

            var user = DbUtil.Db.Users.SingleOrDefault(x => x.Username == userName);

            return new Tuple<int?, int?>(user?.UserId, user?.PeopleId);
        }

        private static string GetUserName(HttpRequestMessage request)
        {
            var auth = request.Headers.Authorization?.Parameter;

            if (string.IsNullOrWhiteSpace(auth))
            {
                return null;
            }

            var authHeader = Encoding.ASCII.GetString(Convert.FromBase64String(auth));
            var tokens = authHeader.Split(':');
            return tokens[0];
        }
    }
}
