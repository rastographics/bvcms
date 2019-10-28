using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "MoveSchedule")]
    public partial class MoveSchedule
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Name;

        private DateTime? _BirthDate;

        private int _FromOrgId;

        private string _FromOrg;

        private DateTime? _LastSunday;

        private int? _MosMax;

        private int? _ToOrgId;

        private string _ToOrg;

        private string _ToLocation;

        public MoveSchedule()
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(138)")]
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

        [Column(Name = "BirthDate", Storage = "_BirthDate", DbType = "date")]
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

        [Column(Name = "FromOrgId", Storage = "_FromOrgId", DbType = "int NOT NULL")]
        public int FromOrgId
        {
            get => _FromOrgId;

            set
            {
                if (_FromOrgId != value)
                {
                    _FromOrgId = value;
                }
            }
        }

        [Column(Name = "FromOrg", Storage = "_FromOrg", DbType = "nvarchar(100)")]
        public string FromOrg
        {
            get => _FromOrg;

            set
            {
                if (_FromOrg != value)
                {
                    _FromOrg = value;
                }
            }
        }

        [Column(Name = "LastSunday", Storage = "_LastSunday", DbType = "date")]
        public DateTime? LastSunday
        {
            get => _LastSunday;

            set
            {
                if (_LastSunday != value)
                {
                    _LastSunday = value;
                }
            }
        }

        [Column(Name = "MosMax", Storage = "_MosMax", DbType = "int")]
        public int? MosMax
        {
            get => _MosMax;

            set
            {
                if (_MosMax != value)
                {
                    _MosMax = value;
                }
            }
        }

        [Column(Name = "ToOrgId", Storage = "_ToOrgId", DbType = "int")]
        public int? ToOrgId
        {
            get => _ToOrgId;

            set
            {
                if (_ToOrgId != value)
                {
                    _ToOrgId = value;
                }
            }
        }

        [Column(Name = "ToOrg", Storage = "_ToOrg", DbType = "nvarchar(100)")]
        public string ToOrg
        {
            get => _ToOrg;

            set
            {
                if (_ToOrg != value)
                {
                    _ToOrg = value;
                }
            }
        }

        [Column(Name = "ToLocation", Storage = "_ToLocation", DbType = "nvarchar(200)")]
        public string ToLocation
        {
            get => _ToLocation;

            set
            {
                if (_ToLocation != value)
                {
                    _ToLocation = value;
                }
            }
        }
    }
}
