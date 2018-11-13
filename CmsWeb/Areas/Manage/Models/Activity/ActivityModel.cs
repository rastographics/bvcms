using CmsData;
using CmsData.View;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CmsWeb.Areas.Manage.Models
{

    public class ActivityModel : PagedTableModel<ActivityLogSearch, ActivityLogSearch>
    {
        public string Activity { get; set; }
        [DisplayName("User Id")]
        public int? UserId { get; set; }
        [DisplayName("Organization Id")]
        public int? OrgId { get; set; }
        [DisplayName("About People Id")]
        public int? PeopleId { get; set; }
        [DisplayName("Lookback Days before EndDate")]
        public int? Lookback { get; set; }
        public DateTime? EndDate { get; set; }

        public ActivityModel()
            : base("Activity", "", true)
        {
            UseDbPager = true;
        }

        private List<ActivityLogSearch> rows;

        //        private int? count;
        //        public virtual int DbCount()
        //        {
        //            if(count.HasValue)
        //                return count.Value;
        //
        //            if (rows == null)
        //            {
        //                rows = GetList();
        //                list = rows.AsQueryable();
        //            }
        //
        //            if (rows.Count > 0)
        //                count = (rows[0].MaxRows ?? 0);
        //            return count ?? 0;
        //        }
        //

        private List<ActivityLogSearch> GetList()
        {
            if (rows != null)
            {
                return rows;
            }

            rows = (from a in DbUtil.Db.ActivityLogSearch(null, Activity, UserId, OrgId, PeopleId, EndDate, Lookback, PageSize, Page)
                    select a).ToList();
            count = rows.Count == 0 ? 0 : rows[0].MaxRows;
            return rows;
        }

        public override IQueryable<ActivityLogSearch> DefineModelList()
        {
            return GetList().AsQueryable();
        }

        public override IQueryable<ActivityLogSearch> DefineModelSort(IQueryable<ActivityLogSearch> q)
        {
            return q;
        }

        public override IEnumerable<ActivityLogSearch> DefineViewList(IQueryable<ActivityLogSearch> q)
        {
            return q;
        }

    }
}
