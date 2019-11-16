using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "FirstLast")]
    public partial class FirstLast
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _First;

        private string _Last;

        public FirstLast()
        {
        }

        [Column(Name = "First", Storage = "_First", DbType = "nvarchar(150)")]
        public string First
        {
            get => _First;

            set
            {
                if (_First != value)
                {
                    _First = value;
                }
            }
        }

        [Column(Name = "Last", Storage = "_Last", DbType = "nvarchar(150)")]
        public string Last
        {
            get => _Last;

            set
            {
                if (_Last != value)
                {
                    _Last = value;
                }
            }
        }
    }
}
