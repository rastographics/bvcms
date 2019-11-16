using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using UtilityExtensions;

namespace CmsData.View
{
    [Table(Name = "CurrOrgMembers2")]
    public partial class CurrOrgMembers2
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrganizationName;

        private string _FirstName;

        private string _LastName;

        private string _Gender;

        private int? _Grade;

        private string _ShirtSize;

        private string _Request;

        private decimal _Amount;

        private decimal _AmountPaid;

        private int _HasBalance;

        private string _Groups;

        private string _Email;

        private string _HomePhone;

        private string _CellPhone;

        private string _WorkPhone;

        private int? _Age;

        private DateTime? _BirthDate;

        private DateTime? _JoinDate;

        private string _MemberStatus;

        private string _SchoolOther;

        private DateTime? _LastAttend;

        private decimal? _AttendPct;

        private string _AttendStr;

        private string _MemberType;

        private string _MemberInfo;

        private string _Questions;

        private DateTime? _InactiveDate;

        private string _Allergies;

        private int _PeopleId;

        private DateTime? _EnrollDate;

        private int? _Tickets;

        private string _PassportNumber;

        private string _PassportExpires;

        public CurrOrgMembers2()
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

        [Column(Name = "FirstName", Storage = "_FirstName", DbType = "nvarchar(25)")]
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

        [Column(Name = "Gender", Storage = "_Gender", DbType = "nvarchar(20)")]
        public string Gender
        {
            get => _Gender;

            set
            {
                if (_Gender != value)
                {
                    _Gender = value;
                }
            }
        }

        [Column(Name = "Grade", Storage = "_Grade", DbType = "int")]
        public int? Grade
        {
            get => _Grade;

            set
            {
                if (_Grade != value)
                {
                    _Grade = value;
                }
            }
        }

        [Column(Name = "ShirtSize", Storage = "_ShirtSize", DbType = "nvarchar(20)")]
        public string ShirtSize
        {
            get => _ShirtSize;

            set
            {
                if (_ShirtSize != value)
                {
                    _ShirtSize = value;
                }
            }
        }

        [Column(Name = "Request", Storage = "_Request", DbType = "nvarchar(140)")]
        public string Request
        {
            get => _Request;

            set
            {
                if (_Request != value)
                {
                    _Request = value;
                }
            }
        }

        [Column(Name = "Amount", Storage = "_Amount", DbType = "money NOT NULL")]
        public decimal Amount
        {
            get => _Amount;

            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                }
            }
        }

        [Column(Name = "AmountPaid", Storage = "_AmountPaid", DbType = "money NOT NULL")]
        public decimal AmountPaid
        {
            get => _AmountPaid;

            set
            {
                if (_AmountPaid != value)
                {
                    _AmountPaid = value;
                }
            }
        }

        [Column(Name = "HasBalance", Storage = "_HasBalance", DbType = "int NOT NULL")]
        public int HasBalance
        {
            get => _HasBalance;

            set
            {
                if (_HasBalance != value)
                {
                    _HasBalance = value;
                }
            }
        }

        [Column(Name = "Groups", Storage = "_Groups", DbType = "nvarchar")]
        public string Groups
        {
            get => _Groups;

            set
            {
                if (_Groups != value)
                {
                    _Groups = value;
                }
            }
        }

        [Column(Name = "Email", Storage = "_Email", DbType = "nvarchar(150)")]
        public string Email
        {
            get => _Email;

            set
            {
                if (_Email != value)
                {
                    _Email = value;
                }
            }
        }

        [Column(Name = "HomePhone", Storage = "_HomePhone", DbType = "nvarchar(30)")]
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

        [Column(Name = "CellPhone", Storage = "_CellPhone", DbType = "nvarchar(30)")]
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

        [Column(Name = "WorkPhone", Storage = "_WorkPhone", DbType = "nvarchar(30)")]
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

        [Column(Name = "BirthDate", Storage = "_BirthDate", DbType = "datetime")]
        public DateTime? BirthDate
        {
            get => _BirthDate;

            set
            {
                if (_BirthDate != value)
                {
                    _BirthDate = value;
                }
            }
        }

        [Column(Name = "JoinDate", Storage = "_JoinDate", DbType = "datetime")]
        public DateTime? JoinDate
        {
            get => _JoinDate;

            set
            {
                if (_JoinDate != value)
                {
                    _JoinDate = value;
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

        [Column(Name = "SchoolOther", Storage = "_SchoolOther", DbType = "nvarchar(100)")]
        public string SchoolOther
        {
            get => _SchoolOther;

            set
            {
                if (_SchoolOther != value)
                {
                    _SchoolOther = value;
                }
            }
        }

        [Column(Name = "LastAttend", Storage = "_LastAttend", DbType = "datetime")]
        public DateTime? LastAttend
        {
            get => _LastAttend;

            set
            {
                if (_LastAttend != value)
                {
                    _LastAttend = value;
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

        [Column(Name = "MemberInfo", Storage = "_MemberInfo", DbType = "nvarchar")]
        public string MemberInfo
        {
            get => _MemberInfo;

            set
            {
                if (_MemberInfo != value)
                {
                    _MemberInfo = value;
                }
            }
        }

        [Column(Name = "Questions", Storage = "_Questions", DbType = "nvarchar")]
        public string Questions
        {
            get => _Questions;

            set
            {
                if (_Questions != value)
                {
                    _Questions = value;
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

        [Column(Name = "Allergies", Storage = "_Allergies", DbType = "nvarchar(1000)")]
        public string Allergies
        {
            get => _Allergies;

            set
            {
                if (_Allergies != value)
                {
                    _Allergies = value;
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

        [Column(Name = "EnrollDate", Storage = "_EnrollDate", DbType = "datetime")]
        public DateTime? EnrollDate
        {
            get => _EnrollDate;

            set
            {
                if (_EnrollDate != value)
                {
                    _EnrollDate = value;
                }
            }
        }

        [Column(Name = "Tickets", Storage = "_Tickets", DbType = "int")]
        public int? Tickets
        {
            get => _Tickets;

            set
            {
                if (_Tickets != value)
                {
                    _Tickets = value;
                }
            }
        }

        [Column(Name = "PassportNumber", Storage = "_PassportNumber", DbType = "nvarchar(max)")]
        public string PassportNumber
        {
            get => Util.Decrypt(_PassportNumber);

            set
            {
                if (_PassportNumber != value)
                {
                    _PassportNumber = value;
                }
            }
        }

        [Column(Name = "PassportExpires", Storage = "_PassportExpires", DbType = "nvarchar(max)")]
        public string PassportExpires
        {
            get => Util.Decrypt(_PassportExpires);

            set
            {
                if (_PassportExpires != value)
                {
                    _PassportExpires = value;
                }
            }
        }
    }
}
