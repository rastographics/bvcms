using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "CMS_VOLUNTEER.INTEREST_TBL")]
    public partial class Interest : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _InterestId;

        private int? _PositionId;

        private int? _InterestPeopleId;

        private int? _CreatedBy;

        private DateTime? _CreatedDate;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private string _RecordStatus;

        private int? _ChurchId;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnInterestIdChanging(int value);
        partial void OnInterestIdChanged();

        partial void OnPositionIdChanging(int? value);
        partial void OnPositionIdChanged();

        partial void OnInterestPeopleIdChanging(int? value);
        partial void OnInterestPeopleIdChanged();

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

        #endregion
        public Interest()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "INTEREST_ID", UpdateCheck = UpdateCheck.Never, Storage = "_InterestId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int InterestId
        {
            get => this._InterestId;

            set
            {
                if (this._InterestId != value)
                {

                    this.OnInterestIdChanging(value);
                    this.SendPropertyChanging();
                    this._InterestId = value;
                    this.SendPropertyChanged("InterestId");
                    this.OnInterestIdChanged();
                }

            }

        }


        [Column(Name = "POSITION_ID", UpdateCheck = UpdateCheck.Never, Storage = "_PositionId", DbType = "int")]
        public int? PositionId
        {
            get => this._PositionId;

            set
            {
                if (this._PositionId != value)
                {

                    this.OnPositionIdChanging(value);
                    this.SendPropertyChanging();
                    this._PositionId = value;
                    this.SendPropertyChanged("PositionId");
                    this.OnPositionIdChanged();
                }

            }

        }


        [Column(Name = "INTEREST_PEOPLE_ID", UpdateCheck = UpdateCheck.Never, Storage = "_InterestPeopleId", DbType = "int")]
        public int? InterestPeopleId
        {
            get => this._InterestPeopleId;

            set
            {
                if (this._InterestPeopleId != value)
                {

                    this.OnInterestPeopleIdChanging(value);
                    this.SendPropertyChanging();
                    this._InterestPeopleId = value;
                    this.SendPropertyChanged("InterestPeopleId");
                    this.OnInterestPeopleIdChanged();
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

