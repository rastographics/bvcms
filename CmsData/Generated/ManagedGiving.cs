using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ManagedGiving")]
    public partial class ManagedGiving : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _PeopleId;

        private DateTime? _StartWhen;

        private DateTime? _NextDate;

        private string _SemiEvery;

        private int? _Day1;

        private int? _Day2;

        private int? _EveryN;

        private string _Period;

        private DateTime? _StopWhen;

        private int? _StopAfter;

        private string _Type;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnStartWhenChanging(DateTime? value);
        partial void OnStartWhenChanged();

        partial void OnNextDateChanging(DateTime? value);
        partial void OnNextDateChanged();

        partial void OnSemiEveryChanging(string value);
        partial void OnSemiEveryChanged();

        partial void OnDay1Changing(int? value);
        partial void OnDay1Changed();

        partial void OnDay2Changing(int? value);
        partial void OnDay2Changed();

        partial void OnEveryNChanging(int? value);
        partial void OnEveryNChanged();

        partial void OnPeriodChanging(string value);
        partial void OnPeriodChanged();

        partial void OnStopWhenChanging(DateTime? value);
        partial void OnStopWhenChanged();

        partial void OnStopAfterChanging(int? value);
        partial void OnStopAfterChanged();

        partial void OnTypeChanging(string value);
        partial void OnTypeChanged();

        #endregion

        public ManagedGiving()
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

        [Column(Name = "StartWhen", UpdateCheck = UpdateCheck.Never, Storage = "_StartWhen", DbType = "datetime")]
        public DateTime? StartWhen
        {
            get => _StartWhen;

            set
            {
                if (_StartWhen != value)
                {
                    OnStartWhenChanging(value);
                    SendPropertyChanging();
                    _StartWhen = value;
                    SendPropertyChanged("StartWhen");
                    OnStartWhenChanged();
                }
            }
        }

        [Column(Name = "NextDate", UpdateCheck = UpdateCheck.Never, Storage = "_NextDate", DbType = "datetime")]
        public DateTime? NextDate
        {
            get => _NextDate;

            set
            {
                if (_NextDate != value)
                {
                    OnNextDateChanging(value);
                    SendPropertyChanging();
                    _NextDate = value;
                    SendPropertyChanged("NextDate");
                    OnNextDateChanged();
                }
            }
        }

        [Column(Name = "SemiEvery", UpdateCheck = UpdateCheck.Never, Storage = "_SemiEvery", DbType = "nvarchar(2)")]
        public string SemiEvery
        {
            get => _SemiEvery;

            set
            {
                if (_SemiEvery != value)
                {
                    OnSemiEveryChanging(value);
                    SendPropertyChanging();
                    _SemiEvery = value;
                    SendPropertyChanged("SemiEvery");
                    OnSemiEveryChanged();
                }
            }
        }

        [Column(Name = "Day1", UpdateCheck = UpdateCheck.Never, Storage = "_Day1", DbType = "int")]
        public int? Day1
        {
            get => _Day1;

            set
            {
                if (_Day1 != value)
                {
                    OnDay1Changing(value);
                    SendPropertyChanging();
                    _Day1 = value;
                    SendPropertyChanged("Day1");
                    OnDay1Changed();
                }
            }
        }

        [Column(Name = "Day2", UpdateCheck = UpdateCheck.Never, Storage = "_Day2", DbType = "int")]
        public int? Day2
        {
            get => _Day2;

            set
            {
                if (_Day2 != value)
                {
                    OnDay2Changing(value);
                    SendPropertyChanging();
                    _Day2 = value;
                    SendPropertyChanged("Day2");
                    OnDay2Changed();
                }
            }
        }

        [Column(Name = "EveryN", UpdateCheck = UpdateCheck.Never, Storage = "_EveryN", DbType = "int")]
        public int? EveryN
        {
            get => _EveryN;

            set
            {
                if (_EveryN != value)
                {
                    OnEveryNChanging(value);
                    SendPropertyChanging();
                    _EveryN = value;
                    SendPropertyChanged("EveryN");
                    OnEveryNChanged();
                }
            }
        }

        [Column(Name = "Period", UpdateCheck = UpdateCheck.Never, Storage = "_Period", DbType = "nvarchar(2)")]
        public string Period
        {
            get => _Period;

            set
            {
                if (_Period != value)
                {
                    OnPeriodChanging(value);
                    SendPropertyChanging();
                    _Period = value;
                    SendPropertyChanged("Period");
                    OnPeriodChanged();
                }
            }
        }

        [Column(Name = "StopWhen", UpdateCheck = UpdateCheck.Never, Storage = "_StopWhen", DbType = "datetime")]
        public DateTime? StopWhen
        {
            get => _StopWhen;

            set
            {
                if (_StopWhen != value)
                {
                    OnStopWhenChanging(value);
                    SendPropertyChanging();
                    _StopWhen = value;
                    SendPropertyChanged("StopWhen");
                    OnStopWhenChanged();
                }
            }
        }

        [Column(Name = "StopAfter", UpdateCheck = UpdateCheck.Never, Storage = "_StopAfter", DbType = "int")]
        public int? StopAfter
        {
            get => _StopAfter;

            set
            {
                if (_StopAfter != value)
                {
                    OnStopAfterChanging(value);
                    SendPropertyChanging();
                    _StopAfter = value;
                    SendPropertyChanged("StopAfter");
                    OnStopAfterChanged();
                }
            }
        }

        [Column(Name = "type", UpdateCheck = UpdateCheck.Never, Storage = "_Type", DbType = "nvarchar(2)")]
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

        [Association(Name = "FK_ManagedGiving_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.ManagedGivings.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.ManagedGivings.Add(this);

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
