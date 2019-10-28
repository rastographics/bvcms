using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "CMS_VOLUNTEER.APPLICATION_PREREQ_TBL")]
    public partial class ApplicationPrereq : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _ApplicationPrereqId;

        private int? _ApplicationId;

        private int? _PrerequisiteId;

        private int? _CheckedBy;

        private DateTime? _CheckedDate;

        private DateTime? _DateSatisfied;

        private int? _CreatedBy;

        private DateTime? _CreatedDate;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private string _RecordStatus;

        private int? _ChurchId;

        private int? _Status;

        private string _Comments;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnApplicationPrereqIdChanging(int value);
        partial void OnApplicationPrereqIdChanged();

        partial void OnApplicationIdChanging(int? value);
        partial void OnApplicationIdChanged();

        partial void OnPrerequisiteIdChanging(int? value);
        partial void OnPrerequisiteIdChanged();

        partial void OnCheckedByChanging(int? value);
        partial void OnCheckedByChanged();

        partial void OnCheckedDateChanging(DateTime? value);
        partial void OnCheckedDateChanged();

        partial void OnDateSatisfiedChanging(DateTime? value);
        partial void OnDateSatisfiedChanged();

        partial void OnCreatedByChanging(int? value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime? value);
        partial void OnCreatedDateChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        partial void OnRecordStatusChanging(string value);
        partial void OnRecordStatusChanged();

        partial void OnChurchIdChanging(int? value);
        partial void OnChurchIdChanged();

        partial void OnStatusChanging(int? value);
        partial void OnStatusChanged();

        partial void OnCommentsChanging(string value);
        partial void OnCommentsChanged();

        #endregion
        public ApplicationPrereq()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "APPLICATION_PREREQ_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ApplicationPrereqId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ApplicationPrereqId
        {
            get => this._ApplicationPrereqId;

            set
            {
                if (this._ApplicationPrereqId != value)
                {

                    this.OnApplicationPrereqIdChanging(value);
                    this.SendPropertyChanging();
                    this._ApplicationPrereqId = value;
                    this.SendPropertyChanged("ApplicationPrereqId");
                    this.OnApplicationPrereqIdChanged();
                }

            }

        }


        [Column(Name = "APPLICATION_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ApplicationId", DbType = "int")]
        public int? ApplicationId
        {
            get => this._ApplicationId;

            set
            {
                if (this._ApplicationId != value)
                {

                    this.OnApplicationIdChanging(value);
                    this.SendPropertyChanging();
                    this._ApplicationId = value;
                    this.SendPropertyChanged("ApplicationId");
                    this.OnApplicationIdChanged();
                }

            }

        }


        [Column(Name = "PREREQUISITE_ID", UpdateCheck = UpdateCheck.Never, Storage = "_PrerequisiteId", DbType = "int")]
        public int? PrerequisiteId
        {
            get => this._PrerequisiteId;

            set
            {
                if (this._PrerequisiteId != value)
                {

                    this.OnPrerequisiteIdChanging(value);
                    this.SendPropertyChanging();
                    this._PrerequisiteId = value;
                    this.SendPropertyChanged("PrerequisiteId");
                    this.OnPrerequisiteIdChanged();
                }

            }

        }


        [Column(Name = "CHECKED_BY", UpdateCheck = UpdateCheck.Never, Storage = "_CheckedBy", DbType = "int")]
        public int? CheckedBy
        {
            get => this._CheckedBy;

            set
            {
                if (this._CheckedBy != value)
                {

                    this.OnCheckedByChanging(value);
                    this.SendPropertyChanging();
                    this._CheckedBy = value;
                    this.SendPropertyChanged("CheckedBy");
                    this.OnCheckedByChanged();
                }

            }

        }


        [Column(Name = "CHECKED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_CheckedDate", DbType = "datetime")]
        public DateTime? CheckedDate
        {
            get => this._CheckedDate;

            set
            {
                if (this._CheckedDate != value)
                {

                    this.OnCheckedDateChanging(value);
                    this.SendPropertyChanging();
                    this._CheckedDate = value;
                    this.SendPropertyChanged("CheckedDate");
                    this.OnCheckedDateChanged();
                }

            }

        }


        [Column(Name = "DATE_SATISFIED", UpdateCheck = UpdateCheck.Never, Storage = "_DateSatisfied", DbType = "datetime")]
        public DateTime? DateSatisfied
        {
            get => this._DateSatisfied;

            set
            {
                if (this._DateSatisfied != value)
                {

                    this.OnDateSatisfiedChanging(value);
                    this.SendPropertyChanging();
                    this._DateSatisfied = value;
                    this.SendPropertyChanged("DateSatisfied");
                    this.OnDateSatisfiedChanged();
                }

            }

        }


        [Column(Name = "CREATED_BY", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int")]
        public int? CreatedBy
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


        [Column(Name = "CREATED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime? CreatedDate
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


        [Column(Name = "RECORD_STATUS", UpdateCheck = UpdateCheck.Never, Storage = "_RecordStatus", DbType = "varchar(1)")]
        public string RecordStatus
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


        [Column(Name = "CHURCH_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ChurchId", DbType = "int")]
        public int? ChurchId
        {
            get => this._ChurchId;

            set
            {
                if (this._ChurchId != value)
                {

                    this.OnChurchIdChanging(value);
                    this.SendPropertyChanging();
                    this._ChurchId = value;
                    this.SendPropertyChanged("ChurchId");
                    this.OnChurchIdChanged();
                }

            }

        }


        [Column(Name = "STATUS", UpdateCheck = UpdateCheck.Never, Storage = "_Status", DbType = "int")]
        public int? Status
        {
            get => this._Status;

            set
            {
                if (this._Status != value)
                {

                    this.OnStatusChanging(value);
                    this.SendPropertyChanging();
                    this._Status = value;
                    this.SendPropertyChanged("Status");
                    this.OnStatusChanged();
                }

            }

        }


        [Column(Name = "COMMENTS", UpdateCheck = UpdateCheck.Never, Storage = "_Comments", DbType = "varchar(4000)")]
        public string Comments
        {
            get => this._Comments;

            set
            {
                if (this._Comments != value)
                {

                    this.OnCommentsChanging(value);
                    this.SendPropertyChanging();
                    this._Comments = value;
                    this.SendPropertyChanged("Comments");
                    this.OnCommentsChanged();
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

