using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Contactees")]
    public partial class Contactee : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ContactId;

        private int _PeopleId;

        private bool? _ProfessionOfFaith;

        private bool? _PrayedForPerson;

        private EntityRef<Contact> _contact;

        private EntityRef<Person> _person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnContactIdChanging(int value);
        partial void OnContactIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnProfessionOfFaithChanging(bool? value);
        partial void OnProfessionOfFaithChanged();

        partial void OnPrayedForPersonChanging(bool? value);
        partial void OnPrayedForPersonChanged();

        #endregion

        public Contactee()
        {
            _contact = default(EntityRef<Contact>);

            _person = default(EntityRef<Person>);

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

        [Column(Name = "ProfessionOfFaith", UpdateCheck = UpdateCheck.Never, Storage = "_ProfessionOfFaith", DbType = "bit")]
        public bool? ProfessionOfFaith
        {
            get => _ProfessionOfFaith;

            set
            {
                if (_ProfessionOfFaith != value)
                {
                    OnProfessionOfFaithChanging(value);
                    SendPropertyChanging();
                    _ProfessionOfFaith = value;
                    SendPropertyChanged("ProfessionOfFaith");
                    OnProfessionOfFaithChanged();
                }
            }
        }

        [Column(Name = "PrayedForPerson", UpdateCheck = UpdateCheck.Never, Storage = "_PrayedForPerson", DbType = "bit")]
        public bool? PrayedForPerson
        {
            get => _PrayedForPerson;

            set
            {
                if (_PrayedForPerson != value)
                {
                    OnPrayedForPersonChanging(value);
                    SendPropertyChanging();
                    _PrayedForPerson = value;
                    SendPropertyChanged("PrayedForPerson");
                    OnPrayedForPersonChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "contactees__contact", Storage = "_contact", ThisKey = "ContactId", IsForeignKey = true)]
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
                        previousValue.contactees.Remove(this);
                    }

                    _contact.Entity = value;
                    if (value != null)
                    {
                        value.contactees.Add(this);

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

        [Association(Name = "contactsHad__person", Storage = "_person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.contactsHad.Remove(this);
                    }

                    _person.Entity = value;
                    if (value != null)
                    {
                        value.contactsHad.Add(this);

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
