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
	[Table(Name="dbo.ApiSession")]
	public partial class ApiSession : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _UserId;
		
		private Guid _SessionToken;
		
		private int? _Pin;
		
		private DateTime _LastAccessedDate;
		
		private DateTime _CreatedDate;
		
   		
    	
		private EntityRef<User> _User;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnUserIdChanging(int value);
		partial void OnUserIdChanged();
		
		partial void OnSessionTokenChanging(Guid value);
		partial void OnSessionTokenChanged();
		
		partial void OnPinChanging(int? value);
		partial void OnPinChanged();
		
		partial void OnLastAccessedDateChanging(DateTime value);
		partial void OnLastAccessedDateChanged();
		
		partial void OnCreatedDateChanging(DateTime value);
		partial void OnCreatedDateChanged();
		
    #endregion
		public ApiSession()
		{
			
			
			this._User = default(EntityRef<User>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="UserId", UpdateCheck=UpdateCheck.Never, Storage="_UserId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int UserId
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

		
		[Column(Name="SessionToken", UpdateCheck=UpdateCheck.Never, Storage="_SessionToken", DbType="uniqueidentifier NOT NULL")]
		public Guid SessionToken
		{
			get { return this._SessionToken; }

			set
			{
				if (this._SessionToken != value)
				{
				
                    this.OnSessionTokenChanging(value);
					this.SendPropertyChanging();
					this._SessionToken = value;
					this.SendPropertyChanged("SessionToken");
					this.OnSessionTokenChanged();
				}

			}

		}

		
		[Column(Name="PIN", UpdateCheck=UpdateCheck.Never, Storage="_Pin", DbType="int")]
		public int? Pin
		{
			get { return this._Pin; }

			set
			{
				if (this._Pin != value)
				{
				
                    this.OnPinChanging(value);
					this.SendPropertyChanging();
					this._Pin = value;
					this.SendPropertyChanged("Pin");
					this.OnPinChanged();
				}

			}

		}

		
		[Column(Name="LastAccessedDate", UpdateCheck=UpdateCheck.Never, Storage="_LastAccessedDate", DbType="datetime NOT NULL")]
		public DateTime LastAccessedDate
		{
			get { return this._LastAccessedDate; }

			set
			{
				if (this._LastAccessedDate != value)
				{
				
                    this.OnLastAccessedDateChanging(value);
					this.SendPropertyChanging();
					this._LastAccessedDate = value;
					this.SendPropertyChanged("LastAccessedDate");
					this.OnLastAccessedDateChanged();
				}

			}

		}

		
		[Column(Name="CreatedDate", UpdateCheck=UpdateCheck.Never, Storage="_CreatedDate", DbType="datetime NOT NULL")]
		public DateTime CreatedDate
		{
			get { return this._CreatedDate; }

			set
			{
				if (this._CreatedDate != value)
				{
				
                    this.OnCreatedDateChanging(value);
					this.SendPropertyChanging();
					this._CreatedDate = value;
					this.SendPropertyChanged("CreatedDate");
					this.OnCreatedDateChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_Users_ApiSession", Storage="_User", ThisKey="UserId", IsForeignKey=true)]
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
						previousValue.ApiSessions.Remove(this);
					}

					this._User.Entity = value;
					if (value != null)
					{
						value.ApiSessions.Add(this);
						
						this._UserId = value.UserId;
						
					}

					else
					{
						
						this._UserId = default(int);
						
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

