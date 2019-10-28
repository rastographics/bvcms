using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "Churches")]
    public partial class Church
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _C;

        public Church()
        {
        }

        [Column(Name = "c", Storage = "_C", DbType = "nvarchar(120)")]
        public string C
        {
            get => _C;

            set
            {
                if (_C != value)
                {
                    _C = value;
                }
            }
        }
    }
}
