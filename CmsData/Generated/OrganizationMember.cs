using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.OrganizationMembers")]
    public partial class OrganizationMember : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _OrganizationId;

        private int _PeopleId;

        private int? _CreatedBy;

        private DateTime? _CreatedDate;

        private int _MemberTypeId;

        private DateTime? _EnrollmentDate;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private DateTime? _InactiveDate;

        private string _AttendStr;

        private decimal? _AttendPct;

        private DateTime? _LastAttended;

        private bool? _Pending;

        private string _UserData;

        private decimal? _Amount;

        private string _Request;

        private string _ShirtSize;

        private int? _Grade;

        private int? _Tickets;

        private bool? _Moved;

        private string _RegisterEmail;

        private decimal? _AmountPaid;

        private string _PayLink;

        private int? _TranId;

        private int _Score;

        private int? _DatumId;

        private bool? _Hidden;

        private bool? _SkipInsertTriggerProcessing;

        private int? _RegistrationDataId;

        private string _OnlineRegData;

        private EntitySet<OrgMemberExtra> _OrgMemberExtras;

        private EntitySet<OrgMemMemTag> _OrgMemMemTags;

        private EntityRef<MemberType> _MemberType;

        private EntityRef<RegistrationDatum> _RegistrationDatum;

        private EntityRef<Transaction> _Transaction;

        private EntityRef<Organization> _Organization;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnOrganizationIdChanging(int value);
        partial void OnOrganizationIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnCreatedByChanging(int? value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime? value);
        partial void OnCreatedDateChanged();

        partial void OnMemberTypeIdChanging(int value);
        partial void OnMemberTypeIdChanged();

        partial void OnEnrollmentDateChanging(DateTime? value);
        partial void OnEnrollmentDateChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        partial void OnInactiveDateChanging(DateTime? value);
        partial void OnInactiveDateChanged();

        partial void OnAttendStrChanging(string value);
        partial void OnAttendStrChanged();

        partial void OnAttendPctChanging(decimal? value);
        partial void OnAttendPctChanged();

        partial void OnLastAttendedChanging(DateTime? value);
        partial void OnLastAttendedChanged();

        partial void OnPendingChanging(bool? value);
        partial void OnPendingChanged();

        partial void OnUserDataChanging(string value);
        partial void OnUserDataChanged();

        partial void OnAmountChanging(decimal? value);
        partial void OnAmountChanged();

        partial void OnRequestChanging(string value);
        partial void OnRequestChanged();

        partial void OnShirtSizeChanging(string value);
        partial void OnShirtSizeChanged();

        partial void OnGradeChanging(int? value);
        partial void OnGradeChanged();

        partial void OnTicketsChanging(int? value);
        partial void OnTicketsChanged();

        partial void OnMovedChanging(bool? value);
        partial void OnMovedChanged();

        partial void OnRegisterEmailChanging(string value);
        partial void OnRegisterEmailChanged();

        partial void OnAmountPaidChanging(decimal? value);
        partial void OnAmountPaidChanged();

        partial void OnPayLinkChanging(string value);
        partial void OnPayLinkChanged();

        partial void OnTranIdChanging(int? value);
        partial void OnTranIdChanged();

        partial void OnScoreChanging(int value);
        partial void OnScoreChanged();

        partial void OnDatumIdChanging(int? value);
        partial void OnDatumIdChanged();

        partial void OnHiddenChanging(bool? value);
        partial void OnHiddenChanged();

        partial void OnSkipInsertTriggerProcessingChanging(bool? value);
        partial void OnSkipInsertTriggerProcessingChanged();

        partial void OnRegistrationDataIdChanging(int? value);
        partial void OnRegistrationDataIdChanged();

        partial void OnOnlineRegDataChanging(string value);
        partial void OnOnlineRegDataChanged();

        #endregion

        public OrganizationMember()
        {
            _OrgMemberExtras = new EntitySet<OrgMemberExtra>(new Action<OrgMemberExtra>(attach_OrgMemberExtras), new Action<OrgMemberExtra>(detach_OrgMemberExtras));

            _OrgMemMemTags = new EntitySet<OrgMemMemTag>(new Action<OrgMemMemTag>(attach_OrgMemMemTags), new Action<OrgMemMemTag>(detach_OrgMemMemTags));

            _MemberType = default(EntityRef<MemberType>);

            _RegistrationDatum = default(EntityRef<RegistrationDatum>);

            _Transaction = default(EntityRef<Transaction>);

            _Organization = default(EntityRef<Organization>);

            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

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

        [Column(Name = "ModifiedBy", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedBy", DbType = "int")]
        public int? ModifiedBy
        {
            get => _ModifiedBy;

            set
            {
                if (_ModifiedBy != value)
                {
                    OnModifiedByChanging(value);
                    SendPropertyChanging();
                    _ModifiedBy = value;
                    SendPropertyChanged("ModifiedBy");
                    OnModifiedByChanged();
                }
            }
        }

        [Column(Name = "ModifiedDate", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedDate", DbType = "datetime")]
        public DateTime? ModifiedDate
        {
            get => _ModifiedDate;

            set
            {
                if (_ModifiedDate != value)
                {
                    OnModifiedDateChanging(value);
                    SendPropertyChanging();
                    _ModifiedDate = value;
                    SendPropertyChanged("ModifiedDate");
                    OnModifiedDateChanged();
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

        [Column(Name = "AttendStr", UpdateCheck = UpdateCheck.Never, Storage = "_AttendStr", DbType = "nvarchar(300)")]
        public string AttendStr
        {
            get => _AttendStr;

            set
            {
                if (_AttendStr != value)
                {
                    OnAttendStrChanging(value);
                    SendPropertyChanging();
                    _AttendStr = value;
                    SendPropertyChanged("AttendStr");
                    OnAttendStrChanged();
                }
            }
        }

        [Column(Name = "AttendPct", UpdateCheck = UpdateCheck.Never, Storage = "_AttendPct", DbType = "real")]
        public decimal? AttendPct
        {
            get => _AttendPct;

            set
            {
                if (_AttendPct != value)
                {
                    OnAttendPctChanging(value);
                    SendPropertyChanging();
                    _AttendPct = value;
                    SendPropertyChanged("AttendPct");
                    OnAttendPctChanged();
                }
            }
        }

        [Column(Name = "LastAttended", UpdateCheck = UpdateCheck.Never, Storage = "_LastAttended", DbType = "datetime")]
        public DateTime? LastAttended
        {
            get => _LastAttended;

            set
            {
                if (_LastAttended != value)
                {
                    OnLastAttendedChanging(value);
                    SendPropertyChanging();
                    _LastAttended = value;
                    SendPropertyChanged("LastAttended");
                    OnLastAttendedChanged();
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

        [Column(Name = "Amount", UpdateCheck = UpdateCheck.Never, Storage = "_Amount", DbType = "money")]
        public decimal? Amount
        {
            get => _Amount;

            set
            {
                if (_Amount != value)
                {
                    OnAmountChanging(value);
                    SendPropertyChanging();
                    _Amount = value;
                    SendPropertyChanged("Amount");
                    OnAmountChanged();
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

        [Column(Name = "Moved", UpdateCheck = UpdateCheck.Never, Storage = "_Moved", DbType = "bit")]
        public bool? Moved
        {
            get => _Moved;

            set
            {
                if (_Moved != value)
                {
                    OnMovedChanging(value);
                    SendPropertyChanging();
                    _Moved = value;
                    SendPropertyChanged("Moved");
                    OnMovedChanged();
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

        [Column(Name = "AmountPaid", UpdateCheck = UpdateCheck.Never, Storage = "_AmountPaid", DbType = "money")]
        public decimal? AmountPaid
        {
            get => _AmountPaid;

            set
            {
                if (_AmountPaid != value)
                {
                    OnAmountPaidChanging(value);
                    SendPropertyChanging();
                    _AmountPaid = value;
                    SendPropertyChanged("AmountPaid");
                    OnAmountPaidChanged();
                }
            }
        }

        [Column(Name = "PayLink", UpdateCheck = UpdateCheck.Never, Storage = "_PayLink", DbType = "nvarchar(100)")]
        public string PayLink
        {
            get => _PayLink;

            set
            {
                if (_PayLink != value)
                {
                    OnPayLinkChanging(value);
                    SendPropertyChanging();
                    _PayLink = value;
                    SendPropertyChanged("PayLink");
                    OnPayLinkChanged();
                }
            }
        }

        [Column(Name = "TranId", UpdateCheck = UpdateCheck.Never, Storage = "_TranId", DbType = "int")]
        [IsForeignKey]
        public int? TranId
        {
            get => _TranId;

            set
            {
                if (_TranId != value)
                {
                    if (_Transaction.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

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

        [Column(Name = "DatumId", UpdateCheck = UpdateCheck.Never, Storage = "_DatumId", DbType = "int")]
        public int? DatumId
        {
            get => _DatumId;

            set
            {
                if (_DatumId != value)
                {
                    OnDatumIdChanging(value);
                    SendPropertyChanging();
                    _DatumId = value;
                    SendPropertyChanged("DatumId");
                    OnDatumIdChanged();
                }
            }
        }

        [Column(Name = "Hidden", UpdateCheck = UpdateCheck.Never, Storage = "_Hidden", DbType = "bit")]
        public bool? Hidden
        {
            get => _Hidden;

            set
            {
                if (_Hidden != value)
                {
                    OnHiddenChanging(value);
                    SendPropertyChanging();
                    _Hidden = value;
                    SendPropertyChanged("Hidden");
                    OnHiddenChanged();
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

        [Column(Name = "RegistrationDataId", UpdateCheck = UpdateCheck.Never, Storage = "_RegistrationDataId", DbType = "int")]
        [IsForeignKey]
        public int? RegistrationDataId
        {
            get => _RegistrationDataId;

            set
            {
                if (_RegistrationDataId != value)
                {
                    if (_RegistrationDatum.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnRegistrationDataIdChanging(value);
                    SendPropertyChanging();
                    _RegistrationDataId = value;
                    SendPropertyChanged("RegistrationDataId");
                    OnRegistrationDataIdChanged();
                }
            }
        }

        [Column(Name = "OnlineRegData", UpdateCheck = UpdateCheck.Never, Storage = "_OnlineRegData", DbType = "xml")]
        public string OnlineRegData
        {
            get => _OnlineRegData;

            set
            {
                if (_OnlineRegData != value)
                {
                    OnOnlineRegDataChanging(value);
                    SendPropertyChanging();
                    _OnlineRegData = value;
                    SendPropertyChanged("OnlineRegData");
                    OnOnlineRegDataChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_OrgMemberExtra_OrganizationMembers", Storage = "_OrgMemberExtras", OtherKey = "OrganizationId,PeopleId")]
        public EntitySet<OrgMemberExtra> OrgMemberExtras
           {
               get => _OrgMemberExtras;

            set => _OrgMemberExtras.Assign(value);

           }

        [Association(Name = "FK_OrgMemMemTags_OrganizationMembers", Storage = "_OrgMemMemTags", OtherKey = "OrgId,PeopleId")]
        public EntitySet<OrgMemMemTag> OrgMemMemTags
           {
               get => _OrgMemMemTags;

            set => _OrgMemMemTags.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_ORGANIZATION_MEMBERS_TBL_MemberType", Storage = "_MemberType", ThisKey = "MemberTypeId", IsForeignKey = true)]
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
                        previousValue.OrganizationMembers.Remove(this);
                    }

                    _MemberType.Entity = value;
                    if (value != null)
                    {
                        value.OrganizationMembers.Add(this);

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

        [Association(Name = "FK_OrganizationMembers_RegistrationData", Storage = "_RegistrationDatum", ThisKey = "RegistrationDataId", IsForeignKey = true)]
        public RegistrationDatum RegistrationDatum
        {
            get => _RegistrationDatum.Entity;

            set
            {
                RegistrationDatum previousValue = _RegistrationDatum.Entity;
                if (((previousValue != value)
                            || (_RegistrationDatum.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _RegistrationDatum.Entity = null;
                        previousValue.OrganizationMembers.Remove(this);
                    }

                    _RegistrationDatum.Entity = value;
                    if (value != null)
                    {
                        value.OrganizationMembers.Add(this);

                        _RegistrationDataId = value.Id;

                    }

                    else
                    {
                        _RegistrationDataId = default(int?);

                    }

                    SendPropertyChanged("RegistrationDatum");
                }
            }
        }

        [Association(Name = "FK_OrganizationMembers_Transaction", Storage = "_Transaction", ThisKey = "TranId", IsForeignKey = true)]
        public Transaction Transaction
        {
            get => _Transaction.Entity;

            set
            {
                Transaction previousValue = _Transaction.Entity;
                if (((previousValue != value)
                            || (_Transaction.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Transaction.Entity = null;
                        previousValue.OrganizationMembers.Remove(this);
                    }

                    _Transaction.Entity = value;
                    if (value != null)
                    {
                        value.OrganizationMembers.Add(this);

                        _TranId = value.Id;

                    }

                    else
                    {
                        _TranId = default(int?);

                    }

                    SendPropertyChanged("Transaction");
                }
            }
        }

        [Association(Name = "ORGANIZATION_MEMBERS_ORG_FK", Storage = "_Organization", ThisKey = "OrganizationId", IsForeignKey = true)]
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
                        previousValue.OrganizationMembers.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.OrganizationMembers.Add(this);

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

        [Association(Name = "ORGANIZATION_MEMBERS_PPL_FK", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.OrganizationMembers.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.OrganizationMembers.Add(this);

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

        private void attach_OrgMemberExtras(OrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.OrganizationMember = this;
        }

        private void detach_OrgMemberExtras(OrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.OrganizationMember = null;
        }

        private void attach_OrgMemMemTags(OrgMemMemTag entity)
        {
            SendPropertyChanging();
            entity.OrganizationMember = this;
        }

        private void detach_OrgMemMemTags(OrgMemMemTag entity)
        {
            SendPropertyChanging();
            entity.OrganizationMember = null;
        }
    }
}
