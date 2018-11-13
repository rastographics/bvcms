using CmsWeb.Areas.People.Models;

namespace CmsWeb.Services.People
{
    public interface IPeopleService
    {
        PersonModel LoadPersonById(int id);
        int GetPeopleIdForUserId(int? id);
    }
}
