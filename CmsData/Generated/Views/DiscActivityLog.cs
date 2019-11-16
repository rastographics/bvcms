using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "DiscActivityLog")]
    public partial class DiscActivityLog
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Name2;

        private string _PageUrl;

        private string _PageTitle;

        private DateTime? _VisitTime;

        public DiscActivityLog()
        {
        }

        [Column(Name = "Name2", Storage = "_Name2", DbType = "varchar(50)")]
        public string Name2
        {
            get => _Name2;

            set
            {
                if (_Name2 != value)
                {
                    _Name2 = value;
                }
            }
        }

        [Column(Name = "PageUrl", Storage = "_PageUrl", DbType = "varchar(150)")]
        public string PageUrl
        {
            get => _PageUrl;

            set
            {
                if (_PageUrl != value)
                {
                    _PageUrl = value;
                }
            }
        }

        [Column(Name = "PageTitle", Storage = "_PageTitle", DbType = "varchar(100) NOT NULL")]
        public string PageTitle
        {
            get => _PageTitle;

            set
            {
                if (_PageTitle != value)
                {
                    _PageTitle = value;
                }
            }
        }

        [Column(Name = "VisitTime", Storage = "_VisitTime", DbType = "datetime")]
        public DateTime? VisitTime
        {
            get => _VisitTime;

            set
            {
                if (_VisitTime != value)
                {
                    _VisitTime = value;
                }
            }
        }
    }
}
