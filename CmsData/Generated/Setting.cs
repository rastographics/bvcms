using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Setting")]
    public partial class Setting : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs("");

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
			_SettingMetadatas = new EntitySet<SettingMetadatum>(new Action< SettingMetadatum>(attach_SettingMetadatas), new Action< SettingMetadatum>(detach_SettingMetadatas)); 
			
			OnCreated();
		}
		
        #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="nvarchar(50) NOT NULL", IsPrimaryKey=true)]
		public string Id
		{
			get { return _Id; }

			set
			{
				if (_Id != value)
				{
                    OnIdChanging(value);
					SendPropertyChanging();
					_Id = value;
					SendPropertyChanged("Id");
					OnIdChanged();
				}
			}
		}

		[Column(Name="Setting", UpdateCheck=UpdateCheck.Never, Storage="_SettingX", DbType="nvarchar")]
		public string SettingX
		{
			get { return _SettingX; }

			set
			{
				if (_SettingX != value)
				{
                    OnSettingXChanging(value);
					SendPropertyChanging();
					_SettingX = value;
					SendPropertyChanged("SettingX");
					OnSettingXChanged();
				}
			}
		}
		
		[Column(Name="System", UpdateCheck=UpdateCheck.Never, Storage="_System", DbType="bit")]
		public bool? System
		{
			get { return _System; }

			set
			{
				if (_System != value)
				{
                    OnSystemChanging(value);
					SendPropertyChanging();
					_System = value;
					SendPropertyChanged("System");
					OnSystemChanged();
				}
			}
		}

        #endregion
        
        #region Foreign Key Tables
   		
   		[Association(Name="FK_SettingMetadata_Setting", Storage="_SettingMetadatas", OtherKey="SettingId")]
   		public EntitySet<SettingMetadatum> SettingMetadatas
   		{
   		    get { return _SettingMetadatas; }

			set	{ _SettingMetadatas.Assign(value); }
   		}
		
	    #endregion
	    
	    #region Foreign Keys
    	    
	    #endregion
	
		public event PropertyChangingEventHandler PropertyChanging;

		protected virtual void SendPropertyChanging()
		{
			if ((PropertyChanging != null))
				PropertyChanging(this, emptyChangingEventArgs);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((PropertyChanged != null))
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
   		
		private void attach_SettingMetadatas(SettingMetadatum entity)
		{
			SendPropertyChanging();
			entity.Setting = this;
		}

		private void detach_SettingMetadatas(SettingMetadatum entity)
		{
			SendPropertyChanging();
			entity.Setting = null;
		}
	}
}
