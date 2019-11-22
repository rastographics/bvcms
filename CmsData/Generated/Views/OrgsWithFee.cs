using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgsWithFees")]
    public partial class OrgsWithFee
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        public OrgsWithFee()
        {
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
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
    }
}
