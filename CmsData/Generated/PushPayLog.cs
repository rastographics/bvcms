using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.PushPayLog")]
    public partial class PushPayLog : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private long _Id;

        private DateTime? _TransactionDate;

        private string _BatchKey;

        private string _TransactionId;

        private DateTime? _ImportDate;

        private int? _BundleHeaderId;

        private int? _ContributionId;

        private string _SettlementKey;

        private string _OrganizationKey;

        private string _MerchantKey;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(long value);
        partial void OnIdChanged();

        partial void OnTransactionDateChanging(DateTime? value);
        partial void OnTransactionDateChanged();

        partial void OnBatchKeyChanging(string value);
        partial void OnBatchKeyChanged();

        partial void OnTransactionIdChanging(string value);
        partial void OnTransactionIdChanged();

        partial void OnImportDateChanging(DateTime? value);
        partial void OnImportDateChanged();

        partial void OnBundleHeaderIdChanging(int? value);
        partial void OnBundleHeaderIdChanged();

        partial void OnContributionIdChanging(int? value);
        partial void OnContributionIdChanged();

        partial void OnSettlementKeyChanging(string value);
        partial void OnSettlementKeyChanged();

        partial void OnOrganizationKeyChanging(string value);
        partial void OnOrganizationKeyChanged();

        partial void OnMerchantKeyChanging(string value);
        partial void OnMerchantKeyChanged();

        #endregion

        public PushPayLog()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "bigint NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public long Id
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

        [Column(Name = "TransactionDate", UpdateCheck = UpdateCheck.Never, Storage = "_TransactionDate", DbType = "datetime")]
        public DateTime? TransactionDate
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

        [Column(Name = "BatchKey", UpdateCheck = UpdateCheck.Never, Storage = "_BatchKey", DbType = "nvarchar(100)")]
        public string BatchKey
        {
            get => _BatchKey;

            set
            {
                if (_BatchKey != value)
                {
                    OnBatchKeyChanging(value);
                    SendPropertyChanging();
                    _BatchKey = value;
                    SendPropertyChanged("BatchKey");
                    OnBatchKeyChanged();
                }
            }
        }

        [Column(Name = "TransactionId", UpdateCheck = UpdateCheck.Never, Storage = "_TransactionId", DbType = "nvarchar(100)")]
        public string TransactionId
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

        [Column(Name = "ImportDate", UpdateCheck = UpdateCheck.Never, Storage = "_ImportDate", DbType = "datetime")]
        public DateTime? ImportDate
        {
            get => _ImportDate;

            set
            {
                if (_ImportDate != value)
                {
                    OnImportDateChanging(value);
                    SendPropertyChanging();
                    _ImportDate = value;
                    SendPropertyChanged("ImportDate");
                    OnImportDateChanged();
                }
            }
        }

        [Column(Name = "BundleHeaderId", UpdateCheck = UpdateCheck.Never, Storage = "_BundleHeaderId", DbType = "int")]
        public int? BundleHeaderId
        {
            get => _BundleHeaderId;

            set
            {
                if (_BundleHeaderId != value)
                {
                    OnBundleHeaderIdChanging(value);
                    SendPropertyChanging();
                    _BundleHeaderId = value;
                    SendPropertyChanged("BundleHeaderId");
                    OnBundleHeaderIdChanged();
                }
            }
        }

        [Column(Name = "ContributionId", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionId", DbType = "int")]
        public int? ContributionId
        {
            get => _ContributionId;

            set
            {
                if (_ContributionId != value)
                {
                    OnContributionIdChanging(value);
                    SendPropertyChanging();
                    _ContributionId = value;
                    SendPropertyChanged("ContributionId");
                    OnContributionIdChanged();
                }
            }
        }

        [Column(Name = "SettlementKey", UpdateCheck = UpdateCheck.Never, Storage = "_SettlementKey", DbType = "nvarchar(100)")]
        public string SettlementKey
        {
            get => _SettlementKey;

            set
            {
                if (_SettlementKey != value)
                {
                    OnSettlementKeyChanging(value);
                    SendPropertyChanging();
                    _SettlementKey = value;
                    SendPropertyChanged("SettlementKey");
                    OnSettlementKeyChanged();
                }
            }
        }

        [Column(Name = "OrganizationKey", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationKey", DbType = "nvarchar(100)")]
        public string OrganizationKey
        {
            get => _OrganizationKey;

            set
            {
                if (_OrganizationKey != value)
                {
                    OnOrganizationKeyChanging(value);
                    SendPropertyChanging();
                    _OrganizationKey = value;
                    SendPropertyChanged("OrganizationKey");
                    OnOrganizationKeyChanged();
                }
            }
        }

        [Column(Name = "MerchantKey", UpdateCheck = UpdateCheck.Never, Storage = "_MerchantKey", DbType = "nvarchar(100)")]
        public string MerchantKey
        {
            get => _MerchantKey;

            set
            {
                if (_MerchantKey != value)
                {
                    OnMerchantKeyChanging(value);
                    SendPropertyChanging();
                    _MerchantKey = value;
                    SendPropertyChanged("MerchantKey");
                    OnMerchantKeyChanged();
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
