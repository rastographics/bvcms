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
	[Table(Name="dbo.Setting")]
	public partial class Setting : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private string _Id;
		
		private string _SettingX;
		
		private bool? _System;
		
		private int? _SettingTypeId;
		
		private int? _SettingCategoryId;
		
   		
    	
		private EntityRef<SettingCategory> _SettingCategory;
		
		private EntityRef<SettingType> _SettingType;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(string value);
		partial void OnIdChanged();
		
		partial void OnSettingXChanging(string value);
		partial void OnSettingXChanged();
		
		partial void OnSystemChanging(bool? value);
		partial void OnSystemChanged();
		
		partial void OnSettingTypeIdChanging(int? value);
		partial void OnSettingTypeIdChanged();
		
		partial void OnSettingCategoryIdChanging(int? value);
		partial void OnSettingCategoryIdChanged();
		
    #endregion
		public Setting()
		{
			
			
			this._SettingCategory = default(EntityRef<SettingCategory>); 
			
			this._SettingType = default(EntityRef<SettingType>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="nvarchar(50) NOT NULL", IsPrimaryKey=true)]
		public string Id
		{
			get { return this._Id; }

			set
			{
				if (this._Id != value)
				{
				
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}

			}

		}

		
		[Column(Name="Setting", UpdateCheck=UpdateCheck.Never, Storage="_SettingX", DbType="nvarchar")]
		public string SettingX
		{
			get { return this._SettingX; }

			set
			{
				if (this._SettingX != value)
				{
				
                    this.OnSettingXChanging(value);
					this.SendPropertyChanging();
					this._SettingX = value;
					this.SendPropertyChanged("SettingX");
					this.OnSettingXChanged();
				}

			}

		}

		
		[Column(Name="System", UpdateCheck=UpdateCheck.Never, Storage="_System", DbType="bit")]
		public bool? System
		{
			get { return this._System; }

			set
			{
				if (this._System != value)
				{
				
                    this.OnSystemChanging(value);
					this.SendPropertyChanging();
					this._System = value;
					this.SendPropertyChanged("System");
					this.OnSystemChanged();
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
    	
		[Association(Name="FK_Setting_SettingCategory", Storage="_SettingCategory", ThisKey="SettingCategoryId", IsForeignKey=true)]
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
						previousValue.Settings.Remove(this);
					}

					this._SettingCategory.Entity = value;
					if (value != null)
					{
						value.Settings.Add(this);
						
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

		
		[Association(Name="FK_Setting_SettingType", Storage="_SettingType", ThisKey="SettingTypeId", IsForeignKey=true)]
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
						previousValue.Settings.Remove(this);
					}

					this._SettingType.Entity = value;
					if (value != null)
					{
						value.Settings.Add(this);
						
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

