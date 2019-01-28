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
	[Table(Name="dbo.BundleHeader")]
	public partial class BundleHeader : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _BundleHeaderId;
		
		private int _ChurchId;
		
		private int _CreatedBy;
		
		private DateTime _CreatedDate;
		
		private bool _RecordStatus;
		
		private int _BundleStatusId;
		
		private DateTime _ContributionDate;
		
		private int _BundleHeaderTypeId;
		
		private DateTime? _DepositDate;
		
		private decimal? _BundleTotal;
		
		private decimal? _TotalCash;
		
		private decimal? _TotalChecks;
		
		private decimal? _TotalEnvelopes;
		
		private int? _ModifiedBy;
		
		private DateTime? _ModifiedDate;
		
		private int? _FundId;
		
		private string _ReferenceId;
		
		private int? _ReferenceIdType;
		
   		
   		private EntitySet<BundleDetail> _BundleDetails;
		
    	
		private EntityRef<ContributionFund> _Fund;
		
		private EntityRef<BundleHeaderType> _BundleHeaderType;
		
		private EntityRef<BundleStatusType> _BundleStatusType;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnBundleHeaderIdChanging(int value);
		partial void OnBundleHeaderIdChanged();
		
		partial void OnChurchIdChanging(int value);
		partial void OnChurchIdChanged();
		
		partial void OnCreatedByChanging(int value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime value);
		partial void OnCreatedDateChanged();
		
		partial void OnRecordStatusChanging(bool value);
		partial void OnRecordStatusChanged();
		
		partial void OnBundleStatusIdChanging(int value);
		partial void OnBundleStatusIdChanged();
		
		partial void OnContributionDateChanging(DateTime value);
		partial void OnContributionDateChanged();
		
		partial void OnBundleHeaderTypeIdChanging(int value);
		partial void OnBundleHeaderTypeIdChanged();
		
		partial void OnDepositDateChanging(DateTime? value);
		partial void OnDepositDateChanged();
		
		partial void OnBundleTotalChanging(decimal? value);
		partial void OnBundleTotalChanged();
		
		partial void OnTotalCashChanging(decimal? value);
		partial void OnTotalCashChanged();
		
		partial void OnTotalChecksChanging(decimal? value);
		partial void OnTotalChecksChanged();
		
		partial void OnTotalEnvelopesChanging(decimal? value);
		partial void OnTotalEnvelopesChanged();
		
		partial void OnModifiedByChanging(int? value);
		partial void OnModifiedByChanged();
		
		partial void OnModifiedDateChanging(DateTime? value);
		partial void OnModifiedDateChanged();
		
		partial void OnFundIdChanging(int? value);
		partial void OnFundIdChanged();
		
		partial void OnReferenceIdChanging(string value);
		partial void OnReferenceIdChanged();
		
		partial void OnReferenceIdTypeChanging(int? value);
		partial void OnReferenceIdTypeChanged();
		
    #endregion
		public BundleHeader()
		{
			
			this._BundleDetails = new EntitySet<BundleDetail>(new Action< BundleDetail>(this.attach_BundleDetails), new Action< BundleDetail>(this.detach_BundleDetails)); 
			
			
			this._Fund = default(EntityRef<ContributionFund>); 
			
			this._BundleHeaderType = default(EntityRef<BundleHeaderType>); 
			
			this._BundleStatusType = default(EntityRef<BundleStatusType>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="BundleHeaderId", UpdateCheck=UpdateCheck.Never, Storage="_BundleHeaderId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int BundleHeaderId
		{
			get { return this._BundleHeaderId; }

			set
			{
				if (this._BundleHeaderId != value)
				{
				
                    this.OnBundleHeaderIdChanging(value);
					this.SendPropertyChanging();
					this._BundleHeaderId = value;
					this.SendPropertyChanged("BundleHeaderId");
					this.OnBundleHeaderIdChanged();
				}

			}

		}

		
		[Column(Name="ChurchId", UpdateCheck=UpdateCheck.Never, Storage="_ChurchId", DbType="int NOT NULL")]
		public int ChurchId
		{
			get { return this._ChurchId; }

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

		
		[Column(Name="BundleStatusId", UpdateCheck=UpdateCheck.Never, Storage="_BundleStatusId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int BundleStatusId
		{
			get { return this._BundleStatusId; }

			set
			{
				if (this._BundleStatusId != value)
				{
				
					if (this._BundleStatusType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnBundleStatusIdChanging(value);
					this.SendPropertyChanging();
					this._BundleStatusId = value;
					this.SendPropertyChanged("BundleStatusId");
					this.OnBundleStatusIdChanged();
				}

			}

		}

		
		[Column(Name="ContributionDate", UpdateCheck=UpdateCheck.Never, Storage="_ContributionDate", DbType="datetime NOT NULL")]
		public DateTime ContributionDate
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

		
		[Column(Name="BundleHeaderTypeId", UpdateCheck=UpdateCheck.Never, Storage="_BundleHeaderTypeId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int BundleHeaderTypeId
		{
			get { return this._BundleHeaderTypeId; }

			set
			{
				if (this._BundleHeaderTypeId != value)
				{
				
					if (this._BundleHeaderType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnBundleHeaderTypeIdChanging(value);
					this.SendPropertyChanging();
					this._BundleHeaderTypeId = value;
					this.SendPropertyChanged("BundleHeaderTypeId");
					this.OnBundleHeaderTypeIdChanged();
				}

			}

		}

		
		[Column(Name="DepositDate", UpdateCheck=UpdateCheck.Never, Storage="_DepositDate", DbType="datetime")]
		public DateTime? DepositDate
		{
			get { return this._DepositDate; }

			set
			{
				if (this._DepositDate != value)
				{
				
                    this.OnDepositDateChanging(value);
					this.SendPropertyChanging();
					this._DepositDate = value;
					this.SendPropertyChanged("DepositDate");
					this.OnDepositDateChanged();
				}

			}

		}

		
		[Column(Name="BundleTotal", UpdateCheck=UpdateCheck.Never, Storage="_BundleTotal", DbType="Decimal(10,2)")]
		public decimal? BundleTotal
		{
			get { return this._BundleTotal; }

			set
			{
				if (this._BundleTotal != value)
				{
				
                    this.OnBundleTotalChanging(value);
					this.SendPropertyChanging();
					this._BundleTotal = value;
					this.SendPropertyChanged("BundleTotal");
					this.OnBundleTotalChanged();
				}

			}

		}

		
		[Column(Name="TotalCash", UpdateCheck=UpdateCheck.Never, Storage="_TotalCash", DbType="Decimal(10,2)")]
		public decimal? TotalCash
		{
			get { return this._TotalCash; }

			set
			{
				if (this._TotalCash != value)
				{
				
                    this.OnTotalCashChanging(value);
					this.SendPropertyChanging();
					this._TotalCash = value;
					this.SendPropertyChanged("TotalCash");
					this.OnTotalCashChanged();
				}

			}

		}

		
		[Column(Name="TotalChecks", UpdateCheck=UpdateCheck.Never, Storage="_TotalChecks", DbType="Decimal(10,2)")]
		public decimal? TotalChecks
		{
			get { return this._TotalChecks; }

			set
			{
				if (this._TotalChecks != value)
				{
				
                    this.OnTotalChecksChanging(value);
					this.SendPropertyChanging();
					this._TotalChecks = value;
					this.SendPropertyChanged("TotalChecks");
					this.OnTotalChecksChanged();
				}

			}

		}

		
		[Column(Name="TotalEnvelopes", UpdateCheck=UpdateCheck.Never, Storage="_TotalEnvelopes", DbType="Decimal(10,2)")]
		public decimal? TotalEnvelopes
		{
			get { return this._TotalEnvelopes; }

			set
			{
				if (this._TotalEnvelopes != value)
				{
				
                    this.OnTotalEnvelopesChanging(value);
					this.SendPropertyChanging();
					this._TotalEnvelopes = value;
					this.SendPropertyChanged("TotalEnvelopes");
					this.OnTotalEnvelopesChanged();
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

		
		[Column(Name="FundId", UpdateCheck=UpdateCheck.Never, Storage="_FundId", DbType="int")]
		[IsForeignKey]
		public int? FundId
		{
			get { return this._FundId; }

			set
			{
				if (this._FundId != value)
				{
				
					if (this._Fund.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnFundIdChanging(value);
					this.SendPropertyChanging();
					this._FundId = value;
					this.SendPropertyChanged("FundId");
					this.OnFundIdChanged();
				}

			}

		}

		
		[Column(Name="ReferenceId", UpdateCheck=UpdateCheck.Never, Storage="_ReferenceId", DbType="nvarchar(100)")]
		public string ReferenceId
		{
			get { return this._ReferenceId; }

			set
			{
				if (this._ReferenceId != value)
				{
				
                    this.OnReferenceIdChanging(value);
					this.SendPropertyChanging();
					this._ReferenceId = value;
					this.SendPropertyChanged("ReferenceId");
					this.OnReferenceIdChanged();
				}

			}

		}

		
		[Column(Name="ReferenceIdType", UpdateCheck=UpdateCheck.Never, Storage="_ReferenceIdType", DbType="int")]
		public int? ReferenceIdType
		{
			get { return this._ReferenceIdType; }

			set
			{
				if (this._ReferenceIdType != value)
				{
				
                    this.OnReferenceIdTypeChanging(value);
					this.SendPropertyChanging();
					this._ReferenceIdType = value;
					this.SendPropertyChanged("ReferenceIdType");
					this.OnReferenceIdTypeChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="BUNDLE_DETAIL_BUNDLE_FK", Storage="_BundleDetails", OtherKey="BundleHeaderId")]
   		public EntitySet<BundleDetail> BundleDetails
   		{
   		    get { return this._BundleDetails; }

			set	{ this._BundleDetails.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="BundleHeaders__Fund", Storage="_Fund", ThisKey="FundId", IsForeignKey=true)]
		public ContributionFund Fund
		{
			get { return this._Fund.Entity; }

			set
			{
				ContributionFund previousValue = this._Fund.Entity;
				if (((previousValue != value) 
							|| (this._Fund.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Fund.Entity = null;
						previousValue.BundleHeaders.Remove(this);
					}

					this._Fund.Entity = value;
					if (value != null)
					{
						value.BundleHeaders.Add(this);
						
						this._FundId = value.FundId;
						
					}

					else
					{
						
						this._FundId = default(int?);
						
					}

					this.SendPropertyChanged("Fund");
				}

			}

		}

		
		[Association(Name="FK_BUNDLE_HEADER_TBL_BundleHeaderTypes", Storage="_BundleHeaderType", ThisKey="BundleHeaderTypeId", IsForeignKey=true)]
		public BundleHeaderType BundleHeaderType
		{
			get { return this._BundleHeaderType.Entity; }

			set
			{
				BundleHeaderType previousValue = this._BundleHeaderType.Entity;
				if (((previousValue != value) 
							|| (this._BundleHeaderType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._BundleHeaderType.Entity = null;
						previousValue.BundleHeaders.Remove(this);
					}

					this._BundleHeaderType.Entity = value;
					if (value != null)
					{
						value.BundleHeaders.Add(this);
						
						this._BundleHeaderTypeId = value.Id;
						
					}

					else
					{
						
						this._BundleHeaderTypeId = default(int);
						
					}

					this.SendPropertyChanged("BundleHeaderType");
				}

			}

		}

		
		[Association(Name="FK_BUNDLE_HEADER_TBL_BundleStatusTypes", Storage="_BundleStatusType", ThisKey="BundleStatusId", IsForeignKey=true)]
		public BundleStatusType BundleStatusType
		{
			get { return this._BundleStatusType.Entity; }

			set
			{
				BundleStatusType previousValue = this._BundleStatusType.Entity;
				if (((previousValue != value) 
							|| (this._BundleStatusType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._BundleStatusType.Entity = null;
						previousValue.BundleHeaders.Remove(this);
					}

					this._BundleStatusType.Entity = value;
					if (value != null)
					{
						value.BundleHeaders.Add(this);
						
						this._BundleStatusId = value.Id;
						
					}

					else
					{
						
						this._BundleStatusId = default(int);
						
					}

					this.SendPropertyChanged("BundleStatusType");
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
			entity.BundleHeader = this;
		}

		private void detach_BundleDetails(BundleDetail entity)
		{
			this.SendPropertyChanging();
			entity.BundleHeader = null;
		}

		
	}

}

