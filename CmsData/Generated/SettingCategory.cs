using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name="dbo.SettingCategory")]
	public partial class SettingCategory : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	    #region Private Fields
		
		private int _SettingCategoryId;
		
		private string _Name;
		
		private int _SettingTypeId;
		
		private int _DisplayOrder;
   		
   		private EntitySet<Setting> _Settings;
    	
		private EntityRef<SettingType> _SettingType;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();

        partial void OnValidate(System.Data.Linq.ChangeAction action);

        partial void OnCreated();
		
		partial void OnSettingCategoryIdChanging(int value);

        partial void OnSettingCategoryIdChanged();
		
		partial void OnNameChanging(string value);

        partial void OnNameChanged();
		
		partial void OnSettingTypeIdChanging(int value);

        partial void OnSettingTypeIdChanged();

		partial void OnDisplayOrderChanging(int value);

        partial void OnDisplayOrderChanged();

        #endregion

        public SettingCategory()
		{
			this._Settings = new EntitySet<Setting>(new Action< Setting>(this.attach_Settings), new Action< Setting>(this.detach_Settings)); 
			
			this._SettingType = default(EntityRef<SettingType>); 
			
			OnCreated();
		}
		
        #region Columns
		
		[Column(Name="SettingCategoryId", UpdateCheck=UpdateCheck.Never, Storage="_SettingCategoryId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int SettingCategoryId
		{
			get { return this._SettingCategoryId; }

			set
			{
				if (this._SettingCategoryId != value)
				{
                    this.OnSettingCategoryIdChanging(value);
					this.SendPropertyChanging();
					this._SettingCategoryId = value;
					this.SendPropertyChanged("SettingCategoryId");
					this.OnSettingCategoryIdChanged();
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

		[Column(Name="SettingTypeId", UpdateCheck=UpdateCheck.Never, Storage="_SettingTypeId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int SettingTypeId
		{
			get { return this._SettingTypeId; }

			set
			{
				if (this._SettingTypeId != value)
				{
					if (this._SettingType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnSettingTypeIdChanging(value);
					this.SendPropertyChanging();
					this._SettingTypeId = value;
					this.SendPropertyChanged("SettingTypeId");
					this.OnSettingTypeIdChanged();
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
   		
   		[Association(Name="FK_Setting_SettingCategory", Storage="_Settings", OtherKey="SettingCategoryId")]
   		public EntitySet<Setting> Settings
   		{
   		    get { return this._Settings; }

			set	{ this._Settings.Assign(value); }
   		}

	    #endregion
	
	    #region Foreign Keys
    	
		[Association(Name="FK_SettingCategory_SettingType", Storage="_SettingType", ThisKey="SettingTypeId", IsForeignKey=true)]
		public SettingType SettingType
		{
			get { return this._SettingType.Entity; }

			set
			{
				SettingType previousValue = this._SettingType.Entity;
				if (((previousValue != value) 
							|| (this._SettingType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._SettingType.Entity = null;
						previousValue.SettingCategories.Remove(this);
					}

					this._SettingType.Entity = value;
					if (value != null)
					{
						value.SettingCategories.Add(this);
						
						this._SettingTypeId = value.SettingTypeId;
					}

					else
					{
						this._SettingTypeId = default(int);
					}

					this.SendPropertyChanged("SettingType");
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
   		
		private void attach_Settings(Setting entity)
		{
			this.SendPropertyChanging();
			entity.SettingCategory = this;
		}

		private void detach_Settings(Setting entity)
		{
			this.SendPropertyChanging();
			entity.SettingCategory = null;
		}
	}
}
