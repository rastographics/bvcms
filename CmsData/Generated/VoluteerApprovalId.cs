using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.VoluteerApprovalIds")]
    public partial class VoluteerApprovalId : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _PeopleId;

        private int _ApprovalId;

        private EntityRef<Person> _Person;

        private EntityRef<Volunteer> _Volunteer;

        private EntityRef<VolunteerCode> _VolunteerCode;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnApprovalIdChanging(int value);
        partial void OnApprovalIdChanged();

        #endregion

        public VoluteerApprovalId()
        {
            _Person = default(EntityRef<Person>);

            _Volunteer = default(EntityRef<Volunteer>);

            _VolunteerCode = default(EntityRef<VolunteerCode>);

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
                    if (_Volunteer.HasLoadedOrAssignedValue)
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

        [Column(Name = "ApprovalId", UpdateCheck = UpdateCheck.Never, Storage = "_ApprovalId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int ApprovalId
        {
            get => _ApprovalId;

            set
            {
                if (_ApprovalId != value)
                {
                    if (_VolunteerCode.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnApprovalIdChanging(value);
                    SendPropertyChanging();
                    _ApprovalId = value;
                    SendPropertyChanged("ApprovalId");
                    OnApprovalIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_VoluteerApprovalIds_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.VoluteerApprovalIds.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.VoluteerApprovalIds.Add(this);

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

        [Association(Name = "FK_VoluteerApprovalIds_Volunteer", Storage = "_Volunteer", ThisKey = "PeopleId", IsForeignKey = true)]
        public Volunteer Volunteer
        {
            get => _Volunteer.Entity;

            set
            {
                Volunteer previousValue = _Volunteer.Entity;
                if (((previousValue != value)
                            || (_Volunteer.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Volunteer.Entity = null;
                        previousValue.VoluteerApprovalIds.Remove(this);
                    }

                    _Volunteer.Entity = value;
                    if (value != null)
                    {
                        value.VoluteerApprovalIds.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int);

                    }

                    SendPropertyChanged("Volunteer");
                }
            }
        }

        [Association(Name = "FK_VoluteerApprovalIds_VolunteerCodes", Storage = "_VolunteerCode", ThisKey = "ApprovalId", IsForeignKey = true)]
        public VolunteerCode VolunteerCode
        {
            get => _VolunteerCode.Entity;

            set
            {
                VolunteerCode previousValue = _VolunteerCode.Entity;
                if (((previousValue != value)
                            || (_VolunteerCode.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _VolunteerCode.Entity = null;
                        previousValue.VoluteerApprovalIds.Remove(this);
                    }

                    _VolunteerCode.Entity = value;
                    if (value != null)
                    {
                        value.VoluteerApprovalIds.Add(this);

                        _ApprovalId = value.Id;

                    }

                    else
                    {
                        _ApprovalId = default(int);

                    }

                    SendPropertyChanged("VolunteerCode");
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
