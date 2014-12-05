namespace CmsWeb.Areas.Manage.Models
{
    public enum UserValidationStatus
    {
        // credentials related
        BadDatabase,
        NoUserFound,
        IncorrectPassword,
        TooManyFailedPasswordAttempts,
        LockedOut,
        UserNotApproved,
        CannotImpersonateFinanceUser,
        UserNotInRole,

        // mobile related
        PinExpired,
        SessionTokenExpired,
        SessionTokenNotFound,

        Success
    }
}