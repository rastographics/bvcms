using CmsData;

namespace CmsWeb.Areas.Manage.Models
{
    public class UserValidationResult
    {
        public string ErrorMessage { get; private set; }
        public UserValidationStatus Status { get; set; }
        public User User { get; private set; }

        public bool IsValid
        {
            get
            {
                return Status == UserValidationStatus.Success;
            }
        }

        private UserValidationResult() {}

        public static UserValidationResult Valid(User user)
        {
            return new UserValidationResult { User = user, Status = UserValidationStatus.Success };
        }

        public static UserValidationResult Invalid(UserValidationStatus status, string errorMessage = null, User user = null)
        {
            DbUtil.LogActivity($"Invalid log in {status}:{errorMessage} ({user?.Username})");
            return new UserValidationResult { User = user, ErrorMessage = errorMessage, Status = status };
        }
    }
}
