using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "MeetingsDataForDateRange")]
    public partial class MeetingsDataForDateRange
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Organization;

        private string _LeaderName;

        private DateTime? _Dt;

        private int? _Cnt;

        public MeetingsDataForDateRange()
        {
        }

        [Column(Name = "Organization", Storage = "_Organization", DbType = "nvarchar(100) NOT NULL")]
        public string Organization
        {
            get => _Organization;

            set
            {
                if (_Organization != value)
                {
                    _Organization = value;
                }
            }
        }

        [Column(Name = "LeaderName", Storage = "_LeaderName", DbType = "nvarchar(50)")]
        public string LeaderName
        {
            get => _LeaderName;

            set
            {
                if (_LeaderName != value)
                {
                    _LeaderName = value;
                }
            }
        }

        [Column(Name = "dt", Storage = "_Dt", DbType = "datetime")]
        public DateTime? Dt
        {
            get => _Dt;

            set
            {
                if (_Dt != value)
                {
                    _Dt = value;
                }
            }
        }

        [Column(Name = "cnt", Storage = "_Cnt", DbType = "int")]
        public int? Cnt
        {
            get => _Cnt;

            set
            {
                if (_Cnt != value)
                {
                    _Cnt = value;
                }
            }
        }
    }
}
