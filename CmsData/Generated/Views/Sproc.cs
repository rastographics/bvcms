using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "Sprocs")]
    public partial class Sproc
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Type;

        private string _Name;

        private string _Code;

        public Sproc()
        {
        }

        [Column(Name = "type", Storage = "_Type", DbType = "nvarchar(20)")]
        public string Type
        {
            get => _Type;

            set
            {
                if (_Type != value)
                {
                    _Type = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(128) NOT NULL")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    _Name = value;
                }
            }
        }

        [Column(Name = "Code", Storage = "_Code", DbType = "nvarchar(4000)")]
        public string Code
        {
            get => _Code;

            set
            {
                if (_Code != value)
                {
                    _Code = value;
                }
            }
        }
    }
}
