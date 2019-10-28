using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "MembersWhoAttendedOrgs")]
    public partial class MembersWhoAttendedOrg
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Name2;

        private int? _Age;

        private int? _BibleFellowshipClassId;

        private string _OrganizationName;

        private string _LeaderName;

        public MembersWhoAttendedOrg()
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

        [Column(Name = "Name2", Storage = "_Name2", DbType = "varchar(90)")]
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

        [Column(Name = "Age", Storage = "_Age", DbType = "int")]
        public int? Age
        {
            get => _Age;

            set
            {
                if (_Age != value)
                {
                    _Age = value;
                }
            }
        }

        [Column(Name = "BibleFellowshipClassId", Storage = "_BibleFellowshipClassId", DbType = "int")]
        public int? BibleFellowshipClassId
        {
            get => _BibleFellowshipClassId;

            set
            {
                if (_BibleFellowshipClassId != value)
                {
                    _BibleFellowshipClassId = value;
                }
            }
        }

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "varchar(100)")]
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

        [Column(Name = "LeaderName", Storage = "_LeaderName", DbType = "varchar(90)")]
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
    }
}
