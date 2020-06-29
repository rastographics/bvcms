using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using CmsData.Infrastructure;

namespace CmsData
{
    [Table(Name = "dbo.GivingPages")]
    public partial class GivingPage : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields
        private int _GivingPageId;

        private string _PageName;

        private string _PageUrl;

        private int _PageType;

        private int? _FundId;

        private bool _Enabled;

        private bool? _DefaultPage;

        private bool? _MainCampusPageFlag;

        private string _DisabledRedirect;

        private int? _SkinFileId;

        private string _TopText;

        private string _ThankYouText;

        private string _OnlineNotifyPerson;

        private int? _ConfirmationEmailPledgeId;

        private int? _ConfirmationEmailOneTimeId;

        private int? _ConfirmationEmailRecurringId;

        private int? _CampusId;

        private int? _EntryPointId;

        private EntitySet<GivingPageFund> _GivingPageFunds;

        private EntityRef<Campu> _Campu;

        private EntityRef<ContributionFund> _ContributionFund;

        private EntityRef<EntryPoint> _EntryPoint;

        private EntityRef<Content> _SkinFile;

        private EntityRef<Content> _ConfirmationEmailPledge;

        private EntityRef<Content> _ConfirmationEmailOneTime;

        private EntityRef<Content> _ConfirmationEmailRecurring;

        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnGivingPageIdChanging(int value);
        partial void OnGivingPageIdChanged();

        partial void OnPageNameChanging(string value);
        partial void OnPageNameChanged();

        partial void OnPageUrlChanging(string value);
        partial void OnPageUrlChanged();

        partial void OnPageTypeChanging(int value);
        partial void OnPageTypeChanged();

        partial void OnFundIdChanging(int? value);
        partial void OnFundIdChanged();

        partial void OnEnabledChanging(bool value);
        partial void OnEnabledChanged();

        partial void OnDefaultPageChanging(bool? value);
        partial void OnDefaultPageChanged();

        partial void OnMainCampusPageFlagChanging(bool? value);
        partial void OnMainCampusPageFlagChanged();

        partial void OnDisabledRedirectChanging(string value);
        partial void OnDisabledRedirectChanged();

        partial void OnSkinFileIdChanging(int? value);
        partial void OnSkinFileIdChanged();

        partial void OnTopTextChanging(string value);
        partial void OnTopTextChanged();

        partial void OnThankYouTextChanging(string value);
        partial void OnThankYouTextChanged();

        partial void OnOnlineNotifyPersonChanging(string value);
        partial void OnOnlineNotifyPersonChanged();

        partial void OnConfirmationEmailPledgeIdChanging(int? value);
        partial void OnConfirmationEmailPledgeIdChanged();

        partial void OnConfirmationEmailOneTimeIdChanging(int? value);
        partial void OnConfirmationEmailOneTimeIdChanged();

        partial void OnConfirmationEmailRecurringIdChanging(int? value);
        partial void OnConfirmationEmailRecurringIdChanged();

        partial void OnCampusIdChanging(int? value);
        partial void OnCampusIdChanged();

        partial void OnEntryPointIdChanging(int? value);
        partial void OnEntryPointIdChanged();
        #endregion

        public GivingPage()
        {
            _GivingPageFunds = new EntitySet<GivingPageFund>(new Action<GivingPageFund>(attach_GivingPageFunds), new Action<GivingPageFund>(detach_GivingPageFunds));

            _Campu = default;

            _ContributionFund = default;

            _EntryPoint = default;

            _SkinFile = default;

            _ConfirmationEmailPledge = default;

            _ConfirmationEmailOneTime = default;

            _ConfirmationEmailRecurring = default;

            OnCreated();
        }

        #region Columns
        [Column(Name = "GivingPageId", UpdateCheck = UpdateCheck.Never, Storage = "_GivingPageId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int GivingPageId
        {
            get => _GivingPageId;

            set
            {
                if (_GivingPageId != value)
                {
                    OnGivingPageIdChanging(value);
                    SendPropertyChanging();
                    _GivingPageId = value;
                    SendPropertyChanged("GivingPageId");
                    OnGivingPageIdChanged();
                }
            }
        }

        [Column(Name = "PageName", UpdateCheck = UpdateCheck.Never, Storage = "_PageName", DbType = "nvarchar NOT NULL")]
        public string PageName
        {
            get => _PageName;

            set
            {
                if (_PageName != value)
                {
                    OnPageNameChanging(value);
                    SendPropertyChanging();
                    _PageName = value;
                    SendPropertyChanged("PageName");
                    OnPageNameChanged();
                }
            }
        }

        [Column(Name = "PageUrl", UpdateCheck = UpdateCheck.Never, Storage = "_PageUrl", DbType = "nvarchar NOT NULL")]
        public string PageUrl
        {
            get => _PageUrl;

            set
            {
                if (_PageUrl != value)
                {
                    OnPageUrlChanging(value);
                    SendPropertyChanging();
                    _PageUrl = value;
                    SendPropertyChanged("PageUrl");
                    OnPageUrlChanged();
                }
            }
        }

        [Column(Name = "PageType", UpdateCheck = UpdateCheck.Never, Storage = "_PageType", DbType = "int NOT NULL")]
        public int PageType
        {
            get => _PageType;

            set
            {
                if (_PageType != value)
                {
                    OnPageTypeChanging(value);
                    SendPropertyChanging();
                    _PageType = value;
                    SendPropertyChanged("PageType");
                    OnPageTypeChanged();
                }
            }
        }

        [Column(Name = "FundId", UpdateCheck = UpdateCheck.Never, Storage = "_FundId", DbType = "int")]
        [IsForeignKey]
        public int? FundId
        {
            get => _FundId;

            set
            {
                if (_FundId != value)
                {
                    if (_ContributionFund.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }
                    OnFundIdChanging(value);
                    SendPropertyChanging();
                    _FundId = value;
                    SendPropertyChanged("FundId");
                    OnFundIdChanged();
                }
            }
        }

        [Column(Name = "Enabled", UpdateCheck = UpdateCheck.Never, Storage = "_Enabled", DbType = "bit NOT NULL")]
        public bool Enabled
        {
            get => _Enabled;

            set
            {
                if (_Enabled != value)
                {
                    OnEnabledChanging(value);
                    SendPropertyChanging();
                    _Enabled = value;
                    SendPropertyChanged("Enabled");
                    OnEnabledChanged();
                }
            }
        }

        [Column(Name = "DefaultPage", UpdateCheck = UpdateCheck.Never, Storage = "_DefaultPage", DbType = "bit")]
        public bool? DefaultPage
        {
            get => _DefaultPage;

            set
            {
                if (_DefaultPage != value)
                {
                    OnDefaultPageChanging(value);
                    SendPropertyChanging();
                    _DefaultPage = value;
                    SendPropertyChanged("DefaultPage");
                    OnDefaultPageChanged();
                }
            }
        }

        [Column(Name = "MainCampusPageFlag", UpdateCheck = UpdateCheck.Never, Storage = "_MainCampusPageFlag", DbType = "bit")]
        public bool? MainCampusPageFlag
        {
            get => _MainCampusPageFlag;

            set
            {
                if (_MainCampusPageFlag != value)
                {
                    OnMainCampusPageFlagChanging(value);
                    SendPropertyChanging();
                    _MainCampusPageFlag = value;
                    SendPropertyChanged("MainCampusPageFlag");
                    OnMainCampusPageFlagChanged();
                }
            }
        }

        [Column(Name = "DisabledRedirect", UpdateCheck = UpdateCheck.Never, Storage = "_DisabledRedirect", DbType = "nvarchar")]
        public string DisabledRedirect
        {
            get => _DisabledRedirect;

            set
            {
                if (_DisabledRedirect != value)
                {
                    OnDisabledRedirectChanging(value);
                    SendPropertyChanging();
                    _DisabledRedirect = value;
                    SendPropertyChanged("DisabledRedirect");
                    OnDisabledRedirectChanged();
                }
            }
        }

        [Column(Name = "SkinFileId", UpdateCheck = UpdateCheck.Never, Storage = "_SkinFileId", DbType = "int")]
        public int? SkinFileId
        {
            get => _SkinFileId;

            set
            {
                if (_SkinFileId != value)
                {
                    OnSkinFileIdChanging(value);
                    SendPropertyChanging();
                    _SkinFileId = value;
                    SendPropertyChanged("SkinFileId");
                    OnSkinFileIdChanged();
                }
            }
        }

        [Column(Name = "TopText", UpdateCheck = UpdateCheck.Never, Storage = "_TopText", DbType = "nvarchar")]
        public string TopText
        {
            get => _TopText;

            set
            {
                if (_TopText != value)
                {
                    OnTopTextChanging(value);
                    SendPropertyChanging();
                    _TopText = value;
                    SendPropertyChanged("TopText");
                    OnTopTextChanged();
                }
            }
        }

        [Column(Name = "ThankYouText", UpdateCheck = UpdateCheck.Never, Storage = "_ThankYouText", DbType = "nvarchar")]
        public string ThankYouText
        {
            get => _ThankYouText;

            set
            {
                if (_ThankYouText != value)
                {
                    OnThankYouTextChanging(value);
                    SendPropertyChanging();
                    _ThankYouText = value;
                    SendPropertyChanged("ThankYouText");
                    OnThankYouTextChanged();
                }
            }
        }

        [Column(Name = "OnlineNotifyPerson", UpdateCheck = UpdateCheck.Never, Storage = "_OnlineNotifyPerson", DbType = "nvarchar")]
        public string OnlineNotifyPerson
        {
            get => _OnlineNotifyPerson;

            set
            {
                if (_OnlineNotifyPerson != value)
                {
                    OnOnlineNotifyPersonChanging(value);
                    SendPropertyChanging();
                    _OnlineNotifyPerson = value;
                    SendPropertyChanged("OnlineNotifyPerson");
                    OnOnlineNotifyPersonChanged();
                }
            }
        }

        [Column(Name = "ConfirmationEmail_PledgeId", UpdateCheck = UpdateCheck.Never, Storage = "_ConfirmationEmailPledgeId", DbType = "int")]
        public int? ConfirmationEmailPledgeId
        {
            get => _ConfirmationEmailPledgeId;

            set
            {
                if (_ConfirmationEmailPledgeId != value)
                {
                    OnConfirmationEmailPledgeIdChanging(value);
                    SendPropertyChanging();
                    _ConfirmationEmailPledgeId = value;
                    SendPropertyChanged("ConfirmationEmailPledgeId");
                    OnConfirmationEmailPledgeIdChanged();
                }
            }
        }

        [Column(Name = "ConfirmationEmail_OneTimeId", UpdateCheck = UpdateCheck.Never, Storage = "_ConfirmationEmailOneTimeId", DbType = "int")]
        public int? ConfirmationEmailOneTimeId
        {
            get => _ConfirmationEmailOneTimeId;

            set
            {
                if (_ConfirmationEmailOneTimeId != value)
                {
                    OnConfirmationEmailOneTimeIdChanging(value);
                    SendPropertyChanging();
                    _ConfirmationEmailOneTimeId = value;
                    SendPropertyChanged("ConfirmationEmailOneTimeId");
                    OnConfirmationEmailOneTimeIdChanged();
                }
            }
        }

        [Column(Name = "ConfirmationEmail_RecurringId", UpdateCheck = UpdateCheck.Never, Storage = "_ConfirmationEmailRecurringId", DbType = "int")]
        public int? ConfirmationEmailRecurringId
        {
            get => _ConfirmationEmailRecurringId;

            set
            {
                if (_ConfirmationEmailRecurringId != value)
                {
                    OnConfirmationEmailRecurringIdChanging(value);
                    SendPropertyChanging();
                    _ConfirmationEmailRecurringId = value;
                    SendPropertyChanged("ConfirmationEmailRecurringId");
                    OnConfirmationEmailRecurringIdChanged();
                }
            }
        }

        [Column(Name = "CampusId", UpdateCheck = UpdateCheck.Never, Storage = "_CampusId", DbType = "int")]
        [IsForeignKey]
        public int? CampusId
        {
            get => _CampusId;

            set
            {
                if (_CampusId != value)
                {
                    if (_Campu.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }
                    OnCampusIdChanging(value);
                    SendPropertyChanging();
                    _CampusId = value;
                    SendPropertyChanged("CampusId");
                    OnCampusIdChanged();
                }
            }
        }

        [Column(Name = "EntryPointId", UpdateCheck = UpdateCheck.Never, Storage = "_EntryPointId", DbType = "int")]
        [IsForeignKey]
        public int? EntryPointId
        {
            get => _EntryPointId;

            set
            {
                if (_EntryPointId != value)
                {
                    if (_EntryPoint.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }
                    OnEntryPointIdChanging(value);
                    SendPropertyChanging();
                    _EntryPointId = value;
                    SendPropertyChanged("EntryPointId");
                    OnEntryPointIdChanged();
                }
            }
        }
        #endregion

        #region Foreign Key Tables
        [Association(Name = "FK_GivingPageFunds_GivingPages", Storage = "_GivingPageFunds", OtherKey = "GivingPageId")]
        public EntitySet<GivingPageFund> GivingPageFunds
        {
            get => _GivingPageFunds;
            set => _GivingPageFunds.Assign(value);
        }
        #endregion

        #region Foreign Keys
        [Association(Name = "FK_GivingPages_Campus", Storage = "_Campu", ThisKey = "CampusId", IsForeignKey = true)]
        public Campu Campu
        {
            get => _Campu.Entity;

            set
            {
                Campu previousValue = _Campu.Entity;
                if ((previousValue != value) || (_Campu.HasLoadedOrAssignedValue == false))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Campu.Entity = null;
                        previousValue.GivingPages.Remove(this);
                    }
                    _Campu.Entity = value;
                    if (value != null)
                    {
                        value.GivingPages.Add(this);
                        _CampusId = value.Id;
                    }
                    else
                    {
                        _CampusId = default;
                    }
                    SendPropertyChanged("Campu");
                }
            }
        }

        [Association(Name = "FK_GivingPages_ContributionFund", Storage = "_ContributionFund", ThisKey = "FundId", IsForeignKey = true)]
        public ContributionFund ContributionFund
        {
            get => _ContributionFund.Entity;

            set
            {
                ContributionFund previousValue = _ContributionFund.Entity;
                if ((previousValue != value) || (_ContributionFund.HasLoadedOrAssignedValue == false))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ContributionFund.Entity = null;
                        previousValue.GivingPages.Remove(this);
                    }
                    _ContributionFund.Entity = value;
                    if (value != null)
                    {
                        value.GivingPages.Add(this);
                        _FundId = value.FundId;
                    }
                    else
                    {
                        _FundId = default;
                    }
                    SendPropertyChanged("ContributionFund");
                }
            }
        }

        [Association(Name = "FK_GivingPages_EntryPoint", Storage = "_EntryPoint", ThisKey = "EntryPointId", IsForeignKey = true)]
        public EntryPoint EntryPoint
        {
            get => _EntryPoint.Entity;

            set
            {
                EntryPoint previousValue = _EntryPoint.Entity;
                if ((previousValue != value) || (_EntryPoint.HasLoadedOrAssignedValue == false))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _EntryPoint.Entity = null;
                        previousValue.GivingPages.Remove(this);
                    }
                    _EntryPoint.Entity = value;
                    if (value != null)
                    {
                        value.GivingPages.Add(this);
                        _EntryPointId = value.Id;
                    }
                    else
                    {
                        _EntryPointId = default;
                    }
                    SendPropertyChanged("EntryPoint");
                }
            }
        }

        [Association(Name = "FK_GivingPages_SkinFile", Storage = "_SkinFile", ThisKey = "SkinFileId", IsForeignKey = true)]
        public Content SkinFile
        {
            get => _SkinFile.Entity;

            set
            {
                var previousValue = _SkinFile.Entity;
                if ((previousValue != value) || (_SkinFile.HasLoadedOrAssignedValue == false))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _SkinFile.Entity = null;
                    }
                    _SkinFile.Entity = value;
                    if (value != null)
                    {
                        _SkinFileId = value.Id;
                    }
                    else
                    {
                        _SkinFileId = default;
                    }
                    SendPropertyChanged("SkinFile");
                }
            }
        }

        [Association(Name = "FK_GivingPages_ConfirmationEmailPledge", Storage = "_ConfirmationEmailPledge", ThisKey = "ConfirmationEmailPledgeId", IsForeignKey = true)]
        public Content ConfirmationEmailPledge
        {
            get => _ConfirmationEmailPledge.Entity;

            set
            {
                var previousValue = _ConfirmationEmailPledge.Entity;
                if ((previousValue != value) || (_ConfirmationEmailPledge.HasLoadedOrAssignedValue == false))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ConfirmationEmailPledge.Entity = null;
                    }
                    _ConfirmationEmailPledge.Entity = value;
                    if (value != null)
                    {
                        _ConfirmationEmailPledgeId = value.Id;
                    }
                    else
                    {
                        _ConfirmationEmailPledgeId = default;
                    }
                    SendPropertyChanged("ConfirmationEmailPledge");
                }
            }
        }

        [Association(Name = "FK_GivingPages_ConfirmationEmailOneTime", Storage = "_ConfirmationEmailOneTime", ThisKey = "ConfirmationEmailOneTimeId", IsForeignKey = true)]
        public Content ConfirmationEmailOneTime
        {
            get => _ConfirmationEmailOneTime.Entity;

            set
            {
                var previousValue = _ConfirmationEmailOneTime.Entity;
                if ((previousValue != value) || (_ConfirmationEmailOneTime.HasLoadedOrAssignedValue == false))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ConfirmationEmailOneTime.Entity = null;
                    }
                    _ConfirmationEmailOneTime.Entity = value;
                    if (value != null)
                    {
                        _ConfirmationEmailOneTimeId = value.Id;
                    }
                    else
                    {
                        _ConfirmationEmailOneTimeId = default;
                    }
                    SendPropertyChanged("ConfirmationEmailOneTime");
                }
            }
        }

        [Association(Name = "FK_GivingPages_ConfirmationEmailRecurring", Storage = "_ConfirmationEmailRecurring", ThisKey = "ConfirmationEmailRecurringId", IsForeignKey = true)]
        public Content ConfirmationEmailRecurring
        {
            get => _ConfirmationEmailRecurring.Entity;

            set
            {
                var previousValue = _ConfirmationEmailRecurring.Entity;
                if ((previousValue != value) || (_ConfirmationEmailRecurring.HasLoadedOrAssignedValue == false))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ConfirmationEmailRecurring.Entity = null;
                    }
                    _ConfirmationEmailRecurring.Entity = value;
                    if (value != null)
                    {
                        _ConfirmationEmailRecurringId = value.Id;
                    }
                    else
                    {
                        _ConfirmationEmailRecurringId = default;
                    }
                    SendPropertyChanged("ConfirmationEmailRecurring");
                }
            }
        }

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void attach_GivingPageFunds(GivingPageFund entity)
        {
            SendPropertyChanging();
            entity.GivingPage = this;
        }

        private void detach_GivingPageFunds(GivingPageFund entity)
        {
            SendPropertyChanging();
            entity.GivingPage = null;
        }
    }
}
