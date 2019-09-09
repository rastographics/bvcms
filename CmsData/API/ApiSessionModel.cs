using System;
using System.Data.SqlTypes;
using System.Linq;

namespace CmsData.API
{
    public static class ApiSessionModel
    {
        public static ApiSessionResult DetermineApiSessionStatus(CMSDataContext db, Guid sessionToken, bool requirePin, int? pin)
        {
            const int minutesSessionIsValid = 30;

            var session = db.ApiSessions.SingleOrDefault(x => x.SessionToken == sessionToken);
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

        public static void SaveApiSession(CMSDataContext db, User user, bool updatePin, int? pin)
        {
            var apiSession = db.ApiSessions.Where(s => s.UserId == user.UserId).SingleOrDefault();
            if (apiSession != null)
            {
                apiSession.LastAccessedDate = DateTime.Now;
                if (updatePin)
                    apiSession.Pin = pin;
            }
            else
            {
                var now = DateTime.Now;
                apiSession = new ApiSession {
                    SessionToken = Guid.NewGuid(),
                    LastAccessedDate = now,
                    CreatedDate = now,
                    Pin = pin,
                    UserId = user.UserId,
                };
                db.ApiSessions.InsertOnSubmit(apiSession);
            }

            db.SubmitChanges();
        }

        public static bool ResetSessionExpiration(CMSDataContext db, User user, int? pin)
        {
            var apiSession = db.ApiSessions.Where(s => s.UserId == user.UserId).SingleOrDefault();
            if (apiSession == null)
                return false;

            if (apiSession.Pin.HasValue)
            {
                if (!pin.HasValue || pin.Value != apiSession.Pin.Value)
                    return false;
            }

            apiSession.LastAccessedDate = DateTime.Now;
            db.SubmitChanges();

            return true;
        }

        public static void DeleteSession(CMSDataContext db, User user)
        {
            var apiSession = db.ApiSessions.Where(s => s.UserId == user.UserId).SingleOrDefault();
            if (apiSession == null)
                return;

            db.ApiSessions.DeleteOnSubmit(apiSession);
            db.SubmitChanges();
        }

        public static void ExpireSession(CMSDataContext db, Guid sessionToken)
        {
            var session = db.ApiSessions.SingleOrDefault(x => x.SessionToken == sessionToken);
            if (session == null)
                return;

            session.LastAccessedDate = SqlDateTime.MinValue.Value;

            db.SubmitChanges();
        }
    }
}
