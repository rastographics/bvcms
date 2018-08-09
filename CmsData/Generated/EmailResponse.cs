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
	[Table(Name="dbo.EmailResponses")]
	public partial class EmailResponse : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _EmailQueueId;
		
		private int _PeopleId;
		
		private string _Type;
		
		private DateTime _Dt;
		
   		
    	
		private EntityRef<EmailQueue> _EmailQueue;
		
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnEmailQueueIdChanging(int value);
		partial void OnEmailQueueIdChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnTypeChanging(string value);
		partial void OnTypeChanged();
		
		partial void OnDtChanging(DateTime value);
		partial void OnDtChanged();
		
    #endregion
		public EmailResponse()
		{
			
			
			this._EmailQueue = default(EntityRef<EmailQueue>); 
			
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

		
		[Column(Name="EmailQueueId", UpdateCheck=UpdateCheck.Never, Storage="_EmailQueueId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int EmailQueueId
		{
			get { return this._EmailQueueId; }

			set
			{
				if (this._EmailQueueId != value)
				{
				
					if (this._EmailQueue.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnEmailQueueIdChanging(value);
					this.SendPropertyChanging();
					this._EmailQueueId = value;
					this.SendPropertyChanged("EmailQueueId");
					this.OnEmailQueueIdChanged();
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

		
		[Column(Name="Type", UpdateCheck=UpdateCheck.Never, Storage="_Type", DbType="char(1) NOT NULL")]
		public string Type
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

		
		[Column(Name="Dt", UpdateCheck=UpdateCheck.Never, Storage="_Dt", DbType="datetime NOT NULL")]
		public DateTime Dt
		{
			get { return this._Dt; }

			set
			{
				if (this._Dt != value)
				{
				
                    this.OnDtChanging(value);
					this.SendPropertyChanging();
					this._Dt = value;
					this.SendPropertyChanged("Dt");
					this.OnDtChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_EmailResponses_EmailQueue", Storage="_EmailQueue", ThisKey="EmailQueueId", IsForeignKey=true)]
		public EmailQueue EmailQueue
		{
			get { return this._EmailQueue.Entity; }

			set
			{
				EmailQueue previousValue = this._EmailQueue.Entity;
				if (((previousValue != value) 
							|| (this._EmailQueue.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._EmailQueue.Entity = null;
						previousValue.EmailResponses.Remove(this);
					}

					this._EmailQueue.Entity = value;
					if (value != null)
					{
						value.EmailResponses.Add(this);
						
						this._EmailQueueId = value.Id;
						
					}

					else
					{
						
						this._EmailQueueId = default(int);
						
					}

					this.SendPropertyChanged("EmailQueue");
				}

			}

		}

		
		[Association(Name="FK_EmailResponses_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.EmailResponses.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.EmailResponses.Add(this);
						
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

