using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name="dbo.SettingMetadata")]
	public partial class SettingMetadatum : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	    #region Private Fields
		
		private string _SettingId;
		
		private string _DisplayName;
		
		private string _Description;
		
		private int? _DataType;
		
		private int? _SettingTypeId;
		
		private int? _SettingCategoryId;
    	
		private EntityRef<Setting> _Setting;
		
		private EntityRef<SettingCategory> _SettingCategory;
		
		private EntityRef<SettingType> _SettingType;
		
	    #endregion
	
        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
		
		partial void OnSettingIdChanging(string value);
		partial void OnSettingIdChanged();
		
		partial void OnDisplayNameChanging(string value);
		partial void OnDisplayNameChanged();
		
		partial void OnDescriptionChanging(string value);
		partial void OnDescriptionChanged();
		
		partial void OnDataTypeChanging(int? value);
		partial void OnDataTypeChanged();
		
		partial void OnSettingTypeIdChanging(int? value);
		partial void OnSettingTypeIdChanged();
		
		partial void OnSettingCategoryIdChanging(int? value);
		partial void OnSettingCategoryIdChanged();
		
        #endregion

		public SettingMetadatum()
		{
			this._Setting = default(EntityRef<Setting>); 
			
			this._SettingCategory = default(EntityRef<SettingCategory>); 
			
			this._SettingType = default(EntityRef<SettingType>); 
			
			OnCreated();
		}
		
        #region Columns
		
		[Column(Name="SettingId", UpdateCheck=UpdateCheck.Never, Storage="_SettingId", DbType="nvarchar(50) NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public string SettingId
		{
			get { return this._SettingId; }

			set
			{
				if (this._SettingId != value)
				{
					if (this._Setting.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnSettingIdChanging(value);
					this.SendPropertyChanging();
					this._SettingId = value;
					this.SendPropertyChanged("SettingId");
					this.OnSettingIdChanged();
				}
			}
		}
		
		[Column(Name="DisplayName", UpdateCheck=UpdateCheck.Never, Storage="_DisplayName", DbType="varchar")]
		public string DisplayName
		{
			get { return this._DisplayName; }

			set
			{
				if (this._DisplayName != value)
				{
                    this.OnDisplayNameChanging(value);
					this.SendPropertyChanging();
					this._DisplayName = value;
					this.SendPropertyChanged("DisplayName");
					this.OnDisplayNameChanged();
				}
			}
		}
		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="varchar")]
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
		
		[Column(Name="DataType", UpdateCheck=UpdateCheck.Never, Storage="_DataType", DbType="int")]
		public int? DataType
		{
			get { return this._DataType; }

			set
			{
				if (this._DataType != value)
				{
                    this.OnDataTypeChanging(value);
					this.SendPropertyChanging();
					this._DataType = value;
					this.SendPropertyChanged("DataType");
					this.OnDataTypeChanged();
				}
			}
		}
		
		[Column(Name="SettingTypeId", UpdateCheck=UpdateCheck.Never, Storage="_SettingTypeId", DbType="int")]
		[IsForeignKey]
		public int? SettingTypeId
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
		
		[Column(Name="SettingCategoryId", UpdateCheck=UpdateCheck.Never, Storage="_SettingCategoryId", DbType="int")]
		[IsForeignKey]
		public int? SettingCategoryId
		{
			get { return this._SettingCategoryId; }

			set
			{
				if (this._SettingCategoryId != value)
				{
					if (this._SettingCategory.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnSettingCategoryIdChanging(value);
					this.SendPropertyChanging();
					this._SettingCategoryId = value;
					this.SendPropertyChanged("SettingCategoryId");
					this.OnSettingCategoryIdChanged();
				}
			}
		}

        #endregion
            
        #region Foreign Key Tables
   		    
	    #endregion
	    
	    #region Foreign Keys
    	
		[Association(Name="FK_SettingMetadata_Setting", Storage="_Setting", ThisKey="SettingId", IsForeignKey=true)]
		public Setting Setting
		{
			get { return this._Setting.Entity; }

			set
			{
				Setting previousValue = this._Setting.Entity;
				if (((previousValue != value) 
							|| (this._Setting.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Setting.Entity = null;
						previousValue.SettingMetadatas.Remove(this);
					}

					this._Setting.Entity = value;
					if (value != null)
					{
						value.SettingMetadatas.Add(this);
						
						this._SettingId = value.Id;
					}

					else
					{
						this._SettingId = default(string);
					}

					this.SendPropertyChanged("Setting");
				}
			}
		}
		
		[Association(Name="FK_SettingMetadata_SettingCategory", Storage="_SettingCategory", ThisKey="SettingCategoryId", IsForeignKey=true)]
		public SettingCategory SettingCategory
		{
			get { return this._SettingCategory.Entity; }

			set
			{
				SettingCategory previousValue = this._SettingCategory.Entity;
				if (((previousValue != value) 
							|| (this._SettingCategory.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._SettingCategory.Entity = null;
						previousValue.SettingMetadatas.Remove(this);
					}

					this._SettingCategory.Entity = value;
					if (value != null)
					{
						value.SettingMetadatas.Add(this);
						
						this._SettingCategoryId = value.SettingCategoryId;
					}

					else
					{
						this._SettingCategoryId = default(int?);
					}

					this.SendPropertyChanged("SettingCategory");
				}
			}
		}
		
		[Association(Name="FK_SettingMetadata_SettingType", Storage="_SettingType", ThisKey="SettingTypeId", IsForeignKey=true)]
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
						previousValue.SettingMetadatas.Remove(this);
					}

					this._SettingType.Entity = value;
					if (value != null)
					{
						value.SettingMetadatas.Add(this);
						
						this._SettingTypeId = value.SettingTypeId;
					}

					else
					{
						this._SettingTypeId = default(int?);
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
	}
}
