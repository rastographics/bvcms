using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ConsecutiveAbsents")]
    public partial class ConsecutiveAbsent
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private int _PeopleId;

        private int? _Consecutive;

        private DateTime? _Lastattend;

        public ConsecutiveAbsent()
        {
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int NOT NULL")]
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

        [Column(Name = "consecutive", Storage = "_Consecutive", DbType = "int")]
        public int? Consecutive
        {
            get => _Consecutive;

            set
            {
                if (_Consecutive != value)
                {
                    _Consecutive = value;
                }
            }
        }

        [Column(Name = "lastattend", Storage = "_Lastattend", DbType = "datetime")]
        public DateTime? Lastattend
        {
            get => _Lastattend;

            set
            {
                if (_Lastattend != value)
                {
                    _Lastattend = value;
                }
            }
        }
    }
}
