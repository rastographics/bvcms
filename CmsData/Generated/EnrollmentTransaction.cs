using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.EnrollmentTransaction")]
    public partial class EnrollmentTransaction : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _TransactionId;

        private bool _TransactionStatus;

        private int? _CreatedBy;

        private DateTime? _CreatedDate;

        private DateTime _TransactionDate;

        private int _TransactionTypeId;

        private int _OrganizationId;

        private string _OrganizationName;

        private int _PeopleId;

        private int _MemberTypeId;

        private DateTime? _EnrollmentDate;

        private decimal? _AttendancePercentage;

        private DateTime? _NextTranChangeDate;

        private int? _EnrollmentTransactionId;

        private bool? _Pending;

        private DateTime? _InactiveDate;

        private string _UserData;

        private string _Request;

        private string _ShirtSize;

        private int? _Grade;

        private int? _Tickets;

        private string _RegisterEmail;

        private int? _TranId;

        private int _Score;

        private string _SmallGroups;

        private bool? _SkipInsertTriggerProcessing;

        private EntitySet<EnrollmentTransaction> _DescTransactions;

        private EntitySet<PrevOrgMemberExtra> _PrevOrgMemberExtras;

        private EntityRef<EnrollmentTransaction> _FirstTransaction;

        private EntityRef<Organization> _Organization;

        private EntityRef<Person> _Person;

        private EntityRef<MemberType> _MemberType;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnTransactionIdChanging(int value);
        partial void OnTransactionIdChanged();

        partial void OnTransactionStatusChanging(bool value);
        partial void OnTransactionStatusChanged();

        partial void OnCreatedByChanging(int? value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime? value);
        partial void OnCreatedDateChanged();

        partial void OnTransactionDateChanging(DateTime value);
        partial void OnTransactionDateChanged();

        partial void OnTransactionTypeIdChanging(int value);
        partial void OnTransactionTypeIdChanged();

        partial void OnOrganizationIdChanging(int value);
        partial void OnOrganizationIdChanged();

        partial void OnOrganizationNameChanging(string value);
        partial void OnOrganizationNameChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnMemberTypeIdChanging(int value);
        partial void OnMemberTypeIdChanged();

        partial void OnEnrollmentDateChanging(DateTime? value);
        partial void OnEnrollmentDateChanged();

        partial void OnAttendancePercentageChanging(decimal? value);
        partial void OnAttendancePercentageChanged();

        partial void OnNextTranChangeDateChanging(DateTime? value);
        partial void OnNextTranChangeDateChanged();

        partial void OnEnrollmentTransactionIdChanging(int? value);
        partial void OnEnrollmentTransactionIdChanged();

        partial void OnPendingChanging(bool? value);
        partial void OnPendingChanged();

        partial void OnInactiveDateChanging(DateTime? value);
        partial void OnInactiveDateChanged();

        partial void OnUserDataChanging(string value);
        partial void OnUserDataChanged();

        partial void OnRequestChanging(string value);
        partial void OnRequestChanged();

        partial void OnShirtSizeChanging(string value);
        partial void OnShirtSizeChanged();

        partial void OnGradeChanging(int? value);
        partial void OnGradeChanged();

        partial void OnTicketsChanging(int? value);
        partial void OnTicketsChanged();

        partial void OnRegisterEmailChanging(string value);
        partial void OnRegisterEmailChanged();

        partial void OnTranIdChanging(int? value);
        partial void OnTranIdChanged();

        partial void OnScoreChanging(int value);
        partial void OnScoreChanged();

        partial void OnSmallGroupsChanging(string value);
        partial void OnSmallGroupsChanged();

        partial void OnSkipInsertTriggerProcessingChanging(bool? value);
        partial void OnSkipInsertTriggerProcessingChanged();

        #endregion

        public EnrollmentTransaction()
        {
            _DescTransactions = new EntitySet<EnrollmentTransaction>(new Action<EnrollmentTransaction>(attach_DescTransactions), new Action<EnrollmentTransaction>(detach_DescTransactions));

            _PrevOrgMemberExtras = new EntitySet<PrevOrgMemberExtra>(new Action<PrevOrgMemberExtra>(attach_PrevOrgMemberExtras), new Action<PrevOrgMemberExtra>(detach_PrevOrgMemberExtras));

            _FirstTransaction = default(EntityRef<EnrollmentTransaction>);

            _Organization = default(EntityRef<Organization>);

            _Person = default(EntityRef<Person>);

            _MemberType = default(EntityRef<MemberType>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "TransactionId", UpdateCheck = UpdateCheck.Never, Storage = "_TransactionId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TransactionId
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

        [Column(Name = "TransactionStatus", UpdateCheck = UpdateCheck.Never, Storage = "_TransactionStatus", DbType = "bit NOT NULL")]
        public bool TransactionStatus
        {
            get => _TransactionStatus;

            set
            {
                if (_TransactionStatus != value)
                {
                    OnTransactionStatusChanging(value);
                    SendPropertyChanging();
                    _TransactionStatus = value;
                    SendPropertyChanged("TransactionStatus");
                    OnTransactionStatusChanged();
                }
            }
        }

        [Column(Name = "CreatedBy", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int")]
        public int? CreatedBy
        {
            get => _CreatedBy;

            set
            {
                if (_CreatedBy != value)
                {
                    OnCreatedByChanging(value);
                    SendPropertyChanging();
                    _CreatedBy = value;
                    SendPropertyChanged("CreatedBy");
                    OnCreatedByChanged();
                }
            }
        }

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime? CreatedDate
        {
            get => _CreatedDate;

            set
            {
                if (_CreatedDate != value)
                {
                    OnCreatedDateChanging(value);
                    SendPropertyChanging();
                    _CreatedDate = value;
                    SendPropertyChanged("CreatedDate");
                    OnCreatedDateChanged();
                }
            }
        }

        [Column(Name = "TransactionDate", UpdateCheck = UpdateCheck.Never, Storage = "_TransactionDate", DbType = "datetime NOT NULL")]
        public DateTime TransactionDate
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

        [Column(Name = "TransactionTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_TransactionTypeId", DbType = "int NOT NULL")]
        public int TransactionTypeId
        {
            get => _TransactionTypeId;

            set
            {
                if (_TransactionTypeId != value)
                {
                    OnTransactionTypeIdChanging(value);
                    SendPropertyChanging();
                    _TransactionTypeId = value;
                    SendPropertyChanged("TransactionTypeId");
                    OnTransactionTypeIdChanged();
                }
            }
        }

        [Column(Name = "OrganizationId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationId", DbType = "int NOT NULL")]
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

        [Column(Name = "OrganizationName", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationName", DbType = "nvarchar(100) NOT NULL")]
        public string OrganizationName
        {
            get => _OrganizationName;

            set
            {
                if (_OrganizationName != value)
                {
                    OnOrganizationNameChanging(value);
                    SendPropertyChanging();
                    _OrganizationName = value;
                    SendPropertyChanged("OrganizationName");
                    OnOrganizationNameChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL")]
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

        [Column(Name = "MemberTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_MemberTypeId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int MemberTypeId
        {
            get => _MemberTypeId;

            set
            {
                if (_MemberTypeId != value)
                {
                    if (_MemberType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnMemberTypeIdChanging(value);
                    SendPropertyChanging();
                    _MemberTypeId = value;
                    SendPropertyChanged("MemberTypeId");
                    OnMemberTypeIdChanged();
                }
            }
        }

        [Column(Name = "EnrollmentDate", UpdateCheck = UpdateCheck.Never, Storage = "_EnrollmentDate", DbType = "datetime")]
        public DateTime? EnrollmentDate
        {
            get => _EnrollmentDate;

            set
            {
                if (_EnrollmentDate != value)
                {
                    OnEnrollmentDateChanging(value);
                    SendPropertyChanging();
                    _EnrollmentDate = value;
                    SendPropertyChanged("EnrollmentDate");
                    OnEnrollmentDateChanged();
                }
            }
        }

        [Column(Name = "AttendancePercentage", UpdateCheck = UpdateCheck.Never, Storage = "_AttendancePercentage", DbType = "real")]
        public decimal? AttendancePercentage
        {
            get => _AttendancePercentage;

            set
            {
                if (_AttendancePercentage != value)
                {
                    OnAttendancePercentageChanging(value);
                    SendPropertyChanging();
                    _AttendancePercentage = value;
                    SendPropertyChanged("AttendancePercentage");
                    OnAttendancePercentageChanged();
                }
            }
        }

        [Column(Name = "NextTranChangeDate", UpdateCheck = UpdateCheck.Never, Storage = "_NextTranChangeDate", DbType = "datetime")]
        public DateTime? NextTranChangeDate
        {
            get => _NextTranChangeDate;

            set
            {
                if (_NextTranChangeDate != value)
                {
                    OnNextTranChangeDateChanging(value);
                    SendPropertyChanging();
                    _NextTranChangeDate = value;
                    SendPropertyChanged("NextTranChangeDate");
                    OnNextTranChangeDateChanged();
                }
            }
        }

        [Column(Name = "EnrollmentTransactionId", UpdateCheck = UpdateCheck.Never, Storage = "_EnrollmentTransactionId", DbType = "int")]
        [IsForeignKey]
        public int? EnrollmentTransactionId
        {
            get => _EnrollmentTransactionId;

            set
            {
                if (_EnrollmentTransactionId != value)
                {
                    if (_FirstTransaction.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnEnrollmentTransactionIdChanging(value);
                    SendPropertyChanging();
                    _EnrollmentTransactionId = value;
                    SendPropertyChanged("EnrollmentTransactionId");
                    OnEnrollmentTransactionIdChanged();
                }
            }
        }

        [Column(Name = "Pending", UpdateCheck = UpdateCheck.Never, Storage = "_Pending", DbType = "bit")]
        public bool? Pending
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

        [Column(Name = "InactiveDate", UpdateCheck = UpdateCheck.Never, Storage = "_InactiveDate", DbType = "datetime")]
        public DateTime? InactiveDate
        {
            get => _InactiveDate;

            set
            {
                if (_InactiveDate != value)
                {
                    OnInactiveDateChanging(value);
                    SendPropertyChanging();
                    _InactiveDate = value;
                    SendPropertyChanged("InactiveDate");
                    OnInactiveDateChanged();
                }
            }
        }

        [Column(Name = "UserData", UpdateCheck = UpdateCheck.Never, Storage = "_UserData", DbType = "nvarchar")]
        public string UserData
        {
            get => _UserData;

            set
            {
                if (_UserData != value)
                {
                    OnUserDataChanging(value);
                    SendPropertyChanging();
                    _UserData = value;
                    SendPropertyChanged("UserData");
                    OnUserDataChanged();
                }
            }
        }

        [Column(Name = "Request", UpdateCheck = UpdateCheck.Never, Storage = "_Request", DbType = "nvarchar(140)")]
        public string Request
        {
            get => _Request;

            set
            {
                if (_Request != value)
                {
                    OnRequestChanging(value);
                    SendPropertyChanging();
                    _Request = value;
                    SendPropertyChanged("Request");
                    OnRequestChanged();
                }
            }
        }

        [Column(Name = "ShirtSize", UpdateCheck = UpdateCheck.Never, Storage = "_ShirtSize", DbType = "nvarchar(50)")]
        public string ShirtSize
        {
            get => _ShirtSize;

            set
            {
                if (_ShirtSize != value)
                {
                    OnShirtSizeChanging(value);
                    SendPropertyChanging();
                    _ShirtSize = value;
                    SendPropertyChanged("ShirtSize");
                    OnShirtSizeChanged();
                }
            }
        }

        [Column(Name = "Grade", UpdateCheck = UpdateCheck.Never, Storage = "_Grade", DbType = "int")]
        public int? Grade
        {
            get => _Grade;

            set
            {
                if (_Grade != value)
                {
                    OnGradeChanging(value);
                    SendPropertyChanging();
                    _Grade = value;
                    SendPropertyChanged("Grade");
                    OnGradeChanged();
                }
            }
        }

        [Column(Name = "Tickets", UpdateCheck = UpdateCheck.Never, Storage = "_Tickets", DbType = "int")]
        public int? Tickets
        {
            get => _Tickets;

            set
            {
                if (_Tickets != value)
                {
                    OnTicketsChanging(value);
                    SendPropertyChanging();
                    _Tickets = value;
                    SendPropertyChanged("Tickets");
                    OnTicketsChanged();
                }
            }
        }

        [Column(Name = "RegisterEmail", UpdateCheck = UpdateCheck.Never, Storage = "_RegisterEmail", DbType = "nvarchar(80)")]
        public string RegisterEmail
        {
            get => _RegisterEmail;

            set
            {
                if (_RegisterEmail != value)
                {
                    OnRegisterEmailChanging(value);
                    SendPropertyChanging();
                    _RegisterEmail = value;
                    SendPropertyChanged("RegisterEmail");
                    OnRegisterEmailChanged();
                }
            }
        }

        [Column(Name = "TranId", UpdateCheck = UpdateCheck.Never, Storage = "_TranId", DbType = "int")]
        public int? TranId
        {
            get => _TranId;

            set
            {
                if (_TranId != value)
                {
                    OnTranIdChanging(value);
                    SendPropertyChanging();
                    _TranId = value;
                    SendPropertyChanged("TranId");
                    OnTranIdChanged();
                }
            }
        }

        [Column(Name = "Score", UpdateCheck = UpdateCheck.Never, Storage = "_Score", DbType = "int NOT NULL")]
        public int Score
        {
            get => _Score;

            set
            {
                if (_Score != value)
                {
                    OnScoreChanging(value);
                    SendPropertyChanging();
                    _Score = value;
                    SendPropertyChanged("Score");
                    OnScoreChanged();
                }
            }
        }

        [Column(Name = "SmallGroups", UpdateCheck = UpdateCheck.Never, Storage = "_SmallGroups", DbType = "nvarchar(2000)")]
        public string SmallGroups
        {
            get => _SmallGroups;

            set
            {
                if (_SmallGroups != value)
                {
                    OnSmallGroupsChanging(value);
                    SendPropertyChanging();
                    _SmallGroups = value;
                    SendPropertyChanged("SmallGroups");
                    OnSmallGroupsChanged();
                }
            }
        }

        [Column(Name = "SkipInsertTriggerProcessing", UpdateCheck = UpdateCheck.Never, Storage = "_SkipInsertTriggerProcessing", DbType = "bit")]
        public bool? SkipInsertTriggerProcessing
        {
            get => _SkipInsertTriggerProcessing;

            set
            {
                if (_SkipInsertTriggerProcessing != value)
                {
                    OnSkipInsertTriggerProcessingChanging(value);
                    SendPropertyChanging();
                    _SkipInsertTriggerProcessing = value;
                    SendPropertyChanged("SkipInsertTriggerProcessing");
                    OnSkipInsertTriggerProcessingChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "DescTransactions__FirstTransaction", Storage = "_DescTransactions", OtherKey = "EnrollmentTransactionId")]
        public EntitySet<EnrollmentTransaction> DescTransactions
           {
               get => _DescTransactions;

            set => _DescTransactions.Assign(value);

           }

        [Association(Name = "FK_PrevOrgMemberExtra_EnrollmentTransaction", Storage = "_PrevOrgMemberExtras", OtherKey = "EnrollmentTranId")]
        public EntitySet<PrevOrgMemberExtra> PrevOrgMemberExtras
           {
               get => _PrevOrgMemberExtras;

            set => _PrevOrgMemberExtras.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "DescTransactions__FirstTransaction", Storage = "_FirstTransaction", ThisKey = "EnrollmentTransactionId", IsForeignKey = true)]
        public EnrollmentTransaction FirstTransaction
        {
            get => _FirstTransaction.Entity;

            set
            {
                EnrollmentTransaction previousValue = _FirstTransaction.Entity;
                if (((previousValue != value)
                            || (_FirstTransaction.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _FirstTransaction.Entity = null;
                        previousValue.DescTransactions.Remove(this);
                    }

                    _FirstTransaction.Entity = value;
                    if (value != null)
                    {
                        value.DescTransactions.Add(this);

                        _EnrollmentTransactionId = value.TransactionId;

                    }

                    else
                    {
                        _EnrollmentTransactionId = default(int?);

                    }

                    SendPropertyChanged("FirstTransaction");
                }
            }
        }

        [Association(Name = "ENROLLMENT_TRANSACTION_ORG_FK", Storage = "_Organization", ThisKey = "OrganizationId", IsForeignKey = true)]
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
                        previousValue.EnrollmentTransactions.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.EnrollmentTransactions.Add(this);

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

        [Association(Name = "ENROLLMENT_TRANSACTION_PPL_FK", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.EnrollmentTransactions.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.EnrollmentTransactions.Add(this);

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

        [Association(Name = "FK_ENROLLMENT_TRANSACTION_TBL_MemberType", Storage = "_MemberType", ThisKey = "MemberTypeId", IsForeignKey = true)]
        public MemberType MemberType
        {
            get => _MemberType.Entity;

            set
            {
                MemberType previousValue = _MemberType.Entity;
                if (((previousValue != value)
                            || (_MemberType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _MemberType.Entity = null;
                        previousValue.EnrollmentTransactions.Remove(this);
                    }

                    _MemberType.Entity = value;
                    if (value != null)
                    {
                        value.EnrollmentTransactions.Add(this);

                        _MemberTypeId = value.Id;

                    }

                    else
                    {
                        _MemberTypeId = default(int);

                    }

                    SendPropertyChanged("MemberType");
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

        private void attach_DescTransactions(EnrollmentTransaction entity)
        {
            SendPropertyChanging();
            entity.FirstTransaction = this;
        }

        private void detach_DescTransactions(EnrollmentTransaction entity)
        {
            SendPropertyChanging();
            entity.FirstTransaction = null;
        }

        private void attach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.EnrollmentTransaction = this;
        }

        private void detach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.EnrollmentTransaction = null;
        }
    }
}
