using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ContributionSearch")]
    public partial class ContributionSearch
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _ContributionId;

        public ContributionSearch()
        {
        }

        [Column(Name = "ContributionId", Storage = "_ContributionId", DbType = "int NOT NULL")]
        public int ContributionId
        {
            get => _ContributionId;

            set
            {
                if (_ContributionId != value)
                {
                    _ContributionId = value;
                }
            }
        }
    }
}
