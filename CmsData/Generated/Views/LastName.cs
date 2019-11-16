using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "LastName")]
    public partial class LastName
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _LastNameX;

        private int? _Count;

        public LastName()
        {
        }

        [Column(Name = "LastName", Storage = "_LastNameX", DbType = "nvarchar(100) NOT NULL")]
        public string LastNameX
        {
            get => _LastNameX;

            set
            {
                if (_LastNameX != value)
                {
                    _LastNameX = value;
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
