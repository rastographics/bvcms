using CmsWeb.Areas.People.Models;
using CmsWeb.Lifecycle;

namespace CmsWeb.Services.People
{
    public class PeopleService : CMSBaseService, IPeopleService
    {
        public PeopleService(IRequestManager requestManager) : base(requestManager)
        {
        }

        public int GetPeopleIdForUserId(int? id)
        {
            throw new System.NotImplementedException();
        }

        public PersonModel LoadPersonById(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
