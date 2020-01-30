using CmsData;

namespace CmsWeb.Models
{
    public interface IDbBinder
    {
        CMSDataContext CurrentDatabase { get; set; }
    }
}
