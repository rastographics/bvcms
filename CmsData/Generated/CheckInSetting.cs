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
	[Table(Name="dbo.CheckInSettings")]
	public partial class CheckInSetting : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Name;
		
		private string _Settings;
		
		private int _Version;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnSettingsChanging(string value);
		partial void OnSettingsChanged();
		
		partial void OnVersionChanging(int value);
		partial void OnVersionChanged();
		
    #endregion
		public CheckInSetting()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
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

		
		[Column(Name="name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(50) NOT NULL")]
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

		
		[Column(Name="settings", UpdateCheck=UpdateCheck.Never, Storage="_Settings", DbType="nvarchar NOT NULL")]
		public string Settings
		{
			get { return this._Settings; }

			set
			{
				if (this._Settings != value)
				{
				
                    this.OnSettingsChanging(value);
					this.SendPropertyChanging();
					this._Settings = value;
					this.SendPropertyChanged("Settings");
					this.OnSettingsChanged();
				}

			}

		}

		
		[Column(Name="version", UpdateCheck=UpdateCheck.Never, Storage="_Version", DbType="int NOT NULL")]
		public int Version
		{
			get { return this._Version; }

			set
			{
				if (this._Version != value)
				{
				
                    this.OnVersionChanging(value);
					this.SendPropertyChanging();
					this._Version = value;
					this.SendPropertyChanged("Version");
					this.OnVersionChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
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

   		
	}

}

