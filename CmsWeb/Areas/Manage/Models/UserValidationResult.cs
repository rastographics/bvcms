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

        public static UserValidationResult Invalid(UserValidationStatus status, string errorMessage = null)
        {
            return new UserValidationResult { ErrorMessage = errorMessage, Status = status };
        }
    }
}