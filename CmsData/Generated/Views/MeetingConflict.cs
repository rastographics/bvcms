using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "MeetingConflicts")]
    public partial class MeetingConflict
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private int? _OrgId1;

        private int? _OrgId2;

        private DateTime _MeetingDate;

        public MeetingConflict()
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

        [Column(Name = "OrgId1", Storage = "_OrgId1", DbType = "int")]
        public int? OrgId1
        {
            get => _OrgId1;

            set
            {
                if (_OrgId1 != value)
                {
                    _OrgId1 = value;
                }
            }
        }

        [Column(Name = "OrgId2", Storage = "_OrgId2", DbType = "int")]
        public int? OrgId2
        {
            get => _OrgId2;

            set
            {
                if (_OrgId2 != value)
                {
                    _OrgId2 = value;
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
    }
}
