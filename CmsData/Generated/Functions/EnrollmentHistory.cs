using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "EnrollmentHistory")]
    public partial class EnrollmentHistory
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _TransactionId;

        private bool _TransactionStatus;

        private int? _CreatedBy;

        private DateTime? _CreatedDate;

        private DateTime _TransactionDate;

        private int _TransactionTypeId;

        private int _OrganizationId;

        private string _OrganizationName;

        private int _PeopleId;

        private int _MemberTypeId;

        private DateTime? _EnrollmentDate;

        private decimal? _AttendancePercentage;

        private DateTime? _NextTranChangeDate;

        private int? _EnrollmentTransactionId;

        private bool? _Pending;

        private DateTime? _InactiveDate;

        private string _UserData;

        private string _Request;

        private string _ShirtSize;

        private int? _Grade;

        private int? _Tickets;

        private string _RegisterEmail;

        private int? _TranId;

        private int _Score;

        private string _SmallGroups;

        private bool? _SkipInsertTriggerProcessing;

        private int _PrevTranType;

        private int _Isgood;

        private int _Id;

        private string _Code;

        private string _Description;

        private int? _AttendanceTypeId;

        private bool? _Hardwired;

        private string _MemberType;

        public EnrollmentHistory()
        {
        }

        [Column(Name = "TransactionId", Storage = "_TransactionId", DbType = "int NOT NULL")]
        public int TransactionId
        {
            get => _TransactionId;

            set
            {
                if (_TransactionId != value)
                {
                    _TransactionId = value;
                }
            }
        }

        [Column(Name = "TransactionStatus", Storage = "_TransactionStatus", DbType = "bit NOT NULL")]
        public bool TransactionStatus
        {
            get => _TransactionStatus;

            set
            {
                if (_TransactionStatus != value)
                {
                    _TransactionStatus = value;
                }
            }
        }

        [Column(Name = "CreatedBy", Storage = "_CreatedBy", DbType = "int")]
        public int? CreatedBy
        {
            get => _CreatedBy;

            set
            {
                if (_CreatedBy != value)
                {
                    _CreatedBy = value;
                }
            }
        }

        [Column(Name = "CreatedDate", Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime? CreatedDate
        {
            get => _CreatedDate;

            set
            {
                if (_CreatedDate != value)
                {
                    _CreatedDate = value;
                }
            }
        }

        [Column(Name = "TransactionDate", Storage = "_TransactionDate", DbType = "datetime NOT NULL")]
        public DateTime TransactionDate
        {
            get => _TransactionDate;

            set
            {
                if (_TransactionDate != value)
                {
                    _TransactionDate = value;
                }
            }
        }

        [Column(Name = "TransactionTypeId", Storage = "_TransactionTypeId", DbType = "int NOT NULL")]
        public int TransactionTypeId
        {
            get => _TransactionTypeId;

            set
            {
                if (_TransactionTypeId != value)
                {
                    _TransactionTypeId = value;
                }
            }
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

        [Column(Name = "AttendancePercentage", Storage = "_AttendancePercentage", DbType = "real")]
        public decimal? AttendancePercentage
        {
            get => _AttendancePercentage;

            set
            {
                if (_AttendancePercentage != value)
                {
                    _AttendancePercentage = value;
                }
            }
        }

        [Column(Name = "NextTranChangeDate", Storage = "_NextTranChangeDate", DbType = "datetime")]
        public DateTime? NextTranChangeDate
        {
            get => _NextTranChangeDate;

            set
            {
                if (_NextTranChangeDate != value)
                {
                    _NextTranChangeDate = value;
                }
            }
        }

        [Column(Name = "EnrollmentTransactionId", Storage = "_EnrollmentTransactionId", DbType = "int")]
        public int? EnrollmentTransactionId
        {
            get => _EnrollmentTransactionId;

            set
            {
                if (_EnrollmentTransactionId != value)
                {
                    _EnrollmentTransactionId = value;
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

        [Column(Name = "UserData", Storage = "_UserData", DbType = "nvarchar")]
        public string UserData
        {
            get => _UserData;

            set
            {
                if (_UserData != value)
                {
                    _UserData = value;
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

        [Column(Name = "ShirtSize", Storage = "_ShirtSize", DbType = "nvarchar(50)")]
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

        [Column(Name = "RegisterEmail", Storage = "_RegisterEmail", DbType = "nvarchar(80)")]
        public string RegisterEmail
        {
            get => _RegisterEmail;

            set
            {
                if (_RegisterEmail != value)
                {
                    _RegisterEmail = value;
                }
            }
        }

        [Column(Name = "TranId", Storage = "_TranId", DbType = "int")]
        public int? TranId
        {
            get => _TranId;

            set
            {
                if (_TranId != value)
                {
                    _TranId = value;
                }
            }
        }

        [Column(Name = "Score", Storage = "_Score", DbType = "int NOT NULL")]
        public int Score
        {
            get => _Score;

            set
            {
                if (_Score != value)
                {
                    _Score = value;
                }
            }
        }

        [Column(Name = "SmallGroups", Storage = "_SmallGroups", DbType = "nvarchar(2000)")]
        public string SmallGroups
        {
            get => _SmallGroups;

            set
            {
                if (_SmallGroups != value)
                {
                    _SmallGroups = value;
                }
            }
        }

        [Column(Name = "SkipInsertTriggerProcessing", Storage = "_SkipInsertTriggerProcessing", DbType = "bit")]
        public bool? SkipInsertTriggerProcessing
        {
            get => _SkipInsertTriggerProcessing;

            set
            {
                if (_SkipInsertTriggerProcessing != value)
                {
                    _SkipInsertTriggerProcessing = value;
                }
            }
        }

        [Column(Name = "PrevTranType", Storage = "_PrevTranType", DbType = "int NOT NULL")]
        public int PrevTranType
        {
            get => _PrevTranType;

            set
            {
                if (_PrevTranType != value)
                {
                    _PrevTranType = value;
                }
            }
        }

        [Column(Name = "isgood", Storage = "_Isgood", DbType = "int NOT NULL")]
        public int Isgood
        {
            get => _Isgood;

            set
            {
                if (_Isgood != value)
                {
                    _Isgood = value;
                }
            }
        }

        [Column(Name = "Id", Storage = "_Id", DbType = "int NOT NULL")]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    _Id = value;
                }
            }
        }

        [Column(Name = "Code", Storage = "_Code", DbType = "nvarchar(20)")]
        public string Code
        {
            get => _Code;

            set
            {
                if (_Code != value)
                {
                    _Code = value;
                }
            }
        }

        [Column(Name = "Description", Storage = "_Description", DbType = "nvarchar(100)")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    _Description = value;
                }
            }
        }

        [Column(Name = "AttendanceTypeId", Storage = "_AttendanceTypeId", DbType = "int")]
        public int? AttendanceTypeId
        {
            get => _AttendanceTypeId;

            set
            {
                if (_AttendanceTypeId != value)
                {
                    _AttendanceTypeId = value;
                }
            }
        }

        [Column(Name = "Hardwired", Storage = "_Hardwired", DbType = "bit")]
        public bool? Hardwired
        {
            get => _Hardwired;

            set
            {
                if (_Hardwired != value)
                {
                    _Hardwired = value;
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
    }
}
