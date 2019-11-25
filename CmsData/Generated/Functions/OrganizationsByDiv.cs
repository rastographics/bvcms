using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrganizationsByDiv")]
    public partial class OrganizationsByDiv
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrganizationName;

        private string _Location;

        private string _OrganizationCode;

        private int? _LeaderId;

        private string _FirstName;

        private string _LastName;

        private int? _MemberCount;

        public OrganizationsByDiv()
        {
        }

        [Column(Name = "ORGANIZATION_ID", Storage = "_OrganizationId", DbType = "int NOT NULL")]
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

        [Column(Name = "ORGANIZATION_NAME", Storage = "_OrganizationName", DbType = "varchar(40) NOT NULL")]
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

        [Column(Name = "LOCATION", Storage = "_Location", DbType = "varchar(40)")]
        public string Location
        {
            get => _Location;

            set
            {
                if (_Location != value)
                {
                    _Location = value;
                }
            }
        }

        [Column(Name = "ORGANIZATION_CODE", Storage = "_OrganizationCode", DbType = "varchar(10) NOT NULL")]
        public string OrganizationCode
        {
            get => _OrganizationCode;

            set
            {
                if (_OrganizationCode != value)
                {
                    _OrganizationCode = value;
                }
            }
        }

        [Column(Name = "LeaderId", Storage = "_LeaderId", DbType = "int")]
        public int? LeaderId
        {
            get => _LeaderId;

            set
            {
                if (_LeaderId != value)
                {
                    _LeaderId = value;
                }
            }
        }

        [Column(Name = "FIRST_NAME", Storage = "_FirstName", DbType = "varchar(15)")]
        public string FirstName
        {
            get => _FirstName;

            set
            {
                if (_FirstName != value)
                {
                    _FirstName = value;
                }
            }
        }

        [Column(Name = "LAST_NAME", Storage = "_LastName", DbType = "varchar(20)")]
        public string LastName
        {
            get => _LastName;

            set
            {
                if (_LastName != value)
                {
                    _LastName = value;
                }
            }
        }

        [Column(Name = "MemberCount", Storage = "_MemberCount", DbType = "int")]
        public int? MemberCount
        {
            get => _MemberCount;

            set
            {
                if (_MemberCount != value)
                {
                    _MemberCount = value;
                }
            }
        }
    }
}
