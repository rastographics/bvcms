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
	[Table(Name="dbo.Transaction")]
	public partial class Transaction : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private DateTime? _TransactionDate;
		
		private string _TransactionGateway;
		
		private int? _DatumId;
		
		private bool? _Testing;
		
		private decimal? _Amt;
		
		private string _ApprovalCode;
		
		private bool? _Approved;
		
		private string _TransactionId;
		
		private string _Message;
		
		private string _AuthCode;
		
		private decimal? _Amtdue;
		
		private string _Url;
		
		private string _Description;
		
		private string _Name;
		
		private string _Address;
		
		private string _City;
		
		private string _State;
		
		private string _Zip;
		
		private string _Phone;
		
		private string _Emails;
		
		private string _Participants;
		
		private int? _OrgId;
		
		private int? _OriginalId;
		
		private decimal? _Regfees;
		
		private decimal? _Donate;
		
		private string _Fund;
		
		private bool? _Financeonly;
		
		private bool? _Voided;
		
		private bool? _Credited;
		
		private bool? _Coupon;
		
		private bool? _Moneytran;
		
		private DateTime? _Settled;
		
		private DateTime? _Batch;
		
		private string _Batchref;
		
		private string _Batchtyp;
		
		private bool? _Fromsage;
		
		private int? _LoginPeopleId;
		
		private string _First;
		
		private string _MiddleInitial;
		
		private string _Last;
		
		private string _Suffix;
		
		private bool? _AdjustFee;
		
		private string _LastFourCC;
		
		private string _LastFourACH;
		
		private string _PaymentType;
		
		private string _Address2;
		
		private string _Country;
		
   		
   		private EntitySet<OrganizationMember> _OrganizationMembers;
		
   		private EntitySet<TransactionPerson> _TransactionPeople;
		
   		private EntitySet<Transaction> _Transactions;
		
    	
		private EntityRef<Person> _Person;
		
		private EntityRef<Transaction> _OriginalTransaction;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnTransactionDateChanging(DateTime? value);
		partial void OnTransactionDateChanged();
		
		partial void OnTransactionGatewayChanging(string value);
		partial void OnTransactionGatewayChanged();
		
		partial void OnDatumIdChanging(int? value);
		partial void OnDatumIdChanged();
		
		partial void OnTestingChanging(bool? value);
		partial void OnTestingChanged();
		
		partial void OnAmtChanging(decimal? value);
		partial void OnAmtChanged();
		
		partial void OnApprovalCodeChanging(string value);
		partial void OnApprovalCodeChanged();
		
		partial void OnApprovedChanging(bool? value);
		partial void OnApprovedChanged();
		
		partial void OnTransactionIdChanging(string value);
		partial void OnTransactionIdChanged();
		
		partial void OnMessageChanging(string value);
		partial void OnMessageChanged();
		
		partial void OnAuthCodeChanging(string value);
		partial void OnAuthCodeChanged();
		
		partial void OnAmtdueChanging(decimal? value);
		partial void OnAmtdueChanged();
		
		partial void OnUrlChanging(string value);
		partial void OnUrlChanged();
		
		partial void OnDescriptionChanging(string value);
		partial void OnDescriptionChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
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
		
		partial void OnEmailsChanging(string value);
		partial void OnEmailsChanged();
		
		partial void OnParticipantsChanging(string value);
		partial void OnParticipantsChanged();
		
		partial void OnOrgIdChanging(int? value);
		partial void OnOrgIdChanged();
		
		partial void OnOriginalIdChanging(int? value);
		partial void OnOriginalIdChanged();
		
		partial void OnRegfeesChanging(decimal? value);
		partial void OnRegfeesChanged();
		
		partial void OnDonateChanging(decimal? value);
		partial void OnDonateChanged();
		
		partial void OnFundChanging(string value);
		partial void OnFundChanged();
		
		partial void OnFinanceonlyChanging(bool? value);
		partial void OnFinanceonlyChanged();
		
		partial void OnVoidedChanging(bool? value);
		partial void OnVoidedChanged();
		
		partial void OnCreditedChanging(bool? value);
		partial void OnCreditedChanged();
		
		partial void OnCouponChanging(bool? value);
		partial void OnCouponChanged();
		
		partial void OnMoneytranChanging(bool? value);
		partial void OnMoneytranChanged();
		
		partial void OnSettledChanging(DateTime? value);
		partial void OnSettledChanged();
		
		partial void OnBatchChanging(DateTime? value);
		partial void OnBatchChanged();
		
		partial void OnBatchrefChanging(string value);
		partial void OnBatchrefChanged();
		
		partial void OnBatchtypChanging(string value);
		partial void OnBatchtypChanged();
		
		partial void OnFromsageChanging(bool? value);
		partial void OnFromsageChanged();
		
		partial void OnLoginPeopleIdChanging(int? value);
		partial void OnLoginPeopleIdChanged();
		
		partial void OnFirstChanging(string value);
		partial void OnFirstChanged();
		
		partial void OnMiddleInitialChanging(string value);
		partial void OnMiddleInitialChanged();
		
		partial void OnLastChanging(string value);
		partial void OnLastChanged();
		
		partial void OnSuffixChanging(string value);
		partial void OnSuffixChanged();
		
		partial void OnAdjustFeeChanging(bool? value);
		partial void OnAdjustFeeChanged();
		
		partial void OnLastFourCCChanging(string value);
		partial void OnLastFourCCChanged();
		
		partial void OnLastFourACHChanging(string value);
		partial void OnLastFourACHChanged();
		
		partial void OnPaymentTypeChanging(string value);
		partial void OnPaymentTypeChanged();
		
		partial void OnAddress2Changing(string value);
		partial void OnAddress2Changed();
		
		partial void OnCountryChanging(string value);
		partial void OnCountryChanged();
		
    #endregion
		public Transaction()
		{
			
			this._OrganizationMembers = new EntitySet<OrganizationMember>(new Action< OrganizationMember>(this.attach_OrganizationMembers), new Action< OrganizationMember>(this.detach_OrganizationMembers)); 
			
			this._TransactionPeople = new EntitySet<TransactionPerson>(new Action< TransactionPerson>(this.attach_TransactionPeople), new Action< TransactionPerson>(this.detach_TransactionPeople)); 
			
			this._Transactions = new EntitySet<Transaction>(new Action< Transaction>(this.attach_Transactions), new Action< Transaction>(this.detach_Transactions)); 
			
			
			this._Person = default(EntityRef<Person>); 
			
			this._OriginalTransaction = default(EntityRef<Transaction>); 
			
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

		
		[Column(Name="TransactionDate", UpdateCheck=UpdateCheck.Never, Storage="_TransactionDate", DbType="datetime")]
		public DateTime? TransactionDate
		{
			get { return this._TransactionDate; }

			set
			{
				if (this._TransactionDate != value)
				{
				
                    this.OnTransactionDateChanging(value);
					this.SendPropertyChanging();
					this._TransactionDate = value;
					this.SendPropertyChanged("TransactionDate");
					this.OnTransactionDateChanged();
				}

			}

		}

		
		[Column(Name="TransactionGateway", UpdateCheck=UpdateCheck.Never, Storage="_TransactionGateway", DbType="nvarchar(50)")]
		public string TransactionGateway
		{
			get { return this._TransactionGateway; }

			set
			{
				if (this._TransactionGateway != value)
				{
				
                    this.OnTransactionGatewayChanging(value);
					this.SendPropertyChanging();
					this._TransactionGateway = value;
					this.SendPropertyChanged("TransactionGateway");
					this.OnTransactionGatewayChanged();
				}

			}

		}

		
		[Column(Name="DatumId", UpdateCheck=UpdateCheck.Never, Storage="_DatumId", DbType="int")]
		public int? DatumId
		{
			get { return this._DatumId; }

			set
			{
				if (this._DatumId != value)
				{
				
                    this.OnDatumIdChanging(value);
					this.SendPropertyChanging();
					this._DatumId = value;
					this.SendPropertyChanged("DatumId");
					this.OnDatumIdChanged();
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

		
		[Column(Name="amt", UpdateCheck=UpdateCheck.Never, Storage="_Amt", DbType="money")]
		public decimal? Amt
		{
			get { return this._Amt; }

			set
			{
				if (this._Amt != value)
				{
				
                    this.OnAmtChanging(value);
					this.SendPropertyChanging();
					this._Amt = value;
					this.SendPropertyChanged("Amt");
					this.OnAmtChanged();
				}

			}

		}

		
		[Column(Name="ApprovalCode", UpdateCheck=UpdateCheck.Never, Storage="_ApprovalCode", DbType="nvarchar(150)")]
		public string ApprovalCode
		{
			get { return this._ApprovalCode; }

			set
			{
				if (this._ApprovalCode != value)
				{
				
                    this.OnApprovalCodeChanging(value);
					this.SendPropertyChanging();
					this._ApprovalCode = value;
					this.SendPropertyChanged("ApprovalCode");
					this.OnApprovalCodeChanged();
				}

			}

		}

		
		[Column(Name="Approved", UpdateCheck=UpdateCheck.Never, Storage="_Approved", DbType="bit")]
		public bool? Approved
		{
			get { return this._Approved; }

			set
			{
				if (this._Approved != value)
				{
				
                    this.OnApprovedChanging(value);
					this.SendPropertyChanging();
					this._Approved = value;
					this.SendPropertyChanged("Approved");
					this.OnApprovedChanged();
				}

			}

		}

		
		[Column(Name="TransactionId", UpdateCheck=UpdateCheck.Never, Storage="_TransactionId", DbType="nvarchar(50)")]
		public string TransactionId
		{
			get { return this._TransactionId; }

			set
			{
				if (this._TransactionId != value)
				{
				
                    this.OnTransactionIdChanging(value);
					this.SendPropertyChanging();
					this._TransactionId = value;
					this.SendPropertyChanged("TransactionId");
					this.OnTransactionIdChanged();
				}

			}

		}

		
		[Column(Name="Message", UpdateCheck=UpdateCheck.Never, Storage="_Message", DbType="nvarchar(150)")]
		public string Message
		{
			get { return this._Message; }

			set
			{
				if (this._Message != value)
				{
				
                    this.OnMessageChanging(value);
					this.SendPropertyChanging();
					this._Message = value;
					this.SendPropertyChanged("Message");
					this.OnMessageChanged();
				}

			}

		}

		
		[Column(Name="AuthCode", UpdateCheck=UpdateCheck.Never, Storage="_AuthCode", DbType="nvarchar(150)")]
		public string AuthCode
		{
			get { return this._AuthCode; }

			set
			{
				if (this._AuthCode != value)
				{
				
                    this.OnAuthCodeChanging(value);
					this.SendPropertyChanging();
					this._AuthCode = value;
					this.SendPropertyChanged("AuthCode");
					this.OnAuthCodeChanged();
				}

			}

		}

		
		[Column(Name="amtdue", UpdateCheck=UpdateCheck.Never, Storage="_Amtdue", DbType="money")]
		public decimal? Amtdue
		{
			get { return this._Amtdue; }

			set
			{
				if (this._Amtdue != value)
				{
				
                    this.OnAmtdueChanging(value);
					this.SendPropertyChanging();
					this._Amtdue = value;
					this.SendPropertyChanged("Amtdue");
					this.OnAmtdueChanged();
				}

			}

		}

		
		[Column(Name="URL", UpdateCheck=UpdateCheck.Never, Storage="_Url", DbType="nvarchar(300)")]
		public string Url
		{
			get { return this._Url; }

			set
			{
				if (this._Url != value)
				{
				
                    this.OnUrlChanging(value);
					this.SendPropertyChanging();
					this._Url = value;
					this.SendPropertyChanged("Url");
					this.OnUrlChanged();
				}

			}

		}

		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar(180)")]
		public string Description
		{
			get { return this._Description; }

			set
			{
				if (this._Description != value)
				{
				
                    this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}

			}

		}

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(100)")]
		public string Name
		{
			get { return this._Name; }

			set
			{
				if (this._Name != value)
				{
				
                    this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
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

		
		[Column(Name="State", UpdateCheck=UpdateCheck.Never, Storage="_State", DbType="nvarchar(20)")]
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

		
		[Column(Name="Phone", UpdateCheck=UpdateCheck.Never, Storage="_Phone", DbType="nvarchar(20)")]
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

		
		[Column(Name="Emails", UpdateCheck=UpdateCheck.Never, Storage="_Emails", DbType="nvarchar")]
		public string Emails
		{
			get { return this._Emails; }

			set
			{
				if (this._Emails != value)
				{
				
                    this.OnEmailsChanging(value);
					this.SendPropertyChanging();
					this._Emails = value;
					this.SendPropertyChanged("Emails");
					this.OnEmailsChanged();
				}

			}

		}

		
		[Column(Name="Participants", UpdateCheck=UpdateCheck.Never, Storage="_Participants", DbType="nvarchar")]
		public string Participants
		{
			get { return this._Participants; }

			set
			{
				if (this._Participants != value)
				{
				
                    this.OnParticipantsChanging(value);
					this.SendPropertyChanging();
					this._Participants = value;
					this.SendPropertyChanged("Participants");
					this.OnParticipantsChanged();
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

		
		[Column(Name="OriginalId", UpdateCheck=UpdateCheck.Never, Storage="_OriginalId", DbType="int")]
		[IsForeignKey]
		public int? OriginalId
		{
			get { return this._OriginalId; }

			set
			{
				if (this._OriginalId != value)
				{
				
					if (this._OriginalTransaction.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOriginalIdChanging(value);
					this.SendPropertyChanging();
					this._OriginalId = value;
					this.SendPropertyChanged("OriginalId");
					this.OnOriginalIdChanged();
				}

			}

		}

		
		[Column(Name="regfees", UpdateCheck=UpdateCheck.Never, Storage="_Regfees", DbType="money")]
		public decimal? Regfees
		{
			get { return this._Regfees; }

			set
			{
				if (this._Regfees != value)
				{
				
                    this.OnRegfeesChanging(value);
					this.SendPropertyChanging();
					this._Regfees = value;
					this.SendPropertyChanged("Regfees");
					this.OnRegfeesChanged();
				}

			}

		}

		
		[Column(Name="donate", UpdateCheck=UpdateCheck.Never, Storage="_Donate", DbType="money")]
		public decimal? Donate
		{
			get { return this._Donate; }

			set
			{
				if (this._Donate != value)
				{
				
                    this.OnDonateChanging(value);
					this.SendPropertyChanging();
					this._Donate = value;
					this.SendPropertyChanged("Donate");
					this.OnDonateChanged();
				}

			}

		}

		
		[Column(Name="fund", UpdateCheck=UpdateCheck.Never, Storage="_Fund", DbType="nvarchar(50)")]
		public string Fund
		{
			get { return this._Fund; }

			set
			{
				if (this._Fund != value)
				{
				
                    this.OnFundChanging(value);
					this.SendPropertyChanging();
					this._Fund = value;
					this.SendPropertyChanged("Fund");
					this.OnFundChanged();
				}

			}

		}

		
		[Column(Name="financeonly", UpdateCheck=UpdateCheck.Never, Storage="_Financeonly", DbType="bit")]
		public bool? Financeonly
		{
			get { return this._Financeonly; }

			set
			{
				if (this._Financeonly != value)
				{
				
                    this.OnFinanceonlyChanging(value);
					this.SendPropertyChanging();
					this._Financeonly = value;
					this.SendPropertyChanged("Financeonly");
					this.OnFinanceonlyChanged();
				}

			}

		}

		
		[Column(Name="voided", UpdateCheck=UpdateCheck.Never, Storage="_Voided", DbType="bit")]
		public bool? Voided
		{
			get { return this._Voided; }

			set
			{
				if (this._Voided != value)
				{
				
                    this.OnVoidedChanging(value);
					this.SendPropertyChanging();
					this._Voided = value;
					this.SendPropertyChanged("Voided");
					this.OnVoidedChanged();
				}

			}

		}

		
		[Column(Name="credited", UpdateCheck=UpdateCheck.Never, Storage="_Credited", DbType="bit")]
		public bool? Credited
		{
			get { return this._Credited; }

			set
			{
				if (this._Credited != value)
				{
				
                    this.OnCreditedChanging(value);
					this.SendPropertyChanging();
					this._Credited = value;
					this.SendPropertyChanged("Credited");
					this.OnCreditedChanged();
				}

			}

		}

		
		[Column(Name="coupon", UpdateCheck=UpdateCheck.Never, Storage="_Coupon", DbType="bit")]
		public bool? Coupon
		{
			get { return this._Coupon; }

			set
			{
				if (this._Coupon != value)
				{
				
                    this.OnCouponChanging(value);
					this.SendPropertyChanging();
					this._Coupon = value;
					this.SendPropertyChanged("Coupon");
					this.OnCouponChanged();
				}

			}

		}

		
		[Column(Name="moneytran", UpdateCheck=UpdateCheck.Never, Storage="_Moneytran", DbType="bit", IsDbGenerated=true)]
		public bool? Moneytran
		{
			get { return this._Moneytran; }

			set
			{
				if (this._Moneytran != value)
				{
				
                    this.OnMoneytranChanging(value);
					this.SendPropertyChanging();
					this._Moneytran = value;
					this.SendPropertyChanged("Moneytran");
					this.OnMoneytranChanged();
				}

			}

		}

		
		[Column(Name="settled", UpdateCheck=UpdateCheck.Never, Storage="_Settled", DbType="datetime")]
		public DateTime? Settled
		{
			get { return this._Settled; }

			set
			{
				if (this._Settled != value)
				{
				
                    this.OnSettledChanging(value);
					this.SendPropertyChanging();
					this._Settled = value;
					this.SendPropertyChanged("Settled");
					this.OnSettledChanged();
				}

			}

		}

		
		[Column(Name="batch", UpdateCheck=UpdateCheck.Never, Storage="_Batch", DbType="datetime")]
		public DateTime? Batch
		{
			get { return this._Batch; }

			set
			{
				if (this._Batch != value)
				{
				
                    this.OnBatchChanging(value);
					this.SendPropertyChanging();
					this._Batch = value;
					this.SendPropertyChanged("Batch");
					this.OnBatchChanged();
				}

			}

		}

		
		[Column(Name="batchref", UpdateCheck=UpdateCheck.Never, Storage="_Batchref", DbType="nvarchar(50)")]
		public string Batchref
		{
			get { return this._Batchref; }

			set
			{
				if (this._Batchref != value)
				{
				
                    this.OnBatchrefChanging(value);
					this.SendPropertyChanging();
					this._Batchref = value;
					this.SendPropertyChanged("Batchref");
					this.OnBatchrefChanged();
				}

			}

		}

		
		[Column(Name="batchtyp", UpdateCheck=UpdateCheck.Never, Storage="_Batchtyp", DbType="nvarchar(50)")]
		public string Batchtyp
		{
			get { return this._Batchtyp; }

			set
			{
				if (this._Batchtyp != value)
				{
				
                    this.OnBatchtypChanging(value);
					this.SendPropertyChanging();
					this._Batchtyp = value;
					this.SendPropertyChanged("Batchtyp");
					this.OnBatchtypChanged();
				}

			}

		}

		
		[Column(Name="fromsage", UpdateCheck=UpdateCheck.Never, Storage="_Fromsage", DbType="bit")]
		public bool? Fromsage
		{
			get { return this._Fromsage; }

			set
			{
				if (this._Fromsage != value)
				{
				
                    this.OnFromsageChanging(value);
					this.SendPropertyChanging();
					this._Fromsage = value;
					this.SendPropertyChanged("Fromsage");
					this.OnFromsageChanged();
				}

			}

		}

		
		[Column(Name="LoginPeopleId", UpdateCheck=UpdateCheck.Never, Storage="_LoginPeopleId", DbType="int")]
		[IsForeignKey]
		public int? LoginPeopleId
		{
			get { return this._LoginPeopleId; }

			set
			{
				if (this._LoginPeopleId != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnLoginPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._LoginPeopleId = value;
					this.SendPropertyChanged("LoginPeopleId");
					this.OnLoginPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="First", UpdateCheck=UpdateCheck.Never, Storage="_First", DbType="nvarchar(50)")]
		public string First
		{
			get { return this._First; }

			set
			{
				if (this._First != value)
				{
				
                    this.OnFirstChanging(value);
					this.SendPropertyChanging();
					this._First = value;
					this.SendPropertyChanged("First");
					this.OnFirstChanged();
				}

			}

		}

		
		[Column(Name="MiddleInitial", UpdateCheck=UpdateCheck.Never, Storage="_MiddleInitial", DbType="nvarchar(1)")]
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

		
		[Column(Name="Last", UpdateCheck=UpdateCheck.Never, Storage="_Last", DbType="nvarchar(50)")]
		public string Last
		{
			get { return this._Last; }

			set
			{
				if (this._Last != value)
				{
				
                    this.OnLastChanging(value);
					this.SendPropertyChanging();
					this._Last = value;
					this.SendPropertyChanged("Last");
					this.OnLastChanged();
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

		
		[Column(Name="AdjustFee", UpdateCheck=UpdateCheck.Never, Storage="_AdjustFee", DbType="bit")]
		public bool? AdjustFee
		{
			get { return this._AdjustFee; }

			set
			{
				if (this._AdjustFee != value)
				{
				
                    this.OnAdjustFeeChanging(value);
					this.SendPropertyChanging();
					this._AdjustFee = value;
					this.SendPropertyChanged("AdjustFee");
					this.OnAdjustFeeChanged();
				}

			}

		}

		
		[Column(Name="LastFourCC", UpdateCheck=UpdateCheck.Never, Storage="_LastFourCC", DbType="nvarchar(4)")]
		public string LastFourCC
		{
			get { return this._LastFourCC; }

			set
			{
				if (this._LastFourCC != value)
				{
				
                    this.OnLastFourCCChanging(value);
					this.SendPropertyChanging();
					this._LastFourCC = value;
					this.SendPropertyChanged("LastFourCC");
					this.OnLastFourCCChanged();
				}

			}

		}

		
		[Column(Name="LastFourACH", UpdateCheck=UpdateCheck.Never, Storage="_LastFourACH", DbType="nvarchar(4)")]
		public string LastFourACH
		{
			get { return this._LastFourACH; }

			set
			{
				if (this._LastFourACH != value)
				{
				
                    this.OnLastFourACHChanging(value);
					this.SendPropertyChanging();
					this._LastFourACH = value;
					this.SendPropertyChanged("LastFourACH");
					this.OnLastFourACHChanged();
				}

			}

		}

		
		[Column(Name="PaymentType", UpdateCheck=UpdateCheck.Never, Storage="_PaymentType", DbType="nvarchar(1)")]
		public string PaymentType
		{
			get { return this._PaymentType; }

			set
			{
				if (this._PaymentType != value)
				{
				
                    this.OnPaymentTypeChanging(value);
					this.SendPropertyChanging();
					this._PaymentType = value;
					this.SendPropertyChanged("PaymentType");
					this.OnPaymentTypeChanged();
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
   		
   		[Association(Name="FK_OrganizationMembers_Transaction", Storage="_OrganizationMembers", OtherKey="TranId")]
   		public EntitySet<OrganizationMember> OrganizationMembers
   		{
   		    get { return this._OrganizationMembers; }

			set	{ this._OrganizationMembers.Assign(value); }

   		}

		
   		[Association(Name="FK_TransactionPeople_Transaction", Storage="_TransactionPeople", OtherKey="Id")]
   		public EntitySet<TransactionPerson> TransactionPeople
   		{
   		    get { return this._TransactionPeople; }

			set	{ this._TransactionPeople.Assign(value); }

   		}

		
   		[Association(Name="Transactions__OriginalTransaction", Storage="_Transactions", OtherKey="OriginalId")]
   		public EntitySet<Transaction> Transactions
   		{
   		    get { return this._Transactions; }

			set	{ this._Transactions.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_Transaction_People", Storage="_Person", ThisKey="LoginPeopleId", IsForeignKey=true)]
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
						previousValue.Transactions.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.Transactions.Add(this);
						
						this._LoginPeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._LoginPeopleId = default(int?);
						
					}

					this.SendPropertyChanged("Person");
				}

			}

		}

		
		[Association(Name="Transactions__OriginalTransaction", Storage="_OriginalTransaction", ThisKey="OriginalId", IsForeignKey=true)]
		public Transaction OriginalTransaction
		{
			get { return this._OriginalTransaction.Entity; }

			set
			{
				Transaction previousValue = this._OriginalTransaction.Entity;
				if (((previousValue != value) 
							|| (this._OriginalTransaction.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._OriginalTransaction.Entity = null;
						previousValue.Transactions.Remove(this);
					}

					this._OriginalTransaction.Entity = value;
					if (value != null)
					{
						value.Transactions.Add(this);
						
						this._OriginalId = value.Id;
						
					}

					else
					{
						
						this._OriginalId = default(int?);
						
					}

					this.SendPropertyChanged("OriginalTransaction");
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

   		
		private void attach_OrganizationMembers(OrganizationMember entity)
		{
			this.SendPropertyChanging();
			entity.Transaction = this;
		}

		private void detach_OrganizationMembers(OrganizationMember entity)
		{
			this.SendPropertyChanging();
			entity.Transaction = null;
		}

		
		private void attach_TransactionPeople(TransactionPerson entity)
		{
			this.SendPropertyChanging();
			entity.Transaction = this;
		}

		private void detach_TransactionPeople(TransactionPerson entity)
		{
			this.SendPropertyChanging();
			entity.Transaction = null;
		}

		
		private void attach_Transactions(Transaction entity)
		{
			this.SendPropertyChanging();
			entity.OriginalTransaction = this;
		}

		private void detach_Transactions(Transaction entity)
		{
			this.SendPropertyChanging();
			entity.OriginalTransaction = null;
		}

		
	}

}

