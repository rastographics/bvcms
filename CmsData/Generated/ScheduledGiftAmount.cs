using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ScheduledGiftAmount")]
    public partial class ScheduledGiftAmount : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ScheduledGiftAmountId;

        private Guid _ScheduledGiftId;

        private int _FundId;

        private decimal _Amount;

        private EntityRef<ContributionFund> _ContributionFund;

        private EntityRef<ScheduledGift> _ScheduledGift;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(ChangeAction action);
        partial void OnCreated();

        partial void OnScheduledGiftAmountIdChanging(int value);
        partial void OnScheduledGiftAmountIdChanged();

        partial void OnScheduledGiftIdChanging(Guid value);
        partial void OnScheduledGiftIdChanged();

        partial void OnFundIdChanging(int value);
        partial void OnFundIdChanged();

        partial void OnAmountChanging(decimal value);
        partial void OnAmountChanged();

        #endregion

        public ScheduledGiftAmount()
        {
            _ContributionFund = default;

            OnCreated();
        }

        #region Columns

        [Column(Name = "ScheduledGiftAmountId", UpdateCheck = UpdateCheck.Never, Storage = "_ScheduledGiftAmountId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ScheduledGiftAmountId
        {
            get => _ScheduledGiftAmountId;
            set
            {
                if (_ScheduledGiftAmountId != value)
                {
                    OnScheduledGiftAmountIdChanging(value);
                    SendPropertyChanging();
                    _ScheduledGiftAmountId = value;
                    SendPropertyChanged("ScheduledGiftAmountId");
                    OnScheduledGiftAmountIdChanged();
                }
            }
        }

        [Column(Name = "ScheduledGiftId", UpdateCheck = UpdateCheck.Never, Storage = "_ScheduledGiftId", DbType = "uniqueidentifier")]
        [IsForeignKey]
        public Guid ScheduledGiftId
        {
            get => _ScheduledGiftId;
            set
            {
                if (_ScheduledGiftId != value)
                {
                    OnScheduledGiftIdChanging(value);
                    SendPropertyChanging();
                    _ScheduledGiftId = value;
                    SendPropertyChanged("ScheduledGiftId");
                    OnScheduledGiftIdChanged();
                }
            }
        }

        [Column(Name = "FundId", UpdateCheck = UpdateCheck.Never, Storage = "_FundId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int FundId
        {
            get => _FundId;
            set
            {
                if (_FundId != value)
                {
                    if (_ContributionFund.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnFundIdChanging(value);
                    SendPropertyChanging();
                    _FundId = value;
                    SendPropertyChanged("FundId");
                    OnFundIdChanged();
                }
            }
        }

        [Column(Name = "Amount", UpdateCheck = UpdateCheck.Never, Storage = "_Amount", DbType = "Decimal(10,2)")]
        public decimal Amount
        {
            get => _Amount;
            set
            {
                if (_Amount != value)
                {
                    OnAmountChanging(value);
                    SendPropertyChanging();
                    _Amount = value;
                    SendPropertyChanged("Amount");
                    OnAmountChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_ScheduledGiftAmounts_ContributionFund", Storage = "_ContributionFund", ThisKey = "FundId", IsForeignKey = true)]
        public ContributionFund ContributionFund
        {
            get => _ContributionFund.Entity;
            set
            {
                ContributionFund previousValue = _ContributionFund.Entity;
                if ((previousValue != value) || (_ContributionFund.HasLoadedOrAssignedValue == false))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ContributionFund.Entity = null;
                    }

                    _ContributionFund.Entity = value;
                    if (value != null)
                    {
                        _FundId = value.FundId;
                    }
                    else
                    {
                        _FundId = default;
                    }

                    SendPropertyChanged("ContributionFund");
                }
            }
        }

        [Association(Name = "FK_ScheduledGiftAmounts_ScheduledGift", Storage = "_ScheduledGift", ThisKey = "ScheduledGiftId", IsForeignKey = true)]
        public ScheduledGift ScheduledGift
        {
            get => _ScheduledGift.Entity;
            set
            {
                ScheduledGift previousValue = _ScheduledGift.Entity;
                if ((previousValue != value) || (_ScheduledGift.HasLoadedOrAssignedValue == false))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ScheduledGift.Entity = null;
                    }

                    _ScheduledGift.Entity = value;
                    if (value != null)
                    {
                        _ScheduledGiftId = value.ScheduledGiftId;
                    }
                    else
                    {
                        _ScheduledGiftId = default;
                    }

                    SendPropertyChanged("ScheduledGift");
                }
            }
        }

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            PropertyChanging?.Invoke(this, emptyChangingEventArgs);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
