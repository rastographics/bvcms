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
	[Table(Name="dbo.ActivityLog")]
	public partial class ActivityLog : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private long _Id;
		
		private DateTime? _ActivityDate;
		
		private int? _UserId;
		
		private string _Activity;
		
		private string _PageUrl;
		
		private string _Machine;
		
		private int? _OrgId;
		
		private int? _PeopleId;
		
   		
    	
		private EntityRef< Organization> _Organization;
		
		private EntityRef< Person> _Person;
		
		private EntityRef< User> _User;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(long value);
		partial void OnIdChanged();
		
		partial void OnActivityDateChanging(DateTime? value);
		partial void OnActivityDateChanged();
		
		partial void OnUserIdChanging(int? value);
		partial void OnUserIdChanged();
		
		partial void OnActivityChanging(string value);
		partial void OnActivityChanged();
		
		partial void OnPageUrlChanging(string value);
		partial void OnPageUrlChanged();
		
		partial void OnMachineChanging(string value);
		partial void OnMachineChanged();
		
		partial void OnOrgIdChanging(int? value);
		partial void OnOrgIdChanged();
		
		partial void OnPeopleIdChanging(int? value);
		partial void OnPeopleIdChanged();
		
    #endregion
		public ActivityLog()
		{
			
			
			this._Organization = default(EntityRef< Organization>); 
			
			this._Person = default(EntityRef< Person>); 
			
			this._User = default(EntityRef< User>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="bigint NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public long Id
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

		
		[Column(Name="ActivityDate", UpdateCheck=UpdateCheck.Never, Storage="_ActivityDate", DbType="datetime")]
		public DateTime? ActivityDate
		{
			get { return this._ActivityDate; }

			set
			{
				if (this._ActivityDate != value)
				{
				
                    this.OnActivityDateChanging(value);
					this.SendPropertyChanging();
					this._ActivityDate = value;
					this.SendPropertyChanged("ActivityDate");
					this.OnActivityDateChanged();
				}

			}

		}

		
		[Column(Name="UserId", UpdateCheck=UpdateCheck.Never, Storage="_UserId", DbType="int")]
		public int? UserId
		{
			get { return this._UserId; }

			set
			{
				if (this._UserId != value)
				{
				
					if (this._User.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnUserIdChanging(value);
					this.SendPropertyChanging();
					this._UserId = value;
					this.SendPropertyChanged("UserId");
					this.OnUserIdChanged();
				}

			}

		}

		
		[Column(Name="Activity", UpdateCheck=UpdateCheck.Never, Storage="_Activity", DbType="nvarchar(200)")]
		public string Activity
		{
			get { return this._Activity; }

			set
			{
				if (this._Activity != value)
				{
				
                    this.OnActivityChanging(value);
					this.SendPropertyChanging();
					this._Activity = value;
					this.SendPropertyChanged("Activity");
					this.OnActivityChanged();
				}

			}

		}

		
		[Column(Name="PageUrl", UpdateCheck=UpdateCheck.Never, Storage="_PageUrl", DbType="nvarchar(410)")]
		public string PageUrl
		{
			get { return this._PageUrl; }

			set
			{
				if (this._PageUrl != value)
				{
				
                    this.OnPageUrlChanging(value);
					this.SendPropertyChanging();
					this._PageUrl = value;
					this.SendPropertyChanged("PageUrl");
					this.OnPageUrlChanged();
				}

			}

		}

		
		[Column(Name="Machine", UpdateCheck=UpdateCheck.Never, Storage="_Machine", DbType="nvarchar(50)")]
		public string Machine
		{
			get { return this._Machine; }

			set
			{
				if (this._Machine != value)
				{
				
                    this.OnMachineChanging(value);
					this.SendPropertyChanging();
					this._Machine = value;
					this.SendPropertyChanged("Machine");
					this.OnMachineChanged();
				}

			}

		}

		
		[Column(Name="OrgId", UpdateCheck=UpdateCheck.Never, Storage="_OrgId", DbType="int")]
		public int? OrgId
		{
			get { return this._OrgId; }

			set
			{
				if (this._OrgId != value)
				{
				
					if (this._Organization.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrgIdChanging(value);
					this.SendPropertyChanging();
					this._OrgId = value;
					this.SendPropertyChanged("OrgId");
					this.OnOrgIdChanged();
				}

			}

		}

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int")]
		public int? PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_ActivityLog_Organizations", Storage="_Organization", ThisKey="OrgId", IsForeignKey=true)]
		public Organization Organization
		{
			get { return this._Organization.Entity; }

			set
			{
				Organization previousValue = this._Organization.Entity;
				if (((previousValue != value) 
							|| (this._Organization.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Organization.Entity = null;
						previousValue.ActivityLogs.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.ActivityLogs.Add(this);
						
						this._OrgId = value.OrganizationId;
						
					}

					else
					{
						
						this._OrgId = default(int?);
						
					}

					this.SendPropertyChanged("Organization");
				}

			}

		}

		
		[Association(Name="FK_ActivityLog_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
		public Person Person
		{
			get { return this._Person.Entity; }

			set
			{
				Person previousValue = this._Person.Entity;
				if (((previousValue != value) 
							|| (this._Person.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Person.Entity = null;
						previousValue.ActivityLogs.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.ActivityLogs.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int?);
						
					}

					this.SendPropertyChanged("Person");
				}

			}

		}

		
		[Association(Name="FK_ActivityLog_Users", Storage="_User", ThisKey="UserId", IsForeignKey=true)]
		public User User
		{
			get { return this._User.Entity; }

			set
			{
				User previousValue = this._User.Entity;
				if (((previousValue != value) 
							|| (this._User.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._User.Entity = null;
						previousValue.ActivityLogs.Remove(this);
					}

					this._User.Entity = value;
					if (value != null)
					{
						value.ActivityLogs.Add(this);
						
						this._UserId = value.UserId;
						
					}

					else
					{
						
						this._UserId = default(int?);
						
					}

					this.SendPropertyChanged("User");
				}

			}

		}

		
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

