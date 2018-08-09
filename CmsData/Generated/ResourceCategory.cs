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
	[Table(Name="dbo.ResourceCategory")]
	public partial class ResourceCategory : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
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
			
			this._Resources = new EntitySet<Resource>(new Action< Resource>(this.attach_Resources), new Action< Resource>(this.detach_Resources)); 
			
			
			this._ResourceType = default(EntityRef<ResourceType>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ResourceCategoryId", UpdateCheck=UpdateCheck.Never, Storage="_ResourceCategoryId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ResourceCategoryId
		{
			get { return this._ResourceCategoryId; }

			set
			{
				if (this._ResourceCategoryId != value)
				{
				
                    this.OnResourceCategoryIdChanging(value);
					this.SendPropertyChanging();
					this._ResourceCategoryId = value;
					this.SendPropertyChanged("ResourceCategoryId");
					this.OnResourceCategoryIdChanged();
				}

			}

		}

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(50) NOT NULL")]
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

		
		[Column(Name="DisplayOrder", UpdateCheck=UpdateCheck.Never, Storage="_DisplayOrder", DbType="int NOT NULL")]
		public int DisplayOrder
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

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_Resource_ResourceCategory", Storage="_Resources", OtherKey="ResourceCategoryId")]
   		public EntitySet<Resource> Resources
   		{
   		    get { return this._Resources; }

			set	{ this._Resources.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_ResourceCategory_ResourceType", Storage="_ResourceType", ThisKey="ResourceTypeId", IsForeignKey=true)]
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
						previousValue.ResourceCategories.Remove(this);
					}

					this._ResourceType.Entity = value;
					if (value != null)
					{
						value.ResourceCategories.Add(this);
						
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

   		
		private void attach_Resources(Resource entity)
		{
			this.SendPropertyChanging();
			entity.ResourceCategory = this;
		}

		private void detach_Resources(Resource entity)
		{
			this.SendPropertyChanging();
			entity.ResourceCategory = null;
		}

		
	}

}

