using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "LastAttendOrg")]
    public partial class LastAttendOrg
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private DateTime _LastAttended;

        public LastAttendOrg()
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

        [Column(Name = "LastAttended", Storage = "_LastAttended", DbType = "datetime NOT NULL")]
        public DateTime LastAttended
        {
            get => _LastAttended;

            set
            {
                if (_LastAttended != value)
                {
                    _LastAttended = value;
                }
            }
        }
    }
}
