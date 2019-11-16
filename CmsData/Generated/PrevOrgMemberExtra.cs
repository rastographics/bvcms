using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.PrevOrgMemberExtra")]
    public partial class PrevOrgMemberExtra : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _EnrollmentTranId;

        private int _OrganizationId;

        private int _PeopleId;

        private string _Field;

        private string _Data;

        private string _DataType;

        private string _StrValue;

        private DateTime? _DateValue;

        private int? _IntValue;

        private bool? _BitValue;

        private DateTime? _TransactionTime;

        private bool? _UseAllValues;

        private string _Type;

        private EntityRef<EnrollmentTransaction> _EnrollmentTransaction;

        private EntityRef<Organization> _Organization;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnEnrollmentTranIdChanging(int value);
        partial void OnEnrollmentTranIdChanged();

        partial void OnOrganizationIdChanging(int value);
        partial void OnOrganizationIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnFieldChanging(string value);
        partial void OnFieldChanged();

        partial void OnDataChanging(string value);
        partial void OnDataChanged();

        partial void OnDataTypeChanging(string value);
        partial void OnDataTypeChanged();

        partial void OnStrValueChanging(string value);
        partial void OnStrValueChanged();

        partial void OnDateValueChanging(DateTime? value);
        partial void OnDateValueChanged();

        partial void OnIntValueChanging(int? value);
        partial void OnIntValueChanged();

        partial void OnBitValueChanging(bool? value);
        partial void OnBitValueChanged();

        partial void OnTransactionTimeChanging(DateTime? value);
        partial void OnTransactionTimeChanged();

        partial void OnUseAllValuesChanging(bool? value);
        partial void OnUseAllValuesChanged();

        partial void OnTypeChanging(string value);
        partial void OnTypeChanged();

        #endregion

        public PrevOrgMemberExtra()
        {
            _EnrollmentTransaction = default(EntityRef<EnrollmentTransaction>);

            _Organization = default(EntityRef<Organization>);

            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "EnrollmentTranId", UpdateCheck = UpdateCheck.Never, Storage = "_EnrollmentTranId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int EnrollmentTranId
        {
            get => _EnrollmentTranId;

            set
            {
                if (_EnrollmentTranId != value)
                {
                    if (_EnrollmentTransaction.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnEnrollmentTranIdChanging(value);
                    SendPropertyChanging();
                    _EnrollmentTranId = value;
                    SendPropertyChanged("EnrollmentTranId");
                    OnEnrollmentTranIdChanged();
                }
            }
        }

        [Column(Name = "OrganizationId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    if (_Organization.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOrganizationIdChanging(value);
                    SendPropertyChanging();
                    _OrganizationId = value;
                    SendPropertyChanged("OrganizationId");
                    OnOrganizationIdChanged();
                }
            }
        }

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

        [Column(Name = "Field", UpdateCheck = UpdateCheck.Never, Storage = "_Field", DbType = "nvarchar(50) NOT NULL", IsPrimaryKey = true)]
        public string Field
        {
            get => _Field;

            set
            {
                if (_Field != value)
                {
                    OnFieldChanging(value);
                    SendPropertyChanging();
                    _Field = value;
                    SendPropertyChanged("Field");
                    OnFieldChanged();
                }
            }
        }

        [Column(Name = "Data", UpdateCheck = UpdateCheck.Never, Storage = "_Data", DbType = "nvarchar")]
        public string Data
        {
            get => _Data;

            set
            {
                if (_Data != value)
                {
                    OnDataChanging(value);
                    SendPropertyChanging();
                    _Data = value;
                    SendPropertyChanged("Data");
                    OnDataChanged();
                }
            }
        }

        [Column(Name = "DataType", UpdateCheck = UpdateCheck.Never, Storage = "_DataType", DbType = "nvarchar(5)")]
        public string DataType
        {
            get => _DataType;

            set
            {
                if (_DataType != value)
                {
                    OnDataTypeChanging(value);
                    SendPropertyChanging();
                    _DataType = value;
                    SendPropertyChanged("DataType");
                    OnDataTypeChanged();
                }
            }
        }

        [Column(Name = "StrValue", UpdateCheck = UpdateCheck.Never, Storage = "_StrValue", DbType = "nvarchar(200)")]
        public string StrValue
        {
            get => _StrValue;

            set
            {
                if (_StrValue != value)
                {
                    OnStrValueChanging(value);
                    SendPropertyChanging();
                    _StrValue = value;
                    SendPropertyChanged("StrValue");
                    OnStrValueChanged();
                }
            }
        }

        [Column(Name = "DateValue", UpdateCheck = UpdateCheck.Never, Storage = "_DateValue", DbType = "datetime")]
        public DateTime? DateValue
        {
            get => _DateValue;

            set
            {
                if (_DateValue != value)
                {
                    OnDateValueChanging(value);
                    SendPropertyChanging();
                    _DateValue = value;
                    SendPropertyChanged("DateValue");
                    OnDateValueChanged();
                }
            }
        }

        [Column(Name = "IntValue", UpdateCheck = UpdateCheck.Never, Storage = "_IntValue", DbType = "int")]
        public int? IntValue
        {
            get => _IntValue;

            set
            {
                if (_IntValue != value)
                {
                    OnIntValueChanging(value);
                    SendPropertyChanging();
                    _IntValue = value;
                    SendPropertyChanged("IntValue");
                    OnIntValueChanged();
                }
            }
        }

        [Column(Name = "BitValue", UpdateCheck = UpdateCheck.Never, Storage = "_BitValue", DbType = "bit")]
        public bool? BitValue
        {
            get => _BitValue;

            set
            {
                if (_BitValue != value)
                {
                    OnBitValueChanging(value);
                    SendPropertyChanging();
                    _BitValue = value;
                    SendPropertyChanged("BitValue");
                    OnBitValueChanged();
                }
            }
        }

        [Column(Name = "TransactionTime", UpdateCheck = UpdateCheck.Never, Storage = "_TransactionTime", DbType = "datetime")]
        public DateTime? TransactionTime
        {
            get => _TransactionTime;

            set
            {
                if (_TransactionTime != value)
                {
                    OnTransactionTimeChanging(value);
                    SendPropertyChanging();
                    _TransactionTime = value;
                    SendPropertyChanged("TransactionTime");
                    OnTransactionTimeChanged();
                }
            }
        }

        [Column(Name = "UseAllValues", UpdateCheck = UpdateCheck.Never, Storage = "_UseAllValues", DbType = "bit")]
        public bool? UseAllValues
        {
            get => _UseAllValues;

            set
            {
                if (_UseAllValues != value)
                {
                    OnUseAllValuesChanging(value);
                    SendPropertyChanging();
                    _UseAllValues = value;
                    SendPropertyChanged("UseAllValues");
                    OnUseAllValuesChanged();
                }
            }
        }

        [Column(Name = "Type", UpdateCheck = UpdateCheck.Never, Storage = "_Type", DbType = "varchar(22) NOT NULL", IsDbGenerated = true)]
        public string Type
        {
            get => _Type;

            set
            {
                if (_Type != value)
                {
                    OnTypeChanging(value);
                    SendPropertyChanging();
                    _Type = value;
                    SendPropertyChanged("Type");
                    OnTypeChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_PrevOrgMemberExtra_EnrollmentTransaction", Storage = "_EnrollmentTransaction", ThisKey = "EnrollmentTranId", IsForeignKey = true)]
        public EnrollmentTransaction EnrollmentTransaction
        {
            get => _EnrollmentTransaction.Entity;

            set
            {
                EnrollmentTransaction previousValue = _EnrollmentTransaction.Entity;
                if (((previousValue != value)
                            || (_EnrollmentTransaction.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _EnrollmentTransaction.Entity = null;
                        previousValue.PrevOrgMemberExtras.Remove(this);
                    }

                    _EnrollmentTransaction.Entity = value;
                    if (value != null)
                    {
                        value.PrevOrgMemberExtras.Add(this);

                        _EnrollmentTranId = value.TransactionId;

                    }

                    else
                    {
                        _EnrollmentTranId = default(int);

                    }

                    SendPropertyChanged("EnrollmentTransaction");
                }
            }
        }

        [Association(Name = "FK_PrevOrgMemberExtra_Organization", Storage = "_Organization", ThisKey = "OrganizationId", IsForeignKey = true)]
        public Organization Organization
        {
            get => _Organization.Entity;

            set
            {
                Organization previousValue = _Organization.Entity;
                if (((previousValue != value)
                            || (_Organization.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Organization.Entity = null;
                        previousValue.PrevOrgMemberExtras.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.PrevOrgMemberExtras.Add(this);

                        _OrganizationId = value.OrganizationId;

                    }

                    else
                    {
                        _OrganizationId = default(int);

                    }

                    SendPropertyChanged("Organization");
                }
            }
        }

        [Association(Name = "FK_PrevOrgMemberExtra_People]", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.PrevOrgMemberExtras.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.PrevOrgMemberExtras.Add(this);

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
