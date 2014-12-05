namespace CmsWeb.Areas.Manage.Models
{
    public enum UserValidationStatus
    {
        BadDatabase = 0,
        Success = 1,

        // mobile related
        PinExpired = 2,
        SessionTokenExpired = 3,
        SessionTokenNotFound = 4,
        PinInvalid = 5,

        // credentials related
        NoUserFound = 6,
        IncorrectPassword = 7,
        TooManyFailedPasswordAttempts = 8,
        LockedOut = 9,
        UserNotApproved = 10,
        CannotImpersonateFinanceUser = 11,
        UserNotInRole = 12
    }
}