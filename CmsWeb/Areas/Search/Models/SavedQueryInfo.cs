using System;
using System.ComponentModel;
using CmsData;
using CmsWeb.Code;

namespace CmsWeb.Areas.Search.Models
{
    public class SavedQueryInfo
    {
        [NoUpdate]
        public Guid QueryId { get; set; }

        public bool Ispublic { get; set; }

        [DisplayName("Owner")]
        public string SavedBy { get; set; }

        public string Name { get; set; }

        [SkipField]
        public DateTime? Modified { get; set; }

        private Query clause;

        public SavedQueryInfo()
        {
        }
        [NoUpdate]
        public bool CanDelete { get; set; }

        public SavedQueryInfo(Guid id)
        {
            clause = DbUtil.Db.LoadQueryById2(id);
            this.CopyPropertiesFrom(clause);
        }

        public void UpdateModel()
        {
            if (clause == null)
                clause = DbUtil.Db.LoadQueryById2(QueryId);
            this.CopyPropertiesTo(clause);
            DbUtil.Db.SubmitChanges();
        }
    }
}