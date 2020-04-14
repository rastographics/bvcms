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

        private string _PageTitle;

        private int _PageType;

        private int _FundId;

        private bool _Enabled;

        private string _DisabledReDirect;

        private string _SkinFile;

        private string _TopText;

        private string _ThankYouText;

        private string _ConfirmationEmailPledge;

        private string _ConfirmationEmailOneTime;

        private string _ConfirmationEmailRecurring;

        private int? _CampusId;

        private int? _EntryPointId;

        private EntitySet<GivingPageFund> _GivingPageFunds;

        private EntityRef<Campu> _Campu;

        private EntityRef<ContributionFund> _ContributionFund;

        private EntityRef<EntryPoint> _EntryPoint;
        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnGivingPageIdChanging(int value);
        partial void OnGivingPageIdChanged();

        partial void OnPageNameChanging(string value);
        partial void OnPageNameChanged();

        partial void OnPageTitleChanging(string value);
        partial void OnPageTitleChanged();

        partial void OnPageTypeChanging(int value);
        partial void OnPageTypeChanged();

        partial void OnFundIdChanging(int value);
        partial void OnFundIdChanged();

        partial void OnEnabledChanging(bool value);
        partial void OnEnabledChanged();

        partial void OnDisabledReDirectChanging(string value);
        partial void OnDisabledReDirectChanged();

        partial void OnSkinFileChanging(string value);
        partial void OnSkinFileChanged();

        partial void OnTopTextChanging(string value);
        partial void OnTopTextChanged();

        partial void OnThankYouTextChanging(string value);
        partial void OnThankYouTextChanged();

        partial void OnConfirmationEmailPledgeChanging(string value);
        partial void OnConfirmationEmailPledgeChanged();

        partial void OnConfirmationEmailOneTimeChanging(string value);
        partial void OnConfirmationEmailOneTimeChanged();

        partial void OnConfirmationEmailRecurringChanging(string value);
        partial void OnConfirmationEmailRecurringChanged();

        partial void OnCampusIdChanging(int? value);
        partial void OnCampusIdChanged();

        partial void OnEntryPointIdChanging(int? value);
        partial void OnEntryPointIdChanged();
        #endregion

        public GivingPage()
        {
            _GivingPageFunds = new EntitySet<GivingPageFund>(new Action<GivingPageFund>(attach_GivingPageFunds), new Action<GivingPageFund>(detach_GivingPageFunds));

            _Campu = default(EntityRef<Campu>);

            _ContributionFund = default(EntityRef<ContributionFund>);

            _EntryPoint = default(EntityRef<EntryPoint>);

            OnCreated();
        }

        #region Columns
        [Column(Name = "GivingPageId", UpdateCheck = UpdateCheck.Never, Storage = "_GivingPageId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int GivingPageId
        {
            get { return _GivingPageId; }

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
            get { return _PageName; }

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

        [Column(Name = "PageTitle", UpdateCheck = UpdateCheck.Never, Storage = "_PageTitle", DbType = "nvarchar NOT NULL")]
        public string PageTitle
        {
            get { return _PageTitle; }

            set
            {
                if (_PageTitle != value)
                {
                    OnPageTitleChanging(value);
                    SendPropertyChanging();
                    _PageTitle = value;
                    SendPropertyChanged("PageTitle");
                    OnPageTitleChanged();
                }
            }
        }

        [Column(Name = "PageType", UpdateCheck = UpdateCheck.Never, Storage = "_PageType", DbType = "int NOT NULL")]
        public int PageType
        {
            get { return _PageType; }

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

        [Column(Name = "FundId", UpdateCheck = UpdateCheck.Never, Storage = "_FundId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int FundId
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
            get { return _Enabled; }

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

        [Column(Name = "DisabledReDirect", UpdateCheck = UpdateCheck.Never, Storage = "_DisabledReDirect", DbType = "nvarchar NOT NULL")]
        public string DisabledReDirect
        {
            get { return _DisabledReDirect; }

            set
            {
                if (_DisabledReDirect != value)
                {
                    OnDisabledReDirectChanging(value);
                    SendPropertyChanging();
                    _DisabledReDirect = value;
                    SendPropertyChanged("DisabledReDirect");
                    OnDisabledReDirectChanged();
                }
            }
        }

        [Column(Name = "SkinFile", UpdateCheck = UpdateCheck.Never, Storage = "_SkinFile", DbType = "nvarchar")]
        public string SkinFile
        {
            get { return _SkinFile; }

            set
            {
                if (_SkinFile != value)
                {
                    OnSkinFileChanging(value);
                    SendPropertyChanging();
                    _SkinFile = value;
                    SendPropertyChanged("SkinFile");
                    OnSkinFileChanged();
                }
            }
        }

        [Column(Name = "TopText", UpdateCheck = UpdateCheck.Never, Storage = "_TopText", DbType = "nvarchar")]
        public string TopText
        {
            get { return _TopText; }

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
            get { return _ThankYouText; }

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

        [Column(Name = "ConfirmationEmail_Pledge", UpdateCheck = UpdateCheck.Never, Storage = "_ConfirmationEmailPledge", DbType = "nvarchar")]
        public string ConfirmationEmailPledge
        {
            get { return _ConfirmationEmailPledge; }

            set
            {
                if (_ConfirmationEmailPledge != value)
                {
                    OnConfirmationEmailPledgeChanging(value);
                    SendPropertyChanging();
                    _ConfirmationEmailPledge = value;
                    SendPropertyChanged("ConfirmationEmailPledge");
                    OnConfirmationEmailPledgeChanged();
                }
            }
        }

        [Column(Name = "ConfirmationEmail_OneTime", UpdateCheck = UpdateCheck.Never, Storage = "_ConfirmationEmailOneTime", DbType = "nvarchar")]
        public string ConfirmationEmailOneTime
        {
            get { return _ConfirmationEmailOneTime; }

            set
            {
                if (_ConfirmationEmailOneTime != value)
                {
                    OnConfirmationEmailOneTimeChanging(value);
                    SendPropertyChanging();
                    _ConfirmationEmailOneTime = value;
                    SendPropertyChanged("ConfirmationEmailOneTime");
                    OnConfirmationEmailOneTimeChanged();
                }
            }
        }

        [Column(Name = "ConfirmationEmail_Recurring", UpdateCheck = UpdateCheck.Never, Storage = "_ConfirmationEmailRecurring", DbType = "nvarchar")]
        public string ConfirmationEmailRecurring
        {
            get { return _ConfirmationEmailRecurring; }

            set
            {
                if (_ConfirmationEmailRecurring != value)
                {
                    OnConfirmationEmailRecurringChanging(value);
                    SendPropertyChanging();
                    _ConfirmationEmailRecurring = value;
                    SendPropertyChanged("ConfirmationEmailRecurring");
                    OnConfirmationEmailRecurringChanged();
                }
            }
        }

        [Column(Name = "CampusId", UpdateCheck = UpdateCheck.Never, Storage = "_CampusId", DbType = "int")]
        [IsForeignKey]
        public int? CampusId
        {
            get { return _CampusId; }

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
            get { return _GivingPageFunds; }
            set { _GivingPageFunds.Assign(value); }
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
                if (((previousValue != value) || (_Campu.HasLoadedOrAssignedValue == false)))
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
                        _CampusId = default(int?);
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
                if (((previousValue != value) || (_ContributionFund.HasLoadedOrAssignedValue == false)))
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
                        _FundId = default(int);
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
                if (((previousValue != value) || (_EntryPoint.HasLoadedOrAssignedValue == false)))
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
                        _EntryPointId = default(int?);
                    }
                    SendPropertyChanged("EntryPoint");
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
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((PropertyChanged != null))
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
