using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RecentAbsents2")]
    public partial class RecentAbsents2
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _OrganizationId;

        private string _OrganizationName;

        private string _LeaderName;

        private int? _Consecutive;

        private int? _PeopleId;

        private string _Name2;

        private string _HomePhone;

        private string _CellPhone;

        private string _EmailAddress;

        private int? _MeetingId;

        private int? _ConsecutiveAbsentsThreshhold;

        public RecentAbsents2()
        {
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int")]
        public int? OrganizationId
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

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "nvarchar(70)")]
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

        [Column(Name = "LeaderName", Storage = "_LeaderName", DbType = "nvarchar(60)")]
        public string LeaderName
        {
            get => _LeaderName;

            set
            {
                if (_LeaderName != value)
                {
                    _LeaderName = value;
                }
            }
        }

        [Column(Name = "consecutive", Storage = "_Consecutive", DbType = "int")]
        public int? Consecutive
        {
            get => _Consecutive;

            set
            {
                if (_Consecutive != value)
                {
                    _Consecutive = value;
                }
            }
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
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

        [Column(Name = "Name2", Storage = "_Name2", DbType = "nvarchar(50)")]
        public string Name2
        {
            get => _Name2;

            set
            {
                if (_Name2 != value)
                {
                    _Name2 = value;
                }
            }
        }

        [Column(Name = "HomePhone", Storage = "_HomePhone", DbType = "nvarchar(15)")]
        public string HomePhone
        {
            get => _HomePhone;

            set
            {
                if (_HomePhone != value)
                {
                    _HomePhone = value;
                }
            }
        }

        [Column(Name = "CellPhone", Storage = "_CellPhone", DbType = "nvarchar(15)")]
        public string CellPhone
        {
            get => _CellPhone;

            set
            {
                if (_CellPhone != value)
                {
                    _CellPhone = value;
                }
            }
        }

        [Column(Name = "EmailAddress", Storage = "_EmailAddress", DbType = "nvarchar(50)")]
        public string EmailAddress
        {
            get => _EmailAddress;

            set
            {
                if (_EmailAddress != value)
                {
                    _EmailAddress = value;
                }
            }
        }

        [Column(Name = "MeetingId", Storage = "_MeetingId", DbType = "int")]
        public int? MeetingId
        {
            get => _MeetingId;

            set
            {
                if (_MeetingId != value)
                {
                    _MeetingId = value;
                }
            }
        }

        [Column(Name = "ConsecutiveAbsentsThreshhold", Storage = "_ConsecutiveAbsentsThreshhold", DbType = "int")]
        public int? ConsecutiveAbsentsThreshhold
        {
            get => _ConsecutiveAbsentsThreshhold;

            set
            {
                if (_ConsecutiveAbsentsThreshhold != value)
                {
                    _ConsecutiveAbsentsThreshhold = value;
                }
            }
        }
    }
}
