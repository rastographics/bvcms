using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "CMS_VOLUNTEER.APPLICATION_TBL")]
    public partial class Application : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _ApplicationId;

        private int _PositionId;

        private int _PeopleId;

        private DateTime _SubmittedDate;

        private string _RecordStatus;

        private string _ApplicationStatusId;

        private string _SpiritualGift;

        private DateTime? _SignatureDate;

        private int? _ApplicationWitnessId;

        private DateTime? _WitnessDate;

        private string _UltimusIncidentId;

        private string _MinistryReviewStatusId;

        private DateTime? _MinistryReviewDate;

        private string _MinistryComments;

        private string _LayMinReviewStatusId;

        private DateTime? _LayMinReviewDate;

        private string _LayMinComments;

        private string _BibGuidReviewStatusId;

        private DateTime? _BibGuidReviewDate;

        private string _BibGuidComments;

        private string _OfcPastorReviewStatusId;

        private DateTime? _OfcPastorReviewDate;

        private string _OfcPastorComments;

        private int? _ChurchId;

        private int? _CreatedBy;

        private DateTime? _CreatedDate;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnApplicationIdChanging(int value);
        partial void OnApplicationIdChanged();

        partial void OnPositionIdChanging(int value);
        partial void OnPositionIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnSubmittedDateChanging(DateTime value);
        partial void OnSubmittedDateChanged();

        partial void OnRecordStatusChanging(string value);
        partial void OnRecordStatusChanged();

        partial void OnApplicationStatusIdChanging(string value);
        partial void OnApplicationStatusIdChanged();

        partial void OnSpiritualGiftChanging(string value);
        partial void OnSpiritualGiftChanged();

        partial void OnSignatureDateChanging(DateTime? value);
        partial void OnSignatureDateChanged();

        partial void OnApplicationWitnessIdChanging(int? value);
        partial void OnApplicationWitnessIdChanged();

        partial void OnWitnessDateChanging(DateTime? value);
        partial void OnWitnessDateChanged();

        partial void OnUltimusIncidentIdChanging(string value);
        partial void OnUltimusIncidentIdChanged();

        partial void OnMinistryReviewStatusIdChanging(string value);
        partial void OnMinistryReviewStatusIdChanged();

        partial void OnMinistryReviewDateChanging(DateTime? value);
        partial void OnMinistryReviewDateChanged();

        partial void OnMinistryCommentsChanging(string value);
        partial void OnMinistryCommentsChanged();

        partial void OnLayMinReviewStatusIdChanging(string value);
        partial void OnLayMinReviewStatusIdChanged();

        partial void OnLayMinReviewDateChanging(DateTime? value);
        partial void OnLayMinReviewDateChanged();

        partial void OnLayMinCommentsChanging(string value);
        partial void OnLayMinCommentsChanged();

        partial void OnBibGuidReviewStatusIdChanging(string value);
        partial void OnBibGuidReviewStatusIdChanged();

        partial void OnBibGuidReviewDateChanging(DateTime? value);
        partial void OnBibGuidReviewDateChanged();

        partial void OnBibGuidCommentsChanging(string value);
        partial void OnBibGuidCommentsChanged();

        partial void OnOfcPastorReviewStatusIdChanging(string value);
        partial void OnOfcPastorReviewStatusIdChanged();

        partial void OnOfcPastorReviewDateChanging(DateTime? value);
        partial void OnOfcPastorReviewDateChanged();

        partial void OnOfcPastorCommentsChanging(string value);
        partial void OnOfcPastorCommentsChanged();

        partial void OnChurchIdChanging(int? value);
        partial void OnChurchIdChanged();

        partial void OnCreatedByChanging(int? value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime? value);
        partial void OnCreatedDateChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        #endregion
        public Application()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "APPLICATION_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ApplicationId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ApplicationId
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


        [Column(Name = "POSITION_ID", UpdateCheck = UpdateCheck.Never, Storage = "_PositionId", DbType = "int NOT NULL")]
        public int PositionId
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


        [Column(Name = "PEOPLE_ID", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL")]
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


        [Column(Name = "SUBMITTED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_SubmittedDate", DbType = "datetime NOT NULL")]
        public DateTime SubmittedDate
        {
            get => this._SubmittedDate;

            set
            {
                if (this._SubmittedDate != value)
                {

                    this.OnSubmittedDateChanging(value);
                    this.SendPropertyChanging();
                    this._SubmittedDate = value;
                    this.SendPropertyChanged("SubmittedDate");
                    this.OnSubmittedDateChanged();
                }

            }

        }


        [Column(Name = "RECORD_STATUS", UpdateCheck = UpdateCheck.Never, Storage = "_RecordStatus", DbType = "varchar(1) NOT NULL")]
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


        [Column(Name = "APPLICATION_STATUS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ApplicationStatusId", DbType = "varchar(2) NOT NULL")]
        public string ApplicationStatusId
        {
            get => this._ApplicationStatusId;

            set
            {
                if (this._ApplicationStatusId != value)
                {

                    this.OnApplicationStatusIdChanging(value);
                    this.SendPropertyChanging();
                    this._ApplicationStatusId = value;
                    this.SendPropertyChanged("ApplicationStatusId");
                    this.OnApplicationStatusIdChanged();
                }

            }

        }


        [Column(Name = "SPIRITUAL_GIFT", UpdateCheck = UpdateCheck.Never, Storage = "_SpiritualGift", DbType = "varchar(50) NOT NULL")]
        public string SpiritualGift
        {
            get => this._SpiritualGift;

            set
            {
                if (this._SpiritualGift != value)
                {

                    this.OnSpiritualGiftChanging(value);
                    this.SendPropertyChanging();
                    this._SpiritualGift = value;
                    this.SendPropertyChanged("SpiritualGift");
                    this.OnSpiritualGiftChanged();
                }

            }

        }


        [Column(Name = "SIGNATURE_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_SignatureDate", DbType = "datetime")]
        public DateTime? SignatureDate
        {
            get => this._SignatureDate;

            set
            {
                if (this._SignatureDate != value)
                {

                    this.OnSignatureDateChanging(value);
                    this.SendPropertyChanging();
                    this._SignatureDate = value;
                    this.SendPropertyChanged("SignatureDate");
                    this.OnSignatureDateChanged();
                }

            }

        }


        [Column(Name = "APPLICATION_WITNESS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ApplicationWitnessId", DbType = "int")]
        public int? ApplicationWitnessId
        {
            get => this._ApplicationWitnessId;

            set
            {
                if (this._ApplicationWitnessId != value)
                {

                    this.OnApplicationWitnessIdChanging(value);
                    this.SendPropertyChanging();
                    this._ApplicationWitnessId = value;
                    this.SendPropertyChanged("ApplicationWitnessId");
                    this.OnApplicationWitnessIdChanged();
                }

            }

        }


        [Column(Name = "WITNESS_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_WitnessDate", DbType = "datetime")]
        public DateTime? WitnessDate
        {
            get => this._WitnessDate;

            set
            {
                if (this._WitnessDate != value)
                {

                    this.OnWitnessDateChanging(value);
                    this.SendPropertyChanging();
                    this._WitnessDate = value;
                    this.SendPropertyChanged("WitnessDate");
                    this.OnWitnessDateChanged();
                }

            }

        }


        [Column(Name = "ULTIMUS_INCIDENT_ID", UpdateCheck = UpdateCheck.Never, Storage = "_UltimusIncidentId", DbType = "varchar(50)")]
        public string UltimusIncidentId
        {
            get => this._UltimusIncidentId;

            set
            {
                if (this._UltimusIncidentId != value)
                {

                    this.OnUltimusIncidentIdChanging(value);
                    this.SendPropertyChanging();
                    this._UltimusIncidentId = value;
                    this.SendPropertyChanged("UltimusIncidentId");
                    this.OnUltimusIncidentIdChanged();
                }

            }

        }


        [Column(Name = "MINISTRY_REVIEW_STATUS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryReviewStatusId", DbType = "varchar(2)")]
        public string MinistryReviewStatusId
        {
            get => this._MinistryReviewStatusId;

            set
            {
                if (this._MinistryReviewStatusId != value)
                {

                    this.OnMinistryReviewStatusIdChanging(value);
                    this.SendPropertyChanging();
                    this._MinistryReviewStatusId = value;
                    this.SendPropertyChanged("MinistryReviewStatusId");
                    this.OnMinistryReviewStatusIdChanged();
                }

            }

        }


        [Column(Name = "MINISTRY_REVIEW_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryReviewDate", DbType = "datetime")]
        public DateTime? MinistryReviewDate
        {
            get => this._MinistryReviewDate;

            set
            {
                if (this._MinistryReviewDate != value)
                {

                    this.OnMinistryReviewDateChanging(value);
                    this.SendPropertyChanging();
                    this._MinistryReviewDate = value;
                    this.SendPropertyChanged("MinistryReviewDate");
                    this.OnMinistryReviewDateChanged();
                }

            }

        }


        [Column(Name = "MINISTRY_COMMENTS", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryComments", DbType = "varchar(256)")]
        public string MinistryComments
        {
            get => this._MinistryComments;

            set
            {
                if (this._MinistryComments != value)
                {

                    this.OnMinistryCommentsChanging(value);
                    this.SendPropertyChanging();
                    this._MinistryComments = value;
                    this.SendPropertyChanged("MinistryComments");
                    this.OnMinistryCommentsChanged();
                }

            }

        }


        [Column(Name = "LAY_MIN_REVIEW_STATUS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_LayMinReviewStatusId", DbType = "varchar(2)")]
        public string LayMinReviewStatusId
        {
            get => this._LayMinReviewStatusId;

            set
            {
                if (this._LayMinReviewStatusId != value)
                {

                    this.OnLayMinReviewStatusIdChanging(value);
                    this.SendPropertyChanging();
                    this._LayMinReviewStatusId = value;
                    this.SendPropertyChanged("LayMinReviewStatusId");
                    this.OnLayMinReviewStatusIdChanged();
                }

            }

        }


        [Column(Name = "LAY_MIN_REVIEW_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_LayMinReviewDate", DbType = "datetime")]
        public DateTime? LayMinReviewDate
        {
            get => this._LayMinReviewDate;

            set
            {
                if (this._LayMinReviewDate != value)
                {

                    this.OnLayMinReviewDateChanging(value);
                    this.SendPropertyChanging();
                    this._LayMinReviewDate = value;
                    this.SendPropertyChanged("LayMinReviewDate");
                    this.OnLayMinReviewDateChanged();
                }

            }

        }


        [Column(Name = "LAY_MIN_COMMENTS", UpdateCheck = UpdateCheck.Never, Storage = "_LayMinComments", DbType = "varchar(256)")]
        public string LayMinComments
        {
            get => this._LayMinComments;

            set
            {
                if (this._LayMinComments != value)
                {

                    this.OnLayMinCommentsChanging(value);
                    this.SendPropertyChanging();
                    this._LayMinComments = value;
                    this.SendPropertyChanged("LayMinComments");
                    this.OnLayMinCommentsChanged();
                }

            }

        }


        [Column(Name = "BIB_GUID_REVIEW_STATUS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_BibGuidReviewStatusId", DbType = "varchar(2)")]
        public string BibGuidReviewStatusId
        {
            get => this._BibGuidReviewStatusId;

            set
            {
                if (this._BibGuidReviewStatusId != value)
                {

                    this.OnBibGuidReviewStatusIdChanging(value);
                    this.SendPropertyChanging();
                    this._BibGuidReviewStatusId = value;
                    this.SendPropertyChanged("BibGuidReviewStatusId");
                    this.OnBibGuidReviewStatusIdChanged();
                }

            }

        }


        [Column(Name = "BIB_GUID_REVIEW_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_BibGuidReviewDate", DbType = "datetime")]
        public DateTime? BibGuidReviewDate
        {
            get => this._BibGuidReviewDate;

            set
            {
                if (this._BibGuidReviewDate != value)
                {

                    this.OnBibGuidReviewDateChanging(value);
                    this.SendPropertyChanging();
                    this._BibGuidReviewDate = value;
                    this.SendPropertyChanged("BibGuidReviewDate");
                    this.OnBibGuidReviewDateChanged();
                }

            }

        }


        [Column(Name = "BIB_GUID_COMMENTS", UpdateCheck = UpdateCheck.Never, Storage = "_BibGuidComments", DbType = "varchar(256)")]
        public string BibGuidComments
        {
            get => this._BibGuidComments;

            set
            {
                if (this._BibGuidComments != value)
                {

                    this.OnBibGuidCommentsChanging(value);
                    this.SendPropertyChanging();
                    this._BibGuidComments = value;
                    this.SendPropertyChanged("BibGuidComments");
                    this.OnBibGuidCommentsChanged();
                }

            }

        }


        [Column(Name = "OFC_PASTOR_REVIEW_STATUS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_OfcPastorReviewStatusId", DbType = "varchar(2)")]
        public string OfcPastorReviewStatusId
        {
            get => this._OfcPastorReviewStatusId;

            set
            {
                if (this._OfcPastorReviewStatusId != value)
                {

                    this.OnOfcPastorReviewStatusIdChanging(value);
                    this.SendPropertyChanging();
                    this._OfcPastorReviewStatusId = value;
                    this.SendPropertyChanged("OfcPastorReviewStatusId");
                    this.OnOfcPastorReviewStatusIdChanged();
                }

            }

        }


        [Column(Name = "OFC_PASTOR_REVIEW_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_OfcPastorReviewDate", DbType = "datetime")]
        public DateTime? OfcPastorReviewDate
        {
            get => this._OfcPastorReviewDate;

            set
            {
                if (this._OfcPastorReviewDate != value)
                {

                    this.OnOfcPastorReviewDateChanging(value);
                    this.SendPropertyChanging();
                    this._OfcPastorReviewDate = value;
                    this.SendPropertyChanged("OfcPastorReviewDate");
                    this.OnOfcPastorReviewDateChanged();
                }

            }

        }


        [Column(Name = "OFC_PASTOR_COMMENTS", UpdateCheck = UpdateCheck.Never, Storage = "_OfcPastorComments", DbType = "varchar(256)")]
        public string OfcPastorComments
        {
            get => this._OfcPastorComments;

            set
            {
                if (this._OfcPastorComments != value)
                {

                    this.OnOfcPastorCommentsChanging(value);
                    this.SendPropertyChanging();
                    this._OfcPastorComments = value;
                    this.SendPropertyChanged("OfcPastorComments");
                    this.OnOfcPastorCommentsChanged();
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

