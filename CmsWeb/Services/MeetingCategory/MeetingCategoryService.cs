using CmsData;
using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Services.MeetingCategory
{
    public class MeetingCategoryService : IMeetingCategoryService
    {
        private readonly CMSDataContext _dataContext;

        public MeetingCategoryService(IRequestManager currentRequest)
        {
            _dataContext = currentRequest.CurrentDatabase;
        }

        public IEnumerable<CmsData.MeetingCategory> GetMeetingCategories(bool includeExpired = false)
        {
            var query = _dataContext.MeetingCategories.Where(x =>
                (x.NotBeforeDate == null || x.NotBeforeDate.Value <= DateTime.UtcNow)
                && (x.NotAfterDate == null || x.NotAfterDate.Value > DateTime.UtcNow));

            if (!includeExpired)
            {
                query = query.Where(x => x.IsExpired == false);
            }

            return query.ToList();
        }

        public bool TryUpdateMeetingCategory(long meetingId, string newMeetingCategory)
        {
            var entry = _dataContext.Meetings.SingleOrDefault(x => x.MeetingId == meetingId);
            entry.Description = newMeetingCategory;
            _dataContext.SubmitChanges();

            return true;
        }

        public CmsData.MeetingCategory AddMeetingCategory(string newMeetingCategory)
        {
            var category = new CmsData.MeetingCategory { Description = newMeetingCategory, IsExpired = false, NotBeforeDate = null, NotAfterDate = null };
            _dataContext.MeetingCategories.InsertOnSubmit(category);
            _dataContext.SubmitChanges();

            return category;
        }

        public CmsData.MeetingCategory GetById(long meetingCategoryId)
        {
            var category = _dataContext.MeetingCategories.SingleOrDefault(x => x.Id == meetingCategoryId);
            return category;
        }

        public CmsData.MeetingCategory CreateOrUpdate(CmsData.MeetingCategory meetingCategory)
        {
            if (meetingCategory.Id != 0)
            {
                var existingItem = GetById(meetingCategory.Id);
                existingItem.Description = meetingCategory.Description;
                existingItem.NotBeforeDate = meetingCategory.NotBeforeDate;
                existingItem.NotAfterDate = meetingCategory.NotAfterDate;
                existingItem.IsExpired = meetingCategory.IsExpired;
            }
            else
            {
                _dataContext.MeetingCategories.InsertOnSubmit(meetingCategory);
            }

            _dataContext.SubmitChanges();

            return meetingCategory;
        }
    }
}
