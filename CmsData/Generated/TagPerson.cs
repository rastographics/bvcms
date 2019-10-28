using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.TagPerson")]
    public partial class TagPerson : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _PeopleId;

        private DateTime? _DateCreated;

        private EntityRef<Tag> _Tag;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnDateCreatedChanging(DateTime? value);
        partial void OnDateCreatedChanged();

        #endregion

        public TagPerson()
        {
            _Tag = default(EntityRef<Tag>);

            _Person = default(EntityRef<Person>);

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
                    if (_Tag.HasLoadedOrAssignedValue)
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

        [Column(Name = "DateCreated", UpdateCheck = UpdateCheck.Never, Storage = "_DateCreated", DbType = "datetime")]
        public DateTime? DateCreated
        {
            get => _DateCreated;

            set
            {
                if (_DateCreated != value)
                {
                    OnDateCreatedChanging(value);
                    SendPropertyChanging();
                    _DateCreated = value;
                    SendPropertyChanged("DateCreated");
                    OnDateCreatedChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "PersonTags__Tag", Storage = "_Tag", ThisKey = "Id", IsForeignKey = true)]
        public Tag Tag
        {
            get => _Tag.Entity;

            set
            {
                Tag previousValue = _Tag.Entity;
                if (((previousValue != value)
                            || (_Tag.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Tag.Entity = null;
                        previousValue.PersonTags.Remove(this);
                    }

                    _Tag.Entity = value;
                    if (value != null)
                    {
                        value.PersonTags.Add(this);

                        _Id = value.Id;

                    }

                    else
                    {
                        _Id = default(int);

                    }

                    SendPropertyChanged("Tag");
                }
            }
        }

        [Association(Name = "Tags__Person", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.Tags.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.Tags.Add(this);

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
