using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "AttendCommitments")]
    public partial class AttendCommitment
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _MeetingId;

        private DateTime _MeetingDate;

        private int _PeopleId;

        private string _Name2;

        private int? _Commitment;

        private bool? _Conflicts;

        public AttendCommitment()
        {
        }

        [Column(Name = "MeetingId", Storage = "_MeetingId", DbType = "int NOT NULL")]
        public int MeetingId
        {
            get => _MeetingId;

            set
            {
                if (_MeetingId != value)
                {
                    _MeetingId = value;
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

        [Column(Name = "Name2", Storage = "_Name2", DbType = "nvarchar(139)")]
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

        [Column(Name = "Commitment", Storage = "_Commitment", DbType = "int")]
        public int? Commitment
        {
            get => _Commitment;

            set
            {
                if (_Commitment != value)
                {
                    _Commitment = value;
                }
            }
        }

        [Column(Name = "conflicts", Storage = "_Conflicts", DbType = "bit")]
        public bool? Conflicts
        {
            get => _Conflicts;

            set
            {
                if (_Conflicts != value)
                {
                    _Conflicts = value;
                }
            }
        }
    }
}
