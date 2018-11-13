using CmsWeb.Lifecycle;

namespace CmsWeb.Services.Family
{
    public class FamilyService : CMSBaseService, IFamilyService
    {
        public FamilyService(RequestManager requestManager) : base(requestManager)
        {
        }
    }
}
