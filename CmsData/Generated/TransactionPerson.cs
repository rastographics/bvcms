using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.TransactionPeople")]
    public partial class TransactionPerson : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _PeopleId;

        private decimal? _Amt;

        private int? _OrgId;

        private bool? _Donor;

        private EntityRef<Person> _Person;

        private EntityRef<Transaction> _Transaction;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnAmtChanging(decimal? value);
        partial void OnAmtChanged();

        partial void OnOrgIdChanging(int? value);
        partial void OnOrgIdChanged();

        partial void OnDonorChanging(bool? value);
        partial void OnDonorChanged();

        #endregion

        public TransactionPerson()
        {
            _Person = default(EntityRef<Person>);

            _Transaction = default(EntityRef<Transaction>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    if (_Transaction.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

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

        [Column(Name = "OrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OrgId", DbType = "int")]
        public int? OrgId
        {
            get => _OrgId;

            set
            {
                if (_OrgId != value)
                {
                    OnOrgIdChanging(value);
                    SendPropertyChanging();
                    _OrgId = value;
                    SendPropertyChanged("OrgId");
                    OnOrgIdChanged();
                }
            }
        }

        [Column(Name = "Donor", UpdateCheck = UpdateCheck.Never, Storage = "_Donor", DbType = "bit")]
        public bool? Donor
        {
            get => _Donor;

            set
            {
                if (_Donor != value)
                {
                    OnDonorChanging(value);
                    SendPropertyChanging();
                    _Donor = value;
                    SendPropertyChanged("Donor");
                    OnDonorChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_TransactionPeople_Person", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.TransactionPeople.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.TransactionPeople.Add(this);

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

        [Association(Name = "FK_TransactionPeople_Transaction", Storage = "_Transaction", ThisKey = "Id", IsForeignKey = true)]
        public Transaction Transaction
        {
            get => _Transaction.Entity;

            set
            {
                Transaction previousValue = _Transaction.Entity;
                if (((previousValue != value)
                            || (_Transaction.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Transaction.Entity = null;
                        previousValue.TransactionPeople.Remove(this);
                    }

                    _Transaction.Entity = value;
                    if (value != null)
                    {
                        value.TransactionPeople.Add(this);

                        _Id = value.Id;

                    }

                    else
                    {
                        _Id = default(int);

                    }

                    SendPropertyChanged("Transaction");
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
