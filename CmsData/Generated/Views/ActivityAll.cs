using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ActivityAll")]
    public partial class ActivityAll
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Machine;

        private DateTime? _ActivityDate;

        private string _Name;

        private int _UserId;

        private string _Activity;

        public ActivityAll()
        {
        }

        [Column(Name = "Machine", Storage = "_Machine", DbType = "nvarchar(50)")]
        public string Machine
        {
            get => _Machine;

            set
            {
                if (_Machine != value)
                {
                    _Machine = value;
                }
            }
        }

        [Column(Name = "ActivityDate", Storage = "_ActivityDate", DbType = "datetime")]
        public DateTime? ActivityDate
        {
            get => _ActivityDate;

            set
            {
                if (_ActivityDate != value)
                {
                    _ActivityDate = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(50) NOT NULL")]
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

        [Column(Name = "UserId", Storage = "_UserId", DbType = "int NOT NULL")]
        public int UserId
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

        [Column(Name = "Activity", Storage = "_Activity", DbType = "nvarchar(200)")]
        public string Activity
        {
            get => _Activity;

            set
            {
                if (_Activity != value)
                {
                    _Activity = value;
                }
            }
        }
    }
}
