using System;
using CmsData;
using System.Linq;

namespace CmsWeb.Areas.Manage.Models
{
    public class ApiSessionModel
    {
        public ApiSessionResult DetermineApiSessionStatus(Guid sessionToken)
        {
            const int minutesSessionIsValid = 30;

            var session = DbUtil.Db.ApiSessions.SingleOrDefault(x => x.SessionToken == sessionToken);
            if (session == null)
                return new ApiSessionResult(null, ApiSessionStatus.SessionTokenNotFound);

            var isExpired = session.LastAccessedDate < DateTime.Now.Subtract(TimeSpan.FromMinutes(minutesSessionIsValid));
            if (isExpired && session.Pin.HasValue)
                return new ApiSessionResult(session.User, ApiSessionStatus.PinExpired);

            return isExpired 
                ? new ApiSessionResult(session.User, ApiSessionStatus.SessionTokenExpired) 
                : new ApiSessionResult(session.User, ApiSessionStatus.Success);
        }
    }
}