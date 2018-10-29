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
	[Table(Name="dbo.SMSItems")]
	public partial class SMSItem : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _ListID;
		
		private int _PeopleID;
		
		private bool _Sent;
		
		private string _Number;
		
		private bool _NoNumber;
		
		private bool _NoOptIn;
		
		private string _ResultStatus;
		
		private string _ErrorMessage;
		
   		
    	
		private EntityRef<Person> _Person;
		
		private EntityRef<SMSList> _SMSList;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnListIDChanging(int value);
		partial void OnListIDChanged();
		
		partial void OnPeopleIDChanging(int value);
		partial void OnPeopleIDChanged();
		
		partial void OnSentChanging(bool value);
		partial void OnSentChanged();
		
		partial void OnNumberChanging(string value);
		partial void OnNumberChanged();
		
		partial void OnNoNumberChanging(bool value);
		partial void OnNoNumberChanged();
		
		partial void OnNoOptInChanging(bool value);
		partial void OnNoOptInChanged();
		
		partial void OnResultStatusChanging(string value);
		partial void OnResultStatusChanged();
		
		partial void OnErrorMessageChanging(string value);
		partial void OnErrorMessageChanged();
		
    #endregion
		public SMSItem()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
			this._SMSList = default(EntityRef<SMSList>); 
			
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

		
		[Column(Name="ListID", UpdateCheck=UpdateCheck.Never, Storage="_ListID", DbType="int NOT NULL")]
		[IsForeignKey]
		public int ListID
		{
			get { return this._ListID; }

			set
			{
				if (this._ListID != value)
				{
				
					if (this._SMSList.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnListIDChanging(value);
					this.SendPropertyChanging();
					this._ListID = value;
					this.SendPropertyChanged("ListID");
					this.OnListIDChanged();
				}

			}

		}

		
		[Column(Name="PeopleID", UpdateCheck=UpdateCheck.Never, Storage="_PeopleID", DbType="int NOT NULL")]
		[IsForeignKey]
		public int PeopleID
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

		
		[Column(Name="Sent", UpdateCheck=UpdateCheck.Never, Storage="_Sent", DbType="bit NOT NULL")]
		public bool Sent
		{
			get { return this._Sent; }

			set
			{
				if (this._Sent != value)
				{
				
                    this.OnSentChanging(value);
					this.SendPropertyChanging();
					this._Sent = value;
					this.SendPropertyChanged("Sent");
					this.OnSentChanged();
				}

			}

		}

		
		[Column(Name="Number", UpdateCheck=UpdateCheck.Never, Storage="_Number", DbType="nvarchar(25) NOT NULL")]
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

		
		[Column(Name="NoNumber", UpdateCheck=UpdateCheck.Never, Storage="_NoNumber", DbType="bit NOT NULL")]
		public bool NoNumber
		{
			get { return this._NoNumber; }

			set
			{
				if (this._NoNumber != value)
				{
				
                    this.OnNoNumberChanging(value);
					this.SendPropertyChanging();
					this._NoNumber = value;
					this.SendPropertyChanged("NoNumber");
					this.OnNoNumberChanged();
				}

			}

		}

		
		[Column(Name="NoOptIn", UpdateCheck=UpdateCheck.Never, Storage="_NoOptIn", DbType="bit NOT NULL")]
		public bool NoOptIn
		{
			get { return this._NoOptIn; }

			set
			{
				if (this._NoOptIn != value)
				{
				
                    this.OnNoOptInChanging(value);
					this.SendPropertyChanging();
					this._NoOptIn = value;
					this.SendPropertyChanged("NoOptIn");
					this.OnNoOptInChanged();
				}

			}

		}

		
		[Column(Name="ResultStatus", UpdateCheck=UpdateCheck.WhenChanged, Storage="_ResultStatus", DbType="varchar(50)")]
		public string ResultStatus
		{
			get { return this._ResultStatus; }

			set
			{
				if (this._ResultStatus != value)
				{
				
                    this.OnResultStatusChanging(value);
					this.SendPropertyChanging();
					this._ResultStatus = value;
					this.SendPropertyChanged("ResultStatus");
					this.OnResultStatusChanged();
				}

			}

		}

		
		[Column(Name="ErrorMessage", UpdateCheck=UpdateCheck.WhenChanged, Storage="_ErrorMessage", DbType="varchar(300)")]
		public string ErrorMessage
		{
			get { return this._ErrorMessage; }

			set
			{
				if (this._ErrorMessage != value)
				{
				
                    this.OnErrorMessageChanging(value);
					this.SendPropertyChanging();
					this._ErrorMessage = value;
					this.SendPropertyChanged("ErrorMessage");
					this.OnErrorMessageChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_SMSItems_People", Storage="_Person", ThisKey="PeopleID", IsForeignKey=true)]
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
						previousValue.SMSItems.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.SMSItems.Add(this);
						
						this._PeopleID = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleID = default(int);
						
					}

					this.SendPropertyChanged("Person");
				}

			}

		}

		
		[Association(Name="FK_SMSItems_SMSList", Storage="_SMSList", ThisKey="ListID", IsForeignKey=true)]
		public SMSList SMSList
		{
			get { return this._SMSList.Entity; }

			set
			{
				SMSList previousValue = this._SMSList.Entity;
				if (((previousValue != value) 
							|| (this._SMSList.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._SMSList.Entity = null;
						previousValue.SMSItems.Remove(this);
					}

					this._SMSList.Entity = value;
					if (value != null)
					{
						value.SMSItems.Add(this);
						
						this._ListID = value.Id;
						
					}

					else
					{
						
						this._ListID = default(int);
						
					}

					this.SendPropertyChanged("SMSList");
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

