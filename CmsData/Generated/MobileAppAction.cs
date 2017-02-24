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
	[Table(Name="dbo.MobileAppActions")]
	public partial class MobileAppAction : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _Type;
		
		private string _Title;
		
		private int _Option;
		
		private string _Data;
		
		private int _Order;
		
		private int _LoginType;
		
		private bool _Enabled;
		
		private string _Roles;
		
		private int _Api;
		
		private DateTime _Active;
		
		private string _AltTitle;
		
		private int _Rebranded;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnTypeChanging(int value);
		partial void OnTypeChanged();
		
		partial void OnTitleChanging(string value);
		partial void OnTitleChanged();
		
		partial void OnOptionChanging(int value);
		partial void OnOptionChanged();
		
		partial void OnDataChanging(string value);
		partial void OnDataChanged();
		
		partial void OnOrderChanging(int value);
		partial void OnOrderChanged();
		
		partial void OnLoginTypeChanging(int value);
		partial void OnLoginTypeChanged();
		
		partial void OnEnabledChanging(bool value);
		partial void OnEnabledChanged();
		
		partial void OnRolesChanging(string value);
		partial void OnRolesChanged();
		
		partial void OnApiChanging(int value);
		partial void OnApiChanged();
		
		partial void OnActiveChanging(DateTime value);
		partial void OnActiveChanged();
		
		partial void OnAltTitleChanging(string value);
		partial void OnAltTitleChanged();
		
		partial void OnRebrandedChanging(int value);
		partial void OnRebrandedChanged();
		
    #endregion
		public MobileAppAction()
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

		
		[Column(Name="type", UpdateCheck=UpdateCheck.Never, Storage="_Type", DbType="int NOT NULL")]
		public int Type
		{
			get { return this._Type; }

			set
			{
				if (this._Type != value)
				{
				
                    this.OnTypeChanging(value);
					this.SendPropertyChanging();
					this._Type = value;
					this.SendPropertyChanged("Type");
					this.OnTypeChanged();
				}

			}

		}

		
		[Column(Name="title", UpdateCheck=UpdateCheck.Never, Storage="_Title", DbType="nvarchar(50) NOT NULL")]
		public string Title
		{
			get { return this._Title; }

			set
			{
				if (this._Title != value)
				{
				
                    this.OnTitleChanging(value);
					this.SendPropertyChanging();
					this._Title = value;
					this.SendPropertyChanged("Title");
					this.OnTitleChanged();
				}

			}

		}

		
		[Column(Name="option", UpdateCheck=UpdateCheck.Never, Storage="_Option", DbType="int NOT NULL")]
		public int Option
		{
			get { return this._Option; }

			set
			{
				if (this._Option != value)
				{
				
                    this.OnOptionChanging(value);
					this.SendPropertyChanging();
					this._Option = value;
					this.SendPropertyChanged("Option");
					this.OnOptionChanged();
				}

			}

		}

		
		[Column(Name="data", UpdateCheck=UpdateCheck.Never, Storage="_Data", DbType="nvarchar NOT NULL")]
		public string Data
		{
			get { return this._Data; }

			set
			{
				if (this._Data != value)
				{
				
                    this.OnDataChanging(value);
					this.SendPropertyChanging();
					this._Data = value;
					this.SendPropertyChanged("Data");
					this.OnDataChanged();
				}

			}

		}

		
		[Column(Name="order", UpdateCheck=UpdateCheck.Never, Storage="_Order", DbType="int NOT NULL")]
		public int Order
		{
			get { return this._Order; }

			set
			{
				if (this._Order != value)
				{
				
                    this.OnOrderChanging(value);
					this.SendPropertyChanging();
					this._Order = value;
					this.SendPropertyChanged("Order");
					this.OnOrderChanged();
				}

			}

		}

		
		[Column(Name="loginType", UpdateCheck=UpdateCheck.Never, Storage="_LoginType", DbType="int NOT NULL")]
		public int LoginType
		{
			get { return this._LoginType; }

			set
			{
				if (this._LoginType != value)
				{
				
                    this.OnLoginTypeChanging(value);
					this.SendPropertyChanging();
					this._LoginType = value;
					this.SendPropertyChanged("LoginType");
					this.OnLoginTypeChanged();
				}

			}

		}

		
		[Column(Name="enabled", UpdateCheck=UpdateCheck.Never, Storage="_Enabled", DbType="bit NOT NULL")]
		public bool Enabled
		{
			get { return this._Enabled; }

			set
			{
				if (this._Enabled != value)
				{
				
                    this.OnEnabledChanging(value);
					this.SendPropertyChanging();
					this._Enabled = value;
					this.SendPropertyChanged("Enabled");
					this.OnEnabledChanged();
				}

			}

		}

		
		[Column(Name="roles", UpdateCheck=UpdateCheck.Never, Storage="_Roles", DbType="nvarchar NOT NULL")]
		public string Roles
		{
			get { return this._Roles; }

			set
			{
				if (this._Roles != value)
				{
				
                    this.OnRolesChanging(value);
					this.SendPropertyChanging();
					this._Roles = value;
					this.SendPropertyChanged("Roles");
					this.OnRolesChanged();
				}

			}

		}

		
		[Column(Name="api", UpdateCheck=UpdateCheck.Never, Storage="_Api", DbType="int NOT NULL")]
		public int Api
		{
			get { return this._Api; }

			set
			{
				if (this._Api != value)
				{
				
                    this.OnApiChanging(value);
					this.SendPropertyChanging();
					this._Api = value;
					this.SendPropertyChanged("Api");
					this.OnApiChanged();
				}

			}

		}

		
		[Column(Name="active", UpdateCheck=UpdateCheck.Never, Storage="_Active", DbType="datetime NOT NULL")]
		public DateTime Active
		{
			get { return this._Active; }

			set
			{
				if (this._Active != value)
				{
				
                    this.OnActiveChanging(value);
					this.SendPropertyChanging();
					this._Active = value;
					this.SendPropertyChanged("Active");
					this.OnActiveChanged();
				}

			}

		}

		
		[Column(Name="altTitle", UpdateCheck=UpdateCheck.Never, Storage="_AltTitle", DbType="nvarchar(50) NOT NULL")]
		public string AltTitle
		{
			get { return this._AltTitle; }

			set
			{
				if (this._AltTitle != value)
				{
				
                    this.OnAltTitleChanging(value);
					this.SendPropertyChanging();
					this._AltTitle = value;
					this.SendPropertyChanged("AltTitle");
					this.OnAltTitleChanged();
				}

			}

		}

		
		[Column(Name="rebranded", UpdateCheck=UpdateCheck.Never, Storage="_Rebranded", DbType="int NOT NULL")]
		public int Rebranded
		{
			get { return this._Rebranded; }

			set
			{
				if (this._Rebranded != value)
				{
				
                    this.OnRebrandedChanging(value);
					this.SendPropertyChanging();
					this._Rebranded = value;
					this.SendPropertyChanged("Rebranded");
					this.OnRebrandedChanged();
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

