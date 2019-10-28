using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.PeopleExtra")]
    public partial class PeopleExtra : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _PeopleId;

        private DateTime _TransactionTime;

        private string _Field;

        private string _StrValue;

        private DateTime? _DateValue;

        private string _Data;

        private int? _IntValue;

        private int? _IntValue2;

        private bool? _BitValue;

        private string _FieldValue;

        private bool? _UseAllValues;

        private int _Instance;

        private bool? _IsAttributes;

        private string _Type;

        private string _Metadata;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnTransactionTimeChanging(DateTime value);
        partial void OnTransactionTimeChanged();

        partial void OnFieldChanging(string value);
        partial void OnFieldChanged();

        partial void OnStrValueChanging(string value);
        partial void OnStrValueChanged();

        partial void OnDateValueChanging(DateTime? value);
        partial void OnDateValueChanged();

        partial void OnDataChanging(string value);
        partial void OnDataChanged();

        partial void OnIntValueChanging(int? value);
        partial void OnIntValueChanged();

        partial void OnIntValue2Changing(int? value);
        partial void OnIntValue2Changed();

        partial void OnBitValueChanging(bool? value);
        partial void OnBitValueChanged();

        partial void OnFieldValueChanging(string value);
        partial void OnFieldValueChanged();

        partial void OnUseAllValuesChanging(bool? value);
        partial void OnUseAllValuesChanged();

        partial void OnInstanceChanging(int value);
        partial void OnInstanceChanged();

        partial void OnIsAttributesChanging(bool? value);
        partial void OnIsAttributesChanged();

        partial void OnTypeChanging(string value);
        partial void OnTypeChanged();

        partial void OnMetadataChanging(string value);
        partial void OnMetadataChanged();

        #endregion

        public PeopleExtra()
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

        [Column(Name = "TransactionTime", UpdateCheck = UpdateCheck.Never, Storage = "_TransactionTime", DbType = "datetime NOT NULL")]
        public DateTime TransactionTime
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

        [Column(Name = "Field", UpdateCheck = UpdateCheck.Never, Storage = "_Field", DbType = "nvarchar(150) NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "IntValue2", UpdateCheck = UpdateCheck.Never, Storage = "_IntValue2", DbType = "int")]
        public int? IntValue2
        {
            get => _IntValue2;

            set
            {
                if (_IntValue2 != value)
                {
                    OnIntValue2Changing(value);
                    SendPropertyChanging();
                    _IntValue2 = value;
                    SendPropertyChanged("IntValue2");
                    OnIntValue2Changed();
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

        [Column(Name = "FieldValue", UpdateCheck = UpdateCheck.Never, Storage = "_FieldValue", DbType = "nvarchar(351)", IsDbGenerated = true)]
        public string FieldValue
        {
            get => _FieldValue;

            set
            {
                if (_FieldValue != value)
                {
                    OnFieldValueChanging(value);
                    SendPropertyChanging();
                    _FieldValue = value;
                    SendPropertyChanged("FieldValue");
                    OnFieldValueChanged();
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

        [Column(Name = "Instance", UpdateCheck = UpdateCheck.Never, Storage = "_Instance", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int Instance
        {
            get => _Instance;

            set
            {
                if (_Instance != value)
                {
                    OnInstanceChanging(value);
                    SendPropertyChanging();
                    _Instance = value;
                    SendPropertyChanged("Instance");
                    OnInstanceChanged();
                }
            }
        }

        [Column(Name = "IsAttributes", UpdateCheck = UpdateCheck.Never, Storage = "_IsAttributes", DbType = "bit")]
        public bool? IsAttributes
        {
            get => _IsAttributes;

            set
            {
                if (_IsAttributes != value)
                {
                    OnIsAttributesChanging(value);
                    SendPropertyChanging();
                    _IsAttributes = value;
                    SendPropertyChanged("IsAttributes");
                    OnIsAttributesChanged();
                }
            }
        }

        [Column(Name = "Type", UpdateCheck = UpdateCheck.Never, Storage = "_Type", DbType = "varchar(18) NOT NULL", IsDbGenerated = true)]
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

        [Column(Name = "Metadata", UpdateCheck = UpdateCheck.Never, Storage = "_Metadata", DbType = "nvarchar")]
        public string Metadata
        {
            get => _Metadata;

            set
            {
                if (_Metadata != value)
                {
                    OnMetadataChanging(value);
                    SendPropertyChanging();
                    _Metadata = value;
                    SendPropertyChanged("Metadata");
                    OnMetadataChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_PeopleExtra_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.PeopleExtras.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.PeopleExtras.Add(this);

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
