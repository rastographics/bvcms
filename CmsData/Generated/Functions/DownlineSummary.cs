using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "DownlineSummary")]
    public partial class DownlineSummary
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Rank;

        private int? _PeopleId;

        private string _LeaderName;

        private int? _Cnt;

        private int? _Levels;

        private int? _MaxRows;

        public DownlineSummary()
        {
        }

        [Column(Name = "Rank", Storage = "_Rank", DbType = "int")]
        public int? Rank
        {
            get => _Rank;

            set
            {
                if (_Rank != value)
                {
                    _Rank = value;
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

        [Column(Name = "LeaderName", Storage = "_LeaderName", DbType = "varchar(100)")]
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

        [Column(Name = "Cnt", Storage = "_Cnt", DbType = "int")]
        public int? Cnt
        {
            get => _Cnt;

            set
            {
                if (_Cnt != value)
                {
                    _Cnt = value;
                }
            }
        }

        [Column(Name = "Levels", Storage = "_Levels", DbType = "int")]
        public int? Levels
        {
            get => _Levels;

            set
            {
                if (_Levels != value)
                {
                    _Levels = value;
                }
            }
        }

        [Column(Name = "MaxRows", Storage = "_MaxRows", DbType = "int")]
        public int? MaxRows
        {
            get => _MaxRows;

            set
            {
                if (_MaxRows != value)
                {
                    _MaxRows = value;
                }
            }
        }
    }
}
