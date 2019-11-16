using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpEnrollHistory")]
    public partial class XpEnrollHistory
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _TransactionId;

        private int _OrganizationId;

        private int _PeopleId;

        private DateTime _TransactionDate;

        private string _OrganizationName;

        private int _MemberTypeId;

        private string _TransactionType;

        private int? _EnrollmentTransactionId;

        public XpEnrollHistory()
        {
        }

        [Column(Name = "TransactionId", Storage = "_TransactionId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
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

        [Column(Name = "TransactionType", Storage = "_TransactionType", DbType = "varchar(4)")]
        public string TransactionType
        {
            get => _TransactionType;

            set
            {
                if (_TransactionType != value)
                {
                    _TransactionType = value;
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
    }
}
