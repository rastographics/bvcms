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
	[Table(Name="dbo.EmailQueueTo")]
	public partial class EmailQueueTo : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _PeopleId;
		
		private int? _OrgId;
		
		private DateTime? _Sent;
		
		private string _AddEmail;
		
		private Guid? _Guid;
		
		private string _Messageid;
		
		private int? _GoerSupportId;
		
   		
    	
		private EntityRef< EmailQueue> _EmailQueue;
		
		private EntityRef< Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnOrgIdChanging(int? value);
		partial void OnOrgIdChanged();
		
		partial void OnSentChanging(DateTime? value);
		partial void OnSentChanged();
		
		partial void OnAddEmailChanging(string value);
		partial void OnAddEmailChanged();
		
		partial void OnGuidChanging(Guid? value);
		partial void OnGuidChanged();
		
		partial void OnMessageidChanging(string value);
		partial void OnMessageidChanged();
		
		partial void OnGoerSupportIdChanging(int? value);
		partial void OnGoerSupportIdChanged();
		
    #endregion
		public EmailQueueTo()
		{
			
			
			this._EmailQueue = default(EntityRef< EmailQueue>); 
			
			this._Person = default(EntityRef< Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="int NOT NULL", IsPrimaryKey=true)]
		public int Id
		{
			get { return this._Id; }

			set
			{
				if (this._Id != value)
				{
				
					if (this._EmailQueue.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}

			}

		}

		
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

		
		[Column(Name="OrgId", UpdateCheck=UpdateCheck.Never, Storage="_OrgId", DbType="int")]
		public int? OrgId
		{
			get { return this._OrgId; }

			set
			{
				if (this._OrgId != value)
				{
				
                    this.OnOrgIdChanging(value);
					this.SendPropertyChanging();
					this._OrgId = value;
					this.SendPropertyChanged("OrgId");
					this.OnOrgIdChanged();
				}

			}

		}

		
		[Column(Name="Sent", UpdateCheck=UpdateCheck.Never, Storage="_Sent", DbType="datetime")]
		public DateTime? Sent
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

		
		[Column(Name="AddEmail", UpdateCheck=UpdateCheck.Never, Storage="_AddEmail", DbType="nvarchar")]
		public string AddEmail
		{
			get { return this._AddEmail; }

			set
			{
				if (this._AddEmail != value)
				{
				
                    this.OnAddEmailChanging(value);
					this.SendPropertyChanging();
					this._AddEmail = value;
					this.SendPropertyChanged("AddEmail");
					this.OnAddEmailChanged();
				}

			}

		}

		
		[Column(Name="guid", UpdateCheck=UpdateCheck.Never, Storage="_Guid", DbType="uniqueidentifier")]
		public Guid? Guid
		{
			get { return this._Guid; }

			set
			{
				if (this._Guid != value)
				{
				
                    this.OnGuidChanging(value);
					this.SendPropertyChanging();
					this._Guid = value;
					this.SendPropertyChanged("Guid");
					this.OnGuidChanged();
				}

			}

		}

		
		[Column(Name="messageid", UpdateCheck=UpdateCheck.Never, Storage="_Messageid", DbType="nvarchar(100)")]
		public string Messageid
		{
			get { return this._Messageid; }

			set
			{
				if (this._Messageid != value)
				{
				
                    this.OnMessageidChanging(value);
					this.SendPropertyChanging();
					this._Messageid = value;
					this.SendPropertyChanged("Messageid");
					this.OnMessageidChanged();
				}

			}

		}

		
		[Column(Name="GoerSupportId", UpdateCheck=UpdateCheck.Never, Storage="_GoerSupportId", DbType="int")]
		public int? GoerSupportId
		{
			get { return this._GoerSupportId; }

			set
			{
				if (this._GoerSupportId != value)
				{
				
                    this.OnGoerSupportIdChanging(value);
					this.SendPropertyChanging();
					this._GoerSupportId = value;
					this.SendPropertyChanged("GoerSupportId");
					this.OnGoerSupportIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_EmailQueueTo_EmailQueue", Storage="_EmailQueue", ThisKey="Id", IsForeignKey=true)]
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
						previousValue.EmailQueueTos.Remove(this);
					}

					this._EmailQueue.Entity = value;
					if (value != null)
					{
						value.EmailQueueTos.Add(this);
						
						this._Id = value.Id;
						
					}

					else
					{
						
						this._Id = default(int);
						
					}

					this.SendPropertyChanged("EmailQueue");
				}

			}

		}

		
		[Association(Name="FK_EmailQueueTo_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.EmailQueueTos.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.EmailQueueTos.Add(this);
						
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

