using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "VisitsAbsents")]
    public partial class VisitsAbsent
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Name;

        private string _Status;

        private string _MemberType;

        private bool _AttendanceFlag;

        private int _MemberTypeId;

        private DateTime? _LastAttended;

        private decimal? _AttendPct;

        private string _AttendStr;

        private DateTime? _Birthday;

        private string _EmailAddress;

        private string _HomePhone;

        private string _CellPhone;

        private string _PrimaryAddress;

        private string _PrimaryCity;

        private string _PrimaryState;

        private string _PrimaryZip;

        public VisitsAbsent()
        {
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(139)")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    _Name = value;
                }
            }
        }

        [Column(Name = "status", Storage = "_Status", DbType = "nvarchar(100)")]
        public string Status
        {
            get => _Status;

            set
            {
                if (_Status != value)
                {
                    _Status = value;
                }
            }
        }

        [Column(Name = "MemberType", Storage = "_MemberType", DbType = "nvarchar(100)")]
        public string MemberType
        {
            get => _MemberType;

            set
            {
                if (_MemberType != value)
                {
                    _MemberType = value;
                }
            }
        }

        [Column(Name = "AttendanceFlag", Storage = "_AttendanceFlag", DbType = "bit NOT NULL")]
        public bool AttendanceFlag
        {
            get => _AttendanceFlag;

            set
            {
                if (_AttendanceFlag != value)
                {
                    _AttendanceFlag = value;
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

        [Column(Name = "LastAttended", Storage = "_LastAttended", DbType = "datetime")]
        public DateTime? LastAttended
        {
            get => _LastAttended;

            set
            {
                if (_LastAttended != value)
                {
                    _LastAttended = value;
                }
            }
        }

        [Column(Name = "AttendPct", Storage = "_AttendPct", DbType = "real")]
        public decimal? AttendPct
        {
            get => _AttendPct;

            set
            {
                if (_AttendPct != value)
                {
                    _AttendPct = value;
                }
            }
        }

        [Column(Name = "AttendStr", Storage = "_AttendStr", DbType = "nvarchar(200)")]
        public string AttendStr
        {
            get => _AttendStr;

            set
            {
                if (_AttendStr != value)
                {
                    _AttendStr = value;
                }
            }
        }

        [Column(Name = "Birthday", Storage = "_Birthday", DbType = "datetime")]
        public DateTime? Birthday
        {
            get => _Birthday;

            set
            {
                if (_Birthday != value)
                {
                    _Birthday = value;
                }
            }
        }

        [Column(Name = "EmailAddress", Storage = "_EmailAddress", DbType = "nvarchar(150)")]
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

        [Column(Name = "HomePhone", Storage = "_HomePhone", DbType = "nvarchar(20)")]
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

        [Column(Name = "CellPhone", Storage = "_CellPhone", DbType = "nvarchar(20)")]
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

        [Column(Name = "PrimaryAddress", Storage = "_PrimaryAddress", DbType = "nvarchar(100)")]
        public string PrimaryAddress
        {
            get => _PrimaryAddress;

            set
            {
                if (_PrimaryAddress != value)
                {
                    _PrimaryAddress = value;
                }
            }
        }

        [Column(Name = "PrimaryCity", Storage = "_PrimaryCity", DbType = "nvarchar(30)")]
        public string PrimaryCity
        {
            get => _PrimaryCity;

            set
            {
                if (_PrimaryCity != value)
                {
                    _PrimaryCity = value;
                }
            }
        }

        [Column(Name = "PrimaryState", Storage = "_PrimaryState", DbType = "nvarchar(20)")]
        public string PrimaryState
        {
            get => _PrimaryState;

            set
            {
                if (_PrimaryState != value)
                {
                    _PrimaryState = value;
                }
            }
        }

        [Column(Name = "PrimaryZip", Storage = "_PrimaryZip", DbType = "nvarchar(15)")]
        public string PrimaryZip
        {
            get => _PrimaryZip;

            set
            {
                if (_PrimaryZip != value)
                {
                    _PrimaryZip = value;
                }
            }
        }
    }
}
