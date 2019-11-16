using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ResourceCategory")]
    public partial class ResourceCategory : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ResourceCategoryId;

        private string _Name;

        private int _ResourceTypeId;

        private int _DisplayOrder;

        private EntitySet<Resource> _Resources;

        private EntityRef<ResourceType> _ResourceType;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnResourceCategoryIdChanging(int value);
        partial void OnResourceCategoryIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnResourceTypeIdChanging(int value);
        partial void OnResourceTypeIdChanged();

        partial void OnDisplayOrderChanging(int value);
        partial void OnDisplayOrderChanged();

        #endregion

        public ResourceCategory()
        {
            _Resources = new EntitySet<Resource>(new Action<Resource>(attach_Resources), new Action<Resource>(detach_Resources));

            _ResourceType = default(EntityRef<ResourceType>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ResourceCategoryId", UpdateCheck = UpdateCheck.Never, Storage = "_ResourceCategoryId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ResourceCategoryId
        {
            get => _ResourceCategoryId;

            set
            {
                if (_ResourceCategoryId != value)
                {
                    OnResourceCategoryIdChanging(value);
                    SendPropertyChanging();
                    _ResourceCategoryId = value;
                    SendPropertyChanged("ResourceCategoryId");
                    OnResourceCategoryIdChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(50) NOT NULL")]
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

        [Column(Name = "ResourceTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_ResourceTypeId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int ResourceTypeId
        {
            get => _ResourceTypeId;

            set
            {
                if (_ResourceTypeId != value)
                {
                    if (_ResourceType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnResourceTypeIdChanging(value);
                    SendPropertyChanging();
                    _ResourceTypeId = value;
                    SendPropertyChanged("ResourceTypeId");
                    OnResourceTypeIdChanged();
                }
            }
        }

        [Column(Name = "DisplayOrder", UpdateCheck = UpdateCheck.Never, Storage = "_DisplayOrder", DbType = "int NOT NULL")]
        public int DisplayOrder
        {
            get => _DisplayOrder;

            set
            {
                if (_DisplayOrder != value)
                {
                    OnDisplayOrderChanging(value);
                    SendPropertyChanging();
                    _DisplayOrder = value;
                    SendPropertyChanged("DisplayOrder");
                    OnDisplayOrderChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_Resource_ResourceCategory", Storage = "_Resources", OtherKey = "ResourceCategoryId")]
        public EntitySet<Resource> Resources
           {
               get => _Resources;

            set => _Resources.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_ResourceCategory_ResourceType", Storage = "_ResourceType", ThisKey = "ResourceTypeId", IsForeignKey = true)]
        public ResourceType ResourceType
        {
            get => _ResourceType.Entity;

            set
            {
                ResourceType previousValue = _ResourceType.Entity;
                if (((previousValue != value)
                            || (_ResourceType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ResourceType.Entity = null;
                        previousValue.ResourceCategories.Remove(this);
                    }

                    _ResourceType.Entity = value;
                    if (value != null)
                    {
                        value.ResourceCategories.Add(this);

                        _ResourceTypeId = value.ResourceTypeId;

                    }

                    else
                    {
                        _ResourceTypeId = default(int);

                    }

                    SendPropertyChanged("ResourceType");
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

        private void attach_Resources(Resource entity)
        {
            SendPropertyChanging();
            entity.ResourceCategory = this;
        }

        private void detach_Resources(Resource entity)
        {
            SendPropertyChanging();
            entity.ResourceCategory = null;
        }
    }
}
