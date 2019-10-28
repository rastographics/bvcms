using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "LastAddrElement")]
    public partial class LastAddrElement
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Prev;

        public LastAddrElement()
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

        [Column(Name = "Prev", Storage = "_Prev", DbType = "nvarchar NOT NULL")]
        public string Prev
        {
            get => _Prev;

            set
            {
                if (_Prev != value)
                {
                    _Prev = value;
                }
            }
        }
    }
}
