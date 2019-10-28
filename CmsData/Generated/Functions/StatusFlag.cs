using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "StatusFlags")]
    public partial class StatusFlag
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _StatusFlags;

        public StatusFlag()
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

        [Column(Name = "StatusFlags", Storage = "_StatusFlags", DbType = "nvarchar")]
        public string StatusFlags
        {
            get => _StatusFlags;

            set
            {
                if (_StatusFlags != value)
                {
                    _StatusFlags = value;
                }
            }
        }
    }
}
