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
	[Table(Name="dbo.PaymentInfo")]
	public partial class PaymentInfo : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private int? _AuNetCustId;
		
		private int? _AuNetCustPayId;
		
		private Guid? _SageBankGuid;
		
		private Guid? _SageCardGuid;
		
		private string _MaskedAccount;
		
		private string _MaskedCard;
		
		private string _Expires;
		
		private bool? _Testing;
		
		private string _PreferredGivingType;
		
		private string _PreferredPaymentType;
		
		private string _Routing;
		
		private string _FirstName;
		
		private string _MiddleInitial;
		
		private string _LastName;
		
		private string _Suffix;
		
		private string _Address;
		
		private string _City;
		
		private string _State;
		
		private string _Zip;
		
		private string _Phone;
		
		private int? _TbnBankVaultId;
		
		private int? _TbnCardVaultId;
		
		private int? _AuNetCustPayBankId;
		
		private string _BluePayCardVaultId;
		
		private string _Address2;
		
		private string _Country;
		
   		
    	
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnAuNetCustIdChanging(int? value);
		partial void OnAuNetCustIdChanged();
		
		partial void OnAuNetCustPayIdChanging(int? value);
		partial void OnAuNetCustPayIdChanged();
		
		partial void OnSageBankGuidChanging(Guid? value);
		partial void OnSageBankGuidChanged();
		
		partial void OnSageCardGuidChanging(Guid? value);
		partial void OnSageCardGuidChanged();
		
		partial void OnMaskedAccountChanging(string value);
		partial void OnMaskedAccountChanged();
		
		partial void OnMaskedCardChanging(string value);
		partial void OnMaskedCardChanged();
		
		partial void OnExpiresChanging(string value);
		partial void OnExpiresChanged();
		
		partial void OnTestingChanging(bool? value);
		partial void OnTestingChanged();
		
		partial void OnPreferredGivingTypeChanging(string value);
		partial void OnPreferredGivingTypeChanged();
		
		partial void OnPreferredPaymentTypeChanging(string value);
		partial void OnPreferredPaymentTypeChanged();
		
		partial void OnRoutingChanging(string value);
		partial void OnRoutingChanged();
		
		partial void OnFirstNameChanging(string value);
		partial void OnFirstNameChanged();
		
		partial void OnMiddleInitialChanging(string value);
		partial void OnMiddleInitialChanged();
		
		partial void OnLastNameChanging(string value);
		partial void OnLastNameChanged();
		
		partial void OnSuffixChanging(string value);
		partial void OnSuffixChanged();
		
		partial void OnAddressChanging(string value);
		partial void OnAddressChanged();
		
		partial void OnCityChanging(string value);
		partial void OnCityChanged();
		
		partial void OnStateChanging(string value);
		partial void OnStateChanged();
		
		partial void OnZipChanging(string value);
		partial void OnZipChanged();
		
		partial void OnPhoneChanging(string value);
		partial void OnPhoneChanged();
		
		partial void OnTbnBankVaultIdChanging(int? value);
		partial void OnTbnBankVaultIdChanged();
		
		partial void OnTbnCardVaultIdChanging(int? value);
		partial void OnTbnCardVaultIdChanged();
		
		partial void OnAuNetCustPayBankIdChanging(int? value);
		partial void OnAuNetCustPayBankIdChanged();
		
		partial void OnBluePayCardVaultIdChanging(string value);
		partial void OnBluePayCardVaultIdChanged();
		
		partial void OnAddress2Changing(string value);
		partial void OnAddress2Changed();
		
		partial void OnCountryChanging(string value);
		partial void OnCountryChanged();
		
    #endregion
		public PaymentInfo()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
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

		
		[Column(Name="AuNetCustId", UpdateCheck=UpdateCheck.Never, Storage="_AuNetCustId", DbType="int")]
		public int? AuNetCustId
		{
			get { return this._AuNetCustId; }

			set
			{
				if (this._AuNetCustId != value)
				{
				
                    this.OnAuNetCustIdChanging(value);
					this.SendPropertyChanging();
					this._AuNetCustId = value;
					this.SendPropertyChanged("AuNetCustId");
					this.OnAuNetCustIdChanged();
				}

			}

		}

		
		[Column(Name="AuNetCustPayId", UpdateCheck=UpdateCheck.Never, Storage="_AuNetCustPayId", DbType="int")]
		public int? AuNetCustPayId
		{
			get { return this._AuNetCustPayId; }

			set
			{
				if (this._AuNetCustPayId != value)
				{
				
                    this.OnAuNetCustPayIdChanging(value);
					this.SendPropertyChanging();
					this._AuNetCustPayId = value;
					this.SendPropertyChanged("AuNetCustPayId");
					this.OnAuNetCustPayIdChanged();
				}

			}

		}

		
		[Column(Name="SageBankGuid", UpdateCheck=UpdateCheck.Never, Storage="_SageBankGuid", DbType="uniqueidentifier")]
		public Guid? SageBankGuid
		{
			get { return this._SageBankGuid; }

			set
			{
				if (this._SageBankGuid != value)
				{
				
                    this.OnSageBankGuidChanging(value);
					this.SendPropertyChanging();
					this._SageBankGuid = value;
					this.SendPropertyChanged("SageBankGuid");
					this.OnSageBankGuidChanged();
				}

			}

		}

		
		[Column(Name="SageCardGuid", UpdateCheck=UpdateCheck.Never, Storage="_SageCardGuid", DbType="uniqueidentifier")]
		public Guid? SageCardGuid
		{
			get { return this._SageCardGuid; }

			set
			{
				if (this._SageCardGuid != value)
				{
				
                    this.OnSageCardGuidChanging(value);
					this.SendPropertyChanging();
					this._SageCardGuid = value;
					this.SendPropertyChanged("SageCardGuid");
					this.OnSageCardGuidChanged();
				}

			}

		}

		
		[Column(Name="MaskedAccount", UpdateCheck=UpdateCheck.Never, Storage="_MaskedAccount", DbType="nvarchar(30)")]
		public string MaskedAccount
		{
			get { return this._MaskedAccount; }

			set
			{
				if (this._MaskedAccount != value)
				{
				
                    this.OnMaskedAccountChanging(value);
					this.SendPropertyChanging();
					this._MaskedAccount = value;
					this.SendPropertyChanged("MaskedAccount");
					this.OnMaskedAccountChanged();
				}

			}

		}

		
		[Column(Name="MaskedCard", UpdateCheck=UpdateCheck.Never, Storage="_MaskedCard", DbType="nvarchar(30)")]
		public string MaskedCard
		{
			get { return this._MaskedCard; }

			set
			{
				if (this._MaskedCard != value)
				{
				
                    this.OnMaskedCardChanging(value);
					this.SendPropertyChanging();
					this._MaskedCard = value;
					this.SendPropertyChanged("MaskedCard");
					this.OnMaskedCardChanged();
				}

			}

		}

		
		[Column(Name="Expires", UpdateCheck=UpdateCheck.Never, Storage="_Expires", DbType="nvarchar(10)")]
		public string Expires
		{
			get { return this._Expires; }

			set
			{
				if (this._Expires != value)
				{
				
                    this.OnExpiresChanging(value);
					this.SendPropertyChanging();
					this._Expires = value;
					this.SendPropertyChanged("Expires");
					this.OnExpiresChanged();
				}

			}

		}

		
		[Column(Name="testing", UpdateCheck=UpdateCheck.Never, Storage="_Testing", DbType="bit")]
		public bool? Testing
		{
			get { return this._Testing; }

			set
			{
				if (this._Testing != value)
				{
				
                    this.OnTestingChanging(value);
					this.SendPropertyChanging();
					this._Testing = value;
					this.SendPropertyChanged("Testing");
					this.OnTestingChanged();
				}

			}

		}

		
		[Column(Name="PreferredGivingType", UpdateCheck=UpdateCheck.Never, Storage="_PreferredGivingType", DbType="nvarchar(2)")]
		public string PreferredGivingType
		{
			get { return this._PreferredGivingType; }

			set
			{
				if (this._PreferredGivingType != value)
				{
				
                    this.OnPreferredGivingTypeChanging(value);
					this.SendPropertyChanging();
					this._PreferredGivingType = value;
					this.SendPropertyChanged("PreferredGivingType");
					this.OnPreferredGivingTypeChanged();
				}

			}

		}

		
		[Column(Name="PreferredPaymentType", UpdateCheck=UpdateCheck.Never, Storage="_PreferredPaymentType", DbType="nvarchar(2)")]
		public string PreferredPaymentType
		{
			get { return this._PreferredPaymentType; }

			set
			{
				if (this._PreferredPaymentType != value)
				{
				
                    this.OnPreferredPaymentTypeChanging(value);
					this.SendPropertyChanging();
					this._PreferredPaymentType = value;
					this.SendPropertyChanged("PreferredPaymentType");
					this.OnPreferredPaymentTypeChanged();
				}

			}

		}

		
		[Column(Name="Routing", UpdateCheck=UpdateCheck.Never, Storage="_Routing", DbType="nvarchar(10)")]
		public string Routing
		{
			get { return this._Routing; }

			set
			{
				if (this._Routing != value)
				{
				
                    this.OnRoutingChanging(value);
					this.SendPropertyChanging();
					this._Routing = value;
					this.SendPropertyChanged("Routing");
					this.OnRoutingChanged();
				}

			}

		}

		
		[Column(Name="FirstName", UpdateCheck=UpdateCheck.Never, Storage="_FirstName", DbType="nvarchar(50)")]
		public string FirstName
		{
			get { return this._FirstName; }

			set
			{
				if (this._FirstName != value)
				{
				
                    this.OnFirstNameChanging(value);
					this.SendPropertyChanging();
					this._FirstName = value;
					this.SendPropertyChanged("FirstName");
					this.OnFirstNameChanged();
				}

			}

		}

		
		[Column(Name="MiddleInitial", UpdateCheck=UpdateCheck.Never, Storage="_MiddleInitial", DbType="nvarchar(10)")]
		public string MiddleInitial
		{
			get { return this._MiddleInitial; }

			set
			{
				if (this._MiddleInitial != value)
				{
				
                    this.OnMiddleInitialChanging(value);
					this.SendPropertyChanging();
					this._MiddleInitial = value;
					this.SendPropertyChanged("MiddleInitial");
					this.OnMiddleInitialChanged();
				}

			}

		}

		
		[Column(Name="LastName", UpdateCheck=UpdateCheck.Never, Storage="_LastName", DbType="nvarchar(50)")]
		public string LastName
		{
			get { return this._LastName; }

			set
			{
				if (this._LastName != value)
				{
				
                    this.OnLastNameChanging(value);
					this.SendPropertyChanging();
					this._LastName = value;
					this.SendPropertyChanged("LastName");
					this.OnLastNameChanged();
				}

			}

		}

		
		[Column(Name="Suffix", UpdateCheck=UpdateCheck.Never, Storage="_Suffix", DbType="nvarchar(10)")]
		public string Suffix
		{
			get { return this._Suffix; }

			set
			{
				if (this._Suffix != value)
				{
				
                    this.OnSuffixChanging(value);
					this.SendPropertyChanging();
					this._Suffix = value;
					this.SendPropertyChanged("Suffix");
					this.OnSuffixChanged();
				}

			}

		}

		
		[Column(Name="Address", UpdateCheck=UpdateCheck.Never, Storage="_Address", DbType="nvarchar(50)")]
		public string Address
		{
			get { return this._Address; }

			set
			{
				if (this._Address != value)
				{
				
                    this.OnAddressChanging(value);
					this.SendPropertyChanging();
					this._Address = value;
					this.SendPropertyChanged("Address");
					this.OnAddressChanged();
				}

			}

		}

		
		[Column(Name="City", UpdateCheck=UpdateCheck.Never, Storage="_City", DbType="nvarchar(50)")]
		public string City
		{
			get { return this._City; }

			set
			{
				if (this._City != value)
				{
				
                    this.OnCityChanging(value);
					this.SendPropertyChanging();
					this._City = value;
					this.SendPropertyChanged("City");
					this.OnCityChanged();
				}

			}

		}

		
		[Column(Name="State", UpdateCheck=UpdateCheck.Never, Storage="_State", DbType="nvarchar(10)")]
		public string State
		{
			get { return this._State; }

			set
			{
				if (this._State != value)
				{
				
                    this.OnStateChanging(value);
					this.SendPropertyChanging();
					this._State = value;
					this.SendPropertyChanged("State");
					this.OnStateChanged();
				}

			}

		}

		
		[Column(Name="Zip", UpdateCheck=UpdateCheck.Never, Storage="_Zip", DbType="nvarchar(15)")]
		public string Zip
		{
			get { return this._Zip; }

			set
			{
				if (this._Zip != value)
				{
				
                    this.OnZipChanging(value);
					this.SendPropertyChanging();
					this._Zip = value;
					this.SendPropertyChanged("Zip");
					this.OnZipChanged();
				}

			}

		}

		
		[Column(Name="Phone", UpdateCheck=UpdateCheck.Never, Storage="_Phone", DbType="nvarchar(25)")]
		public string Phone
		{
			get { return this._Phone; }

			set
			{
				if (this._Phone != value)
				{
				
                    this.OnPhoneChanging(value);
					this.SendPropertyChanging();
					this._Phone = value;
					this.SendPropertyChanged("Phone");
					this.OnPhoneChanged();
				}

			}

		}

		
		[Column(Name="TbnBankVaultId", UpdateCheck=UpdateCheck.Never, Storage="_TbnBankVaultId", DbType="int")]
		public int? TbnBankVaultId
		{
			get { return this._TbnBankVaultId; }

			set
			{
				if (this._TbnBankVaultId != value)
				{
				
                    this.OnTbnBankVaultIdChanging(value);
					this.SendPropertyChanging();
					this._TbnBankVaultId = value;
					this.SendPropertyChanged("TbnBankVaultId");
					this.OnTbnBankVaultIdChanged();
				}

			}

		}

		
		[Column(Name="TbnCardVaultId", UpdateCheck=UpdateCheck.Never, Storage="_TbnCardVaultId", DbType="int")]
		public int? TbnCardVaultId
		{
			get { return this._TbnCardVaultId; }

			set
			{
				if (this._TbnCardVaultId != value)
				{
				
                    this.OnTbnCardVaultIdChanging(value);
					this.SendPropertyChanging();
					this._TbnCardVaultId = value;
					this.SendPropertyChanged("TbnCardVaultId");
					this.OnTbnCardVaultIdChanged();
				}

			}

		}

		
		[Column(Name="AuNetCustPayBankId", UpdateCheck=UpdateCheck.Never, Storage="_AuNetCustPayBankId", DbType="int")]
		public int? AuNetCustPayBankId
		{
			get { return this._AuNetCustPayBankId; }

			set
			{
				if (this._AuNetCustPayBankId != value)
				{
				
                    this.OnAuNetCustPayBankIdChanging(value);
					this.SendPropertyChanging();
					this._AuNetCustPayBankId = value;
					this.SendPropertyChanged("AuNetCustPayBankId");
					this.OnAuNetCustPayBankIdChanged();
				}

			}

		}

		
		[Column(Name="BluePayCardVaultId", UpdateCheck=UpdateCheck.Never, Storage="_BluePayCardVaultId", DbType="nvarchar(50)")]
		public string BluePayCardVaultId
		{
			get { return this._BluePayCardVaultId; }

			set
			{
				if (this._BluePayCardVaultId != value)
				{
				
                    this.OnBluePayCardVaultIdChanging(value);
					this.SendPropertyChanging();
					this._BluePayCardVaultId = value;
					this.SendPropertyChanged("BluePayCardVaultId");
					this.OnBluePayCardVaultIdChanged();
				}

			}

		}

		
		[Column(Name="Address2", UpdateCheck=UpdateCheck.Never, Storage="_Address2", DbType="nvarchar(50)")]
		public string Address2
		{
			get { return this._Address2; }

			set
			{
				if (this._Address2 != value)
				{
				
                    this.OnAddress2Changing(value);
					this.SendPropertyChanging();
					this._Address2 = value;
					this.SendPropertyChanged("Address2");
					this.OnAddress2Changed();
				}

			}

		}

		
		[Column(Name="Country", UpdateCheck=UpdateCheck.Never, Storage="_Country", DbType="nvarchar(50)")]
		public string Country
		{
			get { return this._Country; }

			set
			{
				if (this._Country != value)
				{
				
                    this.OnCountryChanging(value);
					this.SendPropertyChanging();
					this._Country = value;
					this.SendPropertyChanged("Country");
					this.OnCountryChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_PaymentInfo_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.PaymentInfos.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.PaymentInfos.Add(this);
						
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

