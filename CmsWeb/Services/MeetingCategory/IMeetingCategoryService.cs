using System.Collections.Generic;

namespace CmsWeb.Services.MeetingCategory
{
    public interface IMeetingCategoryService
    {
        IEnumerable<CmsData.MeetingCategory> GetMeetingCategories(bool includeExpired);
        bool TryUpdateMeetingCategory(long meetingId, string newMeetingCategory);
        CmsData.MeetingCategory GetById(long id);
        CmsData.MeetingCategory AddMeetingCategory(string description);
        CmsData.MeetingCategory CreateOrUpdate(CmsData.MeetingCategory meetingCategory);

    }
}
