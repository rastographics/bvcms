using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "VisitNumberSinceDate")]
    public partial class VisitNumberSinceDate
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Name;

        private DateTime _MeetingDate;

        private long? _Rank;

        public VisitNumberSinceDate()
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

        [Column(Name = "Name", Storage = "_Name", DbType = "varchar(126)")]
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

        [Column(Name = "MeetingDate", Storage = "_MeetingDate", DbType = "datetime NOT NULL")]
        public DateTime MeetingDate
        {
            get => _MeetingDate;

            set
            {
                if (_MeetingDate != value)
                {
                    _MeetingDate = value;
                }
            }
        }

        [Column(Name = "Rank", Storage = "_Rank", DbType = "bigint")]
        public long? Rank
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
    }
}
