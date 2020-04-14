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
    [Table(Name = "dbo.GivingPageFunds")]
    public partial class GivingPageFund : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _GivingPageFundId;

        private int _GivingPageId;

        private int _FundId;

        private EntityRef<ContributionFund> _ContributionFund;

        private EntityRef<GivingPage> _GivingPage;

        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnGivingPageFundIdChanging(int value);
        partial void OnGivingPageFundIdChanged();

        partial void OnGivingPageIdChanging(int value);
        partial void OnGivingPageIdChanged();

        partial void OnFundIdChanging(int value);
        partial void OnFundIdChanged();

        #endregion
        public GivingPageFund()
        {
            _ContributionFund = default(EntityRef<ContributionFund>);

            _GivingPage = default(EntityRef<GivingPage>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "GivingPageFundId", UpdateCheck = UpdateCheck.Never, Storage = "_GivingPageFundId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int GivingPageFundId
        {
            get { return _GivingPageFundId; }

            set
            {
                if (_GivingPageFundId != value)
                {
                    OnGivingPageFundIdChanging(value);
                    SendPropertyChanging();
                    _GivingPageFundId = value;
                    SendPropertyChanged("GivingPageFundId");
                    OnGivingPageFundIdChanged();
                }
            }
        }

        [Column(Name = "GivingPageId", UpdateCheck = UpdateCheck.Never, Storage = "_GivingPageId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int GivingPageId
        {
            get { return _GivingPageId; }

            set
            {
                if (_GivingPageId != value)
                {
                    if (_GivingPage.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }
                    OnGivingPageIdChanging(value);
                    SendPropertyChanging();
                    _GivingPageId = value;
                    SendPropertyChanged("GivingPageId");
                    OnGivingPageIdChanged();
                }
            }
        }

        [Column(Name = "FundId", UpdateCheck = UpdateCheck.Never, Storage = "_FundId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int FundId
        {
            get { return _FundId; }

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

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_GivingPageFunds_ContributionFund", Storage = "_ContributionFund", ThisKey = "FundId", IsForeignKey = true)]
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
                        previousValue.GivingPageFunds.Remove(this);
                    }
                    _ContributionFund.Entity = value;
                    if (value != null)
                    {
                        value.GivingPageFunds.Add(this);
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

        [Association(Name = "FK_GivingPageFunds_GivingPages", Storage = "_GivingPage", ThisKey = "GivingPageId", IsForeignKey = true)]
        public GivingPage GivingPage
        {
            get { return _GivingPage.Entity; }

            set
            {
                GivingPage previousValue = _GivingPage.Entity;
                if (((previousValue != value) || (_GivingPage.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _GivingPage.Entity = null;
                        previousValue.GivingPageFunds.Remove(this);
                    }
                    _GivingPage.Entity = value;
                    if (value != null)
                    {
                        value.GivingPageFunds.Add(this);
                        _GivingPageId = value.GivingPageId;
                    }
                    else
                    {
                        _GivingPageId = default(int);
                    }
                    SendPropertyChanged("GivingPage");
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

    }
}
