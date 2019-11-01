using System.ComponentModel.DataAnnotations;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Models
{
    public class AccountInfo
    {
        [Required]
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        public string ReturnUrlQueryString => ReturnUrl.HasValue() ? $"?returnUrl={ReturnUrl}" : "";
    }
}
