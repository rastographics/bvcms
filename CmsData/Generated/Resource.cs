using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Resource")]
    public partial class Resource : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ResourceId;

        private string _Name;

        private string _Description;

        private DateTime? _CreationDate;

        private DateTime? _UpdateDate;

        private int? _PeopleId;

        private int? _OrganizationId;

        private int? _CampusId;

        private int? _DivisionId;

        private string _MemberTypeIds;

        private int? _DisplayOrder;

        private int _ResourceTypeId;

        private int _ResourceCategoryId;

        private int? _OrganizationTypeId;

        private string _StatusFlagIds;

        private EntitySet<ResourceAttachment> _ResourceAttachments;

        private EntitySet<ResourceOrganization> _ResourceOrganizations;

        private EntitySet<ResourceOrganizationType> _ResourceOrganizationTypes;

        private EntityRef<Campu> _Campu;

        private EntityRef<Division> _Division;

        private EntityRef<Organization> _Organization;

        private EntityRef<OrganizationType> _OrganizationType;

        private EntityRef<ResourceCategory> _ResourceCategory;

        private EntityRef<ResourceType> _ResourceType;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnResourceIdChanging(int value);
        partial void OnResourceIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnCreationDateChanging(DateTime? value);
        partial void OnCreationDateChanged();

        partial void OnUpdateDateChanging(DateTime? value);
        partial void OnUpdateDateChanged();

        partial void OnPeopleIdChanging(int? value);
        partial void OnPeopleIdChanged();

        partial void OnOrganizationIdChanging(int? value);
        partial void OnOrganizationIdChanged();

        partial void OnCampusIdChanging(int? value);
        partial void OnCampusIdChanged();

        partial void OnDivisionIdChanging(int? value);
        partial void OnDivisionIdChanged();

        partial void OnMemberTypeIdsChanging(string value);
        partial void OnMemberTypeIdsChanged();

        partial void OnDisplayOrderChanging(int? value);
        partial void OnDisplayOrderChanged();

        partial void OnResourceTypeIdChanging(int value);
        partial void OnResourceTypeIdChanged();

        partial void OnResourceCategoryIdChanging(int value);
        partial void OnResourceCategoryIdChanged();

        partial void OnOrganizationTypeIdChanging(int? value);
        partial void OnOrganizationTypeIdChanged();

        partial void OnStatusFlagIdsChanging(string value);
        partial void OnStatusFlagIdsChanged();

        #endregion

        public Resource()
        {
            _ResourceAttachments = new EntitySet<ResourceAttachment>(new Action<ResourceAttachment>(attach_ResourceAttachments), new Action<ResourceAttachment>(detach_ResourceAttachments));

            _ResourceOrganizations = new EntitySet<ResourceOrganization>(new Action<ResourceOrganization>(attach_ResourceOrganizations), new Action<ResourceOrganization>(detach_ResourceOrganizations));

            _ResourceOrganizationTypes = new EntitySet<ResourceOrganizationType>(new Action<ResourceOrganizationType>(attach_ResourceOrganizationTypes), new Action<ResourceOrganizationType>(detach_ResourceOrganizationTypes));

            _Campu = default(EntityRef<Campu>);

            _Division = default(EntityRef<Division>);

            _Organization = default(EntityRef<Organization>);

            _OrganizationType = default(EntityRef<OrganizationType>);

            _ResourceCategory = default(EntityRef<ResourceCategory>);

            _ResourceType = default(EntityRef<ResourceType>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ResourceId", UpdateCheck = UpdateCheck.Never, Storage = "_ResourceId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ResourceId
        {
            get => _ResourceId;

            set
            {
                if (_ResourceId != value)
                {
                    OnResourceIdChanging(value);
                    SendPropertyChanging();
                    _ResourceId = value;
                    SendPropertyChanged("ResourceId");
                    OnResourceIdChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(100)")]
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

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    OnDescriptionChanging(value);
                    SendPropertyChanging();
                    _Description = value;
                    SendPropertyChanged("Description");
                    OnDescriptionChanged();
                }
            }
        }

        [Column(Name = "CreationDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreationDate", DbType = "datetime")]
        public DateTime? CreationDate
        {
            get => _CreationDate;

            set
            {
                if (_CreationDate != value)
                {
                    OnCreationDateChanging(value);
                    SendPropertyChanging();
                    _CreationDate = value;
                    SendPropertyChanged("CreationDate");
                    OnCreationDateChanged();
                }
            }
        }

        [Column(Name = "UpdateDate", UpdateCheck = UpdateCheck.Never, Storage = "_UpdateDate", DbType = "datetime")]
        public DateTime? UpdateDate
        {
            get => _UpdateDate;

            set
            {
                if (_UpdateDate != value)
                {
                    OnUpdateDateChanging(value);
                    SendPropertyChanging();
                    _UpdateDate = value;
                    SendPropertyChanged("UpdateDate");
                    OnUpdateDateChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "OrganizationId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationId", DbType = "int")]
        [IsForeignKey]
        public int? OrganizationId
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

        [Column(Name = "CampusId", UpdateCheck = UpdateCheck.Never, Storage = "_CampusId", DbType = "int")]
        [IsForeignKey]
        public int? CampusId
        {
            get => _CampusId;

            set
            {
                if (_CampusId != value)
                {
                    if (_Campu.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnCampusIdChanging(value);
                    SendPropertyChanging();
                    _CampusId = value;
                    SendPropertyChanged("CampusId");
                    OnCampusIdChanged();
                }
            }
        }

        [Column(Name = "DivisionId", UpdateCheck = UpdateCheck.Never, Storage = "_DivisionId", DbType = "int")]
        [IsForeignKey]
        public int? DivisionId
        {
            get => _DivisionId;

            set
            {
                if (_DivisionId != value)
                {
                    if (_Division.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnDivisionIdChanging(value);
                    SendPropertyChanging();
                    _DivisionId = value;
                    SendPropertyChanged("DivisionId");
                    OnDivisionIdChanged();
                }
            }
        }

        [Column(Name = "MemberTypeIds", UpdateCheck = UpdateCheck.Never, Storage = "_MemberTypeIds", DbType = "nvarchar(50)")]
        public string MemberTypeIds
        {
            get => _MemberTypeIds;

            set
            {
                if (_MemberTypeIds != value)
                {
                    OnMemberTypeIdsChanging(value);
                    SendPropertyChanging();
                    _MemberTypeIds = value;
                    SendPropertyChanged("MemberTypeIds");
                    OnMemberTypeIdsChanged();
                }
            }
        }

        [Column(Name = "DisplayOrder", UpdateCheck = UpdateCheck.Never, Storage = "_DisplayOrder", DbType = "int")]
        public int? DisplayOrder
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

        [Column(Name = "ResourceCategoryId", UpdateCheck = UpdateCheck.Never, Storage = "_ResourceCategoryId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int ResourceCategoryId
        {
            get => _ResourceCategoryId;

            set
            {
                if (_ResourceCategoryId != value)
                {
                    if (_ResourceCategory.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnResourceCategoryIdChanging(value);
                    SendPropertyChanging();
                    _ResourceCategoryId = value;
                    SendPropertyChanged("ResourceCategoryId");
                    OnResourceCategoryIdChanged();
                }
            }
        }

        [Column(Name = "OrganizationTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationTypeId", DbType = "int")]
        [IsForeignKey]
        public int? OrganizationTypeId
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

        [Column(Name = "StatusFlagIds", UpdateCheck = UpdateCheck.Never, Storage = "_StatusFlagIds", DbType = "nvarchar")]
        public string StatusFlagIds
        {
            get => _StatusFlagIds;

            set
            {
                if (_StatusFlagIds != value)
                {
                    OnStatusFlagIdsChanging(value);
                    SendPropertyChanging();
                    _StatusFlagIds = value;
                    SendPropertyChanged("StatusFlagIds");
                    OnStatusFlagIdsChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_ResourceAttachment_Resource", Storage = "_ResourceAttachments", OtherKey = "ResourceId")]
        public EntitySet<ResourceAttachment> ResourceAttachments
           {
               get => _ResourceAttachments;

            set => _ResourceAttachments.Assign(value);

           }

        [Association(Name = "FK_ResourceOrganization_Resource", Storage = "_ResourceOrganizations", OtherKey = "ResourceId")]
        public EntitySet<ResourceOrganization> ResourceOrganizations
           {
               get => _ResourceOrganizations;

            set => _ResourceOrganizations.Assign(value);

           }

        [Association(Name = "FK_ResourceOrganizationType_Resource", Storage = "_ResourceOrganizationTypes", OtherKey = "ResourceId")]
        public EntitySet<ResourceOrganizationType> ResourceOrganizationTypes
           {
               get => _ResourceOrganizationTypes;

            set => _ResourceOrganizationTypes.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Resource_Campus", Storage = "_Campu", ThisKey = "CampusId", IsForeignKey = true)]
        public Campu Campu
        {
            get => _Campu.Entity;

            set
            {
                Campu previousValue = _Campu.Entity;
                if (((previousValue != value)
                            || (_Campu.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Campu.Entity = null;
                        previousValue.Resources.Remove(this);
                    }

                    _Campu.Entity = value;
                    if (value != null)
                    {
                        value.Resources.Add(this);

                        _CampusId = value.Id;

                    }

                    else
                    {
                        _CampusId = default(int?);

                    }

                    SendPropertyChanged("Campu");
                }
            }
        }

        [Association(Name = "FK_Resource_Division", Storage = "_Division", ThisKey = "DivisionId", IsForeignKey = true)]
        public Division Division
        {
            get => _Division.Entity;

            set
            {
                Division previousValue = _Division.Entity;
                if (((previousValue != value)
                            || (_Division.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Division.Entity = null;
                        previousValue.Resources.Remove(this);
                    }

                    _Division.Entity = value;
                    if (value != null)
                    {
                        value.Resources.Add(this);

                        _DivisionId = value.Id;

                    }

                    else
                    {
                        _DivisionId = default(int?);

                    }

                    SendPropertyChanged("Division");
                }
            }
        }

        [Association(Name = "FK_Resource_Organization", Storage = "_Organization", ThisKey = "OrganizationId", IsForeignKey = true)]
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
                        previousValue.Resources.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.Resources.Add(this);

                        _OrganizationId = value.OrganizationId;

                    }

                    else
                    {
                        _OrganizationId = default(int?);

                    }

                    SendPropertyChanged("Organization");
                }
            }
        }

        [Association(Name = "FK_Resource_OrganizationType", Storage = "_OrganizationType", ThisKey = "OrganizationTypeId", IsForeignKey = true)]
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
                        previousValue.Resources.Remove(this);
                    }

                    _OrganizationType.Entity = value;
                    if (value != null)
                    {
                        value.Resources.Add(this);

                        _OrganizationTypeId = value.Id;

                    }

                    else
                    {
                        _OrganizationTypeId = default(int?);

                    }

                    SendPropertyChanged("OrganizationType");
                }
            }
        }

        [Association(Name = "FK_Resource_ResourceCategory", Storage = "_ResourceCategory", ThisKey = "ResourceCategoryId", IsForeignKey = true)]
        public ResourceCategory ResourceCategory
        {
            get => _ResourceCategory.Entity;

            set
            {
                ResourceCategory previousValue = _ResourceCategory.Entity;
                if (((previousValue != value)
                            || (_ResourceCategory.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ResourceCategory.Entity = null;
                        previousValue.Resources.Remove(this);
                    }

                    _ResourceCategory.Entity = value;
                    if (value != null)
                    {
                        value.Resources.Add(this);

                        _ResourceCategoryId = value.ResourceCategoryId;

                    }

                    else
                    {
                        _ResourceCategoryId = default(int);

                    }

                    SendPropertyChanged("ResourceCategory");
                }
            }
        }

        [Association(Name = "FK_Resource_ResourceType", Storage = "_ResourceType", ThisKey = "ResourceTypeId", IsForeignKey = true)]
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
                        previousValue.Resources.Remove(this);
                    }

                    _ResourceType.Entity = value;
                    if (value != null)
                    {
                        value.Resources.Add(this);

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

        private void attach_ResourceAttachments(ResourceAttachment entity)
        {
            SendPropertyChanging();
            entity.Resource = this;
        }

        private void detach_ResourceAttachments(ResourceAttachment entity)
        {
            SendPropertyChanging();
            entity.Resource = null;
        }

        private void attach_ResourceOrganizations(ResourceOrganization entity)
        {
            SendPropertyChanging();
            entity.Resource = this;
        }

        private void detach_ResourceOrganizations(ResourceOrganization entity)
        {
            SendPropertyChanging();
            entity.Resource = null;
        }

        private void attach_ResourceOrganizationTypes(ResourceOrganizationType entity)
        {
            SendPropertyChanging();
            entity.Resource = this;
        }

        private void detach_ResourceOrganizationTypes(ResourceOrganizationType entity)
        {
            SendPropertyChanging();
            entity.Resource = null;
        }
    }
}
