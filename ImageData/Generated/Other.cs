using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace ImageData
{
	[Table(Name="dbo.Other")]
	public partial class Other : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private DateTime _Created;
		
		private int _UserID;
		
		private byte[] _First;
		
		private byte[] _Second;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnCreatedChanging(DateTime value);
		partial void OnCreatedChanged();
		
		partial void OnUserIDChanging(int value);
		partial void OnUserIDChanged();
		
		partial void OnFirstChanging(byte[] value);
		partial void OnFirstChanged();
		
		partial void OnSecondChanging(byte[] value);
		partial void OnSecondChanged();
		
    #endregion
		public Other()
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

		
		[Column(Name="created", UpdateCheck=UpdateCheck.Never, Storage="_Created", DbType="datetime NOT NULL")]
		public DateTime Created
		{
			get { return this._Created; }

			set
			{
				if (this._Created != value)
				{
				
                    this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}

			}

		}

		
		[Column(Name="userID", UpdateCheck=UpdateCheck.Never, Storage="_UserID", DbType="int NOT NULL")]
		public int UserID
		{
			get { return this._UserID; }

			set
			{
				if (this._UserID != value)
				{
				
                    this.OnUserIDChanging(value);
					this.SendPropertyChanging();
					this._UserID = value;
					this.SendPropertyChanged("UserID");
					this.OnUserIDChanged();
				}

			}

		}

		
		[Column(Name="first", UpdateCheck=UpdateCheck.Never, Storage="_First", DbType="varbinary")]
		public byte[] First
		{
			get { return this._First; }

			set
			{
				if (this._First != value)
				{
				
                    this.OnFirstChanging(value);
					this.SendPropertyChanging();
					this._First = value;
					this.SendPropertyChanged("First");
					this.OnFirstChanged();
				}

			}

		}

		
		[Column(Name="second", UpdateCheck=UpdateCheck.Never, Storage="_Second", DbType="varbinary")]
		public byte[] Second
		{
			get { return this._Second; }

			set
			{
				if (this._Second != value)
				{
				
                    this.OnSecondChanging(value);
					this.SendPropertyChanging();
					this._Second = value;
					this.SendPropertyChanged("Second");
					this.OnSecondChanged();
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

