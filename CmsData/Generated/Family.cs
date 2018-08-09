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
	[Table(Name="dbo.Families")]
	public partial class Family : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
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
			
			this._FamilyCheckinLocks = new EntitySet<FamilyCheckinLock>(new Action< FamilyCheckinLock>(this.attach_FamilyCheckinLocks), new Action< FamilyCheckinLock>(this.detach_FamilyCheckinLocks)); 
			
			this._FamilyExtras = new EntitySet<FamilyExtra>(new Action< FamilyExtra>(this.attach_FamilyExtras), new Action< FamilyExtra>(this.detach_FamilyExtras)); 
			
			this._People = new EntitySet<Person>(new Action< Person>(this.attach_People), new Action< Person>(this.detach_People)); 
			
			this._RelatedFamilies1 = new EntitySet<RelatedFamily>(new Action< RelatedFamily>(this.attach_RelatedFamilies1), new Action< RelatedFamily>(this.detach_RelatedFamilies1)); 
			
			this._RelatedFamilies2 = new EntitySet<RelatedFamily>(new Action< RelatedFamily>(this.attach_RelatedFamilies2), new Action< RelatedFamily>(this.detach_RelatedFamilies2)); 
			
			
			this._HeadOfHousehold = default(EntityRef<Person>); 
			
			this._HeadOfHouseholdSpouse = default(EntityRef<Person>); 
			
			this._Picture = default(EntityRef<Picture>); 
			
			this._ResidentCode = default(EntityRef<ResidentCode>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="FamilyId", UpdateCheck=UpdateCheck.Never, Storage="_FamilyId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int FamilyId
		{
			get { return this._FamilyId; }

			set
			{
				if (this._FamilyId != value)
				{
				
                    this.OnFamilyIdChanging(value);
					this.SendPropertyChanging();
					this._FamilyId = value;
					this.SendPropertyChanged("FamilyId");
					this.OnFamilyIdChanged();
				}

			}

		}

		
		[Column(Name="CreatedBy", UpdateCheck=UpdateCheck.Never, Storage="_CreatedBy", DbType="int NOT NULL")]
		public int CreatedBy
		{
			get { return this._CreatedBy; }

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

		
		[Column(Name="CreatedDate", UpdateCheck=UpdateCheck.Never, Storage="_CreatedDate", DbType="datetime")]
		public DateTime? CreatedDate
		{
			get { return this._CreatedDate; }

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

		
		[Column(Name="RecordStatus", UpdateCheck=UpdateCheck.Never, Storage="_RecordStatus", DbType="bit NOT NULL")]
		public bool RecordStatus
		{
			get { return this._RecordStatus; }

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

		
		[Column(Name="BadAddressFlag", UpdateCheck=UpdateCheck.Never, Storage="_BadAddressFlag", DbType="bit")]
		public bool? BadAddressFlag
		{
			get { return this._BadAddressFlag; }

			set
			{
				if (this._BadAddressFlag != value)
				{
				
                    this.OnBadAddressFlagChanging(value);
					this.SendPropertyChanging();
					this._BadAddressFlag = value;
					this.SendPropertyChanged("BadAddressFlag");
					this.OnBadAddressFlagChanged();
				}

			}

		}

		
		[Column(Name="AltBadAddressFlag", UpdateCheck=UpdateCheck.Never, Storage="_AltBadAddressFlag", DbType="bit")]
		public bool? AltBadAddressFlag
		{
			get { return this._AltBadAddressFlag; }

			set
			{
				if (this._AltBadAddressFlag != value)
				{
				
                    this.OnAltBadAddressFlagChanging(value);
					this.SendPropertyChanging();
					this._AltBadAddressFlag = value;
					this.SendPropertyChanged("AltBadAddressFlag");
					this.OnAltBadAddressFlagChanged();
				}

			}

		}

		
		[Column(Name="ResCodeId", UpdateCheck=UpdateCheck.Never, Storage="_ResCodeId", DbType="int")]
		[IsForeignKey]
		public int? ResCodeId
		{
			get { return this._ResCodeId; }

			set
			{
				if (this._ResCodeId != value)
				{
				
					if (this._ResidentCode.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnResCodeIdChanging(value);
					this.SendPropertyChanging();
					this._ResCodeId = value;
					this.SendPropertyChanged("ResCodeId");
					this.OnResCodeIdChanged();
				}

			}

		}

		
		[Column(Name="AltResCodeId", UpdateCheck=UpdateCheck.Never, Storage="_AltResCodeId", DbType="int")]
		public int? AltResCodeId
		{
			get { return this._AltResCodeId; }

			set
			{
				if (this._AltResCodeId != value)
				{
				
                    this.OnAltResCodeIdChanging(value);
					this.SendPropertyChanging();
					this._AltResCodeId = value;
					this.SendPropertyChanged("AltResCodeId");
					this.OnAltResCodeIdChanged();
				}

			}

		}

		
		[Column(Name="AddressFromDate", UpdateCheck=UpdateCheck.Never, Storage="_AddressFromDate", DbType="datetime")]
		public DateTime? AddressFromDate
		{
			get { return this._AddressFromDate; }

			set
			{
				if (this._AddressFromDate != value)
				{
				
                    this.OnAddressFromDateChanging(value);
					this.SendPropertyChanging();
					this._AddressFromDate = value;
					this.SendPropertyChanged("AddressFromDate");
					this.OnAddressFromDateChanged();
				}

			}

		}

		
		[Column(Name="AddressToDate", UpdateCheck=UpdateCheck.Never, Storage="_AddressToDate", DbType="datetime")]
		public DateTime? AddressToDate
		{
			get { return this._AddressToDate; }

			set
			{
				if (this._AddressToDate != value)
				{
				
                    this.OnAddressToDateChanging(value);
					this.SendPropertyChanging();
					this._AddressToDate = value;
					this.SendPropertyChanged("AddressToDate");
					this.OnAddressToDateChanged();
				}

			}

		}

		
		[Column(Name="AddressLineOne", UpdateCheck=UpdateCheck.Never, Storage="_AddressLineOne", DbType="nvarchar(100)")]
		public string AddressLineOne
		{
			get { return this._AddressLineOne; }

			set
			{
				if (this._AddressLineOne != value)
				{
				
                    this.OnAddressLineOneChanging(value);
					this.SendPropertyChanging();
					this._AddressLineOne = value;
					this.SendPropertyChanged("AddressLineOne");
					this.OnAddressLineOneChanged();
				}

			}

		}

		
		[Column(Name="AddressLineTwo", UpdateCheck=UpdateCheck.Never, Storage="_AddressLineTwo", DbType="nvarchar(100)")]
		public string AddressLineTwo
		{
			get { return this._AddressLineTwo; }

			set
			{
				if (this._AddressLineTwo != value)
				{
				
                    this.OnAddressLineTwoChanging(value);
					this.SendPropertyChanging();
					this._AddressLineTwo = value;
					this.SendPropertyChanged("AddressLineTwo");
					this.OnAddressLineTwoChanged();
				}

			}

		}

		
		[Column(Name="CityName", UpdateCheck=UpdateCheck.Never, Storage="_CityName", DbType="nvarchar(30)")]
		public string CityName
		{
			get { return this._CityName; }

			set
			{
				if (this._CityName != value)
				{
				
                    this.OnCityNameChanging(value);
					this.SendPropertyChanging();
					this._CityName = value;
					this.SendPropertyChanged("CityName");
					this.OnCityNameChanged();
				}

			}

		}

		
		[Column(Name="StateCode", UpdateCheck=UpdateCheck.Never, Storage="_StateCode", DbType="nvarchar(30)")]
		public string StateCode
		{
			get { return this._StateCode; }

			set
			{
				if (this._StateCode != value)
				{
				
                    this.OnStateCodeChanging(value);
					this.SendPropertyChanging();
					this._StateCode = value;
					this.SendPropertyChanged("StateCode");
					this.OnStateCodeChanged();
				}

			}

		}

		
		[Column(Name="ZipCode", UpdateCheck=UpdateCheck.Never, Storage="_ZipCode", DbType="nvarchar(15)")]
		public string ZipCode
		{
			get { return this._ZipCode; }

			set
			{
				if (this._ZipCode != value)
				{
				
                    this.OnZipCodeChanging(value);
					this.SendPropertyChanging();
					this._ZipCode = value;
					this.SendPropertyChanged("ZipCode");
					this.OnZipCodeChanged();
				}

			}

		}

		
		[Column(Name="CountryName", UpdateCheck=UpdateCheck.Never, Storage="_CountryName", DbType="nvarchar(40)")]
		public string CountryName
		{
			get { return this._CountryName; }

			set
			{
				if (this._CountryName != value)
				{
				
                    this.OnCountryNameChanging(value);
					this.SendPropertyChanging();
					this._CountryName = value;
					this.SendPropertyChanged("CountryName");
					this.OnCountryNameChanged();
				}

			}

		}

		
		[Column(Name="StreetName", UpdateCheck=UpdateCheck.Never, Storage="_StreetName", DbType="nvarchar(40)")]
		public string StreetName
		{
			get { return this._StreetName; }

			set
			{
				if (this._StreetName != value)
				{
				
                    this.OnStreetNameChanging(value);
					this.SendPropertyChanging();
					this._StreetName = value;
					this.SendPropertyChanged("StreetName");
					this.OnStreetNameChanged();
				}

			}

		}

		
		[Column(Name="HomePhone", UpdateCheck=UpdateCheck.Never, Storage="_HomePhone", DbType="nvarchar(20)")]
		public string HomePhone
		{
			get { return this._HomePhone; }

			set
			{
				if (this._HomePhone != value)
				{
				
                    this.OnHomePhoneChanging(value);
					this.SendPropertyChanging();
					this._HomePhone = value;
					this.SendPropertyChanged("HomePhone");
					this.OnHomePhoneChanged();
				}

			}

		}

		
		[Column(Name="ModifiedBy", UpdateCheck=UpdateCheck.Never, Storage="_ModifiedBy", DbType="int")]
		public int? ModifiedBy
		{
			get { return this._ModifiedBy; }

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

		
		[Column(Name="ModifiedDate", UpdateCheck=UpdateCheck.Never, Storage="_ModifiedDate", DbType="datetime")]
		public DateTime? ModifiedDate
		{
			get { return this._ModifiedDate; }

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

		
		[Column(Name="HeadOfHouseholdId", UpdateCheck=UpdateCheck.Never, Storage="_HeadOfHouseholdId", DbType="int")]
		[IsForeignKey]
		public int? HeadOfHouseholdId
		{
			get { return this._HeadOfHouseholdId; }

			set
			{
				if (this._HeadOfHouseholdId != value)
				{
				
					if (this._HeadOfHousehold.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnHeadOfHouseholdIdChanging(value);
					this.SendPropertyChanging();
					this._HeadOfHouseholdId = value;
					this.SendPropertyChanged("HeadOfHouseholdId");
					this.OnHeadOfHouseholdIdChanged();
				}

			}

		}

		
		[Column(Name="HeadOfHouseholdSpouseId", UpdateCheck=UpdateCheck.Never, Storage="_HeadOfHouseholdSpouseId", DbType="int")]
		[IsForeignKey]
		public int? HeadOfHouseholdSpouseId
		{
			get { return this._HeadOfHouseholdSpouseId; }

			set
			{
				if (this._HeadOfHouseholdSpouseId != value)
				{
				
					if (this._HeadOfHouseholdSpouse.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnHeadOfHouseholdSpouseIdChanging(value);
					this.SendPropertyChanging();
					this._HeadOfHouseholdSpouseId = value;
					this.SendPropertyChanged("HeadOfHouseholdSpouseId");
					this.OnHeadOfHouseholdSpouseIdChanged();
				}

			}

		}

		
		[Column(Name="CoupleFlag", UpdateCheck=UpdateCheck.Never, Storage="_CoupleFlag", DbType="int")]
		public int? CoupleFlag
		{
			get { return this._CoupleFlag; }

			set
			{
				if (this._CoupleFlag != value)
				{
				
                    this.OnCoupleFlagChanging(value);
					this.SendPropertyChanging();
					this._CoupleFlag = value;
					this.SendPropertyChanged("CoupleFlag");
					this.OnCoupleFlagChanged();
				}

			}

		}

		
		[Column(Name="HomePhoneLU", UpdateCheck=UpdateCheck.Never, Storage="_HomePhoneLU", DbType="char(7)")]
		public string HomePhoneLU
		{
			get { return this._HomePhoneLU; }

			set
			{
				if (this._HomePhoneLU != value)
				{
				
                    this.OnHomePhoneLUChanging(value);
					this.SendPropertyChanging();
					this._HomePhoneLU = value;
					this.SendPropertyChanged("HomePhoneLU");
					this.OnHomePhoneLUChanged();
				}

			}

		}

		
		[Column(Name="HomePhoneAC", UpdateCheck=UpdateCheck.Never, Storage="_HomePhoneAC", DbType="char(3)")]
		public string HomePhoneAC
		{
			get { return this._HomePhoneAC; }

			set
			{
				if (this._HomePhoneAC != value)
				{
				
                    this.OnHomePhoneACChanging(value);
					this.SendPropertyChanging();
					this._HomePhoneAC = value;
					this.SendPropertyChanged("HomePhoneAC");
					this.OnHomePhoneACChanged();
				}

			}

		}

		
		[Column(Name="Comments", UpdateCheck=UpdateCheck.Never, Storage="_Comments", DbType="nvarchar(3000)")]
		public string Comments
		{
			get { return this._Comments; }

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

		
		[Column(Name="PictureId", UpdateCheck=UpdateCheck.Never, Storage="_PictureId", DbType="int")]
		[IsForeignKey]
		public int? PictureId
		{
			get { return this._PictureId; }

			set
			{
				if (this._PictureId != value)
				{
				
					if (this._Picture.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPictureIdChanging(value);
					this.SendPropertyChanging();
					this._PictureId = value;
					this.SendPropertyChanged("PictureId");
					this.OnPictureIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_FamilyCheckinLock_FamilyCheckinLock1", Storage="_FamilyCheckinLocks", OtherKey="FamilyId")]
   		public EntitySet<FamilyCheckinLock> FamilyCheckinLocks
   		{
   		    get { return this._FamilyCheckinLocks; }

			set	{ this._FamilyCheckinLocks.Assign(value); }

   		}

		
   		[Association(Name="FK_FamilyExtra_Family", Storage="_FamilyExtras", OtherKey="FamilyId")]
   		public EntitySet<FamilyExtra> FamilyExtras
   		{
   		    get { return this._FamilyExtras; }

			set	{ this._FamilyExtras.Assign(value); }

   		}

		
   		[Association(Name="FK_People_Families", Storage="_People", OtherKey="FamilyId")]
   		public EntitySet<Person> People
   		{
   		    get { return this._People; }

			set	{ this._People.Assign(value); }

   		}

		
   		[Association(Name="RelatedFamilies1__RelatedFamily1", Storage="_RelatedFamilies1", OtherKey="FamilyId")]
   		public EntitySet<RelatedFamily> RelatedFamilies1
   		{
   		    get { return this._RelatedFamilies1; }

			set	{ this._RelatedFamilies1.Assign(value); }

   		}

		
   		[Association(Name="RelatedFamilies2__RelatedFamily2", Storage="_RelatedFamilies2", OtherKey="RelatedFamilyId")]
   		public EntitySet<RelatedFamily> RelatedFamilies2
   		{
   		    get { return this._RelatedFamilies2; }

			set	{ this._RelatedFamilies2.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FamiliesHeaded__HeadOfHousehold", Storage="_HeadOfHousehold", ThisKey="HeadOfHouseholdId", IsForeignKey=true)]
		public Person HeadOfHousehold
		{
			get { return this._HeadOfHousehold.Entity; }

			set
			{
				Person previousValue = this._HeadOfHousehold.Entity;
				if (((previousValue != value) 
							|| (this._HeadOfHousehold.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._HeadOfHousehold.Entity = null;
						previousValue.FamiliesHeaded.Remove(this);
					}

					this._HeadOfHousehold.Entity = value;
					if (value != null)
					{
						value.FamiliesHeaded.Add(this);
						
						this._HeadOfHouseholdId = value.PeopleId;
						
					}

					else
					{
						
						this._HeadOfHouseholdId = default(int?);
						
					}

					this.SendPropertyChanged("HeadOfHousehold");
				}

			}

		}

		
		[Association(Name="FamiliesHeaded2__HeadOfHouseholdSpouse", Storage="_HeadOfHouseholdSpouse", ThisKey="HeadOfHouseholdSpouseId", IsForeignKey=true)]
		public Person HeadOfHouseholdSpouse
		{
			get { return this._HeadOfHouseholdSpouse.Entity; }

			set
			{
				Person previousValue = this._HeadOfHouseholdSpouse.Entity;
				if (((previousValue != value) 
							|| (this._HeadOfHouseholdSpouse.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._HeadOfHouseholdSpouse.Entity = null;
						previousValue.FamiliesHeaded2.Remove(this);
					}

					this._HeadOfHouseholdSpouse.Entity = value;
					if (value != null)
					{
						value.FamiliesHeaded2.Add(this);
						
						this._HeadOfHouseholdSpouseId = value.PeopleId;
						
					}

					else
					{
						
						this._HeadOfHouseholdSpouseId = default(int?);
						
					}

					this.SendPropertyChanged("HeadOfHouseholdSpouse");
				}

			}

		}

		
		[Association(Name="FK_Families_Picture", Storage="_Picture", ThisKey="PictureId", IsForeignKey=true)]
		public Picture Picture
		{
			get { return this._Picture.Entity; }

			set
			{
				Picture previousValue = this._Picture.Entity;
				if (((previousValue != value) 
							|| (this._Picture.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Picture.Entity = null;
						previousValue.Families.Remove(this);
					}

					this._Picture.Entity = value;
					if (value != null)
					{
						value.Families.Add(this);
						
						this._PictureId = value.PictureId;
						
					}

					else
					{
						
						this._PictureId = default(int?);
						
					}

					this.SendPropertyChanged("Picture");
				}

			}

		}

		
		[Association(Name="ResCodeFamilies__ResidentCode", Storage="_ResidentCode", ThisKey="ResCodeId", IsForeignKey=true)]
		public ResidentCode ResidentCode
		{
			get { return this._ResidentCode.Entity; }

			set
			{
				ResidentCode previousValue = this._ResidentCode.Entity;
				if (((previousValue != value) 
							|| (this._ResidentCode.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ResidentCode.Entity = null;
						previousValue.ResCodeFamilies.Remove(this);
					}

					this._ResidentCode.Entity = value;
					if (value != null)
					{
						value.ResCodeFamilies.Add(this);
						
						this._ResCodeId = value.Id;
						
					}

					else
					{
						
						this._ResCodeId = default(int?);
						
					}

					this.SendPropertyChanged("ResidentCode");
				}

			}

		}

		
	#endregion
	
		public event PropertyChangingEventHandler PropertyChanging;
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
				this.PropertyChanging(this, emptyChangingEventArgs);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

   		
		private void attach_FamilyCheckinLocks(FamilyCheckinLock entity)
		{
			this.SendPropertyChanging();
			entity.Family = this;
		}

		private void detach_FamilyCheckinLocks(FamilyCheckinLock entity)
		{
			this.SendPropertyChanging();
			entity.Family = null;
		}

		
		private void attach_FamilyExtras(FamilyExtra entity)
		{
			this.SendPropertyChanging();
			entity.Family = this;
		}

		private void detach_FamilyExtras(FamilyExtra entity)
		{
			this.SendPropertyChanging();
			entity.Family = null;
		}

		
		private void attach_People(Person entity)
		{
			this.SendPropertyChanging();
			entity.Family = this;
		}

		private void detach_People(Person entity)
		{
			this.SendPropertyChanging();
			entity.Family = null;
		}

		
		private void attach_RelatedFamilies1(RelatedFamily entity)
		{
			this.SendPropertyChanging();
			entity.RelatedFamily1 = this;
		}

		private void detach_RelatedFamilies1(RelatedFamily entity)
		{
			this.SendPropertyChanging();
			entity.RelatedFamily1 = null;
		}

		
		private void attach_RelatedFamilies2(RelatedFamily entity)
		{
			this.SendPropertyChanging();
			entity.RelatedFamily2 = this;
		}

		private void detach_RelatedFamilies2(RelatedFamily entity)
		{
			this.SendPropertyChanging();
			entity.RelatedFamily2 = null;
		}

		
	}

}

