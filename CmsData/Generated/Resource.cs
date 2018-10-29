using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using CmsData.Infrastructure;

namespace CmsData
{
	[Table(Name="dbo.Resource")]
	public partial class Resource : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
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
			
			this._ResourceAttachments = new EntitySet<ResourceAttachment>(new Action< ResourceAttachment>(this.attach_ResourceAttachments), new Action< ResourceAttachment>(this.detach_ResourceAttachments)); 
			
			this._ResourceOrganizations = new EntitySet<ResourceOrganization>(new Action< ResourceOrganization>(this.attach_ResourceOrganizations), new Action< ResourceOrganization>(this.detach_ResourceOrganizations)); 
			
			this._ResourceOrganizationTypes = new EntitySet<ResourceOrganizationType>(new Action< ResourceOrganizationType>(this.attach_ResourceOrganizationTypes), new Action< ResourceOrganizationType>(this.detach_ResourceOrganizationTypes)); 
			
			
			this._Campu = default(EntityRef<Campu>); 
			
			this._Division = default(EntityRef<Division>); 
			
			this._Organization = default(EntityRef<Organization>); 
			
			this._OrganizationType = default(EntityRef<OrganizationType>); 
			
			this._ResourceCategory = default(EntityRef<ResourceCategory>); 
			
			this._ResourceType = default(EntityRef<ResourceType>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ResourceId", UpdateCheck=UpdateCheck.Never, Storage="_ResourceId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ResourceId
		{
			get { return this._ResourceId; }

			set
			{
				if (this._ResourceId != value)
				{
				
                    this.OnResourceIdChanging(value);
					this.SendPropertyChanging();
					this._ResourceId = value;
					this.SendPropertyChanged("ResourceId");
					this.OnResourceIdChanged();
				}

			}

		}

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(100)")]
		public string Name
		{
			get { return this._Name; }

			set
			{
				if (this._Name != value)
				{
				
                    this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}

			}

		}

		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar")]
		public string Description
		{
			get { return this._Description; }

			set
			{
				if (this._Description != value)
				{
				
                    this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}

			}

		}

		
		[Column(Name="CreationDate", UpdateCheck=UpdateCheck.Never, Storage="_CreationDate", DbType="datetime")]
		public DateTime? CreationDate
		{
			get { return this._CreationDate; }

			set
			{
				if (this._CreationDate != value)
				{
				
                    this.OnCreationDateChanging(value);
					this.SendPropertyChanging();
					this._CreationDate = value;
					this.SendPropertyChanged("CreationDate");
					this.OnCreationDateChanged();
				}

			}

		}

		
		[Column(Name="UpdateDate", UpdateCheck=UpdateCheck.Never, Storage="_UpdateDate", DbType="datetime")]
		public DateTime? UpdateDate
		{
			get { return this._UpdateDate; }

			set
			{
				if (this._UpdateDate != value)
				{
				
                    this.OnUpdateDateChanging(value);
					this.SendPropertyChanging();
					this._UpdateDate = value;
					this.SendPropertyChanged("UpdateDate");
					this.OnUpdateDateChanged();
				}

			}

		}

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int")]
		public int? PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="OrganizationId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationId", DbType="int")]
		[IsForeignKey]
		public int? OrganizationId
		{
			get { return this._OrganizationId; }

			set
			{
				if (this._OrganizationId != value)
				{
				
					if (this._Organization.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrganizationIdChanging(value);
					this.SendPropertyChanging();
					this._OrganizationId = value;
					this.SendPropertyChanged("OrganizationId");
					this.OnOrganizationIdChanged();
				}

			}

		}

		
		[Column(Name="CampusId", UpdateCheck=UpdateCheck.Never, Storage="_CampusId", DbType="int")]
		[IsForeignKey]
		public int? CampusId
		{
			get { return this._CampusId; }

			set
			{
				if (this._CampusId != value)
				{
				
					if (this._Campu.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnCampusIdChanging(value);
					this.SendPropertyChanging();
					this._CampusId = value;
					this.SendPropertyChanged("CampusId");
					this.OnCampusIdChanged();
				}

			}

		}

		
		[Column(Name="DivisionId", UpdateCheck=UpdateCheck.Never, Storage="_DivisionId", DbType="int")]
		[IsForeignKey]
		public int? DivisionId
		{
			get { return this._DivisionId; }

			set
			{
				if (this._DivisionId != value)
				{
				
					if (this._Division.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnDivisionIdChanging(value);
					this.SendPropertyChanging();
					this._DivisionId = value;
					this.SendPropertyChanged("DivisionId");
					this.OnDivisionIdChanged();
				}

			}

		}

		
		[Column(Name="MemberTypeIds", UpdateCheck=UpdateCheck.Never, Storage="_MemberTypeIds", DbType="nvarchar(50)")]
		public string MemberTypeIds
		{
			get { return this._MemberTypeIds; }

			set
			{
				if (this._MemberTypeIds != value)
				{
				
                    this.OnMemberTypeIdsChanging(value);
					this.SendPropertyChanging();
					this._MemberTypeIds = value;
					this.SendPropertyChanged("MemberTypeIds");
					this.OnMemberTypeIdsChanged();
				}

			}

		}

		
		[Column(Name="DisplayOrder", UpdateCheck=UpdateCheck.Never, Storage="_DisplayOrder", DbType="int")]
		public int? DisplayOrder
		{
			get { return this._DisplayOrder; }

			set
			{
				if (this._DisplayOrder != value)
				{
				
                    this.OnDisplayOrderChanging(value);
					this.SendPropertyChanging();
					this._DisplayOrder = value;
					this.SendPropertyChanged("DisplayOrder");
					this.OnDisplayOrderChanged();
				}

			}

		}

		
		[Column(Name="ResourceTypeId", UpdateCheck=UpdateCheck.Never, Storage="_ResourceTypeId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int ResourceTypeId
		{
			get { return this._ResourceTypeId; }

			set
			{
				if (this._ResourceTypeId != value)
				{
				
					if (this._ResourceType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnResourceTypeIdChanging(value);
					this.SendPropertyChanging();
					this._ResourceTypeId = value;
					this.SendPropertyChanged("ResourceTypeId");
					this.OnResourceTypeIdChanged();
				}

			}

		}

		
		[Column(Name="ResourceCategoryId", UpdateCheck=UpdateCheck.Never, Storage="_ResourceCategoryId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int ResourceCategoryId
		{
			get { return this._ResourceCategoryId; }

			set
			{
				if (this._ResourceCategoryId != value)
				{
				
					if (this._ResourceCategory.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnResourceCategoryIdChanging(value);
					this.SendPropertyChanging();
					this._ResourceCategoryId = value;
					this.SendPropertyChanged("ResourceCategoryId");
					this.OnResourceCategoryIdChanged();
				}

			}

		}

		
		[Column(Name="OrganizationTypeId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationTypeId", DbType="int")]
		[IsForeignKey]
		public int? OrganizationTypeId
		{
			get { return this._OrganizationTypeId; }

			set
			{
				if (this._OrganizationTypeId != value)
				{
				
					if (this._OrganizationType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrganizationTypeIdChanging(value);
					this.SendPropertyChanging();
					this._OrganizationTypeId = value;
					this.SendPropertyChanged("OrganizationTypeId");
					this.OnOrganizationTypeIdChanged();
				}

			}

		}

		
		[Column(Name="StatusFlagIds", UpdateCheck=UpdateCheck.Never, Storage="_StatusFlagIds", DbType="nvarchar")]
		public string StatusFlagIds
		{
			get { return this._StatusFlagIds; }

			set
			{
				if (this._StatusFlagIds != value)
				{
				
                    this.OnStatusFlagIdsChanging(value);
					this.SendPropertyChanging();
					this._StatusFlagIds = value;
					this.SendPropertyChanged("StatusFlagIds");
					this.OnStatusFlagIdsChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_ResourceAttachment_Resource", Storage="_ResourceAttachments", OtherKey="ResourceId")]
   		public EntitySet<ResourceAttachment> ResourceAttachments
   		{
   		    get { return this._ResourceAttachments; }

			set	{ this._ResourceAttachments.Assign(value); }

   		}

		
   		[Association(Name="FK_ResourceOrganization_Resource", Storage="_ResourceOrganizations", OtherKey="ResourceId")]
   		public EntitySet<ResourceOrganization> ResourceOrganizations
   		{
   		    get { return this._ResourceOrganizations; }

			set	{ this._ResourceOrganizations.Assign(value); }

   		}

		
   		[Association(Name="FK_ResourceOrganizationType_Resource", Storage="_ResourceOrganizationTypes", OtherKey="ResourceId")]
   		public EntitySet<ResourceOrganizationType> ResourceOrganizationTypes
   		{
   		    get { return this._ResourceOrganizationTypes; }

			set	{ this._ResourceOrganizationTypes.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_Resource_Campus", Storage="_Campu", ThisKey="CampusId", IsForeignKey=true)]
		public Campu Campu
		{
			get { return this._Campu.Entity; }

			set
			{
				Campu previousValue = this._Campu.Entity;
				if (((previousValue != value) 
							|| (this._Campu.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Campu.Entity = null;
						previousValue.Resources.Remove(this);
					}

					this._Campu.Entity = value;
					if (value != null)
					{
						value.Resources.Add(this);
						
						this._CampusId = value.Id;
						
					}

					else
					{
						
						this._CampusId = default(int?);
						
					}

					this.SendPropertyChanged("Campu");
				}

			}

		}

		
		[Association(Name="FK_Resource_Division", Storage="_Division", ThisKey="DivisionId", IsForeignKey=true)]
		public Division Division
		{
			get { return this._Division.Entity; }

			set
			{
				Division previousValue = this._Division.Entity;
				if (((previousValue != value) 
							|| (this._Division.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Division.Entity = null;
						previousValue.Resources.Remove(this);
					}

					this._Division.Entity = value;
					if (value != null)
					{
						value.Resources.Add(this);
						
						this._DivisionId = value.Id;
						
					}

					else
					{
						
						this._DivisionId = default(int?);
						
					}

					this.SendPropertyChanged("Division");
				}

			}

		}

		
		[Association(Name="FK_Resource_Organization", Storage="_Organization", ThisKey="OrganizationId", IsForeignKey=true)]
		public Organization Organization
		{
			get { return this._Organization.Entity; }

			set
			{
				Organization previousValue = this._Organization.Entity;
				if (((previousValue != value) 
							|| (this._Organization.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Organization.Entity = null;
						previousValue.Resources.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.Resources.Add(this);
						
						this._OrganizationId = value.OrganizationId;
						
					}

					else
					{
						
						this._OrganizationId = default(int?);
						
					}

					this.SendPropertyChanged("Organization");
				}

			}

		}

		
		[Association(Name="FK_Resource_OrganizationType", Storage="_OrganizationType", ThisKey="OrganizationTypeId", IsForeignKey=true)]
		public OrganizationType OrganizationType
		{
			get { return this._OrganizationType.Entity; }

			set
			{
				OrganizationType previousValue = this._OrganizationType.Entity;
				if (((previousValue != value) 
							|| (this._OrganizationType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._OrganizationType.Entity = null;
						previousValue.Resources.Remove(this);
					}

					this._OrganizationType.Entity = value;
					if (value != null)
					{
						value.Resources.Add(this);
						
						this._OrganizationTypeId = value.Id;
						
					}

					else
					{
						
						this._OrganizationTypeId = default(int?);
						
					}

					this.SendPropertyChanged("OrganizationType");
				}

			}

		}

		
		[Association(Name="FK_Resource_ResourceCategory", Storage="_ResourceCategory", ThisKey="ResourceCategoryId", IsForeignKey=true)]
		public ResourceCategory ResourceCategory
		{
			get { return this._ResourceCategory.Entity; }

			set
			{
				ResourceCategory previousValue = this._ResourceCategory.Entity;
				if (((previousValue != value) 
							|| (this._ResourceCategory.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ResourceCategory.Entity = null;
						previousValue.Resources.Remove(this);
					}

					this._ResourceCategory.Entity = value;
					if (value != null)
					{
						value.Resources.Add(this);
						
						this._ResourceCategoryId = value.ResourceCategoryId;
						
					}

					else
					{
						
						this._ResourceCategoryId = default(int);
						
					}

					this.SendPropertyChanged("ResourceCategory");
				}

			}

		}

		
		[Association(Name="FK_Resource_ResourceType", Storage="_ResourceType", ThisKey="ResourceTypeId", IsForeignKey=true)]
		public ResourceType ResourceType
		{
			get { return this._ResourceType.Entity; }

			set
			{
				ResourceType previousValue = this._ResourceType.Entity;
				if (((previousValue != value) 
							|| (this._ResourceType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ResourceType.Entity = null;
						previousValue.Resources.Remove(this);
					}

					this._ResourceType.Entity = value;
					if (value != null)
					{
						value.Resources.Add(this);
						
						this._ResourceTypeId = value.ResourceTypeId;
						
					}

					else
					{
						
						this._ResourceTypeId = default(int);
						
					}

					this.SendPropertyChanged("ResourceType");
				}

			}

		}

		
	#endregion
	
		public event PropertyChangingEventHandler PropertyChanging;
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
				this.PropertyChanging(this, emptyChangingEventArgs);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

   		
		private void attach_ResourceAttachments(ResourceAttachment entity)
		{
			this.SendPropertyChanging();
			entity.Resource = this;
		}

		private void detach_ResourceAttachments(ResourceAttachment entity)
		{
			this.SendPropertyChanging();
			entity.Resource = null;
		}

		
		private void attach_ResourceOrganizations(ResourceOrganization entity)
		{
			this.SendPropertyChanging();
			entity.Resource = this;
		}

		private void detach_ResourceOrganizations(ResourceOrganization entity)
		{
			this.SendPropertyChanging();
			entity.Resource = null;
		}

		
		private void attach_ResourceOrganizationTypes(ResourceOrganizationType entity)
		{
			this.SendPropertyChanging();
			entity.Resource = this;
		}

		private void detach_ResourceOrganizationTypes(ResourceOrganizationType entity)
		{
			this.SendPropertyChanging();
			entity.Resource = null;
		}

		
	}

}

