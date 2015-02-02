namespace CmsData.API
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