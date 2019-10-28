using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "CMS_MAILINGS.DISTRIBUTION_LISTS_TBL")]
    public partial class DistributionList : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _DistributionListId;

        private int _CreatedBy;

        private DateTime _CreatedDate;

        private bool _RecordStatus;

        private string _DistributionListName;

        private string _DistributionListPurpose;

        private DateTime? _DateDeactivated;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private int? _MinistryId;

        private int? _UserId;


        private EntitySet<DistListMember> _DistListMembers;


        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnDistributionListIdChanging(int value);
        partial void OnDistributionListIdChanged();

        partial void OnCreatedByChanging(int value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        partial void OnRecordStatusChanging(bool value);
        partial void OnRecordStatusChanged();

        partial void OnDistributionListNameChanging(string value);
        partial void OnDistributionListNameChanged();

        partial void OnDistributionListPurposeChanging(string value);
        partial void OnDistributionListPurposeChanged();

        partial void OnDateDeactivatedChanging(DateTime? value);
        partial void OnDateDeactivatedChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        partial void OnMinistryIdChanging(int? value);
        partial void OnMinistryIdChanged();

        partial void OnUserIdChanging(int? value);
        partial void OnUserIdChanged();

        #endregion
        public DistributionList()
        {

            this._DistListMembers = new EntitySet<DistListMember>(new Action<DistListMember>(this.attach_DistListMembers), new Action<DistListMember>(this.detach_DistListMembers));


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

                    this.OnDistributionListIdChanging(value);
                    this.SendPropertyChanging();
                    this._DistributionListId = value;
                    this.SendPropertyChanged("DistributionListId");
                    this.OnDistributionListIdChanged();
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


        [Column(Name = "RECORD_STATUS", UpdateCheck = UpdateCheck.Never, Storage = "_RecordStatus", DbType = "bit NOT NULL")]
        public bool RecordStatus
        {
            get => this._RecordStatus;

            set
            {
                if (this._RecordStatus != value)
                {

                    this.OnRecordStatusChanging(value);
                    this.SendPropertyChanging();
                    this._RecordStatus = value;
                    this.SendPropertyChanged("RecordStatus");
                    this.OnRecordStatusChanged();
                }

            }

        }


        [Column(Name = "DISTRIBUTION_LIST_NAME", UpdateCheck = UpdateCheck.Never, Storage = "_DistributionListName", DbType = "varchar(25)")]
        public string DistributionListName
        {
            get => this._DistributionListName;

            set
            {
                if (this._DistributionListName != value)
                {

                    this.OnDistributionListNameChanging(value);
                    this.SendPropertyChanging();
                    this._DistributionListName = value;
                    this.SendPropertyChanged("DistributionListName");
                    this.OnDistributionListNameChanged();
                }

            }

        }


        [Column(Name = "DISTRIBUTION_LIST_PURPOSE", UpdateCheck = UpdateCheck.Never, Storage = "_DistributionListPurpose", DbType = "varchar(256)")]
        public string DistributionListPurpose
        {
            get => this._DistributionListPurpose;

            set
            {
                if (this._DistributionListPurpose != value)
                {

                    this.OnDistributionListPurposeChanging(value);
                    this.SendPropertyChanging();
                    this._DistributionListPurpose = value;
                    this.SendPropertyChanged("DistributionListPurpose");
                    this.OnDistributionListPurposeChanged();
                }

            }

        }


        [Column(Name = "DATE_DEACTIVATED", UpdateCheck = UpdateCheck.Never, Storage = "_DateDeactivated", DbType = "datetime")]
        public DateTime? DateDeactivated
        {
            get => this._DateDeactivated;

            set
            {
                if (this._DateDeactivated != value)
                {

                    this.OnDateDeactivatedChanging(value);
                    this.SendPropertyChanging();
                    this._DateDeactivated = value;
                    this.SendPropertyChanged("DateDeactivated");
                    this.OnDateDeactivatedChanged();
                }

            }

        }


        [Column(Name = "MODIFIED_BY", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedBy", DbType = "int")]
        public int? ModifiedBy
        {
            get => this._ModifiedBy;

            set
            {
                if (this._ModifiedBy != value)
                {

                    this.OnModifiedByChanging(value);
                    this.SendPropertyChanging();
                    this._ModifiedBy = value;
                    this.SendPropertyChanged("ModifiedBy");
                    this.OnModifiedByChanged();
                }

            }

        }


        [Column(Name = "MODIFIED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedDate", DbType = "datetime")]
        public DateTime? ModifiedDate
        {
            get => this._ModifiedDate;

            set
            {
                if (this._ModifiedDate != value)
                {

                    this.OnModifiedDateChanging(value);
                    this.SendPropertyChanging();
                    this._ModifiedDate = value;
                    this.SendPropertyChanged("ModifiedDate");
                    this.OnModifiedDateChanged();
                }

            }

        }


        [Column(Name = "MINISTRY_ID", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryId", DbType = "int")]
        public int? MinistryId
        {
            get => this._MinistryId;

            set
            {
                if (this._MinistryId != value)
                {

                    this.OnMinistryIdChanging(value);
                    this.SendPropertyChanging();
                    this._MinistryId = value;
                    this.SendPropertyChanged("MinistryId");
                    this.OnMinistryIdChanged();
                }

            }

        }


        [Column(Name = "USER_ID", UpdateCheck = UpdateCheck.Never, Storage = "_UserId", DbType = "int")]
        public int? UserId
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


        #endregion

        #region Foreign Key Tables

        [Association(Name = "DIST_LIST_MEMBERS_DIST_LIST_FK", Storage = "_DistListMembers", OtherKey = "DistributionListId")]
        public EntitySet<DistListMember> DistListMembers
           {
               get => this._DistListMembers;

            set => this._DistListMembers.Assign(value);

           }


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


        private void attach_DistListMembers(DistListMember entity)
        {
            this.SendPropertyChanging();
            entity.DistributionList = this;
        }

        private void detach_DistListMembers(DistListMember entity)
        {
            this.SendPropertyChanging();
            entity.DistributionList = null;
        }


    }

}

