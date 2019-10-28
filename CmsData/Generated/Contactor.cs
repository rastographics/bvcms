using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Contactors")]
    public partial class Contactor : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ContactId;

        private int _PeopleId;

        private EntityRef<Person> _person;

        private EntityRef<Contact> _contact;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnContactIdChanging(int value);
        partial void OnContactIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        #endregion

        public Contactor()
        {
            _person = default(EntityRef<Person>);

            _contact = default(EntityRef<Contact>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ContactId", UpdateCheck = UpdateCheck.Never, Storage = "_ContactId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int ContactId
        {
            get => _ContactId;

            set
            {
                if (_ContactId != value)
                {
                    if (_contact.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnContactIdChanging(value);
                    SendPropertyChanging();
                    _ContactId = value;
                    SendPropertyChanged("ContactId");
                    OnContactIdChanged();
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
                    if (_person.HasLoadedOrAssignedValue)
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

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "contactsMade__person", Storage = "_person", ThisKey = "PeopleId", IsForeignKey = true)]
        public Person person
        {
            get => _person.Entity;

            set
            {
                Person previousValue = _person.Entity;
                if (((previousValue != value)
                            || (_person.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _person.Entity = null;
                        previousValue.contactsMade.Remove(this);
                    }

                    _person.Entity = value;
                    if (value != null)
                    {
                        value.contactsMade.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int);

                    }

                    SendPropertyChanged("person");
                }
            }
        }

        [Association(Name = "contactsMakers__contact", Storage = "_contact", ThisKey = "ContactId", IsForeignKey = true)]
        public Contact contact
        {
            get => _contact.Entity;

            set
            {
                Contact previousValue = _contact.Entity;
                if (((previousValue != value)
                            || (_contact.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _contact.Entity = null;
                        previousValue.contactsMakers.Remove(this);
                    }

                    _contact.Entity = value;
                    if (value != null)
                    {
                        value.contactsMakers.Add(this);

                        _ContactId = value.ContactId;

                    }

                    else
                    {
                        _ContactId = default(int);

                    }

                    SendPropertyChanged("contact");
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
