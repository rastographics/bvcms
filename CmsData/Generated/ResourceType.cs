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
	[Table(Name="dbo.ResourceType")]
	public partial class ResourceType : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
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
			
			this._Resources = new EntitySet<Resource>(new Action< Resource>(this.attach_Resources), new Action< Resource>(this.detach_Resources)); 
			
			this._ResourceCategories = new EntitySet<ResourceCategory>(new Action< ResourceCategory>(this.attach_ResourceCategories), new Action< ResourceCategory>(this.detach_ResourceCategories)); 
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ResourceTypeId", UpdateCheck=UpdateCheck.Never, Storage="_ResourceTypeId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ResourceTypeId
		{
			get { return this._ResourceTypeId; }

			set
			{
				if (this._ResourceTypeId != value)
				{
				
                    this.OnResourceTypeIdChanging(value);
					this.SendPropertyChanging();
					this._ResourceTypeId = value;
					this.SendPropertyChanged("ResourceTypeId");
					this.OnResourceTypeIdChanged();
				}

			}

		}

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="varchar(50) NOT NULL")]
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
   		
   		[Association(Name="FK_Resource_ResourceType", Storage="_Resources", OtherKey="ResourceTypeId")]
   		public EntitySet<Resource> Resources
   		{
   		    get { return this._Resources; }

			set	{ this._Resources.Assign(value); }

   		}

		
   		[Association(Name="FK_ResourceCategory_ResourceType", Storage="_ResourceCategories", OtherKey="ResourceTypeId")]
   		public EntitySet<ResourceCategory> ResourceCategories
   		{
   		    get { return this._ResourceCategories; }

			set	{ this._ResourceCategories.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
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
			entity.ResourceType = this;
		}

		private void detach_Resources(Resource entity)
		{
			this.SendPropertyChanging();
			entity.ResourceType = null;
		}

		
		private void attach_ResourceCategories(ResourceCategory entity)
		{
			this.SendPropertyChanging();
			entity.ResourceType = this;
		}

		private void detach_ResourceCategories(ResourceCategory entity)
		{
			this.SendPropertyChanging();
			entity.ResourceType = null;
		}

		
	}

}

