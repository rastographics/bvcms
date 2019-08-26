using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

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
		
   		private EntitySet<SettingMetadatum> _SettingMetadatas;

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

        #endregion

        public Setting()
		{
			this._SettingMetadatas = new EntitySet<SettingMetadatum>(new Action< SettingMetadatum>(this.attach_SettingMetadatas), new Action< SettingMetadatum>(this.detach_SettingMetadatas)); 
			
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

        #endregion
        
        #region Foreign Key Tables
   		
   		[Association(Name="FK_SettingMetadata_Setting", Storage="_SettingMetadatas", OtherKey="SettingId")]
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
   		
		private void attach_SettingMetadatas(SettingMetadatum entity)
		{
			this.SendPropertyChanging();
			entity.Setting = this;
		}

		private void detach_SettingMetadatas(SettingMetadatum entity)
		{
			this.SendPropertyChanging();
			entity.Setting = null;
		}
	}
}
