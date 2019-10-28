using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.TagShare")]
    public partial class TagShare : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _TagId;

        private int _PeopleId;

        private EntityRef<Person> _Person;

        private EntityRef<Tag> _Tag;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnTagIdChanging(int value);
        partial void OnTagIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        #endregion

        public TagShare()
        {
            _Person = default(EntityRef<Person>);

            _Tag = default(EntityRef<Tag>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "TagId", UpdateCheck = UpdateCheck.Never, Storage = "_TagId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int TagId
        {
            get => _TagId;

            set
            {
                if (_TagId != value)
                {
                    if (_Tag.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnTagIdChanging(value);
                    SendPropertyChanging();
                    _TagId = value;
                    SendPropertyChanged("TagId");
                    OnTagIdChanged();
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

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_TagShare_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.TagShares.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.TagShares.Add(this);

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

        [Association(Name = "FK_TagShare_Tag", Storage = "_Tag", ThisKey = "TagId", IsForeignKey = true)]
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
                        previousValue.TagShares.Remove(this);
                    }

                    _Tag.Entity = value;
                    if (value != null)
                    {
                        value.TagShares.Add(this);

                        _TagId = value.Id;

                    }

                    else
                    {
                        _TagId = default(int);

                    }

                    SendPropertyChanged("Tag");
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
