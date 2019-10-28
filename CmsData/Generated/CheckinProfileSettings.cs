using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CheckinProfileSettings")]
    public partial class CheckinProfileSettings
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _CheckinProfileId;
        private int? _CampusId;
        private int? _EarlyCheckin;
        private int? _LateCheckin;
        private bool _Testing;
        private int? _TestDay;
        private string _AdminPIN;
        private int? _PINTimeout;
        private bool _DisableJoin;
        private bool _DisableTimer;
        private int? _BackgroundImage;
        private string _BackgroundImageName;
        private string _BackgroundImageURL;
        private int _CutoffAge;
        private string _Logout;
        private bool _Guest;
        private bool _Location;
        private int _SecurityType;
        private int _ShowCheckinConfirmation;

        private EntityRef<CheckinProfiles> _CheckinProfiles;
        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnCheckinProfileIdChanging(int value);
        partial void OnCheckinProfileIdChanged();

        partial void OnCampusIdChanging(int? value);
        partial void OnCampusIdChanged();

        partial void OnEarlyCheckinChanging(int? value);
        partial void OnEarlyCheckinChanged();

        partial void OnLateCheckinChanging(int? value);
        partial void OnLateCheckinChanged();

        partial void OnTestingChanging(bool value);
        partial void OnTestingChanged();

        partial void OnTestDayChanging(int? value);
        partial void OnTestDayChanged();

        partial void OnAdminPINChanging(string value);
        partial void OnAdminPINChanged();

        partial void OnPINTimeoutChanging(int? value);
        partial void OnPINTimeoutChanged();

        partial void OnDisableJoinChanging(bool value);
        partial void OnDisableJoinChanged();

        partial void OnDisableTimerChanging(bool value);
        partial void OnDisableTimerChanged();

        partial void OnBackgroundImageChanging(int? value);
        partial void OnBackgroundImageChanged();

        partial void OnBackgroundImageNameChanging(string value);
        partial void OnBackgroundImageNameChanged();

        partial void OnBackgroundImageURLChanging(string value);
        partial void OnBackgroundImageURLChanged();

        partial void OnCutoffAgeChanging(int value);
        partial void OnCutoffAgeChanged();

        partial void OnLogoutChanging(string value);
        partial void OnLogoutChanged();

        partial void OnGuestChanging(bool value);
        partial void OnGuestChanged();

        partial void OnLocationChanging(bool value);
        partial void OnLocationChanged();

        partial void OnSecurityTypeChanging(int value);
        partial void OnSecurityTypeChanged();

        partial void OnShowCheckinConfirmationChanging(int value);
        partial void OnShowCheckinConfirmationChanged();
        #endregion

        public CheckinProfileSettings()
        {
            _CheckinProfiles = default(EntityRef<CheckinProfiles>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "CheckinProfileId", UpdateCheck = UpdateCheck.Never, Storage = "_CheckinProfileId", DbType = "int NOT NULL UNIQUE", IsPrimaryKey = true)]
        [IsForeignKey]
        public int CheckinProfileId
        {
            get => _CheckinProfileId;

            set
            {
                if (_CheckinProfileId != value)
                {
                    if (_CheckinProfiles.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnCheckinProfileIdChanging(value);
                    SendPropertyChanging();
                    _CheckinProfileId = value;
                    SendPropertyChanged("CheckinProfileId");
                    OnCheckinProfileIdChanged();
                }
            }
        }

        [Column(Name = "CampusId", UpdateCheck = UpdateCheck.Never, Storage = "_CampusId", DbType = "int NULL")]
        public int? CampusId
        {
            get => _CampusId;

            set
            {
                if (_CampusId != value)
                {
                    OnCampusIdChanging(value);
                    SendPropertyChanging();
                    _CampusId = value;
                    SendPropertyChanged("CampusId");
                    OnCampusIdChanged();
                }
            }
        }

        [Column(Name = "EarlyCheckin", UpdateCheck = UpdateCheck.Never, Storage = "_EarlyCheckin", DbType = "int NULL")]
        public int? EarlyCheckin
        {
            get => _EarlyCheckin;

            set
            {
                if (_EarlyCheckin != value)
                {
                    OnEarlyCheckinChanging(value);
                    SendPropertyChanging();
                    _EarlyCheckin = value;
                    SendPropertyChanged("EarlyCheckin");
                    OnEarlyCheckinChanged();
                }
            }
        }

        [Column(Name = "LateCheckin", UpdateCheck = UpdateCheck.Never, Storage = "_LateCheckin", DbType = "int NULL")]
        public int? LateCheckin
        {
            get => _LateCheckin;

            set
            {
                if (_LateCheckin != value)
                {
                    OnLateCheckinChanging(value);
                    SendPropertyChanging();
                    _LateCheckin = value;
                    SendPropertyChanged("LateCheckin");
                    OnLateCheckinChanged();
                }
            }
        }

        [Column(Name = "Testing", UpdateCheck = UpdateCheck.Never, Storage = "_Testing", DbType = "bit NOT NULL")]
        public bool Testing
        {
            get => _Testing;

            set
            {
                if (_Testing != value)
                {
                    OnTestingChanging(value);
                    SendPropertyChanging();
                    _Testing = value;
                    SendPropertyChanged("Testing");
                    OnTestingChanged();
                }
            }
        }

        [Column(Name = "TestDay", UpdateCheck = UpdateCheck.Never, Storage = "_TestDay", DbType = "int NULL")]
        public int? TestDay
        {
            get => _TestDay;

            set
            {
                if (_TestDay != value)
                {
                    OnTestDayChanging(value);
                    SendPropertyChanging();
                    _TestDay = value;
                    SendPropertyChanged("TestDay");
                    OnTestDayChanged();
                }
            }
        }

        [Column(Name = "AdminPIN", UpdateCheck = UpdateCheck.Never, Storage = "_AdminPIN", DbType = "nvarchar(max) NULL")]
        public string AdminPIN
        {
            get => _AdminPIN;

            set
            {
                if (_AdminPIN != value)
                {
                    OnAdminPINChanging(value);
                    SendPropertyChanging();
                    _AdminPIN = value;
                    SendPropertyChanged("AdminPIN");
                    OnAdminPINChanged();
                }
            }
        }

        [Column(Name = "PINTimeout", UpdateCheck = UpdateCheck.Never, Storage = "_PINTimeout", DbType = "int NULL")]
        public int? PINTimeout
        {
            get => _PINTimeout;

            set
            {
                if (_PINTimeout != value)
                {
                    OnPINTimeoutChanging(value);
                    SendPropertyChanging();
                    _PINTimeout = value;
                    SendPropertyChanged("PINTimeout");
                    OnPINTimeoutChanged();
                }
            }
        }

        [Column(Name = "DisableJoin", UpdateCheck = UpdateCheck.Never, Storage = "_DisableJoin", DbType = "bit NOT NULL")]
        public bool DisableJoin
        {
            get => _DisableJoin;

            set
            {
                if (_DisableJoin != value)
                {
                    OnDisableJoinChanging(value);
                    SendPropertyChanging();
                    _DisableJoin = value;
                    SendPropertyChanged("DisableJoin");
                    OnDisableJoinChanged();
                }
            }
        }

        [Column(Name = "DisableTimer", UpdateCheck = UpdateCheck.Never, Storage = "_DisableTimer", DbType = "bit NOT NULL")]
        public bool DisableTimer
        {
            get => _DisableTimer;

            set
            {
                if (_DisableTimer != value)
                {
                    OnDisableTimerChanging(value);
                    SendPropertyChanging();
                    _DisableTimer = value;
                    SendPropertyChanged("DisableTimer");
                    OnDisableTimerChanged();
                }
            }
        }

        [Column(Name = "BackgroundImage", UpdateCheck = UpdateCheck.Never, Storage = "_BackgroundImage", DbType = "int NULL")]
        public int? BackgroundImage
        {
            get => _BackgroundImage;

            set
            {
                if (_BackgroundImage != value)
                {
                    OnBackgroundImageChanging(value);
                    SendPropertyChanging();
                    _BackgroundImage = value;
                    SendPropertyChanged("BackgroundImage");
                    OnBackgroundImageChanged();
                }
            }
        }

        [Column(Name = "BackgroundImageName", UpdateCheck = UpdateCheck.Never, Storage = "_BackgroundImageName", DbType = "nvarchar(max) NULL")]
        public string BackgroundImageName
        {
            get => _BackgroundImageName;

            set
            {
                if (_BackgroundImageName != value)
                {
                    OnBackgroundImageNameChanging(value);
                    SendPropertyChanging();
                    _BackgroundImageName = value;
                    SendPropertyChanged("BackgroundImageName");
                    OnBackgroundImageNameChanged();
                }
            }
        }

        [Column(Name = "BackgroundImageURL", UpdateCheck = UpdateCheck.Never, Storage = "_BackgroundImageURL", DbType = "nvarchar(max) NULL")]
        public string BackgroundImageURL
        {
            get => _BackgroundImageURL;

            set
            {
                if (_BackgroundImageURL != value)
                {
                    OnBackgroundImageURLChanging(value);
                    SendPropertyChanging();
                    _BackgroundImageURL = value;
                    SendPropertyChanged("BackgroundImageURL");
                    OnBackgroundImageURLChanged();
                }
            }
        }

        [Column(Name = "CutoffAge", UpdateCheck = UpdateCheck.Never, Storage = "_CutoffAge", DbType = "int NOT NULL")]
        public int CutoffAge
        {
            get => _CutoffAge;

            set
            {
                if (_CutoffAge != value)
                {
                    OnCutoffAgeChanging(value);
                    SendPropertyChanging();
                    _CutoffAge = value;
                    SendPropertyChanged("CutoffAge");
                    OnCutoffAgeChanged();
                }
            }
        }

        [Column(Name = "Logout", UpdateCheck = UpdateCheck.Never, Storage = "_Logout", DbType = "nvarchar(max) NULL")]
        public string Logout
        {
            get => _Logout;

            set
            {
                if (_Logout != value)
                {
                    OnLogoutChanging(value);
                    SendPropertyChanging();
                    _Logout = value;
                    SendPropertyChanged("Logout");
                    OnLogoutChanged();
                }
            }
        }

        [Column(Name = "Guest", UpdateCheck = UpdateCheck.Never, Storage = "_Guest", DbType = "bit NOT NULL")]
        public bool Guest
        {
            get => _Guest;

            set
            {
                if (_Guest != value)
                {
                    OnGuestChanging(value);
                    SendPropertyChanging();
                    _Guest = value;
                    SendPropertyChanged("Guest");
                    OnGuestChanged();
                }
            }
        }

        [Column(Name = "Location", UpdateCheck = UpdateCheck.Never, Storage = "_Location", DbType = "bit NOT NULL")]
        public bool Location
        {
            get => _Location;

            set
            {
                if (_Location != value)
                {
                    OnLocationChanging(value);
                    SendPropertyChanging();
                    _Location = value;
                    SendPropertyChanged("Location");
                    OnLocationChanged();
                }
            }
        }

        [Column(Name = "SecurityType", UpdateCheck = UpdateCheck.Never, Storage = "_SecurityType", DbType = "int NOT NULL")]
        public int SecurityType
        {
            get => _SecurityType;

            set
            {
                if (_SecurityType != value)
                {
                    OnSecurityTypeChanging(value);
                    SendPropertyChanging();
                    _SecurityType = value;
                    SendPropertyChanged("SecurityType");
                    OnSecurityTypeChanged();
                }
            }
        }

        [Column(Name = "ShowCheckinConfirmation", UpdateCheck = UpdateCheck.Never, Storage = "_ShowCheckinConfirmation", DbType = "int NOT NULL")]
        public int ShowCheckinConfirmation
        {
            get => _ShowCheckinConfirmation;

            set
            {
                if (_ShowCheckinConfirmation != value)
                {
                    OnShowCheckinConfirmationChanging(value);
                    SendPropertyChanging();
                    _ShowCheckinConfirmation = value;
                    SendPropertyChanged("ShowCheckinConfirmation");
                    OnShowCheckinConfirmationChanged();
                }
            }
        }

        #endregion

        #region Foreign Keys

        [Association(Name = "Checking_Profile_Settings_CP_FK", Storage = "_CheckinProfiles", ThisKey = "CheckinProfileId", IsForeignKey = true)]
        public CheckinProfiles CheckinProfiles
        {
            get => _CheckinProfiles.Entity;

            set
            {
                CheckinProfiles previousValue = _CheckinProfiles.Entity;
                if (((previousValue != value)
                            || (_CheckinProfiles.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _CheckinProfiles.Entity = null;
                        previousValue.CheckinProfileSettings.Remove(this);
                    }

                    _CheckinProfiles.Entity = value;
                    if (value != null)
                    {
                        value.CheckinProfileSettings.Add(this);

                        _CheckinProfileId = value.CheckinProfileId;
                    }

                    else
                    {
                        _CheckinProfileId = default(int);
                    }

                    SendPropertyChanged("CheckinProfiles");
                }
            }
        }

        #endregion

        #region Foreign Key Tables

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
