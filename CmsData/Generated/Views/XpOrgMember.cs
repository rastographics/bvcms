using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpOrgMember")]
    public partial class XpOrgMember
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private int _PeopleId;

        private int _MemberTypeId;

        private DateTime? _EnrollmentDate;

        private DateTime? _InactiveDate;

        private bool? _Pending;

        public XpOrgMember()
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

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int NOT NULL")]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    _PeopleId = value;
                }
            }
        }

        [Column(Name = "MemberTypeId", Storage = "_MemberTypeId", DbType = "int NOT NULL")]
        public int MemberTypeId
        {
            get => _MemberTypeId;

            set
            {
                if (_MemberTypeId != value)
                {
                    _MemberTypeId = value;
                }
            }
        }

        [Column(Name = "EnrollmentDate", Storage = "_EnrollmentDate", DbType = "datetime")]
        public DateTime? EnrollmentDate
        {
            get => _EnrollmentDate;

            set
            {
                if (_EnrollmentDate != value)
                {
                    _EnrollmentDate = value;
                }
            }
        }

        [Column(Name = "InactiveDate", Storage = "_InactiveDate", DbType = "datetime")]
        public DateTime? InactiveDate
        {
            get => _InactiveDate;

            set
            {
                if (_InactiveDate != value)
                {
                    _InactiveDate = value;
                }
            }
        }

        [Column(Name = "Pending", Storage = "_Pending", DbType = "bit")]
        public bool? Pending
        {
            get => _Pending;

            set
            {
                if (_Pending != value)
                {
                    _Pending = value;
                }
            }
        }
    }
}
