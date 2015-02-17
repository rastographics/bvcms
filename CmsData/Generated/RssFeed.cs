using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData
{
	[Table(Name="dbo.RssFeed")]
	public partial class RssFeed : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private string _Url;
		
		private string _Data;
		
		private string _ETag;
		
		private DateTime? _LastModified;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnUrlChanging(string value);
		partial void OnUrlChanged();
		
		partial void OnDataChanging(string value);
		partial void OnDataChanged();
		
		partial void OnETagChanging(string value);
		partial void OnETagChanged();
		
		partial void OnLastModifiedChanging(DateTime? value);
		partial void OnLastModifiedChanged();
		
    #endregion
		public RssFeed()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Url", UpdateCheck=UpdateCheck.Never, Storage="_Url", DbType="nvarchar(150) NOT NULL", IsPrimaryKey=true)]
		public string Url
		{
			get { return this._Url; }

			set
			{
				if (this._Url != value)
				{
				
                    this.OnUrlChanging(value);
					this.SendPropertyChanging();
					this._Url = value;
					this.SendPropertyChanged("Url");
					this.OnUrlChanged();
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

		
		[Column(Name="ETag", UpdateCheck=UpdateCheck.Never, Storage="_ETag", DbType="nvarchar(150)")]
		public string ETag
		{
			get { return this._ETag; }

			set
			{
				if (this._ETag != value)
				{
				
                    this.OnETagChanging(value);
					this.SendPropertyChanging();
					this._ETag = value;
					this.SendPropertyChanged("ETag");
					this.OnETagChanged();
				}

			}

		}

		
		[Column(Name="LastModified", UpdateCheck=UpdateCheck.Never, Storage="_LastModified", DbType="datetime")]
		public DateTime? LastModified
		{
			get { return this._LastModified; }

			set
			{
				if (this._LastModified != value)
				{
				
                    this.OnLastModifiedChanging(value);
					this.SendPropertyChanging();
					this._LastModified = value;
					this.SendPropertyChanged("LastModified");
					this.OnLastModifiedChanged();
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

