using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "BadEtsList")]
    public partial class BadEtsList
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Id;

        private int? _Flag;

        private int _PeopleId;

        private int _OrganizationId;

        private int _TransactionId;

        private string _OrganizationName;

        private string _Name2;

        private bool? _Status;

        private DateTime _TransactionDate;

        private int _TransactionTypeId;

        private bool _TransactionStatus;

        public BadEtsList()
        {
        }

        [Column(Name = "id", Storage = "_Id", DbType = "int")]
        public int? Id
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

        [Column(Name = "Flag", Storage = "_Flag", DbType = "int")]
        public int? Flag
        {
            get => _Flag;

            set
            {
                if (_Flag != value)
                {
                    _Flag = value;
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

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "varchar(60) NOT NULL")]
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

        [Column(Name = "Name2", Storage = "_Name2", DbType = "varchar(37)")]
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

        [Column(Name = "Status", Storage = "_Status", DbType = "bit")]
        public bool? Status
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
    }
}
