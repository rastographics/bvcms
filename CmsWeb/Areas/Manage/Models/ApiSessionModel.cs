using System;
using CmsData;
using System.Linq;

namespace CmsWeb.Areas.Manage.Models
{
    internal static class ApiSessionModel
    {
        public static ApiSessionResult DetermineApiSessionStatus(Guid sessionToken)
        {
            const int minutesSessionIsValid = 30;

            var session = DbUtil.Db.ApiSessions.SingleOrDefault(x => x.SessionToken == sessionToken);
            if (session == null)
                return new ApiSessionResult(null, ApiSessionStatus.SessionTokenNotFound);

            var isExpired = session.LastAccessedDate < DateTime.Now.Subtract(TimeSpan.FromMinutes(minutesSessionIsValid));
            if (isExpired)
            {
                return session.Pin.HasValue
                    ? new ApiSessionResult(session.User, ApiSessionStatus.PinExpired)
                    : new ApiSessionResult(session.User, ApiSessionStatus.SessionTokenExpired);
            }

            return new ApiSessionResult(session.User, ApiSessionStatus.Success);
        }

        public static void SaveApiSession(User user, int? pin)
        {
            var apiSession = user.ApiSessions.SingleOrDefault();
            if (apiSession != null)
            {
                apiSession.LastAccessedDate = DateTime.Now;
                apiSession.Pin = pin;
            }
            else
            {
                var now = DateTime.Now;
                apiSession = new ApiSession();
                apiSession.SessionToken = Guid.NewGuid();
                apiSession.LastAccessedDate = now;
                apiSession.CreatedDate = now;
                apiSession.Pin = pin;
                user.ApiSessions.Add(apiSession);
            }

            DbUtil.Db.SubmitChanges();
        }

        public static void ResetSessionExpiration(User user, int? pin)
        {
            var apiSession = user.ApiSessions.SingleOrDefault();
            if (apiSession == null)
                return;

            apiSession.LastAccessedDate = DateTime.Now;
            DbUtil.Db.SubmitChanges();
        }
    }
}
