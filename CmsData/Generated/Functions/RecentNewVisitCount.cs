using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RecentNewVisitCount")]
    public partial class RecentNewVisitCount
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _PeopleId;

        private int? _Cnt;

        public RecentNewVisitCount()
        {
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
    }
}
