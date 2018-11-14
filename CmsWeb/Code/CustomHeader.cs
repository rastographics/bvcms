using CmsData;
using System.Text.RegularExpressions;

namespace CmsWeb.Code
{
    /// <summary>
    /// Helper class to determine the custom headers used in the system.
    /// 
    /// The preferred way to customize the header is to set the "UX-HeaderLogo" and the "UX-SmallHeaderLogo" settings.
    /// Which image is displayed will depend upon the mobile responsive view. If the setting isn't set, then the TouchPoint logo
    /// will be used instead.
    /// </summary>
    public static class CustomHeader
    {
        public static CustomHeaderImage SmallHeaderImage(string fallbackImageUrl)
        {
            var setting = DbUtil.Db.Setting("UX-SmallHeaderLogo", "");

            return string.IsNullOrWhiteSpace(setting)
                ? new CustomHeaderImage(fallbackImageUrl, "50px")
                : new CustomHeaderImage(setting, "45px");
        }

        public static CustomHeaderImage HeaderImage(string fallbackImageUrl)
        {
            var fallbackImage = new CustomHeaderImage(fallbackImageUrl, "50px");

            var headerLogoSetting = DbUtil.Db.Setting("UX-HeaderLogo", "");
            if (!string.IsNullOrWhiteSpace(headerLogoSetting))
            {
                return new CustomHeaderImage(headerLogoSetting, "45px");
            }

            var customHeaderContent = DbUtil.Db.Content("CustomHeader", "");
            if (!string.IsNullOrWhiteSpace(customHeaderContent))
            {
                var match = Regex.Match(customHeaderContent, @"background-image:(\s+)?url\((?<backgroundImage>.+?)\)");
                var backgroundImage = match.Groups["backgroundImage"].Value;

                return string.IsNullOrWhiteSpace(backgroundImage)
                    ? fallbackImage
                    : new CustomHeaderImage(backgroundImage, "45px");
            }

            return fallbackImage;
        }
    }

    public class CustomHeaderImage
    {
        public string Url { get; set; }
        public string Height { get; set; }

        public CustomHeaderImage(string url, string height)
        {
            Url = url;
            Height = height;
        }
    }
}
