using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Tag")]
    public partial class Tag : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Name;

        private int _TypeId;

        private string _Owner;

        private bool? _Active;

        private int? _PeopleId;

        private string _OwnerName;

        private DateTime? _Created;

        private EntitySet<TagShare> _TagShares;

        private EntitySet<TagPerson> _PersonTags;

        private EntityRef<Person> _PersonOwner;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnTypeIdChanging(int value);
        partial void OnTypeIdChanged();

        partial void OnOwnerChanging(string value);
        partial void OnOwnerChanged();

        partial void OnActiveChanging(bool? value);
        partial void OnActiveChanged();

        partial void OnPeopleIdChanging(int? value);
        partial void OnPeopleIdChanged();

        partial void OnOwnerNameChanging(string value);
        partial void OnOwnerNameChanged();

        partial void OnCreatedChanging(DateTime? value);
        partial void OnCreatedChanged();

        #endregion

        public Tag()
        {
            _TagShares = new EntitySet<TagShare>(new Action<TagShare>(attach_TagShares), new Action<TagShare>(detach_TagShares));

            _PersonTags = new EntitySet<TagPerson>(new Action<TagPerson>(attach_PersonTags), new Action<TagPerson>(detach_PersonTags));

            _PersonOwner = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(200) NOT NULL")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    OnNameChanging(value);
                    SendPropertyChanging();
                    _Name = value;
                    SendPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }

        [Column(Name = "TypeId", UpdateCheck = UpdateCheck.Never, Storage = "_TypeId", DbType = "int NOT NULL")]
        public int TypeId
        {
            get => _TypeId;

            set
            {
                if (_TypeId != value)
                {
                    OnTypeIdChanging(value);
                    SendPropertyChanging();
                    _TypeId = value;
                    SendPropertyChanged("TypeId");
                    OnTypeIdChanged();
                }
            }
        }

        [Column(Name = "Owner", UpdateCheck = UpdateCheck.Never, Storage = "_Owner", DbType = "nvarchar(50)")]
        public string Owner
        {
            get => _Owner;

            set
            {
                if (_Owner != value)
                {
                    OnOwnerChanging(value);
                    SendPropertyChanging();
                    _Owner = value;
                    SendPropertyChanged("Owner");
                    OnOwnerChanged();
                }
            }
        }

        [Column(Name = "Active", UpdateCheck = UpdateCheck.Never, Storage = "_Active", DbType = "bit")]
        public bool? Active
        {
            get => _Active;

            set
            {
                if (_Active != value)
                {
                    OnActiveChanging(value);
                    SendPropertyChanging();
                    _Active = value;
                    SendPropertyChanged("Active");
                    OnActiveChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        [IsForeignKey]
        public int? PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    if (_PersonOwner.HasLoadedOrAssignedValue)
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

        [Column(Name = "OwnerName", UpdateCheck = UpdateCheck.Never, Storage = "_OwnerName", DbType = "nvarchar(100)", IsDbGenerated = true)]
        public string OwnerName
        {
            get => _OwnerName;

            set
            {
                if (_OwnerName != value)
                {
                    OnOwnerNameChanging(value);
                    SendPropertyChanging();
                    _OwnerName = value;
                    SendPropertyChanged("OwnerName");
                    OnOwnerNameChanged();
                }
            }
        }

        [Column(Name = "Created", UpdateCheck = UpdateCheck.Never, Storage = "_Created", DbType = "datetime")]
        public DateTime? Created
        {
            get => _Created;

            set
            {
                if (_Created != value)
                {
                    OnCreatedChanging(value);
                    SendPropertyChanging();
                    _Created = value;
                    SendPropertyChanged("Created");
                    OnCreatedChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_TagShare_Tag", Storage = "_TagShares", OtherKey = "TagId")]
        public EntitySet<TagShare> TagShares
           {
               get => _TagShares;

            set => _TagShares.Assign(value);

           }

        [Association(Name = "PersonTags__Tag", Storage = "_PersonTags", OtherKey = "Id")]
        public EntitySet<TagPerson> PersonTags
           {
               get => _PersonTags;

            set => _PersonTags.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "TagsOwned__PersonOwner", Storage = "_PersonOwner", ThisKey = "PeopleId", IsForeignKey = true)]
        public Person PersonOwner
        {
            get => _PersonOwner.Entity;

            set
            {
                Person previousValue = _PersonOwner.Entity;
                if (((previousValue != value)
                            || (_PersonOwner.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _PersonOwner.Entity = null;
                        previousValue.TagsOwned.Remove(this);
                    }

                    _PersonOwner.Entity = value;
                    if (value != null)
                    {
                        value.TagsOwned.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int?);

                    }

                    SendPropertyChanged("PersonOwner");
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

        private void attach_TagShares(TagShare entity)
        {
            SendPropertyChanging();
            entity.Tag = this;
        }

        private void detach_TagShares(TagShare entity)
        {
            SendPropertyChanging();
            entity.Tag = null;
        }

        private void attach_PersonTags(TagPerson entity)
        {
            SendPropertyChanging();
            entity.Tag = this;
        }

        private void detach_PersonTags(TagPerson entity)
        {
            SendPropertyChanging();
            entity.Tag = null;
        }
    }
}
