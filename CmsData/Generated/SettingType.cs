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
	[Table(Name="dbo.SettingType")]
	public partial class SettingType : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _SettingTypeId;
		
		private string _Name;
		
		private int _DisplayOrder;
		
   		
   		private EntitySet<Setting> _Settings;
		
   		private EntitySet<SettingCategory> _SettingCategories;
		
    	
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
			
			this._Settings = new EntitySet<Setting>(new Action< Setting>(this.attach_Settings), new Action< Setting>(this.detach_Settings)); 
			
			this._SettingCategories = new EntitySet<SettingCategory>(new Action< SettingCategory>(this.attach_SettingCategories), new Action< SettingCategory>(this.detach_SettingCategories)); 
			
			
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
   		
   		[Association(Name="FK_Setting_SettingType", Storage="_Settings", OtherKey="SettingTypeId")]
   		public EntitySet<Setting> Settings
   		{
   		    get { return this._Settings; }

			set	{ this._Settings.Assign(value); }

   		}

		
   		[Association(Name="FK_SettingCategory_SettingType", Storage="_SettingCategories", OtherKey="SettingTypeId")]
   		public EntitySet<SettingCategory> SettingCategories
   		{
   		    get { return this._SettingCategories; }

			set	{ this._SettingCategories.Assign(value); }

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

   		
		private void attach_Settings(Setting entity)
		{
			this.SendPropertyChanging();
			entity.SettingType = this;
		}

		private void detach_Settings(Setting entity)
		{
			this.SendPropertyChanging();
			entity.SettingType = null;
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

		
	}

}

