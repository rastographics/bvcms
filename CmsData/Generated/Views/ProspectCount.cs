using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ProspectCounts")]
    public partial class ProspectCount
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private int? _Prospectcount;

        public ProspectCount()
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

        [Column(Name = "prospectcount", Storage = "_Prospectcount", DbType = "int")]
        public int? Prospectcount
        {
            get => _Prospectcount;

            set
            {
                if (_Prospectcount != value)
                {
                    _Prospectcount = value;
                }
            }
        }
    }
}
