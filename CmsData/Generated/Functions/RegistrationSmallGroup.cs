using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RegistrationSmallGroups")]
    public partial class RegistrationSmallGroup
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrganizationName;

        private string _SmallGroup;

        private int? _Cnt;

        public RegistrationSmallGroup()
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

        [Column(Name = "SmallGroup", Storage = "_SmallGroup", DbType = "varchar(100)")]
        public string SmallGroup
        {
            get => _SmallGroup;

            set
            {
                if (_SmallGroup != value)
                {
                    _SmallGroup = value;
                }
            }
        }

        [Column(Name = "Cnt", Storage = "_Cnt", DbType = "int")]
        public int? Cnt
        {
            get => _Cnt;

            set
            {
                if (_Cnt != value)
                {
                    _Cnt = value;
                }
            }
        }
    }
}
