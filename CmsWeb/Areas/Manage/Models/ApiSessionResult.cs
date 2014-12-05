using CmsData;

namespace CmsWeb.Areas.Manage.Models
{
    public class ApiSessionResult
    {
        public User User { get; private set; }
        public ApiSessionStatus Status { get; private set; }

        public ApiSessionResult(User user, ApiSessionStatus status)
        {
            User = user;
            Status = status;
        }
    }
}