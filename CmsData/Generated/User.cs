using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Users")]
    public partial class User : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _UserId;

        private int? _PeopleId;

        private string _Username;

        private string _Comment;

        private string _Password;

        private string _PasswordQuestion;

        private string _PasswordAnswer;

        private bool _IsApproved;

        private DateTime? _LastActivityDate;

        private DateTime? _LastLoginDate;

        private DateTime? _LastPasswordChangedDate;

        private DateTime? _CreationDate;

        private bool _IsLockedOut;

        private DateTime? _LastLockedOutDate;

        private int _FailedPasswordAttemptCount;

        private DateTime? _FailedPasswordAttemptWindowStart;

        private int _FailedPasswordAnswerAttemptCount;

        private DateTime? _FailedPasswordAnswerAttemptWindowStart;

        private string _EmailAddress;

        private int? _ItemsInGrid;

        private string _CurrentCart;

        private bool _MFAEnabled;

        private string _Secret;

        private bool _MustChangePassword;

        private string _Host;

        private string _TempPassword;

        private string _Name;

        private string _Name2;

        private Guid? _ResetPasswordCode;

        private string _DefaultGroup;

        private DateTime? _ResetPasswordExpires;

        private EntitySet<Coupon> _Coupons;

        private EntitySet<MobileAppDevice> _MobileAppDevices;

        private EntitySet<SMSGroupMember> _SMSGroupMembers;

        private EntitySet<Preference> _Preferences;

        private EntitySet<UserRole> _UserRoles;

        private EntitySet<ApiSession> _ApiSessions;

        private EntitySet<VolunteerForm> _VolunteerFormsUploaded;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnUserIdChanging(int value);
        partial void OnUserIdChanged();

        partial void OnPeopleIdChanging(int? value);
        partial void OnPeopleIdChanged();

        partial void OnUsernameChanging(string value);
        partial void OnUsernameChanged();

        partial void OnCommentChanging(string value);
        partial void OnCommentChanged();

        partial void OnPasswordChanging(string value);
        partial void OnPasswordChanged();

        partial void OnPasswordQuestionChanging(string value);
        partial void OnPasswordQuestionChanged();

        partial void OnPasswordAnswerChanging(string value);
        partial void OnPasswordAnswerChanged();

        partial void OnIsApprovedChanging(bool value);
        partial void OnIsApprovedChanged();

        partial void OnLastActivityDateChanging(DateTime? value);
        partial void OnLastActivityDateChanged();

        partial void OnLastLoginDateChanging(DateTime? value);
        partial void OnLastLoginDateChanged();

        partial void OnLastPasswordChangedDateChanging(DateTime? value);
        partial void OnLastPasswordChangedDateChanged();

        partial void OnCreationDateChanging(DateTime? value);
        partial void OnCreationDateChanged();

        partial void OnIsLockedOutChanging(bool value);
        partial void OnIsLockedOutChanged();

        partial void OnLastLockedOutDateChanging(DateTime? value);
        partial void OnLastLockedOutDateChanged();

        partial void OnFailedPasswordAttemptCountChanging(int value);
        partial void OnFailedPasswordAttemptCountChanged();

        partial void OnFailedPasswordAttemptWindowStartChanging(DateTime? value);
        partial void OnFailedPasswordAttemptWindowStartChanged();

        partial void OnFailedPasswordAnswerAttemptCountChanging(int value);
        partial void OnFailedPasswordAnswerAttemptCountChanged();

        partial void OnFailedPasswordAnswerAttemptWindowStartChanging(DateTime? value);
        partial void OnFailedPasswordAnswerAttemptWindowStartChanged();

        partial void OnEmailAddressChanging(string value);
        partial void OnEmailAddressChanged();

        partial void OnItemsInGridChanging(int? value);
        partial void OnItemsInGridChanged();

        partial void OnCurrentCartChanging(string value);
        partial void OnCurrentCartChanged();

        partial void OnMFAEnabledChanging(bool value);
        partial void OnMFAEnabledChanged();

        partial void OnSecretChanging(string value);
        partial void OnSecretChanged();

        partial void OnMustChangePasswordChanging(bool value);
        partial void OnMustChangePasswordChanged();

        partial void OnHostChanging(string value);
        partial void OnHostChanged();

        partial void OnTempPasswordChanging(string value);
        partial void OnTempPasswordChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnName2Changing(string value);
        partial void OnName2Changed();

        partial void OnResetPasswordCodeChanging(Guid? value);
        partial void OnResetPasswordCodeChanged();

        partial void OnDefaultGroupChanging(string value);
        partial void OnDefaultGroupChanged();

        partial void OnResetPasswordExpiresChanging(DateTime? value);
        partial void OnResetPasswordExpiresChanged();

        #endregion

        public User()
        {
            _Coupons = new EntitySet<Coupon>(new Action<Coupon>(attach_Coupons), new Action<Coupon>(detach_Coupons));

            _MobileAppDevices = new EntitySet<MobileAppDevice>(new Action<MobileAppDevice>(attach_MobileAppDevices), new Action<MobileAppDevice>(detach_MobileAppDevices));

            _SMSGroupMembers = new EntitySet<SMSGroupMember>(new Action<SMSGroupMember>(attach_SMSGroupMembers), new Action<SMSGroupMember>(detach_SMSGroupMembers));

            _Preferences = new EntitySet<Preference>(new Action<Preference>(attach_Preferences), new Action<Preference>(detach_Preferences));

            _UserRoles = new EntitySet<UserRole>(new Action<UserRole>(attach_UserRoles), new Action<UserRole>(detach_UserRoles));

            _ApiSessions = new EntitySet<ApiSession>(new Action<ApiSession>(attach_ApiSessions), new Action<ApiSession>(detach_ApiSessions));

            _VolunteerFormsUploaded = new EntitySet<VolunteerForm>(new Action<VolunteerForm>(attach_VolunteerFormsUploaded), new Action<VolunteerForm>(detach_VolunteerFormsUploaded));

            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "UserId", UpdateCheck = UpdateCheck.Never, Storage = "_UserId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int UserId
        {
            get => _UserId;

            set
            {
                if (_UserId != value)
                {
                    OnUserIdChanging(value);
                    SendPropertyChanging();
                    _UserId = value;
                    SendPropertyChanged("UserId");
                    OnUserIdChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        [IsForeignKey]
        public int? PeopleId
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

        [Column(Name = "Username", UpdateCheck = UpdateCheck.Never, Storage = "_Username", DbType = "nvarchar(50) NOT NULL")]
        public string Username
        {
            get => _Username;

            set
            {
                if (_Username != value)
                {
                    OnUsernameChanging(value);
                    SendPropertyChanging();
                    _Username = value;
                    SendPropertyChanged("Username");
                    OnUsernameChanged();
                }
            }
        }

        [Column(Name = "Comment", UpdateCheck = UpdateCheck.Never, Storage = "_Comment", DbType = "nvarchar(255)")]
        public string Comment
        {
            get => _Comment;

            set
            {
                if (_Comment != value)
                {
                    OnCommentChanging(value);
                    SendPropertyChanging();
                    _Comment = value;
                    SendPropertyChanged("Comment");
                    OnCommentChanged();
                }
            }
        }

        [Column(Name = "Password", UpdateCheck = UpdateCheck.Never, Storage = "_Password", DbType = "nvarchar(128) NOT NULL")]
        public string Password
        {
            get => _Password;

            set
            {
                if (_Password != value)
                {
                    OnPasswordChanging(value);
                    SendPropertyChanging();
                    _Password = value;
                    SendPropertyChanged("Password");
                    OnPasswordChanged();
                }
            }
        }

        [Column(Name = "PasswordQuestion", UpdateCheck = UpdateCheck.Never, Storage = "_PasswordQuestion", DbType = "nvarchar(255)")]
        public string PasswordQuestion
        {
            get => _PasswordQuestion;

            set
            {
                if (_PasswordQuestion != value)
                {
                    OnPasswordQuestionChanging(value);
                    SendPropertyChanging();
                    _PasswordQuestion = value;
                    SendPropertyChanged("PasswordQuestion");
                    OnPasswordQuestionChanged();
                }
            }
        }

        [Column(Name = "PasswordAnswer", UpdateCheck = UpdateCheck.Never, Storage = "_PasswordAnswer", DbType = "nvarchar(255)")]
        public string PasswordAnswer
        {
            get => _PasswordAnswer;

            set
            {
                if (_PasswordAnswer != value)
                {
                    OnPasswordAnswerChanging(value);
                    SendPropertyChanging();
                    _PasswordAnswer = value;
                    SendPropertyChanged("PasswordAnswer");
                    OnPasswordAnswerChanged();
                }
            }
        }

        [Column(Name = "IsApproved", UpdateCheck = UpdateCheck.Never, Storage = "_IsApproved", DbType = "bit NOT NULL")]
        public bool IsApproved
        {
            get => _IsApproved;

            set
            {
                if (_IsApproved != value)
                {
                    OnIsApprovedChanging(value);
                    SendPropertyChanging();
                    _IsApproved = value;
                    SendPropertyChanged("IsApproved");
                    OnIsApprovedChanged();
                }
            }
        }

        [Column(Name = "LastActivityDate", UpdateCheck = UpdateCheck.Never, Storage = "_LastActivityDate", DbType = "datetime")]
        public DateTime? LastActivityDate
        {
            get => _LastActivityDate;

            set
            {
                if (_LastActivityDate != value)
                {
                    OnLastActivityDateChanging(value);
                    SendPropertyChanging();
                    _LastActivityDate = value;
                    SendPropertyChanged("LastActivityDate");
                    OnLastActivityDateChanged();
                }
            }
        }

        [Column(Name = "LastLoginDate", UpdateCheck = UpdateCheck.Never, Storage = "_LastLoginDate", DbType = "datetime")]
        public DateTime? LastLoginDate
        {
            get => _LastLoginDate;

            set
            {
                if (_LastLoginDate != value)
                {
                    OnLastLoginDateChanging(value);
                    SendPropertyChanging();
                    _LastLoginDate = value;
                    SendPropertyChanged("LastLoginDate");
                    OnLastLoginDateChanged();
                }
            }
        }

        [Column(Name = "LastPasswordChangedDate", UpdateCheck = UpdateCheck.Never, Storage = "_LastPasswordChangedDate", DbType = "datetime")]
        public DateTime? LastPasswordChangedDate
        {
            get => _LastPasswordChangedDate;

            set
            {
                if (_LastPasswordChangedDate != value)
                {
                    OnLastPasswordChangedDateChanging(value);
                    SendPropertyChanging();
                    _LastPasswordChangedDate = value;
                    SendPropertyChanged("LastPasswordChangedDate");
                    OnLastPasswordChangedDateChanged();
                }
            }
        }

        [Column(Name = "CreationDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreationDate", DbType = "datetime")]
        public DateTime? CreationDate
        {
            get => _CreationDate;

            set
            {
                if (_CreationDate != value)
                {
                    OnCreationDateChanging(value);
                    SendPropertyChanging();
                    _CreationDate = value;
                    SendPropertyChanged("CreationDate");
                    OnCreationDateChanged();
                }
            }
        }

        [Column(Name = "IsLockedOut", UpdateCheck = UpdateCheck.Never, Storage = "_IsLockedOut", DbType = "bit NOT NULL")]
        public bool IsLockedOut
        {
            get => _IsLockedOut;

            set
            {
                if (_IsLockedOut != value)
                {
                    OnIsLockedOutChanging(value);
                    SendPropertyChanging();
                    _IsLockedOut = value;
                    SendPropertyChanged("IsLockedOut");
                    OnIsLockedOutChanged();
                }
            }
        }

        [Column(Name = "LastLockedOutDate", UpdateCheck = UpdateCheck.Never, Storage = "_LastLockedOutDate", DbType = "datetime")]
        public DateTime? LastLockedOutDate
        {
            get => _LastLockedOutDate;

            set
            {
                if (_LastLockedOutDate != value)
                {
                    OnLastLockedOutDateChanging(value);
                    SendPropertyChanging();
                    _LastLockedOutDate = value;
                    SendPropertyChanged("LastLockedOutDate");
                    OnLastLockedOutDateChanged();
                }
            }
        }

        [Column(Name = "FailedPasswordAttemptCount", UpdateCheck = UpdateCheck.Never, Storage = "_FailedPasswordAttemptCount", DbType = "int NOT NULL")]
        public int FailedPasswordAttemptCount
        {
            get => _FailedPasswordAttemptCount;

            set
            {
                if (_FailedPasswordAttemptCount != value)
                {
                    OnFailedPasswordAttemptCountChanging(value);
                    SendPropertyChanging();
                    _FailedPasswordAttemptCount = value;
                    SendPropertyChanged("FailedPasswordAttemptCount");
                    OnFailedPasswordAttemptCountChanged();
                }
            }
        }

        [Column(Name = "FailedPasswordAttemptWindowStart", UpdateCheck = UpdateCheck.Never, Storage = "_FailedPasswordAttemptWindowStart", DbType = "datetime")]
        public DateTime? FailedPasswordAttemptWindowStart
        {
            get => _FailedPasswordAttemptWindowStart;

            set
            {
                if (_FailedPasswordAttemptWindowStart != value)
                {
                    OnFailedPasswordAttemptWindowStartChanging(value);
                    SendPropertyChanging();
                    _FailedPasswordAttemptWindowStart = value;
                    SendPropertyChanged("FailedPasswordAttemptWindowStart");
                    OnFailedPasswordAttemptWindowStartChanged();
                }
            }
        }

        [Column(Name = "FailedPasswordAnswerAttemptCount", UpdateCheck = UpdateCheck.Never, Storage = "_FailedPasswordAnswerAttemptCount", DbType = "int NOT NULL")]
        public int FailedPasswordAnswerAttemptCount
        {
            get => _FailedPasswordAnswerAttemptCount;

            set
            {
                if (_FailedPasswordAnswerAttemptCount != value)
                {
                    OnFailedPasswordAnswerAttemptCountChanging(value);
                    SendPropertyChanging();
                    _FailedPasswordAnswerAttemptCount = value;
                    SendPropertyChanged("FailedPasswordAnswerAttemptCount");
                    OnFailedPasswordAnswerAttemptCountChanged();
                }
            }
        }

        [Column(Name = "FailedPasswordAnswerAttemptWindowStart", UpdateCheck = UpdateCheck.Never, Storage = "_FailedPasswordAnswerAttemptWindowStart", DbType = "datetime")]
        public DateTime? FailedPasswordAnswerAttemptWindowStart
        {
            get => _FailedPasswordAnswerAttemptWindowStart;

            set
            {
                if (_FailedPasswordAnswerAttemptWindowStart != value)
                {
                    OnFailedPasswordAnswerAttemptWindowStartChanging(value);
                    SendPropertyChanging();
                    _FailedPasswordAnswerAttemptWindowStart = value;
                    SendPropertyChanged("FailedPasswordAnswerAttemptWindowStart");
                    OnFailedPasswordAnswerAttemptWindowStartChanged();
                }
            }
        }

        [Column(Name = "EmailAddress", UpdateCheck = UpdateCheck.Never, Storage = "_EmailAddress", DbType = "nvarchar(100)", IsDbGenerated = true)]
        public string EmailAddress
        {
            get => _EmailAddress;

            set
            {
                if (_EmailAddress != value)
                {
                    OnEmailAddressChanging(value);
                    SendPropertyChanging();
                    _EmailAddress = value;
                    SendPropertyChanged("EmailAddress");
                    OnEmailAddressChanged();
                }
            }
        }

        [Column(Name = "ItemsInGrid", UpdateCheck = UpdateCheck.Never, Storage = "_ItemsInGrid", DbType = "int")]
        public int? ItemsInGrid
        {
            get => _ItemsInGrid;

            set
            {
                if (_ItemsInGrid != value)
                {
                    OnItemsInGridChanging(value);
                    SendPropertyChanging();
                    _ItemsInGrid = value;
                    SendPropertyChanged("ItemsInGrid");
                    OnItemsInGridChanged();
                }
            }
        }

        [Column(Name = "CurrentCart", UpdateCheck = UpdateCheck.Never, Storage = "_CurrentCart", DbType = "nvarchar(100)")]
        public string CurrentCart
        {
            get => _CurrentCart;

            set
            {
                if (_CurrentCart != value)
                {
                    OnCurrentCartChanging(value);
                    SendPropertyChanging();
                    _CurrentCart = value;
                    SendPropertyChanged("CurrentCart");
                    OnCurrentCartChanged();
                }
            }
        }

        [Column(Name = "MustChangePassword", UpdateCheck = UpdateCheck.Never, Storage = "_MustChangePassword", DbType = "bit NOT NULL")]
        public bool MustChangePassword
        {
            get => _MustChangePassword;

            set
            {
                if (_MustChangePassword != value)
                {
                    OnMustChangePasswordChanging(value);
                    SendPropertyChanging();
                    _MustChangePassword = value;
                    SendPropertyChanged("MustChangePassword");
                    OnMustChangePasswordChanged();
                }
            }
        }

        [Column(Name = "Host", UpdateCheck = UpdateCheck.Never, Storage = "_Host", DbType = "nvarchar(100)")]
        public string Host
        {
            get => _Host;

            set
            {
                if (_Host != value)
                {
                    OnHostChanging(value);
                    SendPropertyChanging();
                    _Host = value;
                    SendPropertyChanged("Host");
                    OnHostChanged();
                }
            }
        }

        [Column(Name = "TempPassword", UpdateCheck = UpdateCheck.Never, Storage = "_TempPassword", DbType = "nvarchar(128)")]
        public string TempPassword
        {
            get => _TempPassword;

            set
            {
                if (_TempPassword != value)
                {
                    OnTempPasswordChanging(value);
                    SendPropertyChanging();
                    _TempPassword = value;
                    SendPropertyChanged("TempPassword");
                    OnTempPasswordChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(50)")]
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

        [Column(Name = "Name2", UpdateCheck = UpdateCheck.Never, Storage = "_Name2", DbType = "nvarchar(50)")]
        public string Name2
        {
            get => _Name2;

            set
            {
                if (_Name2 != value)
                {
                    OnName2Changing(value);
                    SendPropertyChanging();
                    _Name2 = value;
                    SendPropertyChanged("Name2");
                    OnName2Changed();
                }
            }
        }

        [Column(Name = "ResetPasswordCode", UpdateCheck = UpdateCheck.Never, Storage = "_ResetPasswordCode", DbType = "uniqueidentifier")]
        public Guid? ResetPasswordCode
        {
            get => _ResetPasswordCode;

            set
            {
                if (_ResetPasswordCode != value)
                {
                    OnResetPasswordCodeChanging(value);
                    SendPropertyChanging();
                    _ResetPasswordCode = value;
                    SendPropertyChanged("ResetPasswordCode");
                    OnResetPasswordCodeChanged();
                }
            }
        }

        [Column(Name = "DefaultGroup", UpdateCheck = UpdateCheck.Never, Storage = "_DefaultGroup", DbType = "nvarchar(50)")]
        public string DefaultGroup
        {
            get => _DefaultGroup;

            set
            {
                if (_DefaultGroup != value)
                {
                    OnDefaultGroupChanging(value);
                    SendPropertyChanging();
                    _DefaultGroup = value;
                    SendPropertyChanged("DefaultGroup");
                    OnDefaultGroupChanged();
                }
            }
        }

        [Column(Name = "ResetPasswordExpires", UpdateCheck = UpdateCheck.Never, Storage = "_ResetPasswordExpires", DbType = "datetime")]
        public DateTime? ResetPasswordExpires
        {
            get => _ResetPasswordExpires;

            set
            {
                if (_ResetPasswordExpires != value)
                {
                    OnResetPasswordExpiresChanging(value);
                    SendPropertyChanging();
                    _ResetPasswordExpires = value;
                    SendPropertyChanged("ResetPasswordExpires");
                    OnResetPasswordExpiresChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_Coupons_Users", Storage = "_Coupons", OtherKey = "UserId")]
        public EntitySet<Coupon> Coupons
           {
               get => _Coupons;

            set => _Coupons.Assign(value);

           }

        [Association(Name = "FK_MobileAppDevices_Users", Storage = "_MobileAppDevices", OtherKey = "UserID")]
        public EntitySet<MobileAppDevice> MobileAppDevices
           {
               get => _MobileAppDevices;

            set => _MobileAppDevices.Assign(value);

           }

        [Association(Name = "FK_SMSGroupMembers_Users", Storage = "_SMSGroupMembers", OtherKey = "UserID")]
        public EntitySet<SMSGroupMember> SMSGroupMembers
           {
               get => _SMSGroupMembers;

            set => _SMSGroupMembers.Assign(value);

           }

        [Association(Name = "FK_UserPreferences_Users", Storage = "_Preferences", OtherKey = "UserId")]
        public EntitySet<Preference> Preferences
           {
               get => _Preferences;

            set => _Preferences.Assign(value);

           }

        [Association(Name = "FK_UserRole_Users", Storage = "_UserRoles", OtherKey = "UserId")]
        public EntitySet<UserRole> UserRoles
           {
               get => _UserRoles;

            set => _UserRoles.Assign(value);

           }

        [Association(Name = "FK_Users_ApiSession", Storage = "_ApiSessions", OtherKey = "UserId")]
        public EntitySet<ApiSession> ApiSessions
           {
               get => _ApiSessions;

            set => _ApiSessions.Assign(value);

           }

        [Association(Name = "VolunteerFormsUploaded__Uploader", Storage = "_VolunteerFormsUploaded", OtherKey = "UploaderId")]
        public EntitySet<VolunteerForm> VolunteerFormsUploaded
           {
               get => _VolunteerFormsUploaded;

            set => _VolunteerFormsUploaded.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Users_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.Users.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.Users.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int?);

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

        private void attach_Coupons(Coupon entity)
        {
            SendPropertyChanging();
            entity.User = this;
        }

        private void detach_Coupons(Coupon entity)
        {
            SendPropertyChanging();
            entity.User = null;
        }

        private void attach_MobileAppDevices(MobileAppDevice entity)
        {
            SendPropertyChanging();
            entity.User = this;
        }

        private void detach_MobileAppDevices(MobileAppDevice entity)
        {
            SendPropertyChanging();
            entity.User = null;
        }

        private void attach_SMSGroupMembers(SMSGroupMember entity)
        {
            SendPropertyChanging();
            entity.User = this;
        }

        private void detach_SMSGroupMembers(SMSGroupMember entity)
        {
            SendPropertyChanging();
            entity.User = null;
        }

        private void attach_Preferences(Preference entity)
        {
            SendPropertyChanging();
            entity.User = this;
        }

        private void detach_Preferences(Preference entity)
        {
            SendPropertyChanging();
            entity.User = null;
        }

        private void attach_UserRoles(UserRole entity)
        {
            SendPropertyChanging();
            entity.User = this;
        }

        private void detach_UserRoles(UserRole entity)
        {
            SendPropertyChanging();
            entity.User = null;
        }

        private void attach_ApiSessions(ApiSession entity)
        {
            SendPropertyChanging();
            entity.User = this;
        }

        private void detach_ApiSessions(ApiSession entity)
        {
            SendPropertyChanging();
            entity.User = null;
        }

        private void attach_VolunteerFormsUploaded(VolunteerForm entity)
        {
            SendPropertyChanging();
            entity.Uploader = this;
        }

        private void detach_VolunteerFormsUploaded(VolunteerForm entity)
        {
            SendPropertyChanging();
            entity.Uploader = null;
        }
    }
}
