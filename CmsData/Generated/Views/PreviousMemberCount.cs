using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PreviousMemberCounts")]
    public partial class PreviousMemberCount
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private int? _Prevcount;

        public PreviousMemberCount()
        {
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int NOT NULL")]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
                }
            }
        }

        [Column(Name = "prevcount", Storage = "_Prevcount", DbType = "int")]
        public int? Prevcount
        {
            get => _Prevcount;

            set
            {
                if (_Prevcount != value)
                {
                    _Prevcount = value;
                }
            }
        }
    }
}
