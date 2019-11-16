using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ResourceOrganization")]
    public partial class ResourceOrganization : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ResourceId;

        private int _OrganizationId;

        private EntityRef<Organization> _Organization;

        private EntityRef<Resource> _Resource;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnResourceIdChanging(int value);
        partial void OnResourceIdChanged();

        partial void OnOrganizationIdChanging(int value);
        partial void OnOrganizationIdChanged();

        #endregion

        public ResourceOrganization()
        {
            _Organization = default(EntityRef<Organization>);

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

        [Column(Name = "OrganizationId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    if (_Organization.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOrganizationIdChanging(value);
                    SendPropertyChanging();
                    _OrganizationId = value;
                    SendPropertyChanged("OrganizationId");
                    OnOrganizationIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_ResourceOrganization_Organizations", Storage = "_Organization", ThisKey = "OrganizationId", IsForeignKey = true)]
        public Organization Organization
        {
            get => _Organization.Entity;

            set
            {
                Organization previousValue = _Organization.Entity;
                if (((previousValue != value)
                            || (_Organization.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Organization.Entity = null;
                        previousValue.ResourceOrganizations.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.ResourceOrganizations.Add(this);

                        _OrganizationId = value.OrganizationId;

                    }

                    else
                    {
                        _OrganizationId = default(int);

                    }

                    SendPropertyChanged("Organization");
                }
            }
        }

        [Association(Name = "FK_ResourceOrganization_Resource", Storage = "_Resource", ThisKey = "ResourceId", IsForeignKey = true)]
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
                        previousValue.ResourceOrganizations.Remove(this);
                    }

                    _Resource.Entity = value;
                    if (value != null)
                    {
                        value.ResourceOrganizations.Add(this);

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
