using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "FirstName")]
    public partial class FirstName
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _FirstNameX;

        private int? _Count;

        public FirstName()
        {
        }

        [Column(Name = "FirstName", Storage = "_FirstNameX", DbType = "nvarchar(25)")]
        public string FirstNameX
        {
            get => _FirstNameX;

            set
            {
                if (_FirstNameX != value)
                {
                    _FirstNameX = value;
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
