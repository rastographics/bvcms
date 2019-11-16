using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Transaction")]
    public partial class Transaction : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            _OrganizationMembers = new EntitySet<OrganizationMember>(new Action<OrganizationMember>(attach_OrganizationMembers), new Action<OrganizationMember>(detach_OrganizationMembers));

            _TransactionPeople = new EntitySet<TransactionPerson>(new Action<TransactionPerson>(attach_TransactionPeople), new Action<TransactionPerson>(detach_TransactionPeople));

            _Transactions = new EntitySet<Transaction>(new Action<Transaction>(attach_Transactions), new Action<Transaction>(detach_Transactions));

            _Person = default(EntityRef<Person>);

            _OriginalTransaction = default(EntityRef<Transaction>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

        [Column(Name = "TransactionDate", UpdateCheck = UpdateCheck.Never, Storage = "_TransactionDate", DbType = "datetime")]
        public DateTime? TransactionDate
        {
            get => _TransactionDate;

            set
            {
                if (_TransactionDate != value)
                {
                    OnTransactionDateChanging(value);
                    SendPropertyChanging();
                    _TransactionDate = value;
                    SendPropertyChanged("TransactionDate");
                    OnTransactionDateChanged();
                }
            }
        }

        [Column(Name = "TransactionGateway", UpdateCheck = UpdateCheck.Never, Storage = "_TransactionGateway", DbType = "nvarchar(50)")]
        public string TransactionGateway
        {
            get => _TransactionGateway;

            set
            {
                if (_TransactionGateway != value)
                {
                    OnTransactionGatewayChanging(value);
                    SendPropertyChanging();
                    _TransactionGateway = value;
                    SendPropertyChanged("TransactionGateway");
                    OnTransactionGatewayChanged();
                }
            }
        }

        [Column(Name = "DatumId", UpdateCheck = UpdateCheck.Never, Storage = "_DatumId", DbType = "int")]
        public int? DatumId
        {
            get => _DatumId;

            set
            {
                if (_DatumId != value)
                {
                    OnDatumIdChanging(value);
                    SendPropertyChanging();
                    _DatumId = value;
                    SendPropertyChanged("DatumId");
                    OnDatumIdChanged();
                }
            }
        }

        [Column(Name = "testing", UpdateCheck = UpdateCheck.Never, Storage = "_Testing", DbType = "bit")]
        public bool? Testing
        {
            get => _Testing;

            set
            {
                if (_Testing != value)
                {
                    OnTestingChanging(value);
                    SendPropertyChanging();
                    _Testing = value;
                    SendPropertyChanged("Testing");
                    OnTestingChanged();
                }
            }
        }

        [Column(Name = "amt", UpdateCheck = UpdateCheck.Never, Storage = "_Amt", DbType = "money")]
        public decimal? Amt
        {
            get => _Amt;

            set
            {
                if (_Amt != value)
                {
                    OnAmtChanging(value);
                    SendPropertyChanging();
                    _Amt = value;
                    SendPropertyChanged("Amt");
                    OnAmtChanged();
                }
            }
        }

        [Column(Name = "ApprovalCode", UpdateCheck = UpdateCheck.Never, Storage = "_ApprovalCode", DbType = "nvarchar(150)")]
        public string ApprovalCode
        {
            get => _ApprovalCode;

            set
            {
                if (_ApprovalCode != value)
                {
                    OnApprovalCodeChanging(value);
                    SendPropertyChanging();
                    _ApprovalCode = value;
                    SendPropertyChanged("ApprovalCode");
                    OnApprovalCodeChanged();
                }
            }
        }

        [Column(Name = "Approved", UpdateCheck = UpdateCheck.Never, Storage = "_Approved", DbType = "bit")]
        public bool? Approved
        {
            get => _Approved;

            set
            {
                if (_Approved != value)
                {
                    OnApprovedChanging(value);
                    SendPropertyChanging();
                    _Approved = value;
                    SendPropertyChanged("Approved");
                    OnApprovedChanged();
                }
            }
        }

        [Column(Name = "TransactionId", UpdateCheck = UpdateCheck.Never, Storage = "_TransactionId", DbType = "nvarchar(50)")]
        public string TransactionId
        {
            get => _TransactionId;

            set
            {
                if (_TransactionId != value)
                {
                    OnTransactionIdChanging(value);
                    SendPropertyChanging();
                    _TransactionId = value;
                    SendPropertyChanged("TransactionId");
                    OnTransactionIdChanged();
                }
            }
        }

        [Column(Name = "Message", UpdateCheck = UpdateCheck.Never, Storage = "_Message", DbType = "nvarchar(150)")]
        public string Message
        {
            get => _Message;

            set
            {
                if (_Message != value)
                {
                    OnMessageChanging(value);
                    SendPropertyChanging();
                    _Message = value;
                    SendPropertyChanged("Message");
                    OnMessageChanged();
                }
            }
        }

        [Column(Name = "AuthCode", UpdateCheck = UpdateCheck.Never, Storage = "_AuthCode", DbType = "nvarchar(150)")]
        public string AuthCode
        {
            get => _AuthCode;

            set
            {
                if (_AuthCode != value)
                {
                    OnAuthCodeChanging(value);
                    SendPropertyChanging();
                    _AuthCode = value;
                    SendPropertyChanged("AuthCode");
                    OnAuthCodeChanged();
                }
            }
        }

        [Column(Name = "amtdue", UpdateCheck = UpdateCheck.Never, Storage = "_Amtdue", DbType = "money")]
        public decimal? Amtdue
        {
            get => _Amtdue;

            set
            {
                if (_Amtdue != value)
                {
                    OnAmtdueChanging(value);
                    SendPropertyChanging();
                    _Amtdue = value;
                    SendPropertyChanged("Amtdue");
                    OnAmtdueChanged();
                }
            }
        }

        [Column(Name = "URL", UpdateCheck = UpdateCheck.Never, Storage = "_Url", DbType = "nvarchar(300)")]
        public string Url
        {
            get => _Url;

            set
            {
                if (_Url != value)
                {
                    OnUrlChanging(value);
                    SendPropertyChanging();
                    _Url = value;
                    SendPropertyChanged("Url");
                    OnUrlChanged();
                }
            }
        }

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(180)")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    OnDescriptionChanging(value);
                    SendPropertyChanging();
                    _Description = value;
                    SendPropertyChanged("Description");
                    OnDescriptionChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(100)")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    OnNameChanging(value);
                    SendPropertyChanging();
                    _Name = value;
                    SendPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }

        [Column(Name = "Address", UpdateCheck = UpdateCheck.Never, Storage = "_Address", DbType = "nvarchar(50)")]
        public string Address
        {
            get => _Address;

            set
            {
                if (_Address != value)
                {
                    OnAddressChanging(value);
                    SendPropertyChanging();
                    _Address = value;
                    SendPropertyChanged("Address");
                    OnAddressChanged();
                }
            }
        }

        [Column(Name = "City", UpdateCheck = UpdateCheck.Never, Storage = "_City", DbType = "nvarchar(50)")]
        public string City
        {
            get => _City;

            set
            {
                if (_City != value)
                {
                    OnCityChanging(value);
                    SendPropertyChanging();
                    _City = value;
                    SendPropertyChanged("City");
                    OnCityChanged();
                }
            }
        }

        [Column(Name = "State", UpdateCheck = UpdateCheck.Never, Storage = "_State", DbType = "nvarchar(20)")]
        public string State
        {
            get => _State;

            set
            {
                if (_State != value)
                {
                    OnStateChanging(value);
                    SendPropertyChanging();
                    _State = value;
                    SendPropertyChanged("State");
                    OnStateChanged();
                }
            }
        }

        [Column(Name = "Zip", UpdateCheck = UpdateCheck.Never, Storage = "_Zip", DbType = "nvarchar(15)")]
        public string Zip
        {
            get => _Zip;

            set
            {
                if (_Zip != value)
                {
                    OnZipChanging(value);
                    SendPropertyChanging();
                    _Zip = value;
                    SendPropertyChanged("Zip");
                    OnZipChanged();
                }
            }
        }

        [Column(Name = "Phone", UpdateCheck = UpdateCheck.Never, Storage = "_Phone", DbType = "nvarchar(20)")]
        public string Phone
        {
            get => _Phone;

            set
            {
                if (_Phone != value)
                {
                    OnPhoneChanging(value);
                    SendPropertyChanging();
                    _Phone = value;
                    SendPropertyChanged("Phone");
                    OnPhoneChanged();
                }
            }
        }

        [Column(Name = "Emails", UpdateCheck = UpdateCheck.Never, Storage = "_Emails", DbType = "nvarchar")]
        public string Emails
        {
            get => _Emails;

            set
            {
                if (_Emails != value)
                {
                    OnEmailsChanging(value);
                    SendPropertyChanging();
                    _Emails = value;
                    SendPropertyChanged("Emails");
                    OnEmailsChanged();
                }
            }
        }

        [Column(Name = "Participants", UpdateCheck = UpdateCheck.Never, Storage = "_Participants", DbType = "nvarchar")]
        public string Participants
        {
            get => _Participants;

            set
            {
                if (_Participants != value)
                {
                    OnParticipantsChanging(value);
                    SendPropertyChanging();
                    _Participants = value;
                    SendPropertyChanged("Participants");
                    OnParticipantsChanged();
                }
            }
        }

        [Column(Name = "OrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OrgId", DbType = "int")]
        public int? OrgId
        {
            get => _OrgId;

            set
            {
                if (_OrgId != value)
                {
                    OnOrgIdChanging(value);
                    SendPropertyChanging();
                    _OrgId = value;
                    SendPropertyChanged("OrgId");
                    OnOrgIdChanged();
                }
            }
        }

        [Column(Name = "OriginalId", UpdateCheck = UpdateCheck.Never, Storage = "_OriginalId", DbType = "int")]
        [IsForeignKey]
        public int? OriginalId
        {
            get => _OriginalId;

            set
            {
                if (_OriginalId != value)
                {
                    if (_OriginalTransaction.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOriginalIdChanging(value);
                    SendPropertyChanging();
                    _OriginalId = value;
                    SendPropertyChanged("OriginalId");
                    OnOriginalIdChanged();
                }
            }
        }

        [Column(Name = "regfees", UpdateCheck = UpdateCheck.Never, Storage = "_Regfees", DbType = "money")]
        public decimal? Regfees
        {
            get => _Regfees;

            set
            {
                if (_Regfees != value)
                {
                    OnRegfeesChanging(value);
                    SendPropertyChanging();
                    _Regfees = value;
                    SendPropertyChanged("Regfees");
                    OnRegfeesChanged();
                }
            }
        }

        [Column(Name = "donate", UpdateCheck = UpdateCheck.Never, Storage = "_Donate", DbType = "money")]
        public decimal? Donate
        {
            get => _Donate;

            set
            {
                if (_Donate != value)
                {
                    OnDonateChanging(value);
                    SendPropertyChanging();
                    _Donate = value;
                    SendPropertyChanged("Donate");
                    OnDonateChanged();
                }
            }
        }

        [Column(Name = "fund", UpdateCheck = UpdateCheck.Never, Storage = "_Fund", DbType = "nvarchar(50)")]
        public string Fund
        {
            get => _Fund;

            set
            {
                if (_Fund != value)
                {
                    OnFundChanging(value);
                    SendPropertyChanging();
                    _Fund = value;
                    SendPropertyChanged("Fund");
                    OnFundChanged();
                }
            }
        }

        [Column(Name = "financeonly", UpdateCheck = UpdateCheck.Never, Storage = "_Financeonly", DbType = "bit")]
        public bool? Financeonly
        {
            get => _Financeonly;

            set
            {
                if (_Financeonly != value)
                {
                    OnFinanceonlyChanging(value);
                    SendPropertyChanging();
                    _Financeonly = value;
                    SendPropertyChanged("Financeonly");
                    OnFinanceonlyChanged();
                }
            }
        }

        [Column(Name = "voided", UpdateCheck = UpdateCheck.Never, Storage = "_Voided", DbType = "bit")]
        public bool? Voided
        {
            get => _Voided;

            set
            {
                if (_Voided != value)
                {
                    OnVoidedChanging(value);
                    SendPropertyChanging();
                    _Voided = value;
                    SendPropertyChanged("Voided");
                    OnVoidedChanged();
                }
            }
        }

        [Column(Name = "credited", UpdateCheck = UpdateCheck.Never, Storage = "_Credited", DbType = "bit")]
        public bool? Credited
        {
            get => _Credited;

            set
            {
                if (_Credited != value)
                {
                    OnCreditedChanging(value);
                    SendPropertyChanging();
                    _Credited = value;
                    SendPropertyChanged("Credited");
                    OnCreditedChanged();
                }
            }
        }

        [Column(Name = "coupon", UpdateCheck = UpdateCheck.Never, Storage = "_Coupon", DbType = "bit")]
        public bool? Coupon
        {
            get => _Coupon;

            set
            {
                if (_Coupon != value)
                {
                    OnCouponChanging(value);
                    SendPropertyChanging();
                    _Coupon = value;
                    SendPropertyChanged("Coupon");
                    OnCouponChanged();
                }
            }
        }

        [Column(Name = "moneytran", UpdateCheck = UpdateCheck.Never, Storage = "_Moneytran", DbType = "bit", IsDbGenerated = true)]
        public bool? Moneytran
        {
            get => _Moneytran;

            set
            {
                if (_Moneytran != value)
                {
                    OnMoneytranChanging(value);
                    SendPropertyChanging();
                    _Moneytran = value;
                    SendPropertyChanged("Moneytran");
                    OnMoneytranChanged();
                }
            }
        }

        [Column(Name = "settled", UpdateCheck = UpdateCheck.Never, Storage = "_Settled", DbType = "datetime")]
        public DateTime? Settled
        {
            get => _Settled;

            set
            {
                if (_Settled != value)
                {
                    OnSettledChanging(value);
                    SendPropertyChanging();
                    _Settled = value;
                    SendPropertyChanged("Settled");
                    OnSettledChanged();
                }
            }
        }

        [Column(Name = "batch", UpdateCheck = UpdateCheck.Never, Storage = "_Batch", DbType = "datetime")]
        public DateTime? Batch
        {
            get => _Batch;

            set
            {
                if (_Batch != value)
                {
                    OnBatchChanging(value);
                    SendPropertyChanging();
                    _Batch = value;
                    SendPropertyChanged("Batch");
                    OnBatchChanged();
                }
            }
        }

        [Column(Name = "batchref", UpdateCheck = UpdateCheck.Never, Storage = "_Batchref", DbType = "nvarchar(50)")]
        public string Batchref
        {
            get => _Batchref;

            set
            {
                if (_Batchref != value)
                {
                    OnBatchrefChanging(value);
                    SendPropertyChanging();
                    _Batchref = value;
                    SendPropertyChanged("Batchref");
                    OnBatchrefChanged();
                }
            }
        }

        [Column(Name = "batchtyp", UpdateCheck = UpdateCheck.Never, Storage = "_Batchtyp", DbType = "nvarchar(50)")]
        public string Batchtyp
        {
            get => _Batchtyp;

            set
            {
                if (_Batchtyp != value)
                {
                    OnBatchtypChanging(value);
                    SendPropertyChanging();
                    _Batchtyp = value;
                    SendPropertyChanged("Batchtyp");
                    OnBatchtypChanged();
                }
            }
        }

        [Column(Name = "fromsage", UpdateCheck = UpdateCheck.Never, Storage = "_Fromsage", DbType = "bit")]
        public bool? Fromsage
        {
            get => _Fromsage;

            set
            {
                if (_Fromsage != value)
                {
                    OnFromsageChanging(value);
                    SendPropertyChanging();
                    _Fromsage = value;
                    SendPropertyChanged("Fromsage");
                    OnFromsageChanged();
                }
            }
        }

        [Column(Name = "LoginPeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_LoginPeopleId", DbType = "int")]
        [IsForeignKey]
        public int? LoginPeopleId
        {
            get => _LoginPeopleId;

            set
            {
                if (_LoginPeopleId != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnLoginPeopleIdChanging(value);
                    SendPropertyChanging();
                    _LoginPeopleId = value;
                    SendPropertyChanged("LoginPeopleId");
                    OnLoginPeopleIdChanged();
                }
            }
        }

        [Column(Name = "First", UpdateCheck = UpdateCheck.Never, Storage = "_First", DbType = "nvarchar(50)")]
        public string First
        {
            get => _First;

            set
            {
                if (_First != value)
                {
                    OnFirstChanging(value);
                    SendPropertyChanging();
                    _First = value;
                    SendPropertyChanged("First");
                    OnFirstChanged();
                }
            }
        }

        [Column(Name = "MiddleInitial", UpdateCheck = UpdateCheck.Never, Storage = "_MiddleInitial", DbType = "nvarchar(1)")]
        public string MiddleInitial
        {
            get => _MiddleInitial;

            set
            {
                if (_MiddleInitial != value)
                {
                    OnMiddleInitialChanging(value);
                    SendPropertyChanging();
                    _MiddleInitial = value;
                    SendPropertyChanged("MiddleInitial");
                    OnMiddleInitialChanged();
                }
            }
        }

        [Column(Name = "Last", UpdateCheck = UpdateCheck.Never, Storage = "_Last", DbType = "nvarchar(50)")]
        public string Last
        {
            get => _Last;

            set
            {
                if (_Last != value)
                {
                    OnLastChanging(value);
                    SendPropertyChanging();
                    _Last = value;
                    SendPropertyChanged("Last");
                    OnLastChanged();
                }
            }
        }

        [Column(Name = "Suffix", UpdateCheck = UpdateCheck.Never, Storage = "_Suffix", DbType = "nvarchar(10)")]
        public string Suffix
        {
            get => _Suffix;

            set
            {
                if (_Suffix != value)
                {
                    OnSuffixChanging(value);
                    SendPropertyChanging();
                    _Suffix = value;
                    SendPropertyChanged("Suffix");
                    OnSuffixChanged();
                }
            }
        }

        [Column(Name = "AdjustFee", UpdateCheck = UpdateCheck.Never, Storage = "_AdjustFee", DbType = "bit")]
        public bool? AdjustFee
        {
            get => _AdjustFee;

            set
            {
                if (_AdjustFee != value)
                {
                    OnAdjustFeeChanging(value);
                    SendPropertyChanging();
                    _AdjustFee = value;
                    SendPropertyChanged("AdjustFee");
                    OnAdjustFeeChanged();
                }
            }
        }

        [Column(Name = "LastFourCC", UpdateCheck = UpdateCheck.Never, Storage = "_LastFourCC", DbType = "nvarchar(4)")]
        public string LastFourCC
        {
            get => _LastFourCC;

            set
            {
                if (_LastFourCC != value)
                {
                    OnLastFourCCChanging(value);
                    SendPropertyChanging();
                    _LastFourCC = value;
                    SendPropertyChanged("LastFourCC");
                    OnLastFourCCChanged();
                }
            }
        }

        [Column(Name = "LastFourACH", UpdateCheck = UpdateCheck.Never, Storage = "_LastFourACH", DbType = "nvarchar(4)")]
        public string LastFourACH
        {
            get => _LastFourACH;

            set
            {
                if (_LastFourACH != value)
                {
                    OnLastFourACHChanging(value);
                    SendPropertyChanging();
                    _LastFourACH = value;
                    SendPropertyChanged("LastFourACH");
                    OnLastFourACHChanged();
                }
            }
        }

        [Column(Name = "PaymentType", UpdateCheck = UpdateCheck.Never, Storage = "_PaymentType", DbType = "nvarchar(1)")]
        public string PaymentType
        {
            get => _PaymentType;

            set
            {
                if (_PaymentType != value)
                {
                    OnPaymentTypeChanging(value);
                    SendPropertyChanging();
                    _PaymentType = value;
                    SendPropertyChanged("PaymentType");
                    OnPaymentTypeChanged();
                }
            }
        }

        [Column(Name = "Address2", UpdateCheck = UpdateCheck.Never, Storage = "_Address2", DbType = "nvarchar(50)")]
        public string Address2
        {
            get => _Address2;

            set
            {
                if (_Address2 != value)
                {
                    OnAddress2Changing(value);
                    SendPropertyChanging();
                    _Address2 = value;
                    SendPropertyChanged("Address2");
                    OnAddress2Changed();
                }
            }
        }

        [Column(Name = "Country", UpdateCheck = UpdateCheck.Never, Storage = "_Country", DbType = "nvarchar(50)")]
        public string Country
        {
            get => _Country;

            set
            {
                if (_Country != value)
                {
                    OnCountryChanging(value);
                    SendPropertyChanging();
                    _Country = value;
                    SendPropertyChanged("Country");
                    OnCountryChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_OrganizationMembers_Transaction", Storage = "_OrganizationMembers", OtherKey = "TranId")]
        public EntitySet<OrganizationMember> OrganizationMembers
           {
               get => _OrganizationMembers;

            set => _OrganizationMembers.Assign(value);

           }

        [Association(Name = "FK_TransactionPeople_Transaction", Storage = "_TransactionPeople", OtherKey = "Id")]
        public EntitySet<TransactionPerson> TransactionPeople
           {
               get => _TransactionPeople;

            set => _TransactionPeople.Assign(value);

           }

        [Association(Name = "Transactions__OriginalTransaction", Storage = "_Transactions", OtherKey = "OriginalId")]
        public EntitySet<Transaction> Transactions
           {
               get => _Transactions;

            set => _Transactions.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Transaction_People", Storage = "_Person", ThisKey = "LoginPeopleId", IsForeignKey = true)]
        public Person Person
        {
            get => _Person.Entity;

            set
            {
                Person previousValue = _Person.Entity;
                if (((previousValue != value)
                            || (_Person.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Person.Entity = null;
                        previousValue.Transactions.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.Transactions.Add(this);

                        _LoginPeopleId = value.PeopleId;

                    }

                    else
                    {
                        _LoginPeopleId = default(int?);

                    }

                    SendPropertyChanged("Person");
                }
            }
        }

        [Association(Name = "Transactions__OriginalTransaction", Storage = "_OriginalTransaction", ThisKey = "OriginalId", IsForeignKey = true)]
        public Transaction OriginalTransaction
        {
            get => _OriginalTransaction.Entity;

            set
            {
                Transaction previousValue = _OriginalTransaction.Entity;
                if (((previousValue != value)
                            || (_OriginalTransaction.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _OriginalTransaction.Entity = null;
                        previousValue.Transactions.Remove(this);
                    }

                    _OriginalTransaction.Entity = value;
                    if (value != null)
                    {
                        value.Transactions.Add(this);

                        _OriginalId = value.Id;

                    }

                    else
                    {
                        _OriginalId = default(int?);

                    }

                    SendPropertyChanged("OriginalTransaction");
                }
            }
        }

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((PropertyChanging != null))
            {
                PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void attach_OrganizationMembers(OrganizationMember entity)
        {
            SendPropertyChanging();
            entity.Transaction = this;
        }

        private void detach_OrganizationMembers(OrganizationMember entity)
        {
            SendPropertyChanging();
            entity.Transaction = null;
        }

        private void attach_TransactionPeople(TransactionPerson entity)
        {
            SendPropertyChanging();
            entity.Transaction = this;
        }

        private void detach_TransactionPeople(TransactionPerson entity)
        {
            SendPropertyChanging();
            entity.Transaction = null;
        }

        private void attach_Transactions(Transaction entity)
        {
            SendPropertyChanging();
            entity.OriginalTransaction = this;
        }

        private void detach_Transactions(Transaction entity)
        {
            SendPropertyChanging();
            entity.OriginalTransaction = null;
        }
    }
}
