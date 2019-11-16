using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpBackgroundCheck")]
    public partial class XpBackgroundCheck
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Status;

        private DateTime? _ProcessedDate;

        private string _Comments;

        private int? _MVRStatusId;

        private DateTime? _MVRProcessedDate;

        public XpBackgroundCheck()
        {
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int NOT NULL")]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    _PeopleId = value;
                }
            }
        }

        [Column(Name = "Status", Storage = "_Status", DbType = "nvarchar(50)")]
        public string Status
        {
            get => _Status;

            set
            {
                if (_Status != value)
                {
                    _Status = value;
                }
            }
        }

        [Column(Name = "ProcessedDate", Storage = "_ProcessedDate", DbType = "datetime")]
        public DateTime? ProcessedDate
        {
            get => _ProcessedDate;

            set
            {
                if (_ProcessedDate != value)
                {
                    _ProcessedDate = value;
                }
            }
        }

        [Column(Name = "Comments", Storage = "_Comments", DbType = "nvarchar")]
        public string Comments
        {
            get => _Comments;

            set
            {
                if (_Comments != value)
                {
                    _Comments = value;
                }
            }
        }

        [Column(Name = "MVRStatusId", Storage = "_MVRStatusId", DbType = "int")]
        public int? MVRStatusId
        {
            get => _MVRStatusId;

            set
            {
                if (_MVRStatusId != value)
                {
                    _MVRStatusId = value;
                }
            }
        }

        [Column(Name = "MVRProcessedDate", Storage = "_MVRProcessedDate", DbType = "datetime")]
        public DateTime? MVRProcessedDate
        {
            get => _MVRProcessedDate;

            set
            {
                if (_MVRProcessedDate != value)
                {
                    _MVRProcessedDate = value;
                }
            }
        }
    }
}
