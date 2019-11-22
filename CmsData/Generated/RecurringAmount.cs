using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.RecurringAmounts")]
    public partial class RecurringAmount : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _PeopleId;

        private int _FundId;

        private decimal? _Amt;

        private EntityRef<ContributionFund> _ContributionFund;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnFundIdChanging(int value);
        partial void OnFundIdChanged();

        partial void OnAmtChanging(decimal? value);
        partial void OnAmtChanged();

        #endregion

        public RecurringAmount()
        {
            _ContributionFund = default(EntityRef<ContributionFund>);

            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL", IsPrimaryKey = true)]
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
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
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
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnFundIdChanging(value);
                    SendPropertyChanging();
                    _FundId = value;
                    SendPropertyChanged("FundId");
                    OnFundIdChanged();
                }
            }
        }

        [Column(Name = "Amt", UpdateCheck = UpdateCheck.Never, Storage = "_Amt", DbType = "money")]
        public decimal? Amt
        {
            get => _Amt;

            set
            {
                if (_Amt != value)
                {
                    OnAmtChanging(value);
                    SendPropertyChanging();
                    _Amt = value;
                    SendPropertyChanged("Amt");
                    OnAmtChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_RecurringAmounts_ContributionFund", Storage = "_ContributionFund", ThisKey = "FundId", IsForeignKey = true)]
        public ContributionFund ContributionFund
        {
            get => _ContributionFund.Entity;

            set
            {
                ContributionFund previousValue = _ContributionFund.Entity;
                if (((previousValue != value)
                            || (_ContributionFund.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ContributionFund.Entity = null;
                        previousValue.RecurringAmounts.Remove(this);
                    }

                    _ContributionFund.Entity = value;
                    if (value != null)
                    {
                        value.RecurringAmounts.Add(this);

                        _FundId = value.FundId;

                    }

                    else
                    {
                        _FundId = default(int);

                    }

                    SendPropertyChanged("ContributionFund");
                }
            }
        }

        [Association(Name = "FK_RecurringAmounts_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
        public Person Person
        {
            get => _Person.Entity;

            set
            {
                Person previousValue = _Person.Entity;
                if (((previousValue != value)
                            || (_Person.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Person.Entity = null;
                        previousValue.RecurringAmounts.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.RecurringAmounts.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int);

                    }

                    SendPropertyChanged("Person");
                }
            }
        }

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((PropertyChanging != null))
            {
                PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
