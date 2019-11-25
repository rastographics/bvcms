using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgMembersAsOfDate")]
    public partial class OrgMembersAsOfDate
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private int _FamilyId;

        private string _PreferredName;

        private string _LastName;

        private int? _BirthDay;

        private int? _BirthMonth;

        private int? _BirthYear;

        private string _PrimaryAddress;

        private string _PrimaryAddress2;

        private string _PrimaryCity;

        private string _PrimaryState;

        private string _PrimaryZip;

        private string _HomePhone;

        private string _CellPhone;

        private string _WorkPhone;

        private string _EmailAddress;

        private string _MemberStatus;

        private string _BFTeacher;

        private int? _BFTeacherId;

        private int? _Age;

        private string _MemberType;

        private int _MemberTypeId;

        private DateTime _Joined;

        public OrgMembersAsOfDate()
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

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int NOT NULL")]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    _FamilyId = value;
                }
            }
        }

        [Column(Name = "PreferredName", Storage = "_PreferredName", DbType = "nvarchar(25)")]
        public string PreferredName
        {
            get => _PreferredName;

            set
            {
                if (_PreferredName != value)
                {
                    _PreferredName = value;
                }
            }
        }

        [Column(Name = "LastName", Storage = "_LastName", DbType = "nvarchar(100) NOT NULL")]
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

        [Column(Name = "BirthDay", Storage = "_BirthDay", DbType = "int")]
        public int? BirthDay
        {
            get => _BirthDay;

            set
            {
                if (_BirthDay != value)
                {
                    _BirthDay = value;
                }
            }
        }

        [Column(Name = "BirthMonth", Storage = "_BirthMonth", DbType = "int")]
        public int? BirthMonth
        {
            get => _BirthMonth;

            set
            {
                if (_BirthMonth != value)
                {
                    _BirthMonth = value;
                }
            }
        }

        [Column(Name = "BirthYear", Storage = "_BirthYear", DbType = "int")]
        public int? BirthYear
        {
            get => _BirthYear;

            set
            {
                if (_BirthYear != value)
                {
                    _BirthYear = value;
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

        [Column(Name = "PrimaryAddress2", Storage = "_PrimaryAddress2", DbType = "nvarchar(100)")]
        public string PrimaryAddress2
        {
            get => _PrimaryAddress2;

            set
            {
                if (_PrimaryAddress2 != value)
                {
                    _PrimaryAddress2 = value;
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

        [Column(Name = "WorkPhone", Storage = "_WorkPhone", DbType = "nvarchar(20)")]
        public string WorkPhone
        {
            get => _WorkPhone;

            set
            {
                if (_WorkPhone != value)
                {
                    _WorkPhone = value;
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

        [Column(Name = "MemberStatus", Storage = "_MemberStatus", DbType = "nvarchar(50)")]
        public string MemberStatus
        {
            get => _MemberStatus;

            set
            {
                if (_MemberStatus != value)
                {
                    _MemberStatus = value;
                }
            }
        }

        [Column(Name = "BFTeacher", Storage = "_BFTeacher", DbType = "nvarchar(138)")]
        public string BFTeacher
        {
            get => _BFTeacher;

            set
            {
                if (_BFTeacher != value)
                {
                    _BFTeacher = value;
                }
            }
        }

        [Column(Name = "BFTeacherId", Storage = "_BFTeacherId", DbType = "int")]
        public int? BFTeacherId
        {
            get => _BFTeacherId;

            set
            {
                if (_BFTeacherId != value)
                {
                    _BFTeacherId = value;
                }
            }
        }

        [Column(Name = "Age", Storage = "_Age", DbType = "int")]
        public int? Age
        {
            get => _Age;

            set
            {
                if (_Age != value)
                {
                    _Age = value;
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

        [Column(Name = "Joined", Storage = "_Joined", DbType = "datetime NOT NULL")]
        public DateTime Joined
        {
            get => _Joined;

            set
            {
                if (_Joined != value)
                {
                    _Joined = value;
                }
            }
        }
    }
}
