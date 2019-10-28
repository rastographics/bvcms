using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "Attendance")]
    public partial class Attendance
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Attendstr;

        private decimal? _Pct;

        public Attendance()
        {
        }

        [Column(Name = "People_id", Storage = "_PeopleId", DbType = "int NOT NULL")]
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

        [Column(Name = "attendstr", Storage = "_Attendstr", DbType = "varchar(52)")]
        public string Attendstr
        {
            get => _Attendstr;

            set
            {
                if (_Attendstr != value)
                {
                    _Attendstr = value;
                }
            }
        }

        [Column(Name = "pct", Storage = "_Pct", DbType = "real")]
        public decimal? Pct
        {
            get => _Pct;

            set
            {
                if (_Pct != value)
                {
                    _Pct = value;
                }
            }
        }
    }
}
