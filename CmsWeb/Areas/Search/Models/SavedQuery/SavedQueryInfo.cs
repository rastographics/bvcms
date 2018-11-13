using System;
using CmsData;
using CmsWeb.Code;

namespace CmsWeb.Areas.Search.Models
{
    public class SavedQueryInfo
    {
        [NoUpdate]
        public Guid QueryId { get; set; }

        public bool Ispublic { get; set; }

        public string Owner { get; set; }

        public string Name { get; set; }

        [SkipFieldOnCopyProperties]
        public DateTime? LastRun { get; set; }
        [SkipFieldOnCopyProperties]
        public int RunCount { get; set; }

        public Query query;

        public SavedQueryInfo()
        {
        }
        [NoUpdate]
        public bool CanDelete { get; set; }

        public SavedQueryInfo(Guid id)
        {
            query = CurrentDatabase.LoadQueryById2(id);
            this.CopyPropertiesFrom(query);
        }

        public void UpdateModel()
        {
            if (query == null)
                query = CurrentDatabase.LoadQueryById2(QueryId);
            this.CopyPropertiesTo(query);
            CurrentDatabase.SubmitChanges();
        }
    }
}