using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.MemberType")]
    public partial class MemberType : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Code;

        private string _Description;

        private int? _AttendanceTypeId;

        private bool? _Hardwired;

        private bool _Pending;

        private bool _Inactive;

        private EntitySet<Attend> _Attends;

        private EntitySet<EnrollmentTransaction> _EnrollmentTransactions;

        private EntitySet<OrganizationMember> _OrganizationMembers;

        private EntityRef<AttendType> _AttendType;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnCodeChanging(string value);
        partial void OnCodeChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnAttendanceTypeIdChanging(int? value);
        partial void OnAttendanceTypeIdChanged();

        partial void OnHardwiredChanging(bool? value);
        partial void OnHardwiredChanged();

        partial void OnPendingChanging(bool value);
        partial void OnPendingChanged();

        partial void OnInactiveChanging(bool value);
        partial void OnInactiveChanged();

        #endregion

        public MemberType()
        {
            _Attends = new EntitySet<Attend>(new Action<Attend>(attach_Attends), new Action<Attend>(detach_Attends));

            _EnrollmentTransactions = new EntitySet<EnrollmentTransaction>(new Action<EnrollmentTransaction>(attach_EnrollmentTransactions), new Action<EnrollmentTransaction>(detach_EnrollmentTransactions));

            _OrganizationMembers = new EntitySet<OrganizationMember>(new Action<OrganizationMember>(attach_OrganizationMembers), new Action<OrganizationMember>(detach_OrganizationMembers));

            _AttendType = default(EntityRef<AttendType>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "Code", UpdateCheck = UpdateCheck.Never, Storage = "_Code", DbType = "nvarchar(20)")]
        public string Code
        {
            get => _Code;

            set
            {
                if (_Code != value)
                {
                    OnCodeChanging(value);
                    SendPropertyChanging();
                    _Code = value;
                    SendPropertyChanged("Code");
                    OnCodeChanged();
                }
            }
        }

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(100)")]
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

        [Column(Name = "AttendanceTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_AttendanceTypeId", DbType = "int")]
        [IsForeignKey]
        public int? AttendanceTypeId
        {
            get => _AttendanceTypeId;

            set
            {
                if (_AttendanceTypeId != value)
                {
                    if (_AttendType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnAttendanceTypeIdChanging(value);
                    SendPropertyChanging();
                    _AttendanceTypeId = value;
                    SendPropertyChanged("AttendanceTypeId");
                    OnAttendanceTypeIdChanged();
                }
            }
        }

        [Column(Name = "Hardwired", UpdateCheck = UpdateCheck.Never, Storage = "_Hardwired", DbType = "bit")]
        public bool? Hardwired
        {
            get => _Hardwired;

            set
            {
                if (_Hardwired != value)
                {
                    OnHardwiredChanging(value);
                    SendPropertyChanging();
                    _Hardwired = value;
                    SendPropertyChanged("Hardwired");
                    OnHardwiredChanged();
                }
            }
        }

        [Column(Name = "Pending", UpdateCheck = UpdateCheck.Never, Storage = "_Pending", DbType = "bit NOT NULL")]
        public bool Pending
        {
            get => _Pending;

            set
            {
                if (_Pending != value)
                {
                    OnPendingChanging(value);
                    SendPropertyChanging();
                    _Pending = value;
                    SendPropertyChanged("Pending");
                    OnPendingChanged();
                }
            }
        }

        [Column(Name = "Inactive", UpdateCheck = UpdateCheck.Never, Storage = "_Inactive", DbType = "bit NOT NULL")]
        public bool Inactive
        {
            get => _Inactive;

            set
            {
                if (_Inactive != value)
                {
                    OnInactiveChanging(value);
                    SendPropertyChanging();
                    _Inactive = value;
                    SendPropertyChanged("Inactive");
                    OnInactiveChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_Attend_MemberType", Storage = "_Attends", OtherKey = "MemberTypeId")]
        public EntitySet<Attend> Attends
           {
               get => _Attends;

            set => _Attends.Assign(value);

           }

        [Association(Name = "FK_ENROLLMENT_TRANSACTION_TBL_MemberType", Storage = "_EnrollmentTransactions", OtherKey = "MemberTypeId")]
        public EntitySet<EnrollmentTransaction> EnrollmentTransactions
           {
               get => _EnrollmentTransactions;

            set => _EnrollmentTransactions.Assign(value);

           }

        [Association(Name = "FK_ORGANIZATION_MEMBERS_TBL_MemberType", Storage = "_OrganizationMembers", OtherKey = "MemberTypeId")]
        public EntitySet<OrganizationMember> OrganizationMembers
           {
               get => _OrganizationMembers;

            set => _OrganizationMembers.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_MemberType_AttendType", Storage = "_AttendType", ThisKey = "AttendanceTypeId", IsForeignKey = true)]
        public AttendType AttendType
        {
            get => _AttendType.Entity;

            set
            {
                AttendType previousValue = _AttendType.Entity;
                if (((previousValue != value)
                            || (_AttendType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _AttendType.Entity = null;
                        previousValue.MemberTypes.Remove(this);
                    }

                    _AttendType.Entity = value;
                    if (value != null)
                    {
                        value.MemberTypes.Add(this);

                        _AttendanceTypeId = value.Id;

                    }

                    else
                    {
                        _AttendanceTypeId = default(int?);

                    }

                    SendPropertyChanged("AttendType");
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

        private void attach_Attends(Attend entity)
        {
            SendPropertyChanging();
            entity.MemberType = this;
        }

        private void detach_Attends(Attend entity)
        {
            SendPropertyChanging();
            entity.MemberType = null;
        }

        private void attach_EnrollmentTransactions(EnrollmentTransaction entity)
        {
            SendPropertyChanging();
            entity.MemberType = this;
        }

        private void detach_EnrollmentTransactions(EnrollmentTransaction entity)
        {
            SendPropertyChanging();
            entity.MemberType = null;
        }

        private void attach_OrganizationMembers(OrganizationMember entity)
        {
            SendPropertyChanging();
            entity.MemberType = this;
        }

        private void detach_OrganizationMembers(OrganizationMember entity)
        {
            SendPropertyChanging();
            entity.MemberType = null;
        }
    }
}
