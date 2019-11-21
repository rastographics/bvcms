using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ContributionFund")]
    public partial class ContributionFund : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _FundId;

        private int _CreatedBy;

        private DateTime _CreatedDate;

        private string _FundName;

        private string _FundDescription;

        private int _FundStatusId;

        private int _FundTypeId;

        private bool _FundPledgeFlag;

        private int? _FundAccountCode;

        private string _FundIncomeDept;

        private string _FundIncomeAccount;

        private string _FundIncomeFund;

        private string _FundCashDept;

        private string _FundCashAccount;

        private string _FundCashFund;

        private int? _OnlineSort;

        private bool? _NonTaxDeductible;

        private int _QBIncomeAccount;

        private int _QBAssetAccount;

        private int _FundManagerRoleId;

        private EntitySet<BundleHeader> _BundleHeaders;

        private EntitySet<Contribution> _Contributions;

        private EntitySet<RecurringAmount> _RecurringAmounts;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnFundIdChanging(int value);
        partial void OnFundIdChanged();

        partial void OnCreatedByChanging(int value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        partial void OnFundNameChanging(string value);
        partial void OnFundNameChanged();

        partial void OnFundDescriptionChanging(string value);
        partial void OnFundDescriptionChanged();

        partial void OnFundStatusIdChanging(int value);
        partial void OnFundStatusIdChanged();

        partial void OnFundTypeIdChanging(int value);
        partial void OnFundTypeIdChanged();

        partial void OnFundPledgeFlagChanging(bool value);
        partial void OnFundPledgeFlagChanged();

        partial void OnFundAccountCodeChanging(int? value);
        partial void OnFundAccountCodeChanged();

        partial void OnFundIncomeDeptChanging(string value);
        partial void OnFundIncomeDeptChanged();

        partial void OnFundIncomeAccountChanging(string value);
        partial void OnFundIncomeAccountChanged();

        partial void OnFundIncomeFundChanging(string value);
        partial void OnFundIncomeFundChanged();

        partial void OnFundCashDeptChanging(string value);
        partial void OnFundCashDeptChanged();

        partial void OnFundCashAccountChanging(string value);
        partial void OnFundCashAccountChanged();

        partial void OnFundCashFundChanging(string value);
        partial void OnFundCashFundChanged();

        partial void OnOnlineSortChanging(int? value);
        partial void OnOnlineSortChanged();

        partial void OnNonTaxDeductibleChanging(bool? value);
        partial void OnNonTaxDeductibleChanged();

        partial void OnQBIncomeAccountChanging(int value);
        partial void OnQBIncomeAccountChanged();

        partial void OnQBAssetAccountChanging(int value);
        partial void OnQBAssetAccountChanged();

        partial void OnFundManagerRoleIdChanging(int value);
        partial void OnFundManagerRoleIdChanged();

        #endregion

        public ContributionFund()
        {
            _BundleHeaders = new EntitySet<BundleHeader>(new Action<BundleHeader>(attach_BundleHeaders), new Action<BundleHeader>(detach_BundleHeaders));

            _Contributions = new EntitySet<Contribution>(new Action<Contribution>(attach_Contributions), new Action<Contribution>(detach_Contributions));

            _RecurringAmounts = new EntitySet<RecurringAmount>(new Action<RecurringAmount>(attach_RecurringAmounts), new Action<RecurringAmount>(detach_RecurringAmounts));

            OnCreated();
        }

        #region Columns

        [Column(Name = "FundId", UpdateCheck = UpdateCheck.Never, Storage = "_FundId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int FundId
        {
            get => _FundId;

            set
            {
                if (_FundId != value)
                {
                    OnFundIdChanging(value);
                    SendPropertyChanging();
                    _FundId = value;
                    SendPropertyChanged("FundId");
                    OnFundIdChanged();
                }
            }
        }

        [Column(Name = "CreatedBy", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int NOT NULL")]
        public int CreatedBy
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

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime NOT NULL")]
        public DateTime CreatedDate
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

        [Column(Name = "FundName", UpdateCheck = UpdateCheck.Never, Storage = "_FundName", DbType = "nvarchar(256) NOT NULL")]
        public string FundName
        {
            get => _FundName;

            set
            {
                if (_FundName != value)
                {
                    OnFundNameChanging(value);
                    SendPropertyChanging();
                    _FundName = value;
                    SendPropertyChanged("FundName");
                    OnFundNameChanged();
                }
            }
        }

        [Column(Name = "FundDescription", UpdateCheck = UpdateCheck.Never, Storage = "_FundDescription", DbType = "nvarchar(256)")]
        public string FundDescription
        {
            get => _FundDescription;

            set
            {
                if (_FundDescription != value)
                {
                    OnFundDescriptionChanging(value);
                    SendPropertyChanging();
                    _FundDescription = value;
                    SendPropertyChanged("FundDescription");
                    OnFundDescriptionChanged();
                }
            }
        }

        [Column(Name = "FundStatusId", UpdateCheck = UpdateCheck.Never, Storage = "_FundStatusId", DbType = "int NOT NULL")]
        public int FundStatusId
        {
            get => _FundStatusId;

            set
            {
                if (_FundStatusId != value)
                {
                    OnFundStatusIdChanging(value);
                    SendPropertyChanging();
                    _FundStatusId = value;
                    SendPropertyChanged("FundStatusId");
                    OnFundStatusIdChanged();
                }
            }
        }

        [Column(Name = "FundTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_FundTypeId", DbType = "int NOT NULL")]
        public int FundTypeId
        {
            get => _FundTypeId;

            set
            {
                if (_FundTypeId != value)
                {
                    OnFundTypeIdChanging(value);
                    SendPropertyChanging();
                    _FundTypeId = value;
                    SendPropertyChanged("FundTypeId");
                    OnFundTypeIdChanged();
                }
            }
        }

        [Column(Name = "FundPledgeFlag", UpdateCheck = UpdateCheck.Never, Storage = "_FundPledgeFlag", DbType = "bit NOT NULL")]
        public bool FundPledgeFlag
        {
            get => _FundPledgeFlag;

            set
            {
                if (_FundPledgeFlag != value)
                {
                    OnFundPledgeFlagChanging(value);
                    SendPropertyChanging();
                    _FundPledgeFlag = value;
                    SendPropertyChanged("FundPledgeFlag");
                    OnFundPledgeFlagChanged();
                }
            }
        }

        [Column(Name = "FundAccountCode", UpdateCheck = UpdateCheck.Never, Storage = "_FundAccountCode", DbType = "int")]
        public int? FundAccountCode
        {
            get => _FundAccountCode;

            set
            {
                if (_FundAccountCode != value)
                {
                    OnFundAccountCodeChanging(value);
                    SendPropertyChanging();
                    _FundAccountCode = value;
                    SendPropertyChanged("FundAccountCode");
                    OnFundAccountCodeChanged();
                }
            }
        }

        [Column(Name = "FundIncomeDept", UpdateCheck = UpdateCheck.Never, Storage = "_FundIncomeDept", DbType = "nvarchar(25)")]
        public string FundIncomeDept
        {
            get => _FundIncomeDept;

            set
            {
                if (_FundIncomeDept != value)
                {
                    OnFundIncomeDeptChanging(value);
                    SendPropertyChanging();
                    _FundIncomeDept = value;
                    SendPropertyChanged("FundIncomeDept");
                    OnFundIncomeDeptChanged();
                }
            }
        }

        [Column(Name = "FundIncomeAccount", UpdateCheck = UpdateCheck.Never, Storage = "_FundIncomeAccount", DbType = "nvarchar(25)")]
        public string FundIncomeAccount
        {
            get => _FundIncomeAccount;

            set
            {
                if (_FundIncomeAccount != value)
                {
                    OnFundIncomeAccountChanging(value);
                    SendPropertyChanging();
                    _FundIncomeAccount = value;
                    SendPropertyChanged("FundIncomeAccount");
                    OnFundIncomeAccountChanged();
                }
            }
        }

        [Column(Name = "FundIncomeFund", UpdateCheck = UpdateCheck.Never, Storage = "_FundIncomeFund", DbType = "nvarchar(25)")]
        public string FundIncomeFund
        {
            get => _FundIncomeFund;

            set
            {
                if (_FundIncomeFund != value)
                {
                    OnFundIncomeFundChanging(value);
                    SendPropertyChanging();
                    _FundIncomeFund = value;
                    SendPropertyChanged("FundIncomeFund");
                    OnFundIncomeFundChanged();
                }
            }
        }

        [Column(Name = "FundCashDept", UpdateCheck = UpdateCheck.Never, Storage = "_FundCashDept", DbType = "nvarchar(25)")]
        public string FundCashDept
        {
            get => _FundCashDept;

            set
            {
                if (_FundCashDept != value)
                {
                    OnFundCashDeptChanging(value);
                    SendPropertyChanging();
                    _FundCashDept = value;
                    SendPropertyChanged("FundCashDept");
                    OnFundCashDeptChanged();
                }
            }
        }

        [Column(Name = "FundCashAccount", UpdateCheck = UpdateCheck.Never, Storage = "_FundCashAccount", DbType = "nvarchar(25)")]
        public string FundCashAccount
        {
            get => _FundCashAccount;

            set
            {
                if (_FundCashAccount != value)
                {
                    OnFundCashAccountChanging(value);
                    SendPropertyChanging();
                    _FundCashAccount = value;
                    SendPropertyChanged("FundCashAccount");
                    OnFundCashAccountChanged();
                }
            }
        }

        [Column(Name = "FundCashFund", UpdateCheck = UpdateCheck.Never, Storage = "_FundCashFund", DbType = "nvarchar(25)")]
        public string FundCashFund
        {
            get => _FundCashFund;

            set
            {
                if (_FundCashFund != value)
                {
                    OnFundCashFundChanging(value);
                    SendPropertyChanging();
                    _FundCashFund = value;
                    SendPropertyChanged("FundCashFund");
                    OnFundCashFundChanged();
                }
            }
        }

        [Column(Name = "OnlineSort", UpdateCheck = UpdateCheck.Never, Storage = "_OnlineSort", DbType = "int")]
        public int? OnlineSort
        {
            get => _OnlineSort;

            set
            {
                if (_OnlineSort != value)
                {
                    OnOnlineSortChanging(value);
                    SendPropertyChanging();
                    _OnlineSort = value;
                    SendPropertyChanged("OnlineSort");
                    OnOnlineSortChanged();
                }
            }
        }

        [Column(Name = "NonTaxDeductible", UpdateCheck = UpdateCheck.Never, Storage = "_NonTaxDeductible", DbType = "bit")]
        public bool? NonTaxDeductible
        {
            get => _NonTaxDeductible;

            set
            {
                if (_NonTaxDeductible != value)
                {
                    OnNonTaxDeductibleChanging(value);
                    SendPropertyChanging();
                    _NonTaxDeductible = value;
                    SendPropertyChanged("NonTaxDeductible");
                    OnNonTaxDeductibleChanged();
                }
            }
        }

        [Column(Name = "QBIncomeAccount", UpdateCheck = UpdateCheck.Never, Storage = "_QBIncomeAccount", DbType = "int NOT NULL")]
        public int QBIncomeAccount
        {
            get => _QBIncomeAccount;

            set
            {
                if (_QBIncomeAccount != value)
                {
                    OnQBIncomeAccountChanging(value);
                    SendPropertyChanging();
                    _QBIncomeAccount = value;
                    SendPropertyChanged("QBIncomeAccount");
                    OnQBIncomeAccountChanged();
                }
            }
        }

        [Column(Name = "QBAssetAccount", UpdateCheck = UpdateCheck.Never, Storage = "_QBAssetAccount", DbType = "int NOT NULL")]
        public int QBAssetAccount
        {
            get => _QBAssetAccount;

            set
            {
                if (_QBAssetAccount != value)
                {
                    OnQBAssetAccountChanging(value);
                    SendPropertyChanging();
                    _QBAssetAccount = value;
                    SendPropertyChanged("QBAssetAccount");
                    OnQBAssetAccountChanged();
                }
            }
        }

        [Column(Name = "FundManagerRoleId", UpdateCheck = UpdateCheck.Never, Storage = "_FundManagerRoleId", DbType = "int NOT NULL")]
        public int FundManagerRoleId
        {
            get => _FundManagerRoleId;

            set
            {
                if (_FundManagerRoleId != value)
                {
                    OnFundManagerRoleIdChanging(value);
                    SendPropertyChanging();
                    _FundManagerRoleId = value;
                    SendPropertyChanged("FundManagerRoleId");
                    OnFundManagerRoleIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "BundleHeaders__Fund", Storage = "_BundleHeaders", OtherKey = "FundId")]
        public EntitySet<BundleHeader> BundleHeaders
           {
               get => _BundleHeaders;

            set => _BundleHeaders.Assign(value);

           }

        [Association(Name = "FK_Contribution_ContributionFund", Storage = "_Contributions", OtherKey = "FundId")]
        public EntitySet<Contribution> Contributions
           {
               get => _Contributions;

            set => _Contributions.Assign(value);

           }

        [Association(Name = "FK_RecurringAmounts_ContributionFund", Storage = "_RecurringAmounts", OtherKey = "FundId")]
        public EntitySet<RecurringAmount> RecurringAmounts
           {
               get => _RecurringAmounts;

            set => _RecurringAmounts.Assign(value);

           }

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

        private void attach_BundleHeaders(BundleHeader entity)
        {
            SendPropertyChanging();
            entity.Fund = this;
        }

        private void detach_BundleHeaders(BundleHeader entity)
        {
            SendPropertyChanging();
            entity.Fund = null;
        }

        private void attach_Contributions(Contribution entity)
        {
            SendPropertyChanging();
            entity.ContributionFund = this;
        }

        private void detach_Contributions(Contribution entity)
        {
            SendPropertyChanging();
            entity.ContributionFund = null;
        }

        private void attach_RecurringAmounts(RecurringAmount entity)
        {
            SendPropertyChanging();
            entity.ContributionFund = this;
        }

        private void detach_RecurringAmounts(RecurringAmount entity)
        {
            SendPropertyChanging();
            entity.ContributionFund = null;
        }
    }
}
