using System;
using System.ComponentModel;
using CmsData;
using CmsWeb.Code;

namespace CmsWeb.Areas.Search.Models
{
    public class SavedQueryInfo
    {
        [NoUpdate]
        public int QueryId { get; set; }
        public bool IsPublic { get; set; }
        [DisplayName("Owner")]
        public string SavedBy { get; set; }
        public string Description { get; set; }
        [SkipField]
        public DateTime LastUpdated { get; set; }

        private QueryBuilderClause clause;
        public SavedQueryInfo() { }
        public SavedQueryInfo(int id)
        {
            clause = DbUtil.Db.LoadQueryById(id);
            this.CopyPropertiesFrom(clause);
        }
        public void UpdateModel()
        {
            if (clause == null)
                clause = DbUtil.Db.LoadQueryById(QueryId);
            this.CopyPropertiesTo(clause);
            DbUtil.Db.SubmitChanges();
        }
    }
}