using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "AttendanceChange")]
    public partial class AttendanceChange
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private decimal _Pct1;

        private decimal _Pct2;

        private decimal? _PctChange;

        private string _Change;

        public AttendanceChange()
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

        [Column(Name = "Pct1", Storage = "_Pct1", DbType = "Decimal(28,12) NOT NULL")]
        public decimal Pct1
        {
            get => _Pct1;

            set
            {
                if (_Pct1 != value)
                {
                    _Pct1 = value;
                }
            }
        }

        [Column(Name = "Pct2", Storage = "_Pct2", DbType = "Decimal(28,12) NOT NULL")]
        public decimal Pct2
        {
            get => _Pct2;

            set
            {
                if (_Pct2 != value)
                {
                    _Pct2 = value;
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

        [Column(Name = "CHANGE", Storage = "_Change", DbType = "nvarchar(4000)")]
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
