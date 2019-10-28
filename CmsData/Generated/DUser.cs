using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.dUsers")]
    public partial class DUser : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._UserId;

            set
            {
                if (this._UserId != value)
                {

                    this.OnUserIdChanging(value);
                    this.SendPropertyChanging();
                    this._UserId = value;
                    this.SendPropertyChanged("UserId");
                    this.OnUserIdChanged();
                }

            }

        }


        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
        {
            get => this._PeopleId;

            set
            {
                if (this._PeopleId != value)
                {

                    this.OnPeopleIdChanging(value);
                    this.SendPropertyChanging();
                    this._PeopleId = value;
                    this.SendPropertyChanged("PeopleId");
                    this.OnPeopleIdChanged();
                }

            }

        }


        [Column(Name = "Username", UpdateCheck = UpdateCheck.Never, Storage = "_Username", DbType = "varchar(50) NOT NULL")]
        public string Username
        {
            get => this._Username;

            set
            {
                if (this._Username != value)
                {

                    this.OnUsernameChanging(value);
                    this.SendPropertyChanging();
                    this._Username = value;
                    this.SendPropertyChanged("Username");
                    this.OnUsernameChanged();
                }

            }

        }


        [Column(Name = "Comment", UpdateCheck = UpdateCheck.Never, Storage = "_Comment", DbType = "varchar(255)")]
        public string Comment
        {
            get => this._Comment;

            set
            {
                if (this._Comment != value)
                {

                    this.OnCommentChanging(value);
                    this.SendPropertyChanging();
                    this._Comment = value;
                    this.SendPropertyChanged("Comment");
                    this.OnCommentChanged();
                }

            }

        }


        [Column(Name = "Password", UpdateCheck = UpdateCheck.Never, Storage = "_Password", DbType = "varchar(128) NOT NULL")]
        public string Password
        {
            get => this._Password;

            set
            {
                if (this._Password != value)
                {

                    this.OnPasswordChanging(value);
                    this.SendPropertyChanging();
                    this._Password = value;
                    this.SendPropertyChanged("Password");
                    this.OnPasswordChanged();
                }

            }

        }


        [Column(Name = "PasswordQuestion", UpdateCheck = UpdateCheck.Never, Storage = "_PasswordQuestion", DbType = "varchar(255)")]
        public string PasswordQuestion
        {
            get => this._PasswordQuestion;

            set
            {
                if (this._PasswordQuestion != value)
                {

                    this.OnPasswordQuestionChanging(value);
                    this.SendPropertyChanging();
                    this._PasswordQuestion = value;
                    this.SendPropertyChanged("PasswordQuestion");
                    this.OnPasswordQuestionChanged();
                }

            }

        }


        [Column(Name = "PasswordAnswer", UpdateCheck = UpdateCheck.Never, Storage = "_PasswordAnswer", DbType = "varchar(255)")]
        public string PasswordAnswer
        {
            get => this._PasswordAnswer;

            set
            {
                if (this._PasswordAnswer != value)
                {

                    this.OnPasswordAnswerChanging(value);
                    this.SendPropertyChanging();
                    this._PasswordAnswer = value;
                    this.SendPropertyChanged("PasswordAnswer");
                    this.OnPasswordAnswerChanged();
                }

            }

        }


        [Column(Name = "IsApproved", UpdateCheck = UpdateCheck.Never, Storage = "_IsApproved", DbType = "bit NOT NULL")]
        public bool IsApproved
        {
            get => this._IsApproved;

            set
            {
                if (this._IsApproved != value)
                {

                    this.OnIsApprovedChanging(value);
                    this.SendPropertyChanging();
                    this._IsApproved = value;
                    this.SendPropertyChanged("IsApproved");
                    this.OnIsApprovedChanged();
                }

            }

        }


        [Column(Name = "LastActivityDate", UpdateCheck = UpdateCheck.Never, Storage = "_LastActivityDate", DbType = "datetime")]
        public DateTime? LastActivityDate
        {
            get => this._LastActivityDate;

            set
            {
                if (this._LastActivityDate != value)
                {

                    this.OnLastActivityDateChanging(value);
                    this.SendPropertyChanging();
                    this._LastActivityDate = value;
                    this.SendPropertyChanged("LastActivityDate");
                    this.OnLastActivityDateChanged();
                }

            }

        }


        [Column(Name = "LastLoginDate", UpdateCheck = UpdateCheck.Never, Storage = "_LastLoginDate", DbType = "datetime")]
        public DateTime? LastLoginDate
        {
            get => this._LastLoginDate;

            set
            {
                if (this._LastLoginDate != value)
                {

                    this.OnLastLoginDateChanging(value);
                    this.SendPropertyChanging();
                    this._LastLoginDate = value;
                    this.SendPropertyChanged("LastLoginDate");
                    this.OnLastLoginDateChanged();
                }

            }

        }


        [Column(Name = "LastPasswordChangedDate", UpdateCheck = UpdateCheck.Never, Storage = "_LastPasswordChangedDate", DbType = "datetime")]
        public DateTime? LastPasswordChangedDate
        {
            get => this._LastPasswordChangedDate;

            set
            {
                if (this._LastPasswordChangedDate != value)
                {

                    this.OnLastPasswordChangedDateChanging(value);
                    this.SendPropertyChanging();
                    this._LastPasswordChangedDate = value;
                    this.SendPropertyChanged("LastPasswordChangedDate");
                    this.OnLastPasswordChangedDateChanged();
                }

            }

        }


        [Column(Name = "CreationDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreationDate", DbType = "datetime")]
        public DateTime? CreationDate
        {
            get => this._CreationDate;

            set
            {
                if (this._CreationDate != value)
                {

                    this.OnCreationDateChanging(value);
                    this.SendPropertyChanging();
                    this._CreationDate = value;
                    this.SendPropertyChanged("CreationDate");
                    this.OnCreationDateChanged();
                }

            }

        }


        [Column(Name = "IsLockedOut", UpdateCheck = UpdateCheck.Never, Storage = "_IsLockedOut", DbType = "bit NOT NULL")]
        public bool IsLockedOut
        {
            get => this._IsLockedOut;

            set
            {
                if (this._IsLockedOut != value)
                {

                    this.OnIsLockedOutChanging(value);
                    this.SendPropertyChanging();
                    this._IsLockedOut = value;
                    this.SendPropertyChanged("IsLockedOut");
                    this.OnIsLockedOutChanged();
                }

            }

        }


        [Column(Name = "LastLockedOutDate", UpdateCheck = UpdateCheck.Never, Storage = "_LastLockedOutDate", DbType = "datetime")]
        public DateTime? LastLockedOutDate
        {
            get => this._LastLockedOutDate;

            set
            {
                if (this._LastLockedOutDate != value)
                {

                    this.OnLastLockedOutDateChanging(value);
                    this.SendPropertyChanging();
                    this._LastLockedOutDate = value;
                    this.SendPropertyChanged("LastLockedOutDate");
                    this.OnLastLockedOutDateChanged();
                }

            }

        }


        [Column(Name = "FailedPasswordAttemptCount", UpdateCheck = UpdateCheck.Never, Storage = "_FailedPasswordAttemptCount", DbType = "int NOT NULL")]
        public int FailedPasswordAttemptCount
        {
            get => this._FailedPasswordAttemptCount;

            set
            {
                if (this._FailedPasswordAttemptCount != value)
                {

                    this.OnFailedPasswordAttemptCountChanging(value);
                    this.SendPropertyChanging();
                    this._FailedPasswordAttemptCount = value;
                    this.SendPropertyChanged("FailedPasswordAttemptCount");
                    this.OnFailedPasswordAttemptCountChanged();
                }

            }

        }


        [Column(Name = "FailedPasswordAttemptWindowStart", UpdateCheck = UpdateCheck.Never, Storage = "_FailedPasswordAttemptWindowStart", DbType = "datetime")]
        public DateTime? FailedPasswordAttemptWindowStart
        {
            get => this._FailedPasswordAttemptWindowStart;

            set
            {
                if (this._FailedPasswordAttemptWindowStart != value)
                {

                    this.OnFailedPasswordAttemptWindowStartChanging(value);
                    this.SendPropertyChanging();
                    this._FailedPasswordAttemptWindowStart = value;
                    this.SendPropertyChanged("FailedPasswordAttemptWindowStart");
                    this.OnFailedPasswordAttemptWindowStartChanged();
                }

            }

        }


        [Column(Name = "FailedPasswordAnswerAttemptCount", UpdateCheck = UpdateCheck.Never, Storage = "_FailedPasswordAnswerAttemptCount", DbType = "int NOT NULL")]
        public int FailedPasswordAnswerAttemptCount
        {
            get => this._FailedPasswordAnswerAttemptCount;

            set
            {
                if (this._FailedPasswordAnswerAttemptCount != value)
                {

                    this.OnFailedPasswordAnswerAttemptCountChanging(value);
                    this.SendPropertyChanging();
                    this._FailedPasswordAnswerAttemptCount = value;
                    this.SendPropertyChanged("FailedPasswordAnswerAttemptCount");
                    this.OnFailedPasswordAnswerAttemptCountChanged();
                }

            }

        }


        [Column(Name = "FailedPasswordAnswerAttemptWindowStart", UpdateCheck = UpdateCheck.Never, Storage = "_FailedPasswordAnswerAttemptWindowStart", DbType = "datetime")]
        public DateTime? FailedPasswordAnswerAttemptWindowStart
        {
            get => this._FailedPasswordAnswerAttemptWindowStart;

            set
            {
                if (this._FailedPasswordAnswerAttemptWindowStart != value)
                {

                    this.OnFailedPasswordAnswerAttemptWindowStartChanging(value);
                    this.SendPropertyChanging();
                    this._FailedPasswordAnswerAttemptWindowStart = value;
                    this.SendPropertyChanged("FailedPasswordAnswerAttemptWindowStart");
                    this.OnFailedPasswordAnswerAttemptWindowStartChanged();
                }

            }

        }


        [Column(Name = "EmailAddress", UpdateCheck = UpdateCheck.Never, Storage = "_EmailAddress", DbType = "varchar(50)")]
        public string EmailAddress
        {
            get => this._EmailAddress;

            set
            {
                if (this._EmailAddress != value)
                {

                    this.OnEmailAddressChanging(value);
                    this.SendPropertyChanging();
                    this._EmailAddress = value;
                    this.SendPropertyChanged("EmailAddress");
                    this.OnEmailAddressChanged();
                }

            }

        }


        [Column(Name = "ItemsInGrid", UpdateCheck = UpdateCheck.Never, Storage = "_ItemsInGrid", DbType = "int")]
        public int? ItemsInGrid
        {
            get => this._ItemsInGrid;

            set
            {
                if (this._ItemsInGrid != value)
                {

                    this.OnItemsInGridChanging(value);
                    this.SendPropertyChanging();
                    this._ItemsInGrid = value;
                    this.SendPropertyChanged("ItemsInGrid");
                    this.OnItemsInGridChanged();
                }

            }

        }


        [Column(Name = "CurrentCart", UpdateCheck = UpdateCheck.Never, Storage = "_CurrentCart", DbType = "varchar(100)")]
        public string CurrentCart
        {
            get => this._CurrentCart;

            set
            {
                if (this._CurrentCart != value)
                {

                    this.OnCurrentCartChanging(value);
                    this.SendPropertyChanging();
                    this._CurrentCart = value;
                    this.SendPropertyChanged("CurrentCart");
                    this.OnCurrentCartChanged();
                }

            }

        }


        [Column(Name = "MustChangePassword", UpdateCheck = UpdateCheck.Never, Storage = "_MustChangePassword", DbType = "bit NOT NULL")]
        public bool MustChangePassword
        {
            get => this._MustChangePassword;

            set
            {
                if (this._MustChangePassword != value)
                {

                    this.OnMustChangePasswordChanging(value);
                    this.SendPropertyChanging();
                    this._MustChangePassword = value;
                    this.SendPropertyChanged("MustChangePassword");
                    this.OnMustChangePasswordChanged();
                }

            }

        }


        [Column(Name = "Host", UpdateCheck = UpdateCheck.Never, Storage = "_Host", DbType = "varchar(100)")]
        public string Host
        {
            get => this._Host;

            set
            {
                if (this._Host != value)
                {

                    this.OnHostChanging(value);
                    this.SendPropertyChanging();
                    this._Host = value;
                    this.SendPropertyChanged("Host");
                    this.OnHostChanged();
                }

            }

        }


        [Column(Name = "TempPassword", UpdateCheck = UpdateCheck.Never, Storage = "_TempPassword", DbType = "varchar(128)")]
        public string TempPassword
        {
            get => this._TempPassword;

            set
            {
                if (this._TempPassword != value)
                {

                    this.OnTempPasswordChanging(value);
                    this.SendPropertyChanging();
                    this._TempPassword = value;
                    this.SendPropertyChanged("TempPassword");
                    this.OnTempPasswordChanged();
                }

            }

        }


        [Column(Name = "NotifyAll", UpdateCheck = UpdateCheck.Never, Storage = "_NotifyAll", DbType = "bit")]
        public bool? NotifyAll
        {
            get => this._NotifyAll;

            set
            {
                if (this._NotifyAll != value)
                {

                    this.OnNotifyAllChanging(value);
                    this.SendPropertyChanging();
                    this._NotifyAll = value;
                    this.SendPropertyChanged("NotifyAll");
                    this.OnNotifyAllChanged();
                }

            }

        }


        [Column(Name = "FirstName", UpdateCheck = UpdateCheck.Never, Storage = "_FirstName", DbType = "varchar(50)")]
        public string FirstName
        {
            get => this._FirstName;

            set
            {
                if (this._FirstName != value)
                {

                    this.OnFirstNameChanging(value);
                    this.SendPropertyChanging();
                    this._FirstName = value;
                    this.SendPropertyChanged("FirstName");
                    this.OnFirstNameChanged();
                }

            }

        }


        [Column(Name = "LastName", UpdateCheck = UpdateCheck.Never, Storage = "_LastName", DbType = "varchar(50)")]
        public string LastName
        {
            get => this._LastName;

            set
            {
                if (this._LastName != value)
                {

                    this.OnLastNameChanging(value);
                    this.SendPropertyChanging();
                    this._LastName = value;
                    this.SendPropertyChanged("LastName");
                    this.OnLastNameChanged();
                }

            }

        }


        [Column(Name = "BirthDay", UpdateCheck = UpdateCheck.Never, Storage = "_BirthDay", DbType = "datetime")]
        public DateTime? BirthDay
        {
            get => this._BirthDay;

            set
            {
                if (this._BirthDay != value)
                {

                    this.OnBirthDayChanging(value);
                    this.SendPropertyChanging();
                    this._BirthDay = value;
                    this.SendPropertyChanged("BirthDay");
                    this.OnBirthDayChanged();
                }

            }

        }


        [Column(Name = "DefaultGroup", UpdateCheck = UpdateCheck.Never, Storage = "_DefaultGroup", DbType = "varchar(50)")]
        public string DefaultGroup
        {
            get => this._DefaultGroup;

            set
            {
                if (this._DefaultGroup != value)
                {

                    this.OnDefaultGroupChanging(value);
                    this.SendPropertyChanging();
                    this._DefaultGroup = value;
                    this.SendPropertyChanged("DefaultGroup");
                    this.OnDefaultGroupChanged();
                }

            }

        }


        [Column(Name = "NotifyEnabled", UpdateCheck = UpdateCheck.Never, Storage = "_NotifyEnabled", DbType = "bit")]
        public bool? NotifyEnabled
        {
            get => this._NotifyEnabled;

            set
            {
                if (this._NotifyEnabled != value)
                {

                    this.OnNotifyEnabledChanging(value);
                    this.SendPropertyChanging();
                    this._NotifyEnabled = value;
                    this.SendPropertyChanged("NotifyEnabled");
                    this.OnNotifyEnabledChanged();
                }

            }

        }


        [Column(Name = "ForceLogin", UpdateCheck = UpdateCheck.Never, Storage = "_ForceLogin", DbType = "bit")]
        public bool? ForceLogin
        {
            get => this._ForceLogin;

            set
            {
                if (this._ForceLogin != value)
                {

                    this.OnForceLoginChanging(value);
                    this.SendPropertyChanging();
                    this._ForceLogin = value;
                    this.SendPropertyChanged("ForceLogin");
                    this.OnForceLoginChanged();
                }

            }

        }


        [Column(Name = "cUserId", UpdateCheck = UpdateCheck.Never, Storage = "_CUserId", DbType = "int")]
        public int? CUserId
        {
            get => this._CUserId;

            set
            {
                if (this._CUserId != value)
                {

                    this.OnCUserIdChanging(value);
                    this.SendPropertyChanging();
                    this._CUserId = value;
                    this.SendPropertyChanged("CUserId");
                    this.OnCUserIdChanged();
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
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }

}

