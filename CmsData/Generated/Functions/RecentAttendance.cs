using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RecentAttendance")]
    public partial class RecentAttendance
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Name;

        private DateTime _Lastattend;

        private decimal? _AttendPct;

        private string _AttendStr;

        private string _Attendtype;

        private int _Visitor;

        public RecentAttendance()
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

        [Column(Name = "NAME", Storage = "_Name", DbType = "nvarchar(138)")]
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

        [Column(Name = "lastattend", Storage = "_Lastattend", DbType = "datetime NOT NULL")]
        public DateTime Lastattend
        {
            get => _Lastattend;

            set
            {
                if (_Lastattend != value)
                {
                    _Lastattend = value;
                }
            }
        }

        [Column(Name = "AttendPct", Storage = "_AttendPct", DbType = "real")]
        public decimal? AttendPct
        {
            get => _AttendPct;

            set
            {
                if (_AttendPct != value)
                {
                    _AttendPct = value;
                }
            }
        }

        [Column(Name = "AttendStr", Storage = "_AttendStr", DbType = "nvarchar(200)")]
        public string AttendStr
        {
            get => _AttendStr;

            set
            {
                if (_AttendStr != value)
                {
                    _AttendStr = value;
                }
            }
        }

        [Column(Name = "attendtype", Storage = "_Attendtype", DbType = "nvarchar(100)")]
        public string Attendtype
        {
            get => _Attendtype;

            set
            {
                if (_Attendtype != value)
                {
                    _Attendtype = value;
                }
            }
        }

        [Column(Name = "visitor", Storage = "_Visitor", DbType = "int NOT NULL")]
        public int Visitor
        {
            get => _Visitor;

            set
            {
                if (_Visitor != value)
                {
                    _Visitor = value;
                }
            }
        }
    }
}
