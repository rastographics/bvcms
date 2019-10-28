using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgMinistryInfo")]
    public partial class OrgMinistryInfo
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Tab;

        private string _GroupCode;

        private int _PeopleId;

        private DateTime? _LastContactMadeDt;

        private int? _LastContactMadeId;

        private DateTime? _LastContactReceivedDt;

        private int? _LastContactReceivedId;

        private DateTime? _TaskAboutDt;

        private int? _TaskAboutId;

        private DateTime? _TaskDelegatedDt;

        private int? _TaskDelegatedId;

        public OrgMinistryInfo()
        {
        }

        [Column(Name = "Tab", Storage = "_Tab", DbType = "varchar(9) NOT NULL")]
        public string Tab
        {
            get => _Tab;

            set
            {
                if (_Tab != value)
                {
                    _Tab = value;
                }
            }
        }

        [Column(Name = "GroupCode", Storage = "_GroupCode", DbType = "varchar(2) NOT NULL")]
        public string GroupCode
        {
            get => _GroupCode;

            set
            {
                if (_GroupCode != value)
                {
                    _GroupCode = value;
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

        [Column(Name = "LastContactMadeDt", Storage = "_LastContactMadeDt", DbType = "datetime")]
        public DateTime? LastContactMadeDt
        {
            get => _LastContactMadeDt;

            set
            {
                if (_LastContactMadeDt != value)
                {
                    _LastContactMadeDt = value;
                }
            }
        }

        [Column(Name = "LastContactMadeId", Storage = "_LastContactMadeId", DbType = "int")]
        public int? LastContactMadeId
        {
            get => _LastContactMadeId;

            set
            {
                if (_LastContactMadeId != value)
                {
                    _LastContactMadeId = value;
                }
            }
        }

        [Column(Name = "LastContactReceivedDt", Storage = "_LastContactReceivedDt", DbType = "datetime")]
        public DateTime? LastContactReceivedDt
        {
            get => _LastContactReceivedDt;

            set
            {
                if (_LastContactReceivedDt != value)
                {
                    _LastContactReceivedDt = value;
                }
            }
        }

        [Column(Name = "LastContactReceivedId", Storage = "_LastContactReceivedId", DbType = "int")]
        public int? LastContactReceivedId
        {
            get => _LastContactReceivedId;

            set
            {
                if (_LastContactReceivedId != value)
                {
                    _LastContactReceivedId = value;
                }
            }
        }

        [Column(Name = "TaskAboutDt", Storage = "_TaskAboutDt", DbType = "datetime")]
        public DateTime? TaskAboutDt
        {
            get => _TaskAboutDt;

            set
            {
                if (_TaskAboutDt != value)
                {
                    _TaskAboutDt = value;
                }
            }
        }

        [Column(Name = "TaskAboutId", Storage = "_TaskAboutId", DbType = "int")]
        public int? TaskAboutId
        {
            get => _TaskAboutId;

            set
            {
                if (_TaskAboutId != value)
                {
                    _TaskAboutId = value;
                }
            }
        }

        [Column(Name = "TaskDelegatedDt", Storage = "_TaskDelegatedDt", DbType = "datetime")]
        public DateTime? TaskDelegatedDt
        {
            get => _TaskDelegatedDt;

            set
            {
                if (_TaskDelegatedDt != value)
                {
                    _TaskDelegatedDt = value;
                }
            }
        }

        [Column(Name = "TaskDelegatedId", Storage = "_TaskDelegatedId", DbType = "int")]
        public int? TaskDelegatedId
        {
            get => _TaskDelegatedId;

            set
            {
                if (_TaskDelegatedId != value)
                {
                    _TaskDelegatedId = value;
                }
            }
        }
    }
}
