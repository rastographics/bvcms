using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "MissionTripTotals")]
    public partial class MissionTripTotal
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _Trip;

        private int? _PeopleId;

        private string _Name;

        private string _SortOrder;

        private decimal? _TripCost;

        private decimal? _Raised;

        private decimal? _Due;

        public MissionTripTotal()
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

        [Column(Name = "Trip", Storage = "_Trip", DbType = "nvarchar(100) NOT NULL")]
        public string Trip
        {
            get => _Trip;

            set
            {
                if (_Trip != value)
                {
                    _Trip = value;
                }
            }
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(138)")]
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

        [Column(Name = "SortOrder", Storage = "_SortOrder", DbType = "nvarchar(139)")]
        public string SortOrder
        {
            get => _SortOrder;

            set
            {
                if (_SortOrder != value)
                {
                    _SortOrder = value;
                }
            }
        }

        [Column(Name = "TripCost", Storage = "_TripCost", DbType = "money")]
        public decimal? TripCost
        {
            get => _TripCost;

            set
            {
                if (_TripCost != value)
                {
                    _TripCost = value;
                }
            }
        }

        [Column(Name = "Raised", Storage = "_Raised", DbType = "money")]
        public decimal? Raised
        {
            get => _Raised;

            set
            {
                if (_Raised != value)
                {
                    _Raised = value;
                }
            }
        }

        [Column(Name = "Due", Storage = "_Due", DbType = "money")]
        public decimal? Due
        {
            get => _Due;

            set
            {
                if (_Due != value)
                {
                    _Due = value;
                }
            }
        }
    }
}
