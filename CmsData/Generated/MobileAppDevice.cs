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
	[Table(Name="dbo.MobileAppDevices")]
	public partial class MobileAppDevice : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private DateTime _Created;
		
		private DateTime _LastSeen;
		
		private int _DeviceTypeID;
		
		private string _InstanceID;
		
		private string _NotificationID;
		
		private int? _UserID;
		
		private int? _PeopleID;
		
		private string _Authentication;
		
		private string _Code;
		
		private DateTime _CodeExpires;
		
		private string _CodeEmail;
		
   		
    	
		private EntityRef<Person> _Person;
		
		private EntityRef<User> _User;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnCreatedChanging(DateTime value);
		partial void OnCreatedChanged();
		
		partial void OnLastSeenChanging(DateTime value);
		partial void OnLastSeenChanged();
		
		partial void OnDeviceTypeIDChanging(int value);
		partial void OnDeviceTypeIDChanged();
		
		partial void OnInstanceIDChanging(string value);
		partial void OnInstanceIDChanged();
		
		partial void OnNotificationIDChanging(string value);
		partial void OnNotificationIDChanged();
		
		partial void OnUserIDChanging(int? value);
		partial void OnUserIDChanged();
		
		partial void OnPeopleIDChanging(int? value);
		partial void OnPeopleIDChanged();
		
		partial void OnAuthenticationChanging(string value);
		partial void OnAuthenticationChanged();
		
		partial void OnCodeChanging(string value);
		partial void OnCodeChanged();
		
		partial void OnCodeExpiresChanging(DateTime value);
		partial void OnCodeExpiresChanged();
		
		partial void OnCodeEmailChanging(string value);
		partial void OnCodeEmailChanged();
		
    #endregion
		public MobileAppDevice()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
			this._User = default(EntityRef<User>); 
			
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

		
		[Column(Name="lastSeen", UpdateCheck=UpdateCheck.Never, Storage="_LastSeen", DbType="datetime NOT NULL")]
		public DateTime LastSeen
		{
			get { return this._LastSeen; }

			set
			{
				if (this._LastSeen != value)
				{
				
                    this.OnLastSeenChanging(value);
					this.SendPropertyChanging();
					this._LastSeen = value;
					this.SendPropertyChanged("LastSeen");
					this.OnLastSeenChanged();
				}

			}

		}

		
		[Column(Name="deviceTypeID", UpdateCheck=UpdateCheck.Never, Storage="_DeviceTypeID", DbType="int NOT NULL")]
		public int DeviceTypeID
		{
			get { return this._DeviceTypeID; }

			set
			{
				if (this._DeviceTypeID != value)
				{
				
                    this.OnDeviceTypeIDChanging(value);
					this.SendPropertyChanging();
					this._DeviceTypeID = value;
					this.SendPropertyChanged("DeviceTypeID");
					this.OnDeviceTypeIDChanged();
				}

			}

		}

		
		[Column(Name="instanceID", UpdateCheck=UpdateCheck.Never, Storage="_InstanceID", DbType="varchar(255) NOT NULL")]
		public string InstanceID
		{
			get { return this._InstanceID; }

			set
			{
				if (this._InstanceID != value)
				{
				
                    this.OnInstanceIDChanging(value);
					this.SendPropertyChanging();
					this._InstanceID = value;
					this.SendPropertyChanged("InstanceID");
					this.OnInstanceIDChanged();
				}

			}

		}

		
		[Column(Name="notificationID", UpdateCheck=UpdateCheck.Never, Storage="_NotificationID", DbType="varchar NOT NULL")]
		public string NotificationID
		{
			get { return this._NotificationID; }

			set
			{
				if (this._NotificationID != value)
				{
				
                    this.OnNotificationIDChanging(value);
					this.SendPropertyChanging();
					this._NotificationID = value;
					this.SendPropertyChanged("NotificationID");
					this.OnNotificationIDChanged();
				}

			}

		}

		
		[Column(Name="userID", UpdateCheck=UpdateCheck.Never, Storage="_UserID", DbType="int")]
		[IsForeignKey]
		public int? UserID
		{
			get { return this._UserID; }

			set
			{
				if (this._UserID != value)
				{
				
					if (this._User.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnUserIDChanging(value);
					this.SendPropertyChanging();
					this._UserID = value;
					this.SendPropertyChanged("UserID");
					this.OnUserIDChanged();
				}

			}

		}

		
		[Column(Name="peopleID", UpdateCheck=UpdateCheck.Never, Storage="_PeopleID", DbType="int")]
		[IsForeignKey]
		public int? PeopleID
		{
			get { return this._PeopleID; }

			set
			{
				if (this._PeopleID != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIDChanging(value);
					this.SendPropertyChanging();
					this._PeopleID = value;
					this.SendPropertyChanged("PeopleID");
					this.OnPeopleIDChanged();
				}

			}

		}

		
		[Column(Name="authentication", UpdateCheck=UpdateCheck.Never, Storage="_Authentication", DbType="varchar(64) NOT NULL")]
		public string Authentication
		{
			get { return this._Authentication; }

			set
			{
				if (this._Authentication != value)
				{
				
                    this.OnAuthenticationChanging(value);
					this.SendPropertyChanging();
					this._Authentication = value;
					this.SendPropertyChanged("Authentication");
					this.OnAuthenticationChanged();
				}

			}

		}

		
		[Column(Name="code", UpdateCheck=UpdateCheck.Never, Storage="_Code", DbType="varchar(64) NOT NULL")]
		public string Code
		{
			get { return this._Code; }

			set
			{
				if (this._Code != value)
				{
				
                    this.OnCodeChanging(value);
					this.SendPropertyChanging();
					this._Code = value;
					this.SendPropertyChanged("Code");
					this.OnCodeChanged();
				}

			}

		}

		
		[Column(Name="codeExpires", UpdateCheck=UpdateCheck.Never, Storage="_CodeExpires", DbType="datetime NOT NULL")]
		public DateTime CodeExpires
		{
			get { return this._CodeExpires; }

			set
			{
				if (this._CodeExpires != value)
				{
				
                    this.OnCodeExpiresChanging(value);
					this.SendPropertyChanging();
					this._CodeExpires = value;
					this.SendPropertyChanged("CodeExpires");
					this.OnCodeExpiresChanged();
				}

			}

		}

		
		[Column(Name="codeEmail", UpdateCheck=UpdateCheck.Never, Storage="_CodeEmail", DbType="varchar(255) NOT NULL")]
		public string CodeEmail
		{
			get { return this._CodeEmail; }

			set
			{
				if (this._CodeEmail != value)
				{
				
                    this.OnCodeEmailChanging(value);
					this.SendPropertyChanging();
					this._CodeEmail = value;
					this.SendPropertyChanged("CodeEmail");
					this.OnCodeEmailChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_MobileAppDevices_People", Storage="_Person", ThisKey="PeopleID", IsForeignKey=true)]
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
						previousValue.MobileAppDevices.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.MobileAppDevices.Add(this);
						
						this._PeopleID = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleID = default(int?);
						
					}

					this.SendPropertyChanged("Person");
				}

			}

		}

		
		[Association(Name="FK_MobileAppDevices_Users", Storage="_User", ThisKey="UserID", IsForeignKey=true)]
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
						previousValue.MobileAppDevices.Remove(this);
					}

					this._User.Entity = value;
					if (value != null)
					{
						value.MobileAppDevices.Add(this);
						
						this._UserID = value.UserId;
						
					}

					else
					{
						
						this._UserID = default(int?);
						
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

