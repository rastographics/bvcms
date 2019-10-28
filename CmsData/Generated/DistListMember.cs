using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "CMS_MAILINGS.DIST_LIST_MEMBERS_TBL")]
    public partial class DistListMember : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _DistributionListId;

        private int _PeopleId;

        private int _CreatedBy;

        private DateTime _CreatedDate;

        private EntityRef<DistributionList> _DistributionList;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnDistributionListIdChanging(int value);
        partial void OnDistributionListIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnCreatedByChanging(int value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        #endregion

        public DistListMember()
        {
            _DistributionList = default(EntityRef<DistributionList>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "DISTRIBUTION_LIST_ID", UpdateCheck = UpdateCheck.Never, Storage = "_DistributionListId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int DistributionListId
        {
            get => _DistributionListId;

            set
            {
                if (_DistributionListId != value)
                {
                    if (_DistributionList.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnDistributionListIdChanging(value);
                    SendPropertyChanging();
                    _DistributionListId = value;
                    SendPropertyChanged("DistributionListId");
                    OnDistributionListIdChanged();
                }
            }
        }

        [Column(Name = "PEOPLE_ID", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int PeopleId
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

        [Column(Name = "CREATED_BY", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int NOT NULL")]
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

        [Column(Name = "CREATED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime NOT NULL")]
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

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "DIST_LIST_MEMBERS_DIST_LIST_FK", Storage = "_DistributionList", ThisKey = "DistributionListId", IsForeignKey = true)]
        public DistributionList DistributionList
        {
            get => _DistributionList.Entity;

            set
            {
                DistributionList previousValue = _DistributionList.Entity;
                if (((previousValue != value)
                            || (_DistributionList.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _DistributionList.Entity = null;
                        previousValue.DistListMembers.Remove(this);
                    }

                    _DistributionList.Entity = value;
                    if (value != null)
                    {
                        value.DistListMembers.Add(this);

                        _DistributionListId = value.DistributionListId;

                    }

                    else
                    {
                        _DistributionListId = default(int);

                    }

                    SendPropertyChanged("DistributionList");
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
