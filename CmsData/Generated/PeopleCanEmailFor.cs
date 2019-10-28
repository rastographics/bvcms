using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.PeopleCanEmailFor")]
    public partial class PeopleCanEmailFor : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _CanEmail;

        private int _OnBehalfOf;

        private EntityRef<Person> _PersonCanEmail;

        private EntityRef<Person> _OnBehalfOfPerson;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnCanEmailChanging(int value);
        partial void OnCanEmailChanged();

        partial void OnOnBehalfOfChanging(int value);
        partial void OnOnBehalfOfChanged();

        #endregion

        public PeopleCanEmailFor()
        {
            _PersonCanEmail = default(EntityRef<Person>);

            _OnBehalfOfPerson = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "CanEmail", UpdateCheck = UpdateCheck.Never, Storage = "_CanEmail", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int CanEmail
        {
            get => _CanEmail;

            set
            {
                if (_CanEmail != value)
                {
                    if (_PersonCanEmail.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnCanEmailChanging(value);
                    SendPropertyChanging();
                    _CanEmail = value;
                    SendPropertyChanged("CanEmail");
                    OnCanEmailChanged();
                }
            }
        }

        [Column(Name = "OnBehalfOf", UpdateCheck = UpdateCheck.Never, Storage = "_OnBehalfOf", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int OnBehalfOf
        {
            get => _OnBehalfOf;

            set
            {
                if (_OnBehalfOf != value)
                {
                    if (_OnBehalfOfPerson.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOnBehalfOfChanging(value);
                    SendPropertyChanging();
                    _OnBehalfOf = value;
                    SendPropertyChanged("OnBehalfOf");
                    OnOnBehalfOfChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "OnBehalfOfPeople__PersonCanEmail", Storage = "_PersonCanEmail", ThisKey = "CanEmail", IsForeignKey = true)]
        public Person PersonCanEmail
        {
            get => _PersonCanEmail.Entity;

            set
            {
                Person previousValue = _PersonCanEmail.Entity;
                if (((previousValue != value)
                            || (_PersonCanEmail.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _PersonCanEmail.Entity = null;
                        previousValue.OnBehalfOfPeople.Remove(this);
                    }

                    _PersonCanEmail.Entity = value;
                    if (value != null)
                    {
                        value.OnBehalfOfPeople.Add(this);

                        _CanEmail = value.PeopleId;

                    }

                    else
                    {
                        _CanEmail = default(int);

                    }

                    SendPropertyChanged("PersonCanEmail");
                }
            }
        }

        [Association(Name = "PersonsCanEmail__OnBehalfOfPerson", Storage = "_OnBehalfOfPerson", ThisKey = "OnBehalfOf", IsForeignKey = true)]
        public Person OnBehalfOfPerson
        {
            get => _OnBehalfOfPerson.Entity;

            set
            {
                Person previousValue = _OnBehalfOfPerson.Entity;
                if (((previousValue != value)
                            || (_OnBehalfOfPerson.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _OnBehalfOfPerson.Entity = null;
                        previousValue.PersonsCanEmail.Remove(this);
                    }

                    _OnBehalfOfPerson.Entity = value;
                    if (value != null)
                    {
                        value.PersonsCanEmail.Add(this);

                        _OnBehalfOf = value.PeopleId;

                    }

                    else
                    {
                        _OnBehalfOf = default(int);

                    }

                    SendPropertyChanged("OnBehalfOfPerson");
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
