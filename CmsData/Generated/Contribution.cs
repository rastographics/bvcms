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
	[Table(Name="dbo.Contribution")]
	public partial class Contribution : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _ContributionId;
		
		private int _CreatedBy;
		
		private DateTime _CreatedDate;
		
		private int _FundId;
		
		private int _ContributionTypeId;
		
		private int? _PeopleId;
		
		private DateTime? _ContributionDate;
		
		private decimal? _ContributionAmount;
		
		private string _ContributionDesc;
		
		private int? _ContributionStatusId;
		
		private bool? _PledgeFlag;
		
		private int? _ModifiedBy;
		
		private DateTime? _ModifiedDate;
		
		private DateTime? _PostingDate;
		
		private string _BankAccount;
		
		private int? _ExtraDataId;
		
		private string _CheckNo;
		
		private int? _QBSyncID;
		
		private int? _TranId;
		
		private int? _Source;
		
		private int? _CampusId;
		
		private int _ImageID;
		
		private string _MetaInfo;
		
		private int _Origin;
		
   		
   		private EntitySet<BundleDetail> _BundleDetails;
		
    	
		private EntityRef<ContributionFund> _ContributionFund;
		
		private EntityRef<ContributionStatus> _ContributionStatus;
		
		private EntityRef<ContributionType> _ContributionType;
		
		private EntityRef<ExtraDatum> _ExtraDatum;
		
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnContributionIdChanging(int value);
		partial void OnContributionIdChanged();
		
		partial void OnCreatedByChanging(int value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime value);
		partial void OnCreatedDateChanged();
		
		partial void OnFundIdChanging(int value);
		partial void OnFundIdChanged();
		
		partial void OnContributionTypeIdChanging(int value);
		partial void OnContributionTypeIdChanged();
		
		partial void OnPeopleIdChanging(int? value);
		partial void OnPeopleIdChanged();
		
		partial void OnContributionDateChanging(DateTime? value);
		partial void OnContributionDateChanged();
		
		partial void OnContributionAmountChanging(decimal? value);
		partial void OnContributionAmountChanged();
		
		partial void OnContributionDescChanging(string value);
		partial void OnContributionDescChanged();
		
		partial void OnContributionStatusIdChanging(int? value);
		partial void OnContributionStatusIdChanged();
		
		partial void OnPledgeFlagChanging(bool? value);
		partial void OnPledgeFlagChanged();
		
		partial void OnModifiedByChanging(int? value);
		partial void OnModifiedByChanged();
		
		partial void OnModifiedDateChanging(DateTime? value);
		partial void OnModifiedDateChanged();
		
		partial void OnPostingDateChanging(DateTime? value);
		partial void OnPostingDateChanged();
		
		partial void OnBankAccountChanging(string value);
		partial void OnBankAccountChanged();
		
		partial void OnExtraDataIdChanging(int? value);
		partial void OnExtraDataIdChanged();
		
		partial void OnCheckNoChanging(string value);
		partial void OnCheckNoChanged();
		
		partial void OnQBSyncIDChanging(int? value);
		partial void OnQBSyncIDChanged();
		
		partial void OnTranIdChanging(int? value);
		partial void OnTranIdChanged();
		
		partial void OnSourceChanging(int? value);
		partial void OnSourceChanged();
		
		partial void OnCampusIdChanging(int? value);
		partial void OnCampusIdChanged();
		
		partial void OnImageIDChanging(int value);
		partial void OnImageIDChanged();
		
		partial void OnMetaInfoChanging(string value);
		partial void OnMetaInfoChanged();
		
		partial void OnOriginChanging(int value);
		partial void OnOriginChanged();
		
    #endregion
		public Contribution()
		{
			
			this._BundleDetails = new EntitySet<BundleDetail>(new Action< BundleDetail>(this.attach_BundleDetails), new Action< BundleDetail>(this.detach_BundleDetails)); 
			
			
			this._ContributionFund = default(EntityRef<ContributionFund>); 
			
			this._ContributionStatus = default(EntityRef<ContributionStatus>); 
			
			this._ContributionType = default(EntityRef<ContributionType>); 
			
			this._ExtraDatum = default(EntityRef<ExtraDatum>); 
			
			this._Person = default(EntityRef<Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ContributionId", UpdateCheck=UpdateCheck.Never, Storage="_ContributionId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ContributionId
		{
			get { return this._ContributionId; }

			set
			{
				if (this._ContributionId != value)
				{
				
                    this.OnContributionIdChanging(value);
					this.SendPropertyChanging();
					this._ContributionId = value;
					this.SendPropertyChanged("ContributionId");
					this.OnContributionIdChanged();
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

		
		[Column(Name="CreatedDate", UpdateCheck=UpdateCheck.Never, Storage="_CreatedDate", DbType="datetime NOT NULL")]
		public DateTime CreatedDate
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

		
		[Column(Name="FundId", UpdateCheck=UpdateCheck.Never, Storage="_FundId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int FundId
		{
			get { return this._FundId; }

			set
			{
				if (this._FundId != value)
				{
				
					if (this._ContributionFund.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnFundIdChanging(value);
					this.SendPropertyChanging();
					this._FundId = value;
					this.SendPropertyChanged("FundId");
					this.OnFundIdChanged();
				}

			}

		}

		
		[Column(Name="ContributionTypeId", UpdateCheck=UpdateCheck.Never, Storage="_ContributionTypeId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int ContributionTypeId
		{
			get { return this._ContributionTypeId; }

			set
			{
				if (this._ContributionTypeId != value)
				{
				
					if (this._ContributionType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnContributionTypeIdChanging(value);
					this.SendPropertyChanging();
					this._ContributionTypeId = value;
					this.SendPropertyChanged("ContributionTypeId");
					this.OnContributionTypeIdChanged();
				}

			}

		}

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int")]
		[IsForeignKey]
		public int? PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="ContributionDate", UpdateCheck=UpdateCheck.Never, Storage="_ContributionDate", DbType="datetime")]
		public DateTime? ContributionDate
		{
			get { return this._ContributionDate; }

			set
			{
				if (this._ContributionDate != value)
				{
				
                    this.OnContributionDateChanging(value);
					this.SendPropertyChanging();
					this._ContributionDate = value;
					this.SendPropertyChanged("ContributionDate");
					this.OnContributionDateChanged();
				}

			}

		}

		
		[Column(Name="ContributionAmount", UpdateCheck=UpdateCheck.Never, Storage="_ContributionAmount", DbType="Decimal(11,2)")]
		public decimal? ContributionAmount
		{
			get { return this._ContributionAmount; }

			set
			{
				if (this._ContributionAmount != value)
				{
				
                    this.OnContributionAmountChanging(value);
					this.SendPropertyChanging();
					this._ContributionAmount = value;
					this.SendPropertyChanged("ContributionAmount");
					this.OnContributionAmountChanged();
				}

			}

		}

		
		[Column(Name="ContributionDesc", UpdateCheck=UpdateCheck.Never, Storage="_ContributionDesc", DbType="nvarchar(256)")]
		public string ContributionDesc
		{
			get { return this._ContributionDesc; }

			set
			{
				if (this._ContributionDesc != value)
				{
				
                    this.OnContributionDescChanging(value);
					this.SendPropertyChanging();
					this._ContributionDesc = value;
					this.SendPropertyChanged("ContributionDesc");
					this.OnContributionDescChanged();
				}

			}

		}

		
		[Column(Name="ContributionStatusId", UpdateCheck=UpdateCheck.Never, Storage="_ContributionStatusId", DbType="int")]
		[IsForeignKey]
		public int? ContributionStatusId
		{
			get { return this._ContributionStatusId; }

			set
			{
				if (this._ContributionStatusId != value)
				{
				
					if (this._ContributionStatus.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnContributionStatusIdChanging(value);
					this.SendPropertyChanging();
					this._ContributionStatusId = value;
					this.SendPropertyChanged("ContributionStatusId");
					this.OnContributionStatusIdChanged();
				}

			}

		}

		
		[Column(Name="PledgeFlag", UpdateCheck=UpdateCheck.Never, Storage="_PledgeFlag", DbType="bit")]
		public bool? PledgeFlag
		{
			get { return this._PledgeFlag; }

			set
			{
				if (this._PledgeFlag != value)
				{
				
                    this.OnPledgeFlagChanging(value);
					this.SendPropertyChanging();
					this._PledgeFlag = value;
					this.SendPropertyChanged("PledgeFlag");
					this.OnPledgeFlagChanged();
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

		
		[Column(Name="PostingDate", UpdateCheck=UpdateCheck.Never, Storage="_PostingDate", DbType="datetime")]
		public DateTime? PostingDate
		{
			get { return this._PostingDate; }

			set
			{
				if (this._PostingDate != value)
				{
				
                    this.OnPostingDateChanging(value);
					this.SendPropertyChanging();
					this._PostingDate = value;
					this.SendPropertyChanged("PostingDate");
					this.OnPostingDateChanged();
				}

			}

		}

		
		[Column(Name="BankAccount", UpdateCheck=UpdateCheck.Never, Storage="_BankAccount", DbType="nvarchar(250)")]
		public string BankAccount
		{
			get { return this._BankAccount; }

			set
			{
				if (this._BankAccount != value)
				{
				
                    this.OnBankAccountChanging(value);
					this.SendPropertyChanging();
					this._BankAccount = value;
					this.SendPropertyChanged("BankAccount");
					this.OnBankAccountChanged();
				}

			}

		}

		
		[Column(Name="ExtraDataId", UpdateCheck=UpdateCheck.Never, Storage="_ExtraDataId", DbType="int")]
		[IsForeignKey]
		public int? ExtraDataId
		{
			get { return this._ExtraDataId; }

			set
			{
				if (this._ExtraDataId != value)
				{
				
					if (this._ExtraDatum.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnExtraDataIdChanging(value);
					this.SendPropertyChanging();
					this._ExtraDataId = value;
					this.SendPropertyChanged("ExtraDataId");
					this.OnExtraDataIdChanged();
				}

			}

		}

		
		[Column(Name="CheckNo", UpdateCheck=UpdateCheck.Never, Storage="_CheckNo", DbType="nvarchar(20)")]
		public string CheckNo
		{
			get { return this._CheckNo; }

			set
			{
				if (this._CheckNo != value)
				{
				
                    this.OnCheckNoChanging(value);
					this.SendPropertyChanging();
					this._CheckNo = value;
					this.SendPropertyChanged("CheckNo");
					this.OnCheckNoChanged();
				}

			}

		}

		
		[Column(Name="QBSyncID", UpdateCheck=UpdateCheck.Never, Storage="_QBSyncID", DbType="int")]
		public int? QBSyncID
		{
			get { return this._QBSyncID; }

			set
			{
				if (this._QBSyncID != value)
				{
				
                    this.OnQBSyncIDChanging(value);
					this.SendPropertyChanging();
					this._QBSyncID = value;
					this.SendPropertyChanged("QBSyncID");
					this.OnQBSyncIDChanged();
				}

			}

		}

		
		[Column(Name="TranId", UpdateCheck=UpdateCheck.Never, Storage="_TranId", DbType="int")]
		public int? TranId
		{
			get { return this._TranId; }

			set
			{
				if (this._TranId != value)
				{
				
                    this.OnTranIdChanging(value);
					this.SendPropertyChanging();
					this._TranId = value;
					this.SendPropertyChanged("TranId");
					this.OnTranIdChanged();
				}

			}

		}

		
		[Column(Name="Source", UpdateCheck=UpdateCheck.Never, Storage="_Source", DbType="int")]
		public int? Source
		{
			get { return this._Source; }

			set
			{
				if (this._Source != value)
				{
				
                    this.OnSourceChanging(value);
					this.SendPropertyChanging();
					this._Source = value;
					this.SendPropertyChanged("Source");
					this.OnSourceChanged();
				}

			}

		}

		
		[Column(Name="CampusId", UpdateCheck=UpdateCheck.Never, Storage="_CampusId", DbType="int")]
		public int? CampusId
		{
			get { return this._CampusId; }

			set
			{
				if (this._CampusId != value)
				{
				
                    this.OnCampusIdChanging(value);
					this.SendPropertyChanging();
					this._CampusId = value;
					this.SendPropertyChanged("CampusId");
					this.OnCampusIdChanged();
				}

			}

		}

		
		[Column(Name="ImageID", UpdateCheck=UpdateCheck.Never, Storage="_ImageID", DbType="int NOT NULL")]
		public int ImageID
		{
			get { return this._ImageID; }

			set
			{
				if (this._ImageID != value)
				{
				
                    this.OnImageIDChanging(value);
					this.SendPropertyChanging();
					this._ImageID = value;
					this.SendPropertyChanged("ImageID");
					this.OnImageIDChanged();
				}

			}

		}

		
		[Column(Name="MetaInfo", UpdateCheck=UpdateCheck.Never, Storage="_MetaInfo", DbType="varchar(100)")]
		public string MetaInfo
		{
			get { return this._MetaInfo; }

			set
			{
				if (this._MetaInfo != value)
				{
				
                    this.OnMetaInfoChanging(value);
					this.SendPropertyChanging();
					this._MetaInfo = value;
					this.SendPropertyChanged("MetaInfo");
					this.OnMetaInfoChanged();
				}

			}

		}

		
		[Column(Name="Origin", UpdateCheck=UpdateCheck.Never, Storage="_Origin", DbType="int NOT NULL")]
		public int Origin
		{
			get { return this._Origin; }

			set
			{
				if (this._Origin != value)
				{
				
                    this.OnOriginChanging(value);
					this.SendPropertyChanging();
					this._Origin = value;
					this.SendPropertyChanged("Origin");
					this.OnOriginChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="BUNDLE_DETAIL_CONTR_FK", Storage="_BundleDetails", OtherKey="ContributionId")]
   		public EntitySet<BundleDetail> BundleDetails
   		{
   		    get { return this._BundleDetails; }

			set	{ this._BundleDetails.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_Contribution_ContributionFund", Storage="_ContributionFund", ThisKey="FundId", IsForeignKey=true)]
		public ContributionFund ContributionFund
		{
			get { return this._ContributionFund.Entity; }

			set
			{
				ContributionFund previousValue = this._ContributionFund.Entity;
				if (((previousValue != value) 
							|| (this._ContributionFund.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ContributionFund.Entity = null;
						previousValue.Contributions.Remove(this);
					}

					this._ContributionFund.Entity = value;
					if (value != null)
					{
						value.Contributions.Add(this);
						
						this._FundId = value.FundId;
						
					}

					else
					{
						
						this._FundId = default(int);
						
					}

					this.SendPropertyChanged("ContributionFund");
				}

			}

		}

		
		[Association(Name="FK_Contribution_ContributionStatus", Storage="_ContributionStatus", ThisKey="ContributionStatusId", IsForeignKey=true)]
		public ContributionStatus ContributionStatus
		{
			get { return this._ContributionStatus.Entity; }

			set
			{
				ContributionStatus previousValue = this._ContributionStatus.Entity;
				if (((previousValue != value) 
							|| (this._ContributionStatus.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ContributionStatus.Entity = null;
						previousValue.Contributions.Remove(this);
					}

					this._ContributionStatus.Entity = value;
					if (value != null)
					{
						value.Contributions.Add(this);
						
						this._ContributionStatusId = value.Id;
						
					}

					else
					{
						
						this._ContributionStatusId = default(int?);
						
					}

					this.SendPropertyChanged("ContributionStatus");
				}

			}

		}

		
		[Association(Name="FK_Contribution_ContributionType", Storage="_ContributionType", ThisKey="ContributionTypeId", IsForeignKey=true)]
		public ContributionType ContributionType
		{
			get { return this._ContributionType.Entity; }

			set
			{
				ContributionType previousValue = this._ContributionType.Entity;
				if (((previousValue != value) 
							|| (this._ContributionType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ContributionType.Entity = null;
						previousValue.Contributions.Remove(this);
					}

					this._ContributionType.Entity = value;
					if (value != null)
					{
						value.Contributions.Add(this);
						
						this._ContributionTypeId = value.Id;
						
					}

					else
					{
						
						this._ContributionTypeId = default(int);
						
					}

					this.SendPropertyChanged("ContributionType");
				}

			}

		}

		
		[Association(Name="FK_Contribution_ExtraData", Storage="_ExtraDatum", ThisKey="ExtraDataId", IsForeignKey=true)]
		public ExtraDatum ExtraDatum
		{
			get { return this._ExtraDatum.Entity; }

			set
			{
				ExtraDatum previousValue = this._ExtraDatum.Entity;
				if (((previousValue != value) 
							|| (this._ExtraDatum.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ExtraDatum.Entity = null;
						previousValue.Contributions.Remove(this);
					}

					this._ExtraDatum.Entity = value;
					if (value != null)
					{
						value.Contributions.Add(this);
						
						this._ExtraDataId = value.Id;
						
					}

					else
					{
						
						this._ExtraDataId = default(int?);
						
					}

					this.SendPropertyChanged("ExtraDatum");
				}

			}

		}

		
		[Association(Name="FK_Contribution_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
		public Person Person
		{
			get { return this._Person.Entity; }

			set
			{
				Person previousValue = this._Person.Entity;
				if (((previousValue != value) 
							|| (this._Person.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Person.Entity = null;
						previousValue.Contributions.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.Contributions.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int?);
						
					}

					this.SendPropertyChanged("Person");
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

   		
		private void attach_BundleDetails(BundleDetail entity)
		{
			this.SendPropertyChanging();
			entity.Contribution = this;
		}

		private void detach_BundleDetails(BundleDetail entity)
		{
			this.SendPropertyChanging();
			entity.Contribution = null;
		}

		
	}

}

