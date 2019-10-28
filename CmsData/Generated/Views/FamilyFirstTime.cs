using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "FamilyFirstTimes")]
    public partial class FamilyFirstTime
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _FamilyId;

        private int? _HeadOfHouseholdId;

        private DateTime? _FirstDate;

        private DateTime? _CreatedDate;

        public FamilyFirstTime()
        {
        }

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int NOT NULL")]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    _FamilyId = value;
                }
            }
        }

        [Column(Name = "HeadOfHouseholdId", Storage = "_HeadOfHouseholdId", DbType = "int")]
        public int? HeadOfHouseholdId
        {
            get => _HeadOfHouseholdId;

            set
            {
                if (_HeadOfHouseholdId != value)
                {
                    _HeadOfHouseholdId = value;
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

        [Column(Name = "CreatedDate", Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime? CreatedDate
        {
            get => _CreatedDate;

            set
            {
                if (_CreatedDate != value)
                {
                    _CreatedDate = value;
                }
            }
        }
    }
}
