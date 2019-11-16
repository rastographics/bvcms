using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpContactee")]
    public partial class XpContactee
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _ContactId;

        private int _PeopleId;

        public XpContactee()
        {
        }

        [Column(Name = "ContactId", Storage = "_ContactId", DbType = "int NOT NULL")]
        public int ContactId
        {
            get => _ContactId;

            set
            {
                if (_ContactId != value)
                {
                    _ContactId = value;
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
    }
}
