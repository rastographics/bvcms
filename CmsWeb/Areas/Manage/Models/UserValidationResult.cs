using CmsData;

namespace CmsWeb.Areas.Manage.Models
{
    public class UserValidationResult
    {
        public string ErrorMessage { get; private set; }
        public UserValidationStatus Status { get; private set; }
        public bool IsValid { get; private set; }
        public User User { get; private set; }

        private UserValidationResult() {}

        public static UserValidationResult Valid(User user)
        {
            return new UserValidationResult { IsValid = true, User = user, Status = UserValidationStatus.Success };
        }

        public static UserValidationResult Invalid(UserValidationStatus status, string errorMessage = null)
        {
            return new UserValidationResult { IsValid = false, ErrorMessage = errorMessage, Status = status };
        }
    }
}