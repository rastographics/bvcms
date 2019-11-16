using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MeetingExtra")]
    public partial class MeetingExtra : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _MeetingId;

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

        private EntityRef<Meeting> _Meeting;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnMeetingIdChanging(int value);
        partial void OnMeetingIdChanged();

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

        public MeetingExtra()
        {
            _Meeting = default(EntityRef<Meeting>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "MeetingId", UpdateCheck = UpdateCheck.Never, Storage = "_MeetingId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int MeetingId
        {
            get => _MeetingId;

            set
            {
                if (_MeetingId != value)
                {
                    if (_Meeting.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnMeetingIdChanging(value);
                    SendPropertyChanging();
                    _MeetingId = value;
                    SendPropertyChanged("MeetingId");
                    OnMeetingIdChanged();
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

        [Association(Name = "FK_MeetingExtra_Meetings", Storage = "_Meeting", ThisKey = "MeetingId", IsForeignKey = true)]
        public Meeting Meeting
        {
            get => _Meeting.Entity;

            set
            {
                Meeting previousValue = _Meeting.Entity;
                if (((previousValue != value)
                            || (_Meeting.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Meeting.Entity = null;
                        previousValue.MeetingExtras.Remove(this);
                    }

                    _Meeting.Entity = value;
                    if (value != null)
                    {
                        value.MeetingExtras.Add(this);

                        _MeetingId = value.MeetingId;

                    }

                    else
                    {
                        _MeetingId = default(int);

                    }

                    SendPropertyChanged("Meeting");
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
