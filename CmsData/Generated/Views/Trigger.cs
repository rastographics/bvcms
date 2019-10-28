using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "Triggers")]
    public partial class Trigger
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _TableName;

        private string _TriggerName;

        private DateTime _TriggerCreatedDate;

        private string _Code;

        public Trigger()
        {
        }

        [Column(Name = "TableName", Storage = "_TableName", DbType = "nvarchar(128) NOT NULL")]
        public string TableName
        {
            get => _TableName;

            set
            {
                if (_TableName != value)
                {
                    _TableName = value;
                }
            }
        }

        [Column(Name = "TriggerName", Storage = "_TriggerName", DbType = "nvarchar(128) NOT NULL")]
        public string TriggerName
        {
            get => _TriggerName;

            set
            {
                if (_TriggerName != value)
                {
                    _TriggerName = value;
                }
            }
        }

        [Column(Name = "TriggerCreatedDate", Storage = "_TriggerCreatedDate", DbType = "datetime NOT NULL")]
        public DateTime TriggerCreatedDate
        {
            get => _TriggerCreatedDate;

            set
            {
                if (_TriggerCreatedDate != value)
                {
                    _TriggerCreatedDate = value;
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
