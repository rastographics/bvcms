using CmsData;

namespace CmsWeb.Models
{
    internal interface IDbBinder
    {
        CMSDataContext Db { get; set; }
    }
}
