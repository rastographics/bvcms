using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.dUsers")]
    public partial class DUser : INotifyPropertyChanging, INotifyPropertyChanged
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

        private bool _MustChangePassword;

        private string _Host;

        private string _TempPassword;

        private bool? _NotifyAll;

        private string _FirstName;

        private string _LastName;

        private DateTime? _BirthDay;

        private string _DefaultGroup;

        private bool? _NotifyEnabled;

        private bool? _ForceLogin;

        private int? _CUserId;

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

        partial void OnMustChangePasswordChanging(bool value);
        partial void OnMustChangePasswordChanged();

        partial void OnHostChanging(string value);
        partial void OnHostChanged();

        partial void OnTempPasswordChanging(string value);
        partial void OnTempPasswordChanged();

        partial void OnNotifyAllChanging(bool? value);
        partial void OnNotifyAllChanged();

        partial void OnFirstNameChanging(string value);
        partial void OnFirstNameChanged();

        partial void OnLastNameChanging(string value);
        partial void OnLastNameChanged();

        partial void OnBirthDayChanging(DateTime? value);
        partial void OnBirthDayChanged();

        partial void OnDefaultGroupChanging(string value);
        partial void OnDefaultGroupChanged();

        partial void OnNotifyEnabledChanging(bool? value);
        partial void OnNotifyEnabledChanged();

        partial void OnForceLoginChanging(bool? value);
        partial void OnForceLoginChanged();

        partial void OnCUserIdChanging(int? value);
        partial void OnCUserIdChanged();

        #endregion

        public DUser()
        {
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
        public int? PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "Username", UpdateCheck = UpdateCheck.Never, Storage = "_Username", DbType = "varchar(50) NOT NULL")]
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

        [Column(Name = "Comment", UpdateCheck = UpdateCheck.Never, Storage = "_Comment", DbType = "varchar(255)")]
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

        [Column(Name = "Password", UpdateCheck = UpdateCheck.Never, Storage = "_Password", DbType = "varchar(128) NOT NULL")]
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

        [Column(Name = "PasswordQuestion", UpdateCheck = UpdateCheck.Never, Storage = "_PasswordQuestion", DbType = "varchar(255)")]
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

        [Column(Name = "PasswordAnswer", UpdateCheck = UpdateCheck.Never, Storage = "_PasswordAnswer", DbType = "varchar(255)")]
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

        [Column(Name = "EmailAddress", UpdateCheck = UpdateCheck.Never, Storage = "_EmailAddress", DbType = "varchar(50)")]
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

        [Column(Name = "CurrentCart", UpdateCheck = UpdateCheck.Never, Storage = "_CurrentCart", DbType = "varchar(100)")]
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

        [Column(Name = "Host", UpdateCheck = UpdateCheck.Never, Storage = "_Host", DbType = "varchar(100)")]
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

        [Column(Name = "TempPassword", UpdateCheck = UpdateCheck.Never, Storage = "_TempPassword", DbType = "varchar(128)")]
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

        [Column(Name = "NotifyAll", UpdateCheck = UpdateCheck.Never, Storage = "_NotifyAll", DbType = "bit")]
        public bool? NotifyAll
        {
            get => _NotifyAll;

            set
            {
                if (_NotifyAll != value)
                {
                    OnNotifyAllChanging(value);
                    SendPropertyChanging();
                    _NotifyAll = value;
                    SendPropertyChanged("NotifyAll");
                    OnNotifyAllChanged();
                }
            }
        }

        [Column(Name = "FirstName", UpdateCheck = UpdateCheck.Never, Storage = "_FirstName", DbType = "varchar(50)")]
        public string FirstName
        {
            get => _FirstName;

            set
            {
                if (_FirstName != value)
                {
                    OnFirstNameChanging(value);
                    SendPropertyChanging();
                    _FirstName = value;
                    SendPropertyChanged("FirstName");
                    OnFirstNameChanged();
                }
            }
        }

        [Column(Name = "LastName", UpdateCheck = UpdateCheck.Never, Storage = "_LastName", DbType = "varchar(50)")]
        public string LastName
        {
            get => _LastName;

            set
            {
                if (_LastName != value)
                {
                    OnLastNameChanging(value);
                    SendPropertyChanging();
                    _LastName = value;
                    SendPropertyChanged("LastName");
                    OnLastNameChanged();
                }
            }
        }

        [Column(Name = "BirthDay", UpdateCheck = UpdateCheck.Never, Storage = "_BirthDay", DbType = "datetime")]
        public DateTime? BirthDay
        {
            get => _BirthDay;

            set
            {
                if (_BirthDay != value)
                {
                    OnBirthDayChanging(value);
                    SendPropertyChanging();
                    _BirthDay = value;
                    SendPropertyChanged("BirthDay");
                    OnBirthDayChanged();
                }
            }
        }

        [Column(Name = "DefaultGroup", UpdateCheck = UpdateCheck.Never, Storage = "_DefaultGroup", DbType = "varchar(50)")]
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

        [Column(Name = "NotifyEnabled", UpdateCheck = UpdateCheck.Never, Storage = "_NotifyEnabled", DbType = "bit")]
        public bool? NotifyEnabled
        {
            get => _NotifyEnabled;

            set
            {
                if (_NotifyEnabled != value)
                {
                    OnNotifyEnabledChanging(value);
                    SendPropertyChanging();
                    _NotifyEnabled = value;
                    SendPropertyChanged("NotifyEnabled");
                    OnNotifyEnabledChanged();
                }
            }
        }

        [Column(Name = "ForceLogin", UpdateCheck = UpdateCheck.Never, Storage = "_ForceLogin", DbType = "bit")]
        public bool? ForceLogin
        {
            get => _ForceLogin;

            set
            {
                if (_ForceLogin != value)
                {
                    OnForceLoginChanging(value);
                    SendPropertyChanging();
                    _ForceLogin = value;
                    SendPropertyChanged("ForceLogin");
                    OnForceLoginChanged();
                }
            }
        }

        [Column(Name = "cUserId", UpdateCheck = UpdateCheck.Never, Storage = "_CUserId", DbType = "int")]
        public int? CUserId
        {
            get => _CUserId;

            set
            {
                if (_CUserId != value)
                {
                    OnCUserIdChanging(value);
                    SendPropertyChanging();
                    _CUserId = value;
                    SendPropertyChanged("CUserId");
                    OnCUserIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

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
