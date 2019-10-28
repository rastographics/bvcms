using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "Addr2")]
    public partial class Addr2
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Street;

        private string _City;

        private string _State;

        private string _Zip;

        private int? _Count;

        public Addr2()
        {
        }

        [Column(Name = "Street", Storage = "_Street", DbType = "varchar(50)")]
        public string Street
        {
            get => _Street;

            set
            {
                if (_Street != value)
                {
                    _Street = value;
                }
            }
        }

        [Column(Name = "City", Storage = "_City", DbType = "varchar(50)")]
        public string City
        {
            get => _City;

            set
            {
                if (_City != value)
                {
                    _City = value;
                }
            }
        }

        [Column(Name = "State", Storage = "_State", DbType = "varchar(5)")]
        public string State
        {
            get => _State;

            set
            {
                if (_State != value)
                {
                    _State = value;
                }
            }
        }

        [Column(Name = "Zip", Storage = "_Zip", DbType = "varchar(50)")]
        public string Zip
        {
            get => _Zip;

            set
            {
                if (_Zip != value)
                {
                    _Zip = value;
                }
            }
        }

        [Column(Name = "count", Storage = "_Count", DbType = "int")]
        public int? Count
        {
            get => _Count;

            set
            {
                if (_Count != value)
                {
                    _Count = value;
                }
            }
        }
    }
}
