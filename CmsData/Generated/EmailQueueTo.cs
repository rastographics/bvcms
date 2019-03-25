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
		
		private int? _Parent1;
		
		private int? _Parent2;
		
		private bool? _Bounced;
		
		private bool? _SpamReport;
		
		private bool? _Blocked;
		
		private bool? _Expired;
		
		private bool? _SpamContent;
		
		private bool? _Invalid;
		
		private bool? _BouncedAddress;
		
		private bool? _SpamReporting;
		
		private string _DomainFrom;
		
   		
    	
		private EntityRef<EmailQueue> _EmailQueue;
		
		private EntityRef<Person> _Person;
		
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
		
		partial void OnParent1Changing(int? value);
		partial void OnParent1Changed();
		
		partial void OnParent2Changing(int? value);
		partial void OnParent2Changed();
		
		partial void OnBouncedChanging(bool? value);
		partial void OnBouncedChanged();
		
		partial void OnSpamReportChanging(bool? value);
		partial void OnSpamReportChanged();
		
		partial void OnBlockedChanging(bool? value);
		partial void OnBlockedChanged();
		
		partial void OnExpiredChanging(bool? value);
		partial void OnExpiredChanged();
		
		partial void OnSpamContentChanging(bool? value);
		partial void OnSpamContentChanged();
		
		partial void OnInvalidChanging(bool? value);
		partial void OnInvalidChanged();
		
		partial void OnBouncedAddressChanging(bool? value);
		partial void OnBouncedAddressChanged();
		
		partial void OnSpamReportingChanging(bool? value);
		partial void OnSpamReportingChanged();
		
		partial void OnDomainFromChanging(string value);
		partial void OnDomainFromChanged();
		
    #endregion
		public EmailQueueTo()
		{
			
			
			this._EmailQueue = default(EntityRef<EmailQueue>); 
			
			this._Person = default(EntityRef<Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
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

		
		[Column(Name="Parent1", UpdateCheck=UpdateCheck.Never, Storage="_Parent1", DbType="int")]
		public int? Parent1
		{
			get { return this._Parent1; }

			set
			{
				if (this._Parent1 != value)
				{
				
                    this.OnParent1Changing(value);
					this.SendPropertyChanging();
					this._Parent1 = value;
					this.SendPropertyChanged("Parent1");
					this.OnParent1Changed();
				}

			}

		}

		
		[Column(Name="Parent2", UpdateCheck=UpdateCheck.Never, Storage="_Parent2", DbType="int")]
		public int? Parent2
		{
			get { return this._Parent2; }

			set
			{
				if (this._Parent2 != value)
				{
				
                    this.OnParent2Changing(value);
					this.SendPropertyChanging();
					this._Parent2 = value;
					this.SendPropertyChanged("Parent2");
					this.OnParent2Changed();
				}

			}

		}

		
		[Column(Name="Bounced", UpdateCheck=UpdateCheck.Never, Storage="_Bounced", DbType="bit")]
		public bool? Bounced
		{
			get { return this._Bounced; }

			set
			{
				if (this._Bounced != value)
				{
				
                    this.OnBouncedChanging(value);
					this.SendPropertyChanging();
					this._Bounced = value;
					this.SendPropertyChanged("Bounced");
					this.OnBouncedChanged();
				}

			}

		}

		
		[Column(Name="SpamReport", UpdateCheck=UpdateCheck.Never, Storage="_SpamReport", DbType="bit")]
		public bool? SpamReport
		{
			get { return this._SpamReport; }

			set
			{
				if (this._SpamReport != value)
				{
				
                    this.OnSpamReportChanging(value);
					this.SendPropertyChanging();
					this._SpamReport = value;
					this.SendPropertyChanged("SpamReport");
					this.OnSpamReportChanged();
				}

			}

		}

		
		[Column(Name="Blocked", UpdateCheck=UpdateCheck.Never, Storage="_Blocked", DbType="bit")]
		public bool? Blocked
		{
			get { return this._Blocked; }

			set
			{
				if (this._Blocked != value)
				{
				
                    this.OnBlockedChanging(value);
					this.SendPropertyChanging();
					this._Blocked = value;
					this.SendPropertyChanged("Blocked");
					this.OnBlockedChanged();
				}

			}

		}

		
		[Column(Name="Expired", UpdateCheck=UpdateCheck.Never, Storage="_Expired", DbType="bit")]
		public bool? Expired
		{
			get { return this._Expired; }

			set
			{
				if (this._Expired != value)
				{
				
                    this.OnExpiredChanging(value);
					this.SendPropertyChanging();
					this._Expired = value;
					this.SendPropertyChanged("Expired");
					this.OnExpiredChanged();
				}

			}

		}

		
		[Column(Name="SpamContent", UpdateCheck=UpdateCheck.Never, Storage="_SpamContent", DbType="bit")]
		public bool? SpamContent
		{
			get { return this._SpamContent; }

			set
			{
				if (this._SpamContent != value)
				{
				
                    this.OnSpamContentChanging(value);
					this.SendPropertyChanging();
					this._SpamContent = value;
					this.SendPropertyChanged("SpamContent");
					this.OnSpamContentChanged();
				}

			}

		}

		
		[Column(Name="Invalid", UpdateCheck=UpdateCheck.Never, Storage="_Invalid", DbType="bit")]
		public bool? Invalid
		{
			get { return this._Invalid; }

			set
			{
				if (this._Invalid != value)
				{
				
                    this.OnInvalidChanging(value);
					this.SendPropertyChanging();
					this._Invalid = value;
					this.SendPropertyChanged("Invalid");
					this.OnInvalidChanged();
				}

			}

		}

		
		[Column(Name="BouncedAddress", UpdateCheck=UpdateCheck.Never, Storage="_BouncedAddress", DbType="bit")]
		public bool? BouncedAddress
		{
			get { return this._BouncedAddress; }

			set
			{
				if (this._BouncedAddress != value)
				{
				
                    this.OnBouncedAddressChanging(value);
					this.SendPropertyChanging();
					this._BouncedAddress = value;
					this.SendPropertyChanged("BouncedAddress");
					this.OnBouncedAddressChanged();
				}

			}

		}

		
		[Column(Name="SpamReporting", UpdateCheck=UpdateCheck.Never, Storage="_SpamReporting", DbType="bit")]
		public bool? SpamReporting
		{
			get { return this._SpamReporting; }

			set
			{
				if (this._SpamReporting != value)
				{
				
                    this.OnSpamReportingChanging(value);
					this.SendPropertyChanging();
					this._SpamReporting = value;
					this.SendPropertyChanged("SpamReporting");
					this.OnSpamReportingChanged();
				}

			}

		}

		
		[Column(Name="DomainFrom", UpdateCheck=UpdateCheck.Never, Storage="_DomainFrom", DbType="varchar(30)")]
		public string DomainFrom
		{
			get { return this._DomainFrom; }

			set
			{
				if (this._DomainFrom != value)
				{
				
                    this.OnDomainFromChanging(value);
					this.SendPropertyChanging();
					this._DomainFrom = value;
					this.SendPropertyChanged("DomainFrom");
					this.OnDomainFromChanged();
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

