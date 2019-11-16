using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.PaymentInfo")]
    public partial class PaymentInfo : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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

        private string _AcceptivaPayerId;

        private int _GatewayAccountId;

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

        partial void OnAcceptivaPayerIdChanging(string value);
        partial void OnAcceptivaPayerIdChanged();

        partial void OnGatewayAccountIdChanging(int value);
        partial void OnGatewayAccountIdChanged();

        #endregion

        public PaymentInfo()
        {
            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "AuNetCustId", UpdateCheck = UpdateCheck.Never, Storage = "_AuNetCustId", DbType = "int")]
        public int? AuNetCustId
        {
            get => _AuNetCustId;

            set
            {
                if (_AuNetCustId != value)
                {
                    OnAuNetCustIdChanging(value);
                    SendPropertyChanging();
                    _AuNetCustId = value;
                    SendPropertyChanged("AuNetCustId");
                    OnAuNetCustIdChanged();
                }
            }
        }

        [Column(Name = "AuNetCustPayId", UpdateCheck = UpdateCheck.Never, Storage = "_AuNetCustPayId", DbType = "int")]
        public int? AuNetCustPayId
        {
            get => _AuNetCustPayId;

            set
            {
                if (_AuNetCustPayId != value)
                {
                    OnAuNetCustPayIdChanging(value);
                    SendPropertyChanging();
                    _AuNetCustPayId = value;
                    SendPropertyChanged("AuNetCustPayId");
                    OnAuNetCustPayIdChanged();
                }
            }
        }

        [Column(Name = "SageBankGuid", UpdateCheck = UpdateCheck.Never, Storage = "_SageBankGuid", DbType = "uniqueidentifier")]
        public Guid? SageBankGuid
        {
            get => _SageBankGuid;

            set
            {
                if (_SageBankGuid != value)
                {
                    OnSageBankGuidChanging(value);
                    SendPropertyChanging();
                    _SageBankGuid = value;
                    SendPropertyChanged("SageBankGuid");
                    OnSageBankGuidChanged();
                }
            }
        }

        [Column(Name = "SageCardGuid", UpdateCheck = UpdateCheck.Never, Storage = "_SageCardGuid", DbType = "uniqueidentifier")]
        public Guid? SageCardGuid
        {
            get => _SageCardGuid;

            set
            {
                if (_SageCardGuid != value)
                {
                    OnSageCardGuidChanging(value);
                    SendPropertyChanging();
                    _SageCardGuid = value;
                    SendPropertyChanged("SageCardGuid");
                    OnSageCardGuidChanged();
                }
            }
        }

        [Column(Name = "MaskedAccount", UpdateCheck = UpdateCheck.Never, Storage = "_MaskedAccount", DbType = "nvarchar(30)")]
        public string MaskedAccount
        {
            get => _MaskedAccount;

            set
            {
                if (_MaskedAccount != value)
                {
                    OnMaskedAccountChanging(value);
                    SendPropertyChanging();
                    _MaskedAccount = value;
                    SendPropertyChanged("MaskedAccount");
                    OnMaskedAccountChanged();
                }
            }
        }

        [Column(Name = "MaskedCard", UpdateCheck = UpdateCheck.Never, Storage = "_MaskedCard", DbType = "nvarchar(30)")]
        public string MaskedCard
        {
            get => _MaskedCard;

            set
            {
                if (_MaskedCard != value)
                {
                    OnMaskedCardChanging(value);
                    SendPropertyChanging();
                    _MaskedCard = value;
                    SendPropertyChanged("MaskedCard");
                    OnMaskedCardChanged();
                }
            }
        }

        [Column(Name = "Expires", UpdateCheck = UpdateCheck.Never, Storage = "_Expires", DbType = "nvarchar(10)")]
        public string Expires
        {
            get => _Expires;

            set
            {
                if (_Expires != value)
                {
                    OnExpiresChanging(value);
                    SendPropertyChanging();
                    _Expires = value;
                    SendPropertyChanged("Expires");
                    OnExpiresChanged();
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

        [Column(Name = "PreferredGivingType", UpdateCheck = UpdateCheck.Never, Storage = "_PreferredGivingType", DbType = "nvarchar(2)")]
        public string PreferredGivingType
        {
            get => _PreferredGivingType;

            set
            {
                if (_PreferredGivingType != value)
                {
                    OnPreferredGivingTypeChanging(value);
                    SendPropertyChanging();
                    _PreferredGivingType = value;
                    SendPropertyChanged("PreferredGivingType");
                    OnPreferredGivingTypeChanged();
                }
            }
        }

        [Column(Name = "PreferredPaymentType", UpdateCheck = UpdateCheck.Never, Storage = "_PreferredPaymentType", DbType = "nvarchar(2)")]
        public string PreferredPaymentType
        {
            get => _PreferredPaymentType;

            set
            {
                if (_PreferredPaymentType != value)
                {
                    OnPreferredPaymentTypeChanging(value);
                    SendPropertyChanging();
                    _PreferredPaymentType = value;
                    SendPropertyChanged("PreferredPaymentType");
                    OnPreferredPaymentTypeChanged();
                }
            }
        }

        [Column(Name = "Routing", UpdateCheck = UpdateCheck.Never, Storage = "_Routing", DbType = "nvarchar(10)")]
        public string Routing
        {
            get => _Routing;

            set
            {
                if (_Routing != value)
                {
                    OnRoutingChanging(value);
                    SendPropertyChanging();
                    _Routing = value;
                    SendPropertyChanged("Routing");
                    OnRoutingChanged();
                }
            }
        }

        [Column(Name = "FirstName", UpdateCheck = UpdateCheck.Never, Storage = "_FirstName", DbType = "nvarchar(50)")]
        public string FirstName
        {
            get => _FirstName;

            set
            {
                if (_FirstName != value)
                {
                    OnFirstNameChanging(value);
                    SendPropertyChanging();
                    _FirstName = value;
                    SendPropertyChanged("FirstName");
                    OnFirstNameChanged();
                }
            }
        }

        [Column(Name = "MiddleInitial", UpdateCheck = UpdateCheck.Never, Storage = "_MiddleInitial", DbType = "nvarchar(10)")]
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

        [Column(Name = "LastName", UpdateCheck = UpdateCheck.Never, Storage = "_LastName", DbType = "nvarchar(50)")]
        public string LastName
        {
            get => _LastName;

            set
            {
                if (_LastName != value)
                {
                    OnLastNameChanging(value);
                    SendPropertyChanging();
                    _LastName = value;
                    SendPropertyChanged("LastName");
                    OnLastNameChanged();
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

        [Column(Name = "State", UpdateCheck = UpdateCheck.Never, Storage = "_State", DbType = "nvarchar(10)")]
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

        [Column(Name = "Phone", UpdateCheck = UpdateCheck.Never, Storage = "_Phone", DbType = "nvarchar(25)")]
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

        [Column(Name = "TbnBankVaultId", UpdateCheck = UpdateCheck.Never, Storage = "_TbnBankVaultId", DbType = "int")]
        public int? TbnBankVaultId
        {
            get => _TbnBankVaultId;

            set
            {
                if (_TbnBankVaultId != value)
                {
                    OnTbnBankVaultIdChanging(value);
                    SendPropertyChanging();
                    _TbnBankVaultId = value;
                    SendPropertyChanged("TbnBankVaultId");
                    OnTbnBankVaultIdChanged();
                }
            }
        }

        [Column(Name = "TbnCardVaultId", UpdateCheck = UpdateCheck.Never, Storage = "_TbnCardVaultId", DbType = "int")]
        public int? TbnCardVaultId
        {
            get => _TbnCardVaultId;

            set
            {
                if (_TbnCardVaultId != value)
                {
                    OnTbnCardVaultIdChanging(value);
                    SendPropertyChanging();
                    _TbnCardVaultId = value;
                    SendPropertyChanged("TbnCardVaultId");
                    OnTbnCardVaultIdChanged();
                }
            }
        }

        [Column(Name = "AuNetCustPayBankId", UpdateCheck = UpdateCheck.Never, Storage = "_AuNetCustPayBankId", DbType = "int")]
        public int? AuNetCustPayBankId
        {
            get => _AuNetCustPayBankId;

            set
            {
                if (_AuNetCustPayBankId != value)
                {
                    OnAuNetCustPayBankIdChanging(value);
                    SendPropertyChanging();
                    _AuNetCustPayBankId = value;
                    SendPropertyChanged("AuNetCustPayBankId");
                    OnAuNetCustPayBankIdChanged();
                }
            }
        }

        [Column(Name = "BluePayCardVaultId", UpdateCheck = UpdateCheck.Never, Storage = "_BluePayCardVaultId", DbType = "nvarchar(50)")]
        public string BluePayCardVaultId
        {
            get => _BluePayCardVaultId;

            set
            {
                if (_BluePayCardVaultId != value)
                {
                    OnBluePayCardVaultIdChanging(value);
                    SendPropertyChanging();
                    _BluePayCardVaultId = value;
                    SendPropertyChanged("BluePayCardVaultId");
                    OnBluePayCardVaultIdChanged();
                }
            }
        }

        [Column(Name = "AcceptivaPayerId", UpdateCheck = UpdateCheck.Never, Storage = "_AcceptivaPayerId", DbType = "nvarchar(50)")]
        public string AcceptivaPayerId
        {
            get => _AcceptivaPayerId;

            set
            {
                if (_AcceptivaPayerId != value)
                {
                    OnAcceptivaPayerIdChanging(value);
                    SendPropertyChanging();
                    _AcceptivaPayerId = value;
                    SendPropertyChanged("AcceptivaPayerId");
                    OnAcceptivaPayerIdChanged();
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

        [Column(Name = "GatewayAccountId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayAccountId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int GatewayAccountId
        {
            get => _GatewayAccountId;

            set
            {
                if (_GatewayAccountId != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnGatewayAccountIdChanging(value);
                    SendPropertyChanging();
                    _GatewayAccountId = value;
                    SendPropertyChanged("GatewayAccountId");
                    OnGatewayAccountIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_PaymentInfo_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.PaymentInfos.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.PaymentInfos.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int);

                    }

                    SendPropertyChanged("Person");
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
    }
}
