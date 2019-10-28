using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "VolunteerCalendar")]
    public partial class VolunteerCalendar
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Name;

        private bool? _Commits;

        private bool? _Conflicts;

        public VolunteerCalendar()
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(139)")]
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

        [Column(Name = "commits", Storage = "_Commits", DbType = "bit")]
        public bool? Commits
        {
            get => _Commits;

            set
            {
                if (_Commits != value)
                {
                    _Commits = value;
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
