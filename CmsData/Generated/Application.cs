using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "CMS_VOLUNTEER.APPLICATION_TBL")]
    public partial class Application : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            get => _ApplicationId;

            set
            {
                if (_ApplicationId != value)
                {
                    OnApplicationIdChanging(value);
                    SendPropertyChanging();
                    _ApplicationId = value;
                    SendPropertyChanged("ApplicationId");
                    OnApplicationIdChanged();
                }
            }
        }

        [Column(Name = "POSITION_ID", UpdateCheck = UpdateCheck.Never, Storage = "_PositionId", DbType = "int NOT NULL")]
        public int PositionId
        {
            get => _PositionId;

            set
            {
                if (_PositionId != value)
                {
                    OnPositionIdChanging(value);
                    SendPropertyChanging();
                    _PositionId = value;
                    SendPropertyChanged("PositionId");
                    OnPositionIdChanged();
                }
            }
        }

        [Column(Name = "PEOPLE_ID", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL")]
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

        [Column(Name = "SUBMITTED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_SubmittedDate", DbType = "datetime NOT NULL")]
        public DateTime SubmittedDate
        {
            get => _SubmittedDate;

            set
            {
                if (_SubmittedDate != value)
                {
                    OnSubmittedDateChanging(value);
                    SendPropertyChanging();
                    _SubmittedDate = value;
                    SendPropertyChanged("SubmittedDate");
                    OnSubmittedDateChanged();
                }
            }
        }

        [Column(Name = "RECORD_STATUS", UpdateCheck = UpdateCheck.Never, Storage = "_RecordStatus", DbType = "varchar(1) NOT NULL")]
        public string RecordStatus
        {
            get => _RecordStatus;

            set
            {
                if (_RecordStatus != value)
                {
                    OnRecordStatusChanging(value);
                    SendPropertyChanging();
                    _RecordStatus = value;
                    SendPropertyChanged("RecordStatus");
                    OnRecordStatusChanged();
                }
            }
        }

        [Column(Name = "APPLICATION_STATUS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ApplicationStatusId", DbType = "varchar(2) NOT NULL")]
        public string ApplicationStatusId
        {
            get => _ApplicationStatusId;

            set
            {
                if (_ApplicationStatusId != value)
                {
                    OnApplicationStatusIdChanging(value);
                    SendPropertyChanging();
                    _ApplicationStatusId = value;
                    SendPropertyChanged("ApplicationStatusId");
                    OnApplicationStatusIdChanged();
                }
            }
        }

        [Column(Name = "SPIRITUAL_GIFT", UpdateCheck = UpdateCheck.Never, Storage = "_SpiritualGift", DbType = "varchar(50) NOT NULL")]
        public string SpiritualGift
        {
            get => _SpiritualGift;

            set
            {
                if (_SpiritualGift != value)
                {
                    OnSpiritualGiftChanging(value);
                    SendPropertyChanging();
                    _SpiritualGift = value;
                    SendPropertyChanged("SpiritualGift");
                    OnSpiritualGiftChanged();
                }
            }
        }

        [Column(Name = "SIGNATURE_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_SignatureDate", DbType = "datetime")]
        public DateTime? SignatureDate
        {
            get => _SignatureDate;

            set
            {
                if (_SignatureDate != value)
                {
                    OnSignatureDateChanging(value);
                    SendPropertyChanging();
                    _SignatureDate = value;
                    SendPropertyChanged("SignatureDate");
                    OnSignatureDateChanged();
                }
            }
        }

        [Column(Name = "APPLICATION_WITNESS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ApplicationWitnessId", DbType = "int")]
        public int? ApplicationWitnessId
        {
            get => _ApplicationWitnessId;

            set
            {
                if (_ApplicationWitnessId != value)
                {
                    OnApplicationWitnessIdChanging(value);
                    SendPropertyChanging();
                    _ApplicationWitnessId = value;
                    SendPropertyChanged("ApplicationWitnessId");
                    OnApplicationWitnessIdChanged();
                }
            }
        }

        [Column(Name = "WITNESS_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_WitnessDate", DbType = "datetime")]
        public DateTime? WitnessDate
        {
            get => _WitnessDate;

            set
            {
                if (_WitnessDate != value)
                {
                    OnWitnessDateChanging(value);
                    SendPropertyChanging();
                    _WitnessDate = value;
                    SendPropertyChanged("WitnessDate");
                    OnWitnessDateChanged();
                }
            }
        }

        [Column(Name = "ULTIMUS_INCIDENT_ID", UpdateCheck = UpdateCheck.Never, Storage = "_UltimusIncidentId", DbType = "varchar(50)")]
        public string UltimusIncidentId
        {
            get => _UltimusIncidentId;

            set
            {
                if (_UltimusIncidentId != value)
                {
                    OnUltimusIncidentIdChanging(value);
                    SendPropertyChanging();
                    _UltimusIncidentId = value;
                    SendPropertyChanged("UltimusIncidentId");
                    OnUltimusIncidentIdChanged();
                }
            }
        }

        [Column(Name = "MINISTRY_REVIEW_STATUS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryReviewStatusId", DbType = "varchar(2)")]
        public string MinistryReviewStatusId
        {
            get => _MinistryReviewStatusId;

            set
            {
                if (_MinistryReviewStatusId != value)
                {
                    OnMinistryReviewStatusIdChanging(value);
                    SendPropertyChanging();
                    _MinistryReviewStatusId = value;
                    SendPropertyChanged("MinistryReviewStatusId");
                    OnMinistryReviewStatusIdChanged();
                }
            }
        }

        [Column(Name = "MINISTRY_REVIEW_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryReviewDate", DbType = "datetime")]
        public DateTime? MinistryReviewDate
        {
            get => _MinistryReviewDate;

            set
            {
                if (_MinistryReviewDate != value)
                {
                    OnMinistryReviewDateChanging(value);
                    SendPropertyChanging();
                    _MinistryReviewDate = value;
                    SendPropertyChanged("MinistryReviewDate");
                    OnMinistryReviewDateChanged();
                }
            }
        }

        [Column(Name = "MINISTRY_COMMENTS", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryComments", DbType = "varchar(256)")]
        public string MinistryComments
        {
            get => _MinistryComments;

            set
            {
                if (_MinistryComments != value)
                {
                    OnMinistryCommentsChanging(value);
                    SendPropertyChanging();
                    _MinistryComments = value;
                    SendPropertyChanged("MinistryComments");
                    OnMinistryCommentsChanged();
                }
            }
        }

        [Column(Name = "LAY_MIN_REVIEW_STATUS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_LayMinReviewStatusId", DbType = "varchar(2)")]
        public string LayMinReviewStatusId
        {
            get => _LayMinReviewStatusId;

            set
            {
                if (_LayMinReviewStatusId != value)
                {
                    OnLayMinReviewStatusIdChanging(value);
                    SendPropertyChanging();
                    _LayMinReviewStatusId = value;
                    SendPropertyChanged("LayMinReviewStatusId");
                    OnLayMinReviewStatusIdChanged();
                }
            }
        }

        [Column(Name = "LAY_MIN_REVIEW_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_LayMinReviewDate", DbType = "datetime")]
        public DateTime? LayMinReviewDate
        {
            get => _LayMinReviewDate;

            set
            {
                if (_LayMinReviewDate != value)
                {
                    OnLayMinReviewDateChanging(value);
                    SendPropertyChanging();
                    _LayMinReviewDate = value;
                    SendPropertyChanged("LayMinReviewDate");
                    OnLayMinReviewDateChanged();
                }
            }
        }

        [Column(Name = "LAY_MIN_COMMENTS", UpdateCheck = UpdateCheck.Never, Storage = "_LayMinComments", DbType = "varchar(256)")]
        public string LayMinComments
        {
            get => _LayMinComments;

            set
            {
                if (_LayMinComments != value)
                {
                    OnLayMinCommentsChanging(value);
                    SendPropertyChanging();
                    _LayMinComments = value;
                    SendPropertyChanged("LayMinComments");
                    OnLayMinCommentsChanged();
                }
            }
        }

        [Column(Name = "BIB_GUID_REVIEW_STATUS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_BibGuidReviewStatusId", DbType = "varchar(2)")]
        public string BibGuidReviewStatusId
        {
            get => _BibGuidReviewStatusId;

            set
            {
                if (_BibGuidReviewStatusId != value)
                {
                    OnBibGuidReviewStatusIdChanging(value);
                    SendPropertyChanging();
                    _BibGuidReviewStatusId = value;
                    SendPropertyChanged("BibGuidReviewStatusId");
                    OnBibGuidReviewStatusIdChanged();
                }
            }
        }

        [Column(Name = "BIB_GUID_REVIEW_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_BibGuidReviewDate", DbType = "datetime")]
        public DateTime? BibGuidReviewDate
        {
            get => _BibGuidReviewDate;

            set
            {
                if (_BibGuidReviewDate != value)
                {
                    OnBibGuidReviewDateChanging(value);
                    SendPropertyChanging();
                    _BibGuidReviewDate = value;
                    SendPropertyChanged("BibGuidReviewDate");
                    OnBibGuidReviewDateChanged();
                }
            }
        }

        [Column(Name = "BIB_GUID_COMMENTS", UpdateCheck = UpdateCheck.Never, Storage = "_BibGuidComments", DbType = "varchar(256)")]
        public string BibGuidComments
        {
            get => _BibGuidComments;

            set
            {
                if (_BibGuidComments != value)
                {
                    OnBibGuidCommentsChanging(value);
                    SendPropertyChanging();
                    _BibGuidComments = value;
                    SendPropertyChanged("BibGuidComments");
                    OnBibGuidCommentsChanged();
                }
            }
        }

        [Column(Name = "OFC_PASTOR_REVIEW_STATUS_ID", UpdateCheck = UpdateCheck.Never, Storage = "_OfcPastorReviewStatusId", DbType = "varchar(2)")]
        public string OfcPastorReviewStatusId
        {
            get => _OfcPastorReviewStatusId;

            set
            {
                if (_OfcPastorReviewStatusId != value)
                {
                    OnOfcPastorReviewStatusIdChanging(value);
                    SendPropertyChanging();
                    _OfcPastorReviewStatusId = value;
                    SendPropertyChanged("OfcPastorReviewStatusId");
                    OnOfcPastorReviewStatusIdChanged();
                }
            }
        }

        [Column(Name = "OFC_PASTOR_REVIEW_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_OfcPastorReviewDate", DbType = "datetime")]
        public DateTime? OfcPastorReviewDate
        {
            get => _OfcPastorReviewDate;

            set
            {
                if (_OfcPastorReviewDate != value)
                {
                    OnOfcPastorReviewDateChanging(value);
                    SendPropertyChanging();
                    _OfcPastorReviewDate = value;
                    SendPropertyChanged("OfcPastorReviewDate");
                    OnOfcPastorReviewDateChanged();
                }
            }
        }

        [Column(Name = "OFC_PASTOR_COMMENTS", UpdateCheck = UpdateCheck.Never, Storage = "_OfcPastorComments", DbType = "varchar(256)")]
        public string OfcPastorComments
        {
            get => _OfcPastorComments;

            set
            {
                if (_OfcPastorComments != value)
                {
                    OnOfcPastorCommentsChanging(value);
                    SendPropertyChanging();
                    _OfcPastorComments = value;
                    SendPropertyChanged("OfcPastorComments");
                    OnOfcPastorCommentsChanged();
                }
            }
        }

        [Column(Name = "CHURCH_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ChurchId", DbType = "int")]
        public int? ChurchId
        {
            get => _ChurchId;

            set
            {
                if (_ChurchId != value)
                {
                    OnChurchIdChanging(value);
                    SendPropertyChanging();
                    _ChurchId = value;
                    SendPropertyChanged("ChurchId");
                    OnChurchIdChanged();
                }
            }
        }

        [Column(Name = "CREATED_BY", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int")]
        public int? CreatedBy
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

        [Column(Name = "CREATED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime? CreatedDate
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

        [Column(Name = "MODIFIED_BY", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedBy", DbType = "int")]
        public int? ModifiedBy
        {
            get => _ModifiedBy;

            set
            {
                if (_ModifiedBy != value)
                {
                    OnModifiedByChanging(value);
                    SendPropertyChanging();
                    _ModifiedBy = value;
                    SendPropertyChanged("ModifiedBy");
                    OnModifiedByChanged();
                }
            }
        }

        [Column(Name = "MODIFIED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedDate", DbType = "datetime")]
        public DateTime? ModifiedDate
        {
            get => _ModifiedDate;

            set
            {
                if (_ModifiedDate != value)
                {
                    OnModifiedDateChanging(value);
                    SendPropertyChanging();
                    _ModifiedDate = value;
                    SendPropertyChanged("ModifiedDate");
                    OnModifiedDateChanged();
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
