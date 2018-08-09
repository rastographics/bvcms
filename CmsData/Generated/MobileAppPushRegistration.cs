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
	[Table(Name="dbo.MobileAppPushRegistrations")]
	public partial class MobileAppPushRegistration : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _Type;
		
		private int _PeopleId;
		
		private string _RegistrationId;
		
		private int _Priority;
		
		private bool _Enabled;
		
		private int _Rebranded;
		
   		
    	
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnTypeChanging(int value);
		partial void OnTypeChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnRegistrationIdChanging(string value);
		partial void OnRegistrationIdChanged();
		
		partial void OnPriorityChanging(int value);
		partial void OnPriorityChanged();
		
		partial void OnEnabledChanging(bool value);
		partial void OnEnabledChanged();
		
		partial void OnRebrandedChanging(int value);
		partial void OnRebrandedChanged();
		
    #endregion
		public MobileAppPushRegistration()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
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

		
		[Column(Name="Type", UpdateCheck=UpdateCheck.Never, Storage="_Type", DbType="int NOT NULL")]
		public int Type
		{
			get { return this._Type; }

			set
			{
				if (this._Type != value)
				{
				
                    this.OnTypeChanging(value);
					this.SendPropertyChanging();
					this._Type = value;
					this.SendPropertyChanged("Type");
					this.OnTypeChanged();
				}

			}

		}

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL")]
		[IsForeignKey]
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

		
		[Column(Name="RegistrationId", UpdateCheck=UpdateCheck.Never, Storage="_RegistrationId", DbType="varchar NOT NULL")]
		public string RegistrationId
		{
			get { return this._RegistrationId; }

			set
			{
				if (this._RegistrationId != value)
				{
				
                    this.OnRegistrationIdChanging(value);
					this.SendPropertyChanging();
					this._RegistrationId = value;
					this.SendPropertyChanged("RegistrationId");
					this.OnRegistrationIdChanged();
				}

			}

		}

		
		[Column(Name="Priority", UpdateCheck=UpdateCheck.Never, Storage="_Priority", DbType="int NOT NULL")]
		public int Priority
		{
			get { return this._Priority; }

			set
			{
				if (this._Priority != value)
				{
				
                    this.OnPriorityChanging(value);
					this.SendPropertyChanging();
					this._Priority = value;
					this.SendPropertyChanged("Priority");
					this.OnPriorityChanged();
				}

			}

		}

		
		[Column(Name="Enabled", UpdateCheck=UpdateCheck.Never, Storage="_Enabled", DbType="bit NOT NULL")]
		public bool Enabled
		{
			get { return this._Enabled; }

			set
			{
				if (this._Enabled != value)
				{
				
                    this.OnEnabledChanging(value);
					this.SendPropertyChanging();
					this._Enabled = value;
					this.SendPropertyChanged("Enabled");
					this.OnEnabledChanged();
				}

			}

		}

		
		[Column(Name="rebranded", UpdateCheck=UpdateCheck.Never, Storage="_Rebranded", DbType="int NOT NULL")]
		public int Rebranded
		{
			get { return this._Rebranded; }

			set
			{
				if (this._Rebranded != value)
				{
				
                    this.OnRebrandedChanging(value);
					this.SendPropertyChanging();
					this._Rebranded = value;
					this.SendPropertyChanged("Rebranded");
					this.OnRebrandedChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_MobileAppPushRegistrations_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.MobileAppPushRegistrations.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.MobileAppPushRegistrations.Add(this);
						
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

