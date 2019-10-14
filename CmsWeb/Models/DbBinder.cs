using CmsData;

namespace CmsWeb.Models
{
    internal interface IDbBinder
    {
        CMSDataContext CurrentDatabase { get; set; }
    }
}
