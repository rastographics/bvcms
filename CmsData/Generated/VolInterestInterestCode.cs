using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.VolInterestInterestCodes")]
    public partial class VolInterestInterestCode : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _PeopleId;

        private int _InterestCodeId;

        private EntityRef<Person> _Person;

        private EntityRef<VolInterestCode> _VolInterestCode;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnInterestCodeIdChanging(int value);
        partial void OnInterestCodeIdChanged();

        #endregion

        public VolInterestInterestCode()
        {
            _Person = default(EntityRef<Person>);

            _VolInterestCode = default(EntityRef<VolInterestCode>);

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

        [Column(Name = "InterestCodeId", UpdateCheck = UpdateCheck.Never, Storage = "_InterestCodeId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int InterestCodeId
        {
            get => _InterestCodeId;

            set
            {
                if (_InterestCodeId != value)
                {
                    if (_VolInterestCode.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnInterestCodeIdChanging(value);
                    SendPropertyChanging();
                    _InterestCodeId = value;
                    SendPropertyChanged("InterestCodeId");
                    OnInterestCodeIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_VolInterestInterestCodes_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.VolInterestInterestCodes.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.VolInterestInterestCodes.Add(this);

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

        [Association(Name = "FK_VolInterestInterestCodes_VolInterestCodes", Storage = "_VolInterestCode", ThisKey = "InterestCodeId", IsForeignKey = true)]
        public VolInterestCode VolInterestCode
        {
            get => _VolInterestCode.Entity;

            set
            {
                VolInterestCode previousValue = _VolInterestCode.Entity;
                if (((previousValue != value)
                            || (_VolInterestCode.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _VolInterestCode.Entity = null;
                        previousValue.VolInterestInterestCodes.Remove(this);
                    }

                    _VolInterestCode.Entity = value;
                    if (value != null)
                    {
                        value.VolInterestInterestCodes.Add(this);

                        _InterestCodeId = value.Id;

                    }

                    else
                    {
                        _InterestCodeId = default(int);

                    }

                    SendPropertyChanged("VolInterestCode");
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
