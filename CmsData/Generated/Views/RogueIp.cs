using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RogueIps")]
    public partial class RogueIp
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Ip;

        private string _Db;

        private DateTime? _Tm;

        public RogueIp()
        {
        }

        [Column(Name = "ip", Storage = "_Ip", DbType = "varchar(50) NOT NULL")]
        public string Ip
        {
            get => _Ip;

            set
            {
                if (_Ip != value)
                {
                    _Ip = value;
                }
            }
        }

        [Column(Name = "db", Storage = "_Db", DbType = "varchar(50)")]
        public string Db
        {
            get => _Db;

            set
            {
                if (_Db != value)
                {
                    _Db = value;
                }
            }
        }

        [Column(Name = "tm", Storage = "_Tm", DbType = "datetime")]
        public DateTime? Tm
        {
            get => _Tm;

            set
            {
                if (_Tm != value)
                {
                    _Tm = value;
                }
            }
        }
    }
}
