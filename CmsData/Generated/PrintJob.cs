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
	[Table(Name="dbo.PrintJob")]
	public partial class PrintJob : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private string _Id;
		
		private DateTime _Stamp;
		
		private string _Data;
		
		private string _JsonData;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(string value);
		partial void OnIdChanged();
		
		partial void OnStampChanging(DateTime value);
		partial void OnStampChanged();
		
		partial void OnDataChanging(string value);
		partial void OnDataChanged();
		
		partial void OnJsonDataChanging(string value);
		partial void OnJsonDataChanged();
		
    #endregion
		public PrintJob()
		{
			
			
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

		
		[Column(Name="Stamp", UpdateCheck=UpdateCheck.Never, Storage="_Stamp", DbType="datetime NOT NULL", IsPrimaryKey=true)]
		public DateTime Stamp
		{
			get { return this._Stamp; }

			set
			{
				if (this._Stamp != value)
				{
				
                    this.OnStampChanging(value);
					this.SendPropertyChanging();
					this._Stamp = value;
					this.SendPropertyChanged("Stamp");
					this.OnStampChanged();
				}

			}

		}

		
		[Column(Name="Data", UpdateCheck=UpdateCheck.Never, Storage="_Data", DbType="nvarchar")]
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

		
		[Column(Name="JsonData", UpdateCheck=UpdateCheck.Never, Storage="_JsonData", DbType="nvarchar")]
		public string JsonData
		{
			get { return this._JsonData; }

			set
			{
				if (this._JsonData != value)
				{
				
                    this.OnJsonDataChanging(value);
					this.SendPropertyChanging();
					this._JsonData = value;
					this.SendPropertyChanged("JsonData");
					this.OnJsonDataChanged();
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

