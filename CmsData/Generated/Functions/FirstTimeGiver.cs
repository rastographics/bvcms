using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "FirstTimeGivers")]
    public partial class FirstTimeGiver
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private DateTime? _FirstDate;

        private decimal? _Amt;

        public FirstTimeGiver()
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

        [Column(Name = "FirstDate", Storage = "_FirstDate", DbType = "datetime")]
        public DateTime? FirstDate
        {
            get => _FirstDate;

            set
            {
                if (_FirstDate != value)
                {
                    _FirstDate = value;
                }
            }
        }

        [Column(Name = "Amt", Storage = "_Amt", DbType = "Decimal(38,2)")]
        public decimal? Amt
        {
            get => _Amt;

            set
            {
                if (_Amt != value)
                {
                    _Amt = value;
                }
            }
        }
    }
}
