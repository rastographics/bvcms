using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ContactSummary")]
    public partial class ContactSummary
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Count;

        private string _ContactType;

        private string _ReasonType;

        private string _Ministry;

        private string _Comments;

        private string _ContactDate;

        private string _Contactor;

        public ContactSummary()
        {
        }

        [Column(Name = "Count", Storage = "_Count", DbType = "int")]
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

        [Column(Name = "ContactType", Storage = "_ContactType", DbType = "varchar(100)")]
        public string ContactType
        {
            get => _ContactType;

            set
            {
                if (_ContactType != value)
                {
                    _ContactType = value;
                }
            }
        }

        [Column(Name = "ReasonType", Storage = "_ReasonType", DbType = "varchar(100)")]
        public string ReasonType
        {
            get => _ReasonType;

            set
            {
                if (_ReasonType != value)
                {
                    _ReasonType = value;
                }
            }
        }

        [Column(Name = "Ministry", Storage = "_Ministry", DbType = "varchar(50)")]
        public string Ministry
        {
            get => _Ministry;

            set
            {
                if (_Ministry != value)
                {
                    _Ministry = value;
                }
            }
        }

        [Column(Name = "Comments", Storage = "_Comments", DbType = "varchar(11) NOT NULL")]
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

        [Column(Name = "ContactDate", Storage = "_ContactDate", DbType = "varchar(7) NOT NULL")]
        public string ContactDate
        {
            get => _ContactDate;

            set
            {
                if (_ContactDate != value)
                {
                    _ContactDate = value;
                }
            }
        }

        [Column(Name = "Contactor", Storage = "_Contactor", DbType = "varchar(12) NOT NULL")]
        public string Contactor
        {
            get => _Contactor;

            set
            {
                if (_Contactor != value)
                {
                    _Contactor = value;
                }
            }
        }
    }
}
