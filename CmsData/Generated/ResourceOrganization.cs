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
	[Table(Name="dbo.ResourceOrganization")]
	public partial class ResourceOrganization : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
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
			
			
			this._Organization = default(EntityRef<Organization>); 
			
			this._Resource = default(EntityRef<Resource>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ResourceId", UpdateCheck=UpdateCheck.Never, Storage="_ResourceId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int ResourceId
		{
			get { return this._ResourceId; }

			set
			{
				if (this._ResourceId != value)
				{
				
					if (this._Resource.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnResourceIdChanging(value);
					this.SendPropertyChanging();
					this._ResourceId = value;
					this.SendPropertyChanged("ResourceId");
					this.OnResourceIdChanged();
				}

			}

		}

		
		[Column(Name="OrganizationId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int OrganizationId
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

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_ResourceOrganization_Organizations", Storage="_Organization", ThisKey="OrganizationId", IsForeignKey=true)]
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
						previousValue.ResourceOrganizations.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.ResourceOrganizations.Add(this);
						
						this._OrganizationId = value.OrganizationId;
						
					}

					else
					{
						
						this._OrganizationId = default(int);
						
					}

					this.SendPropertyChanged("Organization");
				}

			}

		}

		
		[Association(Name="FK_ResourceOrganization_Resource", Storage="_Resource", ThisKey="ResourceId", IsForeignKey=true)]
		public Resource Resource
		{
			get { return this._Resource.Entity; }

			set
			{
				Resource previousValue = this._Resource.Entity;
				if (((previousValue != value) 
							|| (this._Resource.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Resource.Entity = null;
						previousValue.ResourceOrganizations.Remove(this);
					}

					this._Resource.Entity = value;
					if (value != null)
					{
						value.ResourceOrganizations.Add(this);
						
						this._ResourceId = value.ResourceId;
						
					}

					else
					{
						
						this._ResourceId = default(int);
						
					}

					this.SendPropertyChanged("Resource");
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

   		
	}

}

