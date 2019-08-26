using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name="dbo.SettingType")]
	public partial class SettingType : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	    #region Private Fields
		
		private int _SettingTypeId;
		
		private string _Name;
		
		private int _DisplayOrder;
   		
   		private EntitySet<SettingCategory> _SettingCategories;
		
   		private EntitySet<SettingMetadatum> _SettingMetadatas;
    	
    	#endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
		    
		partial void OnSettingTypeIdChanging(int value);
		partial void OnSettingTypeIdChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnDisplayOrderChanging(int value);
		partial void OnDisplayOrderChanged();
		
        #endregion

		public SettingType()
		{
			
			this._SettingCategories = new EntitySet<SettingCategory>(new Action< SettingCategory>(this.attach_SettingCategories), new Action< SettingCategory>(this.detach_SettingCategories)); 
			
			this._SettingMetadatas = new EntitySet<SettingMetadatum>(new Action< SettingMetadatum>(this.attach_SettingMetadatas), new Action< SettingMetadatum>(this.detach_SettingMetadatas)); 
			
			OnCreated();
		}
		
        #region Columns
		
		[Column(Name="SettingTypeId", UpdateCheck=UpdateCheck.Never, Storage="_SettingTypeId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int SettingTypeId
		{
			get { return this._SettingTypeId; }

			set
			{
				if (this._SettingTypeId != value)
				{
                    this.OnSettingTypeIdChanging(value);
					this.SendPropertyChanging();
					this._SettingTypeId = value;
					this.SendPropertyChanged("SettingTypeId");
					this.OnSettingTypeIdChanged();
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
   		
   		[Association(Name="FK_SettingCategory_SettingType", Storage="_SettingCategories", OtherKey="SettingTypeId")]
   		public EntitySet<SettingCategory> SettingCategories
   		{
   		    get { return this._SettingCategories; }

			set	{ this._SettingCategories.Assign(value); }
   		}
		
   		[Association(Name="FK_SettingMetadata_SettingType", Storage="_SettingMetadatas", OtherKey="SettingTypeId")]
   		public EntitySet<SettingMetadatum> SettingMetadatas
   		{
   		    get { return this._SettingMetadatas; }

			set	{ this._SettingMetadatas.Assign(value); }
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
   		
		private void attach_SettingCategories(SettingCategory entity)
		{
			this.SendPropertyChanging();
			entity.SettingType = this;
		}

		private void detach_SettingCategories(SettingCategory entity)
		{
			this.SendPropertyChanging();
			entity.SettingType = null;
		}
		
		private void attach_SettingMetadatas(SettingMetadatum entity)
		{
			this.SendPropertyChanging();
			entity.SettingType = this;
		}

		private void detach_SettingMetadatas(SettingMetadatum entity)
		{
			this.SendPropertyChanging();
			entity.SettingType = null;
		}
	}
}
