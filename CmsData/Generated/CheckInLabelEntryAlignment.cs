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
	[Table(Name="dbo.CheckInLabelEntryAlignment")]
	public partial class CheckInLabelEntryAlignment : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _NameX;
		
		private string _NameY;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnNameXChanging(string value);
		partial void OnNameXChanged();
		
		partial void OnNameYChanging(string value);
		partial void OnNameYChanged();
		
    #endregion
		public CheckInLabelEntryAlignment()
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

		
		[Column(Name="nameX", UpdateCheck=UpdateCheck.Never, Storage="_NameX", DbType="nvarchar(10) NOT NULL")]
		public string NameX
		{
			get { return this._NameX; }

			set
			{
				if (this._NameX != value)
				{
				
                    this.OnNameXChanging(value);
					this.SendPropertyChanging();
					this._NameX = value;
					this.SendPropertyChanged("NameX");
					this.OnNameXChanged();
				}

			}

		}

		
		[Column(Name="nameY", UpdateCheck=UpdateCheck.Never, Storage="_NameY", DbType="nvarchar(10) NOT NULL")]
		public string NameY
		{
			get { return this._NameY; }

			set
			{
				if (this._NameY != value)
				{
				
                    this.OnNameYChanging(value);
					this.SendPropertyChanging();
					this._NameY = value;
					this.SendPropertyChanged("NameY");
					this.OnNameYChanged();
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

