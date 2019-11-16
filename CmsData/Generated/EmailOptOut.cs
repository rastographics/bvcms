using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.EmailOptOut")]
    public partial class EmailOptOut : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ToPeopleId;

        private string _FromEmail;

        private DateTime? _DateX;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnToPeopleIdChanging(int value);
        partial void OnToPeopleIdChanged();

        partial void OnFromEmailChanging(string value);
        partial void OnFromEmailChanged();

        partial void OnDateXChanging(DateTime? value);
        partial void OnDateXChanged();

        #endregion

        public EmailOptOut()
        {
            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ToPeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_ToPeopleId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int ToPeopleId
        {
            get => _ToPeopleId;

            set
            {
                if (_ToPeopleId != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnToPeopleIdChanging(value);
                    SendPropertyChanging();
                    _ToPeopleId = value;
                    SendPropertyChanged("ToPeopleId");
                    OnToPeopleIdChanged();
                }
            }
        }

        [Column(Name = "FromEmail", UpdateCheck = UpdateCheck.Never, Storage = "_FromEmail", DbType = "nvarchar(50) NOT NULL", IsPrimaryKey = true)]
        public string FromEmail
        {
            get => _FromEmail;

            set
            {
                if (_FromEmail != value)
                {
                    OnFromEmailChanging(value);
                    SendPropertyChanging();
                    _FromEmail = value;
                    SendPropertyChanged("FromEmail");
                    OnFromEmailChanged();
                }
            }
        }

        [Column(Name = "Date", UpdateCheck = UpdateCheck.Never, Storage = "_DateX", DbType = "datetime")]
        public DateTime? DateX
        {
            get => _DateX;

            set
            {
                if (_DateX != value)
                {
                    OnDateXChanging(value);
                    SendPropertyChanging();
                    _DateX = value;
                    SendPropertyChanged("DateX");
                    OnDateXChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_EmailOptOut_People", Storage = "_Person", ThisKey = "ToPeopleId", IsForeignKey = true)]
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
                        previousValue.EmailOptOuts.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.EmailOptOuts.Add(this);

                        _ToPeopleId = value.PeopleId;

                    }

                    else
                    {
                        _ToPeopleId = default(int);

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
