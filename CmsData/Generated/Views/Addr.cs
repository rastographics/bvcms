using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "Addr")]
    public partial class Addr
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Street;

        private int? _Count;

        public Addr()
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
