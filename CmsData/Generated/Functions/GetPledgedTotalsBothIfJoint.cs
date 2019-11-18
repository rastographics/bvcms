using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetPledgedTotalsBothIfJoint")]
    public partial class GetPledgedTotalsBothIfJoint
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _FamilyId;

        private int? _PeopleId;

        private string _Name;

        private decimal? _PledgeAmount;

        private decimal? _Amount;

        private decimal? _Balance;

        public GetPledgedTotalsBothIfJoint()
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(139)")]
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

        [Column(Name = "PledgeAmount", Storage = "_PledgeAmount", DbType = "Decimal(38,2)")]
        public decimal? PledgeAmount
        {
            get => _PledgeAmount;

            set
            {
                if (_PledgeAmount != value)
                {
                    _PledgeAmount = value;
                }
            }
        }

        [Column(Name = "Amount", Storage = "_Amount", DbType = "Decimal(38,2)")]
        public decimal? Amount
        {
            get => _Amount;

            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                }
            }
        }

        [Column(Name = "Balance", Storage = "_Balance", DbType = "Decimal(38,2)")]
        public decimal? Balance
        {
            get => _Balance;

            set
            {
                if (_Balance != value)
                {
                    _Balance = value;
                }
            }
        }
    }
}
