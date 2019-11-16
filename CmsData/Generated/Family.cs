using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Families")]
    public partial class Family : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _FamilyId;

        private int _CreatedBy;

        private DateTime? _CreatedDate;

        private bool _RecordStatus;

        private bool? _BadAddressFlag;

        private bool? _AltBadAddressFlag;

        private int? _ResCodeId;

        private int? _AltResCodeId;

        private DateTime? _AddressFromDate;

        private DateTime? _AddressToDate;

        private string _AddressLineOne;

        private string _AddressLineTwo;

        private string _CityName;

        private string _StateCode;

        private string _ZipCode;

        private string _CountryName;

        private string _StreetName;

        private string _HomePhone;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private int? _HeadOfHouseholdId;

        private int? _HeadOfHouseholdSpouseId;

        private int? _CoupleFlag;

        private string _HomePhoneLU;

        private string _HomePhoneAC;

        private string _Comments;

        private int? _PictureId;

        private EntitySet<FamilyCheckinLock> _FamilyCheckinLocks;

        private EntitySet<FamilyExtra> _FamilyExtras;

        private EntitySet<Person> _People;

        private EntitySet<RelatedFamily> _RelatedFamilies1;

        private EntitySet<RelatedFamily> _RelatedFamilies2;

        private EntityRef<Person> _HeadOfHousehold;

        private EntityRef<Person> _HeadOfHouseholdSpouse;

        private EntityRef<Picture> _Picture;

        private EntityRef<ResidentCode> _ResidentCode;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnFamilyIdChanging(int value);
        partial void OnFamilyIdChanged();

        partial void OnCreatedByChanging(int value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime? value);
        partial void OnCreatedDateChanged();

        partial void OnRecordStatusChanging(bool value);
        partial void OnRecordStatusChanged();

        partial void OnBadAddressFlagChanging(bool? value);
        partial void OnBadAddressFlagChanged();

        partial void OnAltBadAddressFlagChanging(bool? value);
        partial void OnAltBadAddressFlagChanged();

        partial void OnResCodeIdChanging(int? value);
        partial void OnResCodeIdChanged();

        partial void OnAltResCodeIdChanging(int? value);
        partial void OnAltResCodeIdChanged();

        partial void OnAddressFromDateChanging(DateTime? value);
        partial void OnAddressFromDateChanged();

        partial void OnAddressToDateChanging(DateTime? value);
        partial void OnAddressToDateChanged();

        partial void OnAddressLineOneChanging(string value);
        partial void OnAddressLineOneChanged();

        partial void OnAddressLineTwoChanging(string value);
        partial void OnAddressLineTwoChanged();

        partial void OnCityNameChanging(string value);
        partial void OnCityNameChanged();

        partial void OnStateCodeChanging(string value);
        partial void OnStateCodeChanged();

        partial void OnZipCodeChanging(string value);
        partial void OnZipCodeChanged();

        partial void OnCountryNameChanging(string value);
        partial void OnCountryNameChanged();

        partial void OnStreetNameChanging(string value);
        partial void OnStreetNameChanged();

        partial void OnHomePhoneChanging(string value);
        partial void OnHomePhoneChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        partial void OnHeadOfHouseholdIdChanging(int? value);
        partial void OnHeadOfHouseholdIdChanged();

        partial void OnHeadOfHouseholdSpouseIdChanging(int? value);
        partial void OnHeadOfHouseholdSpouseIdChanged();

        partial void OnCoupleFlagChanging(int? value);
        partial void OnCoupleFlagChanged();

        partial void OnHomePhoneLUChanging(string value);
        partial void OnHomePhoneLUChanged();

        partial void OnHomePhoneACChanging(string value);
        partial void OnHomePhoneACChanged();

        partial void OnCommentsChanging(string value);
        partial void OnCommentsChanged();

        partial void OnPictureIdChanging(int? value);
        partial void OnPictureIdChanged();

        #endregion

        public Family()
        {
            _FamilyCheckinLocks = new EntitySet<FamilyCheckinLock>(new Action<FamilyCheckinLock>(attach_FamilyCheckinLocks), new Action<FamilyCheckinLock>(detach_FamilyCheckinLocks));

            _FamilyExtras = new EntitySet<FamilyExtra>(new Action<FamilyExtra>(attach_FamilyExtras), new Action<FamilyExtra>(detach_FamilyExtras));

            _People = new EntitySet<Person>(new Action<Person>(attach_People), new Action<Person>(detach_People));

            _RelatedFamilies1 = new EntitySet<RelatedFamily>(new Action<RelatedFamily>(attach_RelatedFamilies1), new Action<RelatedFamily>(detach_RelatedFamilies1));

            _RelatedFamilies2 = new EntitySet<RelatedFamily>(new Action<RelatedFamily>(attach_RelatedFamilies2), new Action<RelatedFamily>(detach_RelatedFamilies2));

            _HeadOfHousehold = default(EntityRef<Person>);

            _HeadOfHouseholdSpouse = default(EntityRef<Person>);

            _Picture = default(EntityRef<Picture>);

            _ResidentCode = default(EntityRef<ResidentCode>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "FamilyId", UpdateCheck = UpdateCheck.Never, Storage = "_FamilyId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    OnFamilyIdChanging(value);
                    SendPropertyChanging();
                    _FamilyId = value;
                    SendPropertyChanged("FamilyId");
                    OnFamilyIdChanged();
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

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime")]
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

        [Column(Name = "RecordStatus", UpdateCheck = UpdateCheck.Never, Storage = "_RecordStatus", DbType = "bit NOT NULL")]
        public bool RecordStatus
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

        [Column(Name = "BadAddressFlag", UpdateCheck = UpdateCheck.Never, Storage = "_BadAddressFlag", DbType = "bit")]
        public bool? BadAddressFlag
        {
            get => _BadAddressFlag;

            set
            {
                if (_BadAddressFlag != value)
                {
                    OnBadAddressFlagChanging(value);
                    SendPropertyChanging();
                    _BadAddressFlag = value;
                    SendPropertyChanged("BadAddressFlag");
                    OnBadAddressFlagChanged();
                }
            }
        }

        [Column(Name = "AltBadAddressFlag", UpdateCheck = UpdateCheck.Never, Storage = "_AltBadAddressFlag", DbType = "bit")]
        public bool? AltBadAddressFlag
        {
            get => _AltBadAddressFlag;

            set
            {
                if (_AltBadAddressFlag != value)
                {
                    OnAltBadAddressFlagChanging(value);
                    SendPropertyChanging();
                    _AltBadAddressFlag = value;
                    SendPropertyChanged("AltBadAddressFlag");
                    OnAltBadAddressFlagChanged();
                }
            }
        }

        [Column(Name = "ResCodeId", UpdateCheck = UpdateCheck.Never, Storage = "_ResCodeId", DbType = "int")]
        [IsForeignKey]
        public int? ResCodeId
        {
            get => _ResCodeId;

            set
            {
                if (_ResCodeId != value)
                {
                    if (_ResidentCode.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnResCodeIdChanging(value);
                    SendPropertyChanging();
                    _ResCodeId = value;
                    SendPropertyChanged("ResCodeId");
                    OnResCodeIdChanged();
                }
            }
        }

        [Column(Name = "AltResCodeId", UpdateCheck = UpdateCheck.Never, Storage = "_AltResCodeId", DbType = "int")]
        public int? AltResCodeId
        {
            get => _AltResCodeId;

            set
            {
                if (_AltResCodeId != value)
                {
                    OnAltResCodeIdChanging(value);
                    SendPropertyChanging();
                    _AltResCodeId = value;
                    SendPropertyChanged("AltResCodeId");
                    OnAltResCodeIdChanged();
                }
            }
        }

        [Column(Name = "AddressFromDate", UpdateCheck = UpdateCheck.Never, Storage = "_AddressFromDate", DbType = "datetime")]
        public DateTime? AddressFromDate
        {
            get => _AddressFromDate;

            set
            {
                if (_AddressFromDate != value)
                {
                    OnAddressFromDateChanging(value);
                    SendPropertyChanging();
                    _AddressFromDate = value;
                    SendPropertyChanged("AddressFromDate");
                    OnAddressFromDateChanged();
                }
            }
        }

        [Column(Name = "AddressToDate", UpdateCheck = UpdateCheck.Never, Storage = "_AddressToDate", DbType = "datetime")]
        public DateTime? AddressToDate
        {
            get => _AddressToDate;

            set
            {
                if (_AddressToDate != value)
                {
                    OnAddressToDateChanging(value);
                    SendPropertyChanging();
                    _AddressToDate = value;
                    SendPropertyChanged("AddressToDate");
                    OnAddressToDateChanged();
                }
            }
        }

        [Column(Name = "AddressLineOne", UpdateCheck = UpdateCheck.Never, Storage = "_AddressLineOne", DbType = "nvarchar(100)")]
        public string AddressLineOne
        {
            get => _AddressLineOne;

            set
            {
                if (_AddressLineOne != value)
                {
                    OnAddressLineOneChanging(value);
                    SendPropertyChanging();
                    _AddressLineOne = value;
                    SendPropertyChanged("AddressLineOne");
                    OnAddressLineOneChanged();
                }
            }
        }

        [Column(Name = "AddressLineTwo", UpdateCheck = UpdateCheck.Never, Storage = "_AddressLineTwo", DbType = "nvarchar(100)")]
        public string AddressLineTwo
        {
            get => _AddressLineTwo;

            set
            {
                if (_AddressLineTwo != value)
                {
                    OnAddressLineTwoChanging(value);
                    SendPropertyChanging();
                    _AddressLineTwo = value;
                    SendPropertyChanged("AddressLineTwo");
                    OnAddressLineTwoChanged();
                }
            }
        }

        [Column(Name = "CityName", UpdateCheck = UpdateCheck.Never, Storage = "_CityName", DbType = "nvarchar(30)")]
        public string CityName
        {
            get => _CityName;

            set
            {
                if (_CityName != value)
                {
                    OnCityNameChanging(value);
                    SendPropertyChanging();
                    _CityName = value;
                    SendPropertyChanged("CityName");
                    OnCityNameChanged();
                }
            }
        }

        [Column(Name = "StateCode", UpdateCheck = UpdateCheck.Never, Storage = "_StateCode", DbType = "nvarchar(30)")]
        public string StateCode
        {
            get => _StateCode;

            set
            {
                if (_StateCode != value)
                {
                    OnStateCodeChanging(value);
                    SendPropertyChanging();
                    _StateCode = value;
                    SendPropertyChanged("StateCode");
                    OnStateCodeChanged();
                }
            }
        }

        [Column(Name = "ZipCode", UpdateCheck = UpdateCheck.Never, Storage = "_ZipCode", DbType = "nvarchar(15)")]
        public string ZipCode
        {
            get => _ZipCode;

            set
            {
                if (_ZipCode != value)
                {
                    OnZipCodeChanging(value);
                    SendPropertyChanging();
                    _ZipCode = value;
                    SendPropertyChanged("ZipCode");
                    OnZipCodeChanged();
                }
            }
        }

        [Column(Name = "CountryName", UpdateCheck = UpdateCheck.Never, Storage = "_CountryName", DbType = "nvarchar(40)")]
        public string CountryName
        {
            get => _CountryName;

            set
            {
                if (_CountryName != value)
                {
                    OnCountryNameChanging(value);
                    SendPropertyChanging();
                    _CountryName = value;
                    SendPropertyChanged("CountryName");
                    OnCountryNameChanged();
                }
            }
        }

        [Column(Name = "StreetName", UpdateCheck = UpdateCheck.Never, Storage = "_StreetName", DbType = "nvarchar(40)")]
        public string StreetName
        {
            get => _StreetName;

            set
            {
                if (_StreetName != value)
                {
                    OnStreetNameChanging(value);
                    SendPropertyChanging();
                    _StreetName = value;
                    SendPropertyChanged("StreetName");
                    OnStreetNameChanged();
                }
            }
        }

        [Column(Name = "HomePhone", UpdateCheck = UpdateCheck.Never, Storage = "_HomePhone", DbType = "nvarchar(20)")]
        public string HomePhone
        {
            get => _HomePhone;

            set
            {
                if (_HomePhone != value)
                {
                    OnHomePhoneChanging(value);
                    SendPropertyChanging();
                    _HomePhone = value;
                    SendPropertyChanged("HomePhone");
                    OnHomePhoneChanged();
                }
            }
        }

        [Column(Name = "ModifiedBy", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedBy", DbType = "int")]
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

        [Column(Name = "ModifiedDate", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedDate", DbType = "datetime")]
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

        [Column(Name = "HeadOfHouseholdId", UpdateCheck = UpdateCheck.Never, Storage = "_HeadOfHouseholdId", DbType = "int")]
        [IsForeignKey]
        public int? HeadOfHouseholdId
        {
            get => _HeadOfHouseholdId;

            set
            {
                if (_HeadOfHouseholdId != value)
                {
                    if (_HeadOfHousehold.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnHeadOfHouseholdIdChanging(value);
                    SendPropertyChanging();
                    _HeadOfHouseholdId = value;
                    SendPropertyChanged("HeadOfHouseholdId");
                    OnHeadOfHouseholdIdChanged();
                }
            }
        }

        [Column(Name = "HeadOfHouseholdSpouseId", UpdateCheck = UpdateCheck.Never, Storage = "_HeadOfHouseholdSpouseId", DbType = "int")]
        [IsForeignKey]
        public int? HeadOfHouseholdSpouseId
        {
            get => _HeadOfHouseholdSpouseId;

            set
            {
                if (_HeadOfHouseholdSpouseId != value)
                {
                    if (_HeadOfHouseholdSpouse.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnHeadOfHouseholdSpouseIdChanging(value);
                    SendPropertyChanging();
                    _HeadOfHouseholdSpouseId = value;
                    SendPropertyChanged("HeadOfHouseholdSpouseId");
                    OnHeadOfHouseholdSpouseIdChanged();
                }
            }
        }

        [Column(Name = "CoupleFlag", UpdateCheck = UpdateCheck.Never, Storage = "_CoupleFlag", DbType = "int")]
        public int? CoupleFlag
        {
            get => _CoupleFlag;

            set
            {
                if (_CoupleFlag != value)
                {
                    OnCoupleFlagChanging(value);
                    SendPropertyChanging();
                    _CoupleFlag = value;
                    SendPropertyChanged("CoupleFlag");
                    OnCoupleFlagChanged();
                }
            }
        }

        [Column(Name = "HomePhoneLU", UpdateCheck = UpdateCheck.Never, Storage = "_HomePhoneLU", DbType = "char(7)")]
        public string HomePhoneLU
        {
            get => _HomePhoneLU;

            set
            {
                if (_HomePhoneLU != value)
                {
                    OnHomePhoneLUChanging(value);
                    SendPropertyChanging();
                    _HomePhoneLU = value;
                    SendPropertyChanged("HomePhoneLU");
                    OnHomePhoneLUChanged();
                }
            }
        }

        [Column(Name = "HomePhoneAC", UpdateCheck = UpdateCheck.Never, Storage = "_HomePhoneAC", DbType = "char(3)")]
        public string HomePhoneAC
        {
            get => _HomePhoneAC;

            set
            {
                if (_HomePhoneAC != value)
                {
                    OnHomePhoneACChanging(value);
                    SendPropertyChanging();
                    _HomePhoneAC = value;
                    SendPropertyChanged("HomePhoneAC");
                    OnHomePhoneACChanged();
                }
            }
        }

        [Column(Name = "Comments", UpdateCheck = UpdateCheck.Never, Storage = "_Comments", DbType = "nvarchar(3000)")]
        public string Comments
        {
            get => _Comments;

            set
            {
                if (_Comments != value)
                {
                    OnCommentsChanging(value);
                    SendPropertyChanging();
                    _Comments = value;
                    SendPropertyChanged("Comments");
                    OnCommentsChanged();
                }
            }
        }

        [Column(Name = "PictureId", UpdateCheck = UpdateCheck.Never, Storage = "_PictureId", DbType = "int")]
        [IsForeignKey]
        public int? PictureId
        {
            get => _PictureId;

            set
            {
                if (_PictureId != value)
                {
                    if (_Picture.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPictureIdChanging(value);
                    SendPropertyChanging();
                    _PictureId = value;
                    SendPropertyChanged("PictureId");
                    OnPictureIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_FamilyCheckinLock_FamilyCheckinLock1", Storage = "_FamilyCheckinLocks", OtherKey = "FamilyId")]
        public EntitySet<FamilyCheckinLock> FamilyCheckinLocks
           {
               get => _FamilyCheckinLocks;

            set => _FamilyCheckinLocks.Assign(value);

           }

        [Association(Name = "FK_FamilyExtra_Family", Storage = "_FamilyExtras", OtherKey = "FamilyId")]
        public EntitySet<FamilyExtra> FamilyExtras
           {
               get => _FamilyExtras;

            set => _FamilyExtras.Assign(value);

           }

        [Association(Name = "FK_People_Families", Storage = "_People", OtherKey = "FamilyId")]
        public EntitySet<Person> People
           {
               get => _People;

            set => _People.Assign(value);

           }

        [Association(Name = "RelatedFamilies1__RelatedFamily1", Storage = "_RelatedFamilies1", OtherKey = "FamilyId")]
        public EntitySet<RelatedFamily> RelatedFamilies1
           {
               get => _RelatedFamilies1;

            set => _RelatedFamilies1.Assign(value);

           }

        [Association(Name = "RelatedFamilies2__RelatedFamily2", Storage = "_RelatedFamilies2", OtherKey = "RelatedFamilyId")]
        public EntitySet<RelatedFamily> RelatedFamilies2
           {
               get => _RelatedFamilies2;

            set => _RelatedFamilies2.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FamiliesHeaded__HeadOfHousehold", Storage = "_HeadOfHousehold", ThisKey = "HeadOfHouseholdId", IsForeignKey = true)]
        public Person HeadOfHousehold
        {
            get => _HeadOfHousehold.Entity;

            set
            {
                Person previousValue = _HeadOfHousehold.Entity;
                if (((previousValue != value)
                            || (_HeadOfHousehold.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _HeadOfHousehold.Entity = null;
                        previousValue.FamiliesHeaded.Remove(this);
                    }

                    _HeadOfHousehold.Entity = value;
                    if (value != null)
                    {
                        value.FamiliesHeaded.Add(this);

                        _HeadOfHouseholdId = value.PeopleId;

                    }

                    else
                    {
                        _HeadOfHouseholdId = default(int?);

                    }

                    SendPropertyChanged("HeadOfHousehold");
                }
            }
        }

        [Association(Name = "FamiliesHeaded2__HeadOfHouseholdSpouse", Storage = "_HeadOfHouseholdSpouse", ThisKey = "HeadOfHouseholdSpouseId", IsForeignKey = true)]
        public Person HeadOfHouseholdSpouse
        {
            get => _HeadOfHouseholdSpouse.Entity;

            set
            {
                Person previousValue = _HeadOfHouseholdSpouse.Entity;
                if (((previousValue != value)
                            || (_HeadOfHouseholdSpouse.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _HeadOfHouseholdSpouse.Entity = null;
                        previousValue.FamiliesHeaded2.Remove(this);
                    }

                    _HeadOfHouseholdSpouse.Entity = value;
                    if (value != null)
                    {
                        value.FamiliesHeaded2.Add(this);

                        _HeadOfHouseholdSpouseId = value.PeopleId;

                    }

                    else
                    {
                        _HeadOfHouseholdSpouseId = default(int?);

                    }

                    SendPropertyChanged("HeadOfHouseholdSpouse");
                }
            }
        }

        [Association(Name = "FK_Families_Picture", Storage = "_Picture", ThisKey = "PictureId", IsForeignKey = true)]
        public Picture Picture
        {
            get => _Picture.Entity;

            set
            {
                Picture previousValue = _Picture.Entity;
                if (((previousValue != value)
                            || (_Picture.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Picture.Entity = null;
                        previousValue.Families.Remove(this);
                    }

                    _Picture.Entity = value;
                    if (value != null)
                    {
                        value.Families.Add(this);

                        _PictureId = value.PictureId;

                    }

                    else
                    {
                        _PictureId = default(int?);

                    }

                    SendPropertyChanged("Picture");
                }
            }
        }

        [Association(Name = "ResCodeFamilies__ResidentCode", Storage = "_ResidentCode", ThisKey = "ResCodeId", IsForeignKey = true)]
        public ResidentCode ResidentCode
        {
            get => _ResidentCode.Entity;

            set
            {
                ResidentCode previousValue = _ResidentCode.Entity;
                if (((previousValue != value)
                            || (_ResidentCode.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ResidentCode.Entity = null;
                        previousValue.ResCodeFamilies.Remove(this);
                    }

                    _ResidentCode.Entity = value;
                    if (value != null)
                    {
                        value.ResCodeFamilies.Add(this);

                        _ResCodeId = value.Id;

                    }

                    else
                    {
                        _ResCodeId = default(int?);

                    }

                    SendPropertyChanged("ResidentCode");
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

        private void attach_FamilyCheckinLocks(FamilyCheckinLock entity)
        {
            SendPropertyChanging();
            entity.Family = this;
        }

        private void detach_FamilyCheckinLocks(FamilyCheckinLock entity)
        {
            SendPropertyChanging();
            entity.Family = null;
        }

        private void attach_FamilyExtras(FamilyExtra entity)
        {
            SendPropertyChanging();
            entity.Family = this;
        }

        private void detach_FamilyExtras(FamilyExtra entity)
        {
            SendPropertyChanging();
            entity.Family = null;
        }

        private void attach_People(Person entity)
        {
            SendPropertyChanging();
            entity.Family = this;
        }

        private void detach_People(Person entity)
        {
            SendPropertyChanging();
            entity.Family = null;
        }

        private void attach_RelatedFamilies1(RelatedFamily entity)
        {
            SendPropertyChanging();
            entity.RelatedFamily1 = this;
        }

        private void detach_RelatedFamilies1(RelatedFamily entity)
        {
            SendPropertyChanging();
            entity.RelatedFamily1 = null;
        }

        private void attach_RelatedFamilies2(RelatedFamily entity)
        {
            SendPropertyChanging();
            entity.RelatedFamily2 = this;
        }

        private void detach_RelatedFamilies2(RelatedFamily entity)
        {
            SendPropertyChanging();
            entity.RelatedFamily2 = null;
        }
    }
}
