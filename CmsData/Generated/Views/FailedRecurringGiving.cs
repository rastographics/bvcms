using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "FailedRecurringGiving")]
    public partial class FailedRecurringGiving
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private DateTime? _Dt;

        public FailedRecurringGiving()
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

        [Column(Name = "Dt", Storage = "_Dt", DbType = "datetime")]
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
    }
}
