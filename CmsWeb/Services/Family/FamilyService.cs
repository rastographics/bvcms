using CmsWeb.Lifecycle;

namespace CmsWeb.Services.Family
{
    public class FamilyService : CMSBaseService, IFamilyService
    {
        public FamilyService(IRequestManager requestManager) : base(requestManager)
        {
        }
    }
}
