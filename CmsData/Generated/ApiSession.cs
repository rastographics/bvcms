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
	[Table(Name="dbo.ApiSession")]
	public partial class ApiSession : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private Guid _SessionToken;
		
		private int? _Pin;
		
		private DateTime _LastAccessedDate;
		
		private DateTime _CreatedDate;
		
   		
    	
		private EntityRef< Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
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
			
			
			this._Person = default(EntityRef< Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL", IsPrimaryKey=true)]
		public int PeopleId
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
    	
		[Association(Name="FK_People_ApiSession", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.ApiSessions.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.ApiSessions.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int);
						
					}

					this.SendPropertyChanged("Person");
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

