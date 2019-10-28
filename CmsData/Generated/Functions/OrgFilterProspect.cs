using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgFilterProspects")]
    public partial class OrgFilterProspect
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Tab;

        private string _GroupCode;

        private decimal? _AttPct;

        private DateTime? _LastAttended;

        private DateTime? _Joined;

        private DateTime? _Dropped;

        private DateTime? _InactiveDate;

        private string _MemberCode;

        private string _MemberType;

        private bool _Hidden;

        private string _Groups;

        public OrgFilterProspect()
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

        [Column(Name = "AttPct", Storage = "_AttPct", DbType = "real")]
        public decimal? AttPct
        {
            get => _AttPct;

            set
            {
                if (_AttPct != value)
                {
                    _AttPct = value;
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

        [Column(Name = "Joined", Storage = "_Joined", DbType = "datetime")]
        public DateTime? Joined
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

        [Column(Name = "Dropped", Storage = "_Dropped", DbType = "datetime")]
        public DateTime? Dropped
        {
            get => _Dropped;

            set
            {
                if (_Dropped != value)
                {
                    _Dropped = value;
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

        [Column(Name = "MemberCode", Storage = "_MemberCode", DbType = "nvarchar(20)")]
        public string MemberCode
        {
            get => _MemberCode;

            set
            {
                if (_MemberCode != value)
                {
                    _MemberCode = value;
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

        [Column(Name = "Hidden", Storage = "_Hidden", DbType = "bit NOT NULL")]
        public bool Hidden
        {
            get => _Hidden;

            set
            {
                if (_Hidden != value)
                {
                    _Hidden = value;
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
    }
}
