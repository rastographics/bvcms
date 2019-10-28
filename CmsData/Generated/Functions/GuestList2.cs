using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GuestList2")]
    public partial class GuestList2
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private DateTime _LastAttendDt;

        private bool _Hidden;

        private int? _MemberTypeId;

        public GuestList2()
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

        [Column(Name = "LastAttendDt", Storage = "_LastAttendDt", DbType = "datetime NOT NULL")]
        public DateTime LastAttendDt
        {
            get => _LastAttendDt;

            set
            {
                if (_LastAttendDt != value)
                {
                    _LastAttendDt = value;
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

        [Column(Name = "MemberTypeId", Storage = "_MemberTypeId", DbType = "int")]
        public int? MemberTypeId
        {
            get => _MemberTypeId;

            set
            {
                if (_MemberTypeId != value)
                {
                    _MemberTypeId = value;
                }
            }
        }
    }
}
