namespace CmsWeb.Areas.Manage.Models
{
    public enum ApiSessionStatus
    {
        SessionTokenNotFound,
        SessionTokenExpired,
        PinExpired,
        PinInvalid,
        Success
    }
}