using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.PaymentMethod")]
    public partial class PaymentMethod : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs("");

        #region Private Fields

        private Guid _PaymentMethodId;

        private int _PeopleId;

        private int _PaymentMethodTypeId;

        private string _Name;

        private string _BankName;

        private int? _ExpiresMonth;

        private int? _ExpiresYear;

        private int _GatewayAccountId;

        private bool? _IsDefault;

        private string _Last4;

        private string _MaskedDisplay;

        private string _NameOnAccount;

        private string _VaultId;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(ChangeAction action);
        partial void OnCreated();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnPaymentMethodTypeIdChanging(int value);
        partial void OnPaymentMethodTypeIdChanged();

        partial void OnPaymentMethodIdChanging(Guid value);
        partial void OnPaymentMethodIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnMaskedDisplayChanging(string value);
        partial void OnMaskedDisplayChanged();

        partial void OnBankNameChanging(string value);
        partial void OnBankNameChanged();

        partial void OnIsDefaultChanging(bool? value);
        partial void OnIsDefaultChanged();

        partial void OnVaultIdChanging(string value);
        partial void OnVaultIdChanged();

        partial void OnLast4Changing(string value);
        partial void OnLast4Changed();

        partial void OnNameOnAccountChanging(string value);
        partial void OnNameOnAccountChanged();

        partial void OnExpiresMonthChanging(int? value);
        partial void OnExpiresMonthChanged();

        partial void OnExpiresYearChanging(int? value);
        partial void OnExpiresYearChanged();

        partial void OnGatewayAccountIdChanging(int value);
        partial void OnGatewayAccountIdChanged();

        #endregion

        public PaymentMethod()
        {
            _Person = default;

            OnCreated();
        }

        #region Columns

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
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
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "PaymentMethodTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_PaymentMethodTypeId", DbType = "int")]
        public int PaymentMethodTypeId
        {
            get => _PaymentMethodTypeId;
            set
            {
                if (_PaymentMethodTypeId != value)
                {
                    OnPaymentMethodTypeIdChanging(value);
                    SendPropertyChanging();
                    _PaymentMethodTypeId = value;
                    SendPropertyChanged("PaymentMethodTypeId");
                    OnPaymentMethodTypeIdChanged();
                }
            }
        }

        [Column(Name = "PaymentMethodId", UpdateCheck = UpdateCheck.Never, Storage = "_PaymentMethodId", DbType = "uniqueidentifier", IsDbGenerated = true, IsPrimaryKey = true)]
        public Guid PaymentMethodId
        {
            get => _PaymentMethodId;
            set
            {
                if (_PaymentMethodId != value)
                {
                    OnPaymentMethodIdChanging(value);
                    SendPropertyChanging();
                    _PaymentMethodId = value;
                    SendPropertyChanged("PaymentMethodId");
                    OnPaymentMethodIdChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(max)")]
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

        [Column(Name = "MaskedDisplay", UpdateCheck = UpdateCheck.Never, Storage = "_MaskedDisplay", DbType = "nvarchar(max)")]
        public string MaskedDisplay
        {
            get => _MaskedDisplay;
            set
            {
                if (_MaskedDisplay != value)
                {
                    OnMaskedDisplayChanging(value);
                    SendPropertyChanging();
                    _MaskedDisplay = value;
                    SendPropertyChanged("MaskedDisplay");
                    OnMaskedDisplayChanged();
                }
            }
        }

        [Column(Name = "BankName", UpdateCheck = UpdateCheck.Never, Storage = "_BankName", DbType = "nvarchar(max)")]
        public string BankName
        {
            get => _BankName;
            set
            {
                if (_BankName != value)
                {
                    OnBankNameChanging(value);
                    SendPropertyChanging();
                    _BankName = value;
                    SendPropertyChanged("BankName");
                    OnBankNameChanged();
                }
            }
        }

        [Column(Name = "IsDefault", UpdateCheck = UpdateCheck.Never, Storage = "_IsDefault", DbType = "bit NULL")]
        public bool? IsDefault
        {
            get => _IsDefault;
            set
            {
                if (_IsDefault != value)
                {
                    OnIsDefaultChanging(value);
                    SendPropertyChanging();
                    _IsDefault = value;
                    SendPropertyChanged("IsDefault");
                    OnIsDefaultChanged();
                }
            }
        }

        [Column(Name = "VaultId", UpdateCheck = UpdateCheck.Never, Storage = "_VaultId", DbType = "nvarchar(max)")]
        public string VaultId
        {
            get => _VaultId;
            set
            {
                if (_VaultId != value)
                {
                    OnVaultIdChanging(value);
                    SendPropertyChanging();
                    _VaultId = value;
                    SendPropertyChanged("VaultId");
                    OnVaultIdChanged();
                }
            }
        }

        [Column(Name = "Last4", UpdateCheck = UpdateCheck.Never, Storage = "_Last4", DbType = "nvarchar(max)")]
        public string Last4
        {
            get => _Last4;
            set
            {
                if (_Last4 != value)
                {
                    OnLast4Changing(value);
                    SendPropertyChanging();
                    _Last4 = value;
                    SendPropertyChanged("Last4");
                    OnLast4Changed();
                }
            }
        }

        [Column(Name = "NameOnAccount", UpdateCheck = UpdateCheck.Never, Storage = "_NameOnAccount", DbType = "nvarchar(max)")]
        public string NameOnAccount
        {
            get => _NameOnAccount;
            set
            {
                if (_NameOnAccount != value)
                {
                    OnNameOnAccountChanging(value);
                    SendPropertyChanging();
                    _NameOnAccount = value;
                    SendPropertyChanged("NameOnAccount");
                    OnNameOnAccountChanged();
                }
            }
        }

        [Column(Name = "ExpiresMonth", UpdateCheck = UpdateCheck.Never, Storage = "_ExpiresMonth", DbType = "int NULL")]
        public int? ExpiresMonth
        {
            get => _ExpiresMonth;
            set
            {
                if (_ExpiresMonth != value)
                {
                    OnExpiresMonthChanging(value);
                    SendPropertyChanging();
                    _ExpiresMonth = value;
                    SendPropertyChanged("ExpiresMonth");
                    OnExpiresMonthChanged();
                }
            }
        }

        [Column(Name = "ExpiresYear", UpdateCheck = UpdateCheck.Never, Storage = "_ExpiresYear", DbType = "int NULL")]
        public int? ExpiresYear
        {
            get => _ExpiresYear;
            set
            {
                if (_ExpiresYear != value)
                {
                    OnExpiresYearChanging(value);
                    SendPropertyChanging();
                    _ExpiresYear = value;
                    SendPropertyChanged("ExpiresYear");
                    OnExpiresYearChanged();
                }
            }
        }

        [Column(Name = "GatewayAccountId", UpdateCheck = UpdateCheck.Never, Storage = "_GatewayAccountId", DbType = "int")]
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
                        throw new ForeignKeyReferenceAlreadyHasValueException();
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

        [Association(Name = "FK_PaymentMethod_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
        public Person Person
        {
            get => _Person.Entity;
            set
            {
                Person previousValue = _Person.Entity;
                if ((previousValue != value) || (_Person.HasLoadedOrAssignedValue == false))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Person.Entity = null;
                        previousValue.PaymentMethods.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.PaymentMethods.Add(this);
                        _PeopleId = value.PeopleId;
                    }
                    else
                    {
                        _PeopleId = default;
                    }

                    SendPropertyChanged("Person");
                }
            }
        }

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            PropertyChanging?.Invoke(this, emptyChangingEventArgs);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
