using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "Nick")]
    public partial class Nick
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _NickName;

        private int? _Count;

        public Nick()
        {
        }

        [Column(Name = "NickName", Storage = "_NickName", DbType = "nvarchar(25)")]
        public string NickName
        {
            get => _NickName;

            set
            {
                if (_NickName != value)
                {
                    _NickName = value;
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
