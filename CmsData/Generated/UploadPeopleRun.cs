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
	[Table(Name="dbo.UploadPeopleRun")]
	public partial class UploadPeopleRun : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int? _Meetingid;
		
		private DateTime? _Started;
		
		private int? _Count;
		
		private int? _Processed;
		
		private DateTime? _Completed;
		
		private string _Error;
		
		private int _Running;
		
		private string _Description;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnMeetingidChanging(int? value);
		partial void OnMeetingidChanged();
		
		partial void OnStartedChanging(DateTime? value);
		partial void OnStartedChanged();
		
		partial void OnCountChanging(int? value);
		partial void OnCountChanged();
		
		partial void OnProcessedChanging(int? value);
		partial void OnProcessedChanged();
		
		partial void OnCompletedChanging(DateTime? value);
		partial void OnCompletedChanged();
		
		partial void OnErrorChanging(string value);
		partial void OnErrorChanged();
		
		partial void OnRunningChanging(int value);
		partial void OnRunningChanged();
		
		partial void OnDescriptionChanging(string value);
		partial void OnDescriptionChanged();
		
    #endregion
		public UploadPeopleRun()
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

		
		[Column(Name="meetingid", UpdateCheck=UpdateCheck.Never, Storage="_Meetingid", DbType="int")]
		public int? Meetingid
		{
			get { return this._Meetingid; }

			set
			{
				if (this._Meetingid != value)
				{
				
                    this.OnMeetingidChanging(value);
					this.SendPropertyChanging();
					this._Meetingid = value;
					this.SendPropertyChanged("Meetingid");
					this.OnMeetingidChanged();
				}

			}

		}

		
		[Column(Name="started", UpdateCheck=UpdateCheck.Never, Storage="_Started", DbType="datetime")]
		public DateTime? Started
		{
			get { return this._Started; }

			set
			{
				if (this._Started != value)
				{
				
                    this.OnStartedChanging(value);
					this.SendPropertyChanging();
					this._Started = value;
					this.SendPropertyChanged("Started");
					this.OnStartedChanged();
				}

			}

		}

		
		[Column(Name="count", UpdateCheck=UpdateCheck.Never, Storage="_Count", DbType="int")]
		public int? Count
		{
			get { return this._Count; }

			set
			{
				if (this._Count != value)
				{
				
                    this.OnCountChanging(value);
					this.SendPropertyChanging();
					this._Count = value;
					this.SendPropertyChanged("Count");
					this.OnCountChanged();
				}

			}

		}

		
		[Column(Name="processed", UpdateCheck=UpdateCheck.Never, Storage="_Processed", DbType="int")]
		public int? Processed
		{
			get { return this._Processed; }

			set
			{
				if (this._Processed != value)
				{
				
                    this.OnProcessedChanging(value);
					this.SendPropertyChanging();
					this._Processed = value;
					this.SendPropertyChanged("Processed");
					this.OnProcessedChanged();
				}

			}

		}

		
		[Column(Name="completed", UpdateCheck=UpdateCheck.Never, Storage="_Completed", DbType="datetime")]
		public DateTime? Completed
		{
			get { return this._Completed; }

			set
			{
				if (this._Completed != value)
				{
				
                    this.OnCompletedChanging(value);
					this.SendPropertyChanging();
					this._Completed = value;
					this.SendPropertyChanged("Completed");
					this.OnCompletedChanged();
				}

			}

		}

		
		[Column(Name="error", UpdateCheck=UpdateCheck.Never, Storage="_Error", DbType="nvarchar(200)")]
		public string Error
		{
			get { return this._Error; }

			set
			{
				if (this._Error != value)
				{
				
                    this.OnErrorChanging(value);
					this.SendPropertyChanging();
					this._Error = value;
					this.SendPropertyChanged("Error");
					this.OnErrorChanged();
				}

			}

		}

		
		[Column(Name="running", UpdateCheck=UpdateCheck.Never, Storage="_Running", DbType="int NOT NULL", IsDbGenerated=true)]
		public int Running
		{
			get { return this._Running; }

			set
			{
				if (this._Running != value)
				{
				
                    this.OnRunningChanging(value);
					this.SendPropertyChanging();
					this._Running = value;
					this.SendPropertyChanged("Running");
					this.OnRunningChanged();
				}

			}

		}

		
		[Column(Name="description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar(100)")]
		public string Description
		{
			get { return this._Description; }

			set
			{
				if (this._Description != value)
				{
				
                    this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
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

