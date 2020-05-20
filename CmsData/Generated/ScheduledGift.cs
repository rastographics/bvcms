using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ScheduledGift")]
    public partial class ScheduledGift : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs("");

        #region Private Fields

        private Guid _ScheduledGiftId;

        private int _PeopleId;

        private int _ScheduledGiftTypeId;

        private Guid _PaymentMethodId;

        private DateTime _StartDate;

        private DateTime? _EndDate;

        private EntityRef<Person> _Person;

        private EntitySet<ScheduledGiftAmount> _ScheduledGiftAmounts;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(ChangeAction action);
        partial void OnCreated();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnScheduledGiftTypeIdChanging(int value);
        partial void OnScheduledGiftTypeIdChanged();

        partial void OnScheduledGiftIdChanging(Guid value);
        partial void OnScheduledGiftIdChanged();

        partial void OnStartDateChanging(DateTime value);
        partial void OnStartDateChanged();

        partial void OnEndDateChanging(DateTime? value);
        partial void OnEndDateChanged();

        partial void OnPaymentMethodIdChanging(Guid value);
        partial void OnPaymentMethodIdChanged();

        #endregion

        public ScheduledGift()
        {
            _Person = default;

            _ScheduledGiftAmounts = new EntitySet<ScheduledGiftAmount>(new Action<ScheduledGiftAmount>(attach_ScheduledGiftAmounts), new Action<ScheduledGiftAmount>(detach_ScheduledGiftAmounts));

            OnCreated();
        }

        #region Columns

        [Column(Name = "ScheduledGiftId", UpdateCheck = UpdateCheck.Never, Storage = "_ScheduledGiftId", DbType = "uniqueidentifier", IsDbGenerated = true, IsPrimaryKey = true)]
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

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        [IsForeignKey]
        public int PeopleId
        {
            get => _PeopleId;
            set
            {
                if (_PeopleId != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "ScheduledGiftTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_ScheduledGiftTypeId", DbType = "int")]
        public int ScheduledGiftTypeId
        {
            get => _ScheduledGiftTypeId;
            set
            {
                if (_ScheduledGiftTypeId != value)
                {
                    OnScheduledGiftTypeIdChanging(value);
                    SendPropertyChanging();
                    _ScheduledGiftTypeId = value;
                    SendPropertyChanged("ScheduledGiftTypeId");
                    OnScheduledGiftTypeIdChanged();
                }
            }
        }

        [Column(Name = "StartDate", UpdateCheck = UpdateCheck.Never, Storage = "_StartDate", DbType = "datetime NOT NULL")]
        public DateTime StartDate
        {
            get => _StartDate;
            set
            {
                if (_StartDate != value)
                {
                    OnStartDateChanging(value);
                    SendPropertyChanging();
                    _StartDate = value;
                    SendPropertyChanged("StartDate");
                    OnStartDateChanged();
                }
            }
        }

        [Column(Name = "EndDate", UpdateCheck = UpdateCheck.Never, Storage = "_EndDate", DbType = "datetime")]
        public DateTime? EndDate
        {
            get => _EndDate;
            set
            {
                if (_EndDate != value)
                {
                    OnEndDateChanging(value);
                    SendPropertyChanging();
                    _EndDate = value;
                    SendPropertyChanged("EndDate");
                    OnEndDateChanged();
                }
            }
        }

        [Column(Name = "PaymentMethodId", UpdateCheck = UpdateCheck.Never, Storage = "_PaymentMethodId", DbType = "uniqueidentifier NOT NULL")]
        [IsForeignKey]
        public Guid PaymentMethodId
        {
            get => _PaymentMethodId;
            set
            {
                if (_PaymentMethodId != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPaymentMethodIdChanging(value);
                    SendPropertyChanging();
                    _PaymentMethodId = value;
                    SendPropertyChanged("PaymentMethodId");
                    OnPaymentMethodIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_ScheduledGift_ScheduledGiftAmounts", Storage = "_ScheduledGiftAmounts", OtherKey = "ScheduledGiftAmountId")]
        public EntitySet<ScheduledGiftAmount> ScheduledGiftAmounts
        {
            get => _ScheduledGiftAmounts;
            set => _ScheduledGiftAmounts.Assign(value);

        }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_ScheduledGift_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
        public Person Person
        {
            get => _Person.Entity;
            set
            {
                Person previousValue = _Person.Entity;
                if ((previousValue != value) || (_Person.HasLoadedOrAssignedValue == false))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Person.Entity = null;
                        previousValue.ScheduledGifts.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.ScheduledGifts.Add(this);
                        _PeopleId = value.PeopleId;
                    }
                    else
                    {
                        _PeopleId = default;
                    }

                    SendPropertyChanged("Person");
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

        private void attach_ScheduledGiftAmounts(ScheduledGiftAmount entity)
        {
            SendPropertyChanging();
            entity.ScheduledGift = this;
        }

        private void detach_ScheduledGiftAmounts(ScheduledGiftAmount entity)
        {
            SendPropertyChanging();
            entity.ScheduledGift = null;
        }
    }
}
