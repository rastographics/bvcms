using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GivingChange")]
    public partial class GivingChange
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private decimal _TotalPeriod1;

        private decimal _TotalPeriod2;

        private decimal? _PctChange;

        private string _Change;

        public GivingChange()
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

        [Column(Name = "TotalPeriod1", Storage = "_TotalPeriod1", DbType = "Decimal(38,2) NOT NULL")]
        public decimal TotalPeriod1
        {
            get => _TotalPeriod1;

            set
            {
                if (_TotalPeriod1 != value)
                {
                    _TotalPeriod1 = value;
                }
            }
        }

        [Column(Name = "TotalPeriod2", Storage = "_TotalPeriod2", DbType = "Decimal(38,2) NOT NULL")]
        public decimal TotalPeriod2
        {
            get => _TotalPeriod2;

            set
            {
                if (_TotalPeriod2 != value)
                {
                    _TotalPeriod2 = value;
                }
            }
        }

        [Column(Name = "PctChange", Storage = "_PctChange", DbType = "Decimal(38,6)")]
        public decimal? PctChange
        {
            get => _PctChange;

            set
            {
                if (_PctChange != value)
                {
                    _PctChange = value;
                }
            }
        }

        [Column(Name = "Change", Storage = "_Change", DbType = "nvarchar(4000)")]
        public string Change
        {
            get => _Change;

            set
            {
                if (_Change != value)
                {
                    _Change = value;
                }
            }
        }
    }
}
