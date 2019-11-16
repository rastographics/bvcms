using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "WeeklyAttendsForOrgs")]
    public partial class WeeklyAttendsForOrg
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private bool _Attended;

        private DateTime _Sunday;

        public WeeklyAttendsForOrg()
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

        [Column(Name = "Attended", Storage = "_Attended", DbType = "bit NOT NULL")]
        public bool Attended
        {
            get => _Attended;

            set
            {
                if (_Attended != value)
                {
                    _Attended = value;
                }
            }
        }

        [Column(Name = "Sunday", Storage = "_Sunday", DbType = "datetime NOT NULL")]
        public DateTime Sunday
        {
            get => _Sunday;

            set
            {
                if (_Sunday != value)
                {
                    _Sunday = value;
                }
            }
        }
    }
}
