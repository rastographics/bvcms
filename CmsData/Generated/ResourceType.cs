using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ResourceType")]
    public partial class ResourceType : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ResourceTypeId;

        private string _Name;

        private int _DisplayOrder;

        private EntitySet<Resource> _Resources;

        private EntitySet<ResourceCategory> _ResourceCategories;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnResourceTypeIdChanging(int value);
        partial void OnResourceTypeIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnDisplayOrderChanging(int value);
        partial void OnDisplayOrderChanged();

        #endregion

        public ResourceType()
        {
            _Resources = new EntitySet<Resource>(new Action<Resource>(attach_Resources), new Action<Resource>(detach_Resources));

            _ResourceCategories = new EntitySet<ResourceCategory>(new Action<ResourceCategory>(attach_ResourceCategories), new Action<ResourceCategory>(detach_ResourceCategories));

            OnCreated();
        }

        #region Columns

        [Column(Name = "ResourceTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_ResourceTypeId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ResourceTypeId
        {
            get => _ResourceTypeId;

            set
            {
                if (_ResourceTypeId != value)
                {
                    OnResourceTypeIdChanging(value);
                    SendPropertyChanging();
                    _ResourceTypeId = value;
                    SendPropertyChanged("ResourceTypeId");
                    OnResourceTypeIdChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "varchar(50) NOT NULL")]
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

        [Association(Name = "FK_Resource_ResourceType", Storage = "_Resources", OtherKey = "ResourceTypeId")]
        public EntitySet<Resource> Resources
           {
               get => _Resources;

            set => _Resources.Assign(value);

           }

        [Association(Name = "FK_ResourceCategory_ResourceType", Storage = "_ResourceCategories", OtherKey = "ResourceTypeId")]
        public EntitySet<ResourceCategory> ResourceCategories
           {
               get => _ResourceCategories;

            set => _ResourceCategories.Assign(value);

           }

        #endregion

        #region Foreign Keys

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
            entity.ResourceType = this;
        }

        private void detach_Resources(Resource entity)
        {
            SendPropertyChanging();
            entity.ResourceType = null;
        }

        private void attach_ResourceCategories(ResourceCategory entity)
        {
            SendPropertyChanging();
            entity.ResourceType = this;
        }

        private void detach_ResourceCategories(ResourceCategory entity)
        {
            SendPropertyChanging();
            entity.ResourceType = null;
        }
    }
}
