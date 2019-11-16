using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RegsettingUsage")]
    public partial class RegsettingUsage
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrganizationName;

        private string _Usage;

        public RegsettingUsage()
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

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "nvarchar(100) NOT NULL")]
        public string OrganizationName
        {
            get => _OrganizationName;

            set
            {
                if (_OrganizationName != value)
                {
                    _OrganizationName = value;
                }
            }
        }

        [Column(Name = "Usage", Storage = "_Usage", DbType = "varchar(961) NOT NULL")]
        public string Usage
        {
            get => _Usage;

            set
            {
                if (_Usage != value)
                {
                    _Usage = value;
                }
            }
        }
    }
}
