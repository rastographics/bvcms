using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "City")]
    public partial class City
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _CityX;

        private string _State;

        private string _Zip;

        private int? _Count;

        public City()
        {
        }

        [Column(Name = "City", Storage = "_CityX", DbType = "nvarchar(30)")]
        public string CityX
        {
            get => _CityX;

            set
            {
                if (_CityX != value)
                {
                    _CityX = value;
                }
            }
        }

        [Column(Name = "State", Storage = "_State", DbType = "nvarchar(20)")]
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

        [Column(Name = "Zip", Storage = "_Zip", DbType = "nvarchar(15)")]
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
