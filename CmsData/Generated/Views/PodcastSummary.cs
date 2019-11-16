using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PodcastSummary")]
    public partial class PodcastSummary
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _UserId;

        private DateTime? _LastPosted;

        private int? _Count;

        private string _Username;

        public PodcastSummary()
        {
        }

        [Column(Name = "UserId", Storage = "_UserId", DbType = "int")]
        public int? UserId
        {
            get => _UserId;

            set
            {
                if (_UserId != value)
                {
                    _UserId = value;
                }
            }
        }

        [Column(Name = "lastPosted", Storage = "_LastPosted", DbType = "datetime")]
        public DateTime? LastPosted
        {
            get => _LastPosted;

            set
            {
                if (_LastPosted != value)
                {
                    _LastPosted = value;
                }
            }
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

        [Column(Name = "Username", Storage = "_Username", DbType = "varchar(50) NOT NULL")]
        public string Username
        {
            get => _Username;

            set
            {
                if (_Username != value)
                {
                    _Username = value;
                }
            }
        }
    }
}
