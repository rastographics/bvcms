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
	[Table(Name="dbo.ResourceOrganizationType")]
	public partial class ResourceOrganizationType : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
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
			
			
			this._OrganizationType = default(EntityRef<OrganizationType>); 
			
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

		
		[Column(Name="OrganizationTypeId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationTypeId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int OrganizationTypeId
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

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_ResourceOrganizationType_OrganizationType", Storage="_OrganizationType", ThisKey="OrganizationTypeId", IsForeignKey=true)]
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
						previousValue.ResourceOrganizationTypes.Remove(this);
					}

					this._OrganizationType.Entity = value;
					if (value != null)
					{
						value.ResourceOrganizationTypes.Add(this);
						
						this._OrganizationTypeId = value.Id;
						
					}

					else
					{
						
						this._OrganizationTypeId = default(int);
						
					}

					this.SendPropertyChanged("OrganizationType");
				}

			}

		}

		
		[Association(Name="FK_ResourceOrganizationType_Resource", Storage="_Resource", ThisKey="ResourceId", IsForeignKey=true)]
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
						previousValue.ResourceOrganizationTypes.Remove(this);
					}

					this._Resource.Entity = value;
					if (value != null)
					{
						value.ResourceOrganizationTypes.Add(this);
						
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

