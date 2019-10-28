using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ResourceOrganizationType")]
    public partial class ResourceOrganizationType : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ResourceId;

        private int _OrganizationTypeId;

        private EntityRef<OrganizationType> _OrganizationType;

        private EntityRef<Resource> _Resource;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnResourceIdChanging(int value);
        partial void OnResourceIdChanged();

        partial void OnOrganizationTypeIdChanging(int value);
        partial void OnOrganizationTypeIdChanged();

        #endregion

        public ResourceOrganizationType()
        {
            _OrganizationType = default(EntityRef<OrganizationType>);

            _Resource = default(EntityRef<Resource>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ResourceId", UpdateCheck = UpdateCheck.Never, Storage = "_ResourceId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int ResourceId
        {
            get => _ResourceId;

            set
            {
                if (_ResourceId != value)
                {
                    if (_Resource.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnResourceIdChanging(value);
                    SendPropertyChanging();
                    _ResourceId = value;
                    SendPropertyChanged("ResourceId");
                    OnResourceIdChanged();
                }
            }
        }

        [Column(Name = "OrganizationTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationTypeId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int OrganizationTypeId
        {
            get => _OrganizationTypeId;

            set
            {
                if (_OrganizationTypeId != value)
                {
                    if (_OrganizationType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOrganizationTypeIdChanging(value);
                    SendPropertyChanging();
                    _OrganizationTypeId = value;
                    SendPropertyChanged("OrganizationTypeId");
                    OnOrganizationTypeIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_ResourceOrganizationType_OrganizationType", Storage = "_OrganizationType", ThisKey = "OrganizationTypeId", IsForeignKey = true)]
        public OrganizationType OrganizationType
        {
            get => _OrganizationType.Entity;

            set
            {
                OrganizationType previousValue = _OrganizationType.Entity;
                if (((previousValue != value)
                            || (_OrganizationType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _OrganizationType.Entity = null;
                        previousValue.ResourceOrganizationTypes.Remove(this);
                    }

                    _OrganizationType.Entity = value;
                    if (value != null)
                    {
                        value.ResourceOrganizationTypes.Add(this);

                        _OrganizationTypeId = value.Id;

                    }

                    else
                    {
                        _OrganizationTypeId = default(int);

                    }

                    SendPropertyChanged("OrganizationType");
                }
            }
        }

        [Association(Name = "FK_ResourceOrganizationType_Resource", Storage = "_Resource", ThisKey = "ResourceId", IsForeignKey = true)]
        public Resource Resource
        {
            get => _Resource.Entity;

            set
            {
                Resource previousValue = _Resource.Entity;
                if (((previousValue != value)
                            || (_Resource.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Resource.Entity = null;
                        previousValue.ResourceOrganizationTypes.Remove(this);
                    }

                    _Resource.Entity = value;
                    if (value != null)
                    {
                        value.ResourceOrganizationTypes.Add(this);

                        _ResourceId = value.ResourceId;

                    }

                    else
                    {
                        _ResourceId = default(int);

                    }

                    SendPropertyChanged("Resource");
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
