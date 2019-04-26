using System.ComponentModel;

namespace CmsData.Finance.Acceptiva.Core
{
    public enum ResponseStatus
    {
        [Description("success")]
        Approved,
        [Description("failure")]
        Declined
    }
}
