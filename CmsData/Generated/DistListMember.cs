using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "CMS_MAILINGS.DIST_LIST_MEMBERS_TBL")]
    public partial class DistListMember : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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


            this._DistributionList = default(EntityRef<DistributionList>);

            OnCreated();
        }


        #region Columns

        [Column(Name = "DISTRIBUTION_LIST_ID", UpdateCheck = UpdateCheck.Never, Storage = "_DistributionListId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int DistributionListId
        {
            get => this._DistributionListId;

            set
            {
                if (this._DistributionListId != value)
                {

                    if (this._DistributionList.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    this.OnDistributionListIdChanging(value);
                    this.SendPropertyChanging();
                    this._DistributionListId = value;
                    this.SendPropertyChanged("DistributionListId");
                    this.OnDistributionListIdChanged();
                }

            }

        }


        [Column(Name = "PEOPLE_ID", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int PeopleId
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


        [Column(Name = "CREATED_BY", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int NOT NULL")]
        public int CreatedBy
        {
            get => this._CreatedBy;

            set
            {
                if (this._CreatedBy != value)
                {

                    this.OnCreatedByChanging(value);
                    this.SendPropertyChanging();
                    this._CreatedBy = value;
                    this.SendPropertyChanged("CreatedBy");
                    this.OnCreatedByChanged();
                }

            }

        }


        [Column(Name = "CREATED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime NOT NULL")]
        public DateTime CreatedDate
        {
            get => this._CreatedDate;

            set
            {
                if (this._CreatedDate != value)
                {

                    this.OnCreatedDateChanging(value);
                    this.SendPropertyChanging();
                    this._CreatedDate = value;
                    this.SendPropertyChanged("CreatedDate");
                    this.OnCreatedDateChanged();
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
            get => this._DistributionList.Entity;

            set
            {
                DistributionList previousValue = this._DistributionList.Entity;
                if (((previousValue != value)
                            || (this._DistributionList.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if (previousValue != null)
                    {
                        this._DistributionList.Entity = null;
                        previousValue.DistListMembers.Remove(this);
                    }

                    this._DistributionList.Entity = value;
                    if (value != null)
                    {
                        value.DistListMembers.Add(this);

                        this._DistributionListId = value.DistributionListId;

                    }

                    else
                    {

                        this._DistributionListId = default(int);

                    }

                    this.SendPropertyChanged("DistributionList");
                }

            }

        }


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

