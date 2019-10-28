using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.BundleDetail")]
    public partial class BundleDetail : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _BundleDetailId;

        private int _BundleHeaderId;

        private int _ContributionId;

        private int _CreatedBy;

        private DateTime _CreatedDate;

        private int? _BundleSort1;

        private int? _RefOrgId;

        private int? _RefPeopleId;

        private EntityRef<BundleHeader> _BundleHeader;

        private EntityRef<Contribution> _Contribution;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnBundleDetailIdChanging(int value);
        partial void OnBundleDetailIdChanged();

        partial void OnBundleHeaderIdChanging(int value);
        partial void OnBundleHeaderIdChanged();

        partial void OnContributionIdChanging(int value);
        partial void OnContributionIdChanged();

        partial void OnCreatedByChanging(int value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        partial void OnBundleSort1Changing(int? value);
        partial void OnBundleSort1Changed();

        partial void OnRefOrgIdChanging(int? value);
        partial void OnRefOrgIdChanged();

        partial void OnRefPeopleIdChanging(int? value);
        partial void OnRefPeopleIdChanged();

        #endregion

        public BundleDetail()
        {
            _BundleHeader = default(EntityRef<BundleHeader>);

            _Contribution = default(EntityRef<Contribution>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "BundleDetailId", UpdateCheck = UpdateCheck.Never, Storage = "_BundleDetailId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int BundleDetailId
        {
            get => _BundleDetailId;

            set
            {
                if (_BundleDetailId != value)
                {
                    OnBundleDetailIdChanging(value);
                    SendPropertyChanging();
                    _BundleDetailId = value;
                    SendPropertyChanged("BundleDetailId");
                    OnBundleDetailIdChanged();
                }
            }
        }

        [Column(Name = "BundleHeaderId", UpdateCheck = UpdateCheck.Never, Storage = "_BundleHeaderId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int BundleHeaderId
        {
            get => _BundleHeaderId;

            set
            {
                if (_BundleHeaderId != value)
                {
                    if (_BundleHeader.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnBundleHeaderIdChanging(value);
                    SendPropertyChanging();
                    _BundleHeaderId = value;
                    SendPropertyChanged("BundleHeaderId");
                    OnBundleHeaderIdChanged();
                }
            }
        }

        [Column(Name = "ContributionId", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int ContributionId
        {
            get => _ContributionId;

            set
            {
                if (_ContributionId != value)
                {
                    if (_Contribution.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnContributionIdChanging(value);
                    SendPropertyChanging();
                    _ContributionId = value;
                    SendPropertyChanged("ContributionId");
                    OnContributionIdChanged();
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

        [Column(Name = "BundleSort1", UpdateCheck = UpdateCheck.Never, Storage = "_BundleSort1", DbType = "int")]
        public int? BundleSort1
        {
            get => _BundleSort1;

            set
            {
                if (_BundleSort1 != value)
                {
                    OnBundleSort1Changing(value);
                    SendPropertyChanging();
                    _BundleSort1 = value;
                    SendPropertyChanged("BundleSort1");
                    OnBundleSort1Changed();
                }
            }
        }

        [Column(Name = "RefOrgId", UpdateCheck = UpdateCheck.Never, Storage = "_RefOrgId", DbType = "int")]
        public int? RefOrgId
        {
            get => _RefOrgId;

            set
            {
                if (_RefOrgId != value)
                {
                    OnRefOrgIdChanging(value);
                    SendPropertyChanging();
                    _RefOrgId = value;
                    SendPropertyChanged("RefOrgId");
                    OnRefOrgIdChanged();
                }
            }
        }

        [Column(Name = "RefPeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_RefPeopleId", DbType = "int")]
        public int? RefPeopleId
        {
            get => _RefPeopleId;

            set
            {
                if (_RefPeopleId != value)
                {
                    OnRefPeopleIdChanging(value);
                    SendPropertyChanging();
                    _RefPeopleId = value;
                    SendPropertyChanged("RefPeopleId");
                    OnRefPeopleIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "BUNDLE_DETAIL_BUNDLE_FK", Storage = "_BundleHeader", ThisKey = "BundleHeaderId", IsForeignKey = true)]
        public BundleHeader BundleHeader
        {
            get => _BundleHeader.Entity;

            set
            {
                BundleHeader previousValue = _BundleHeader.Entity;
                if (((previousValue != value)
                            || (_BundleHeader.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _BundleHeader.Entity = null;
                        previousValue.BundleDetails.Remove(this);
                    }

                    _BundleHeader.Entity = value;
                    if (value != null)
                    {
                        value.BundleDetails.Add(this);

                        _BundleHeaderId = value.BundleHeaderId;

                    }

                    else
                    {
                        _BundleHeaderId = default(int);

                    }

                    SendPropertyChanged("BundleHeader");
                }
            }
        }

        [Association(Name = "BUNDLE_DETAIL_CONTR_FK", Storage = "_Contribution", ThisKey = "ContributionId", IsForeignKey = true)]
        public Contribution Contribution
        {
            get => _Contribution.Entity;

            set
            {
                Contribution previousValue = _Contribution.Entity;
                if (((previousValue != value)
                            || (_Contribution.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Contribution.Entity = null;
                        previousValue.BundleDetails.Remove(this);
                    }

                    _Contribution.Entity = value;
                    if (value != null)
                    {
                        value.BundleDetails.Add(this);

                        _ContributionId = value.ContributionId;

                    }

                    else
                    {
                        _ContributionId = default(int);

                    }

                    SendPropertyChanged("Contribution");
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
    }
}
