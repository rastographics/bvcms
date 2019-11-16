using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "VolunteerTimes")]
    public partial class VolunteerTime
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _Time;

        private int? _DayOfWeek;

        public VolunteerTime()
        {
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
                }
            }
        }

        [Column(Name = "Time", Storage = "_Time", DbType = "varchar(50)")]
        public string Time
        {
            get => _Time;

            set
            {
                if (_Time != value)
                {
                    _Time = value;
                }
            }
        }

        [Column(Name = "DayOfWeek", Storage = "_DayOfWeek", DbType = "int")]
        public int? DayOfWeek
        {
            get => _DayOfWeek;

            set
            {
                if (_DayOfWeek != value)
                {
                    _DayOfWeek = value;
                }
            }
        }
    }
}
