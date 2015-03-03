using System;
using System.Data.SqlTypes;
using System.Linq;

namespace CmsData.API
{
    public static class ApiSessionModel
    {
        public static ApiSessionResult DetermineApiSessionStatus(Guid sessionToken, bool requirePin, int? pin)
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

            if (requirePin && session.Pin.HasValue)
            {
                if (!pin.HasValue || pin.Value != session.Pin.Value)
                    return new ApiSessionResult(session.User, ApiSessionStatus.PinInvalid);
            }

            if (pin.HasValue && session.Pin.HasValue)
            {
                if (pin.Value != session.Pin.Value)
                    return new ApiSessionResult(session.User, ApiSessionStatus.PinInvalid);
            }

            return new ApiSessionResult(session.User, ApiSessionStatus.Success);
        }

        public static void SaveApiSession(User user, bool updatePin, int? pin)
        {
            var apiSession = user.ApiSessions.SingleOrDefault();
            if (apiSession != null)
            {
                apiSession.LastAccessedDate = DateTime.Now;
                if (updatePin)
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

        public static bool ResetSessionExpiration(User user, int? pin)
        {
            var apiSession = user.ApiSessions.SingleOrDefault();
            if (apiSession == null)
                return false;

            if (apiSession.Pin.HasValue)
            {
                if (!pin.HasValue || pin.Value != apiSession.Pin.Value)
                    return false;
            }

            apiSession.LastAccessedDate = DateTime.Now;
            DbUtil.Db.SubmitChanges();

            return true;
        }

        public static void DeleteSession(CMSDataContext db, User user)
        {
            var apiSession = user.ApiSessions.SingleOrDefault();
            if (apiSession == null)
                return;

            db.ApiSessions.DeleteOnSubmit(apiSession);
            db.SubmitChanges();
        }

        public static void ExpireSession(Guid sessionToken)
        {
            var session = DbUtil.Db.ApiSessions.SingleOrDefault(x => x.SessionToken == sessionToken);
            if (session == null)
                return;

            session.LastAccessedDate = SqlDateTime.MinValue.Value;

            DbUtil.Db.SubmitChanges();
        }
    }
}
