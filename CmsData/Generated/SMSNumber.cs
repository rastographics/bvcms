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
	[Table(Name="dbo.SMSNumbers")]
	public partial class SMSNumber : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _GroupID;
		
		private string _Number;
		
		private DateTime _LastUpdated;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnGroupIDChanging(int value);
		partial void OnGroupIDChanged();
		
		partial void OnNumberChanging(string value);
		partial void OnNumberChanged();
		
		partial void OnLastUpdatedChanging(DateTime value);
		partial void OnLastUpdatedChanged();

        #endregion
        public SMSNumber()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ID", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
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

		
		[Column(Name="GroupID", UpdateCheck=UpdateCheck.Never, Storage="_GroupID", DbType="int NOT NULL")]
		public int GroupID
		{
			get { return this._GroupID; }

			set
			{
				if (this._GroupID != value)
				{
				
                    this.OnGroupIDChanging(value);
					this.SendPropertyChanging();
					this._GroupID = value;
					this.SendPropertyChanged("GroupID");
					this.OnGroupIDChanged();
				}

			}

		}

		
		[Column(Name="Number", UpdateCheck=UpdateCheck.Never, Storage="_Number", DbType="nvarchar(50) NOT NULL")]
		public string Number
		{
			get { return this._Number; }

			set
			{
				if (this._Number != value)
				{
				
                    this.OnNumberChanging(value);
					this.SendPropertyChanging();
					this._Number = value;
					this.SendPropertyChanged("Number");
					this.OnNumberChanged();
				}

			}

		}

		
		[Column(Name="LastUpdated", UpdateCheck=UpdateCheck.Never, Storage="_LastUpdated", DbType="datetime NOT NULL")]
		public DateTime LastUpdated
		{
			get { return this._LastUpdated; }

			set
			{
				if (this._LastUpdated != value)
				{
				
                    this.OnLastUpdatedChanging(value);
					this.SendPropertyChanging();
					this._LastUpdated = value;
					this.SendPropertyChanged("LastUpdated");
					this.OnLastUpdatedChanged();
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

