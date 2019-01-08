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
	[Table(Name="dbo.ContributionFund")]
	public partial class ContributionFund : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _FundId;
		
		private int _CreatedBy;
		
		private DateTime _CreatedDate;
		
		private string _FundName;
		
		private string _FundDescription;
		
		private int _FundStatusId;
		
		private int _FundTypeId;
		
		private bool _FundPledgeFlag;
		
		private int? _FundAccountCode;
		
		private string _FundIncomeDept;
		
		private string _FundIncomeAccount;
		
		private string _FundIncomeFund;
		
		private string _FundCashDept;
		
		private string _FundCashAccount;
		
		private string _FundCashFund;
		
		private int? _OnlineSort;
		
		private bool? _NonTaxDeductible;
		
		private int _QBIncomeAccount;
		
		private int _QBAssetAccount;
		
		private int _FundManagerRoleId;
		
   		
   		private EntitySet<BundleHeader> _BundleHeaders;
		
   		private EntitySet<Contribution> _Contributions;
		
   		private EntitySet<RecurringAmount> _RecurringAmounts;
		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnFundIdChanging(int value);
		partial void OnFundIdChanged();
		
		partial void OnCreatedByChanging(int value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime value);
		partial void OnCreatedDateChanged();
		
		partial void OnFundNameChanging(string value);
		partial void OnFundNameChanged();
		
		partial void OnFundDescriptionChanging(string value);
		partial void OnFundDescriptionChanged();
		
		partial void OnFundStatusIdChanging(int value);
		partial void OnFundStatusIdChanged();
		
		partial void OnFundTypeIdChanging(int value);
		partial void OnFundTypeIdChanged();
		
		partial void OnFundPledgeFlagChanging(bool value);
		partial void OnFundPledgeFlagChanged();
		
		partial void OnFundAccountCodeChanging(int? value);
		partial void OnFundAccountCodeChanged();
		
		partial void OnFundIncomeDeptChanging(string value);
		partial void OnFundIncomeDeptChanged();
		
		partial void OnFundIncomeAccountChanging(string value);
		partial void OnFundIncomeAccountChanged();
		
		partial void OnFundIncomeFundChanging(string value);
		partial void OnFundIncomeFundChanged();
		
		partial void OnFundCashDeptChanging(string value);
		partial void OnFundCashDeptChanged();
		
		partial void OnFundCashAccountChanging(string value);
		partial void OnFundCashAccountChanged();
		
		partial void OnFundCashFundChanging(string value);
		partial void OnFundCashFundChanged();
		
		partial void OnOnlineSortChanging(int? value);
		partial void OnOnlineSortChanged();
		
		partial void OnNonTaxDeductibleChanging(bool? value);
		partial void OnNonTaxDeductibleChanged();
		
		partial void OnQBIncomeAccountChanging(int value);
		partial void OnQBIncomeAccountChanged();
		
		partial void OnQBAssetAccountChanging(int value);
		partial void OnQBAssetAccountChanged();
		
		partial void OnFundManagerRoleIdChanging(int value);
		partial void OnFundManagerRoleIdChanged();
		
    #endregion
		public ContributionFund()
		{
			
			this._BundleHeaders = new EntitySet<BundleHeader>(new Action< BundleHeader>(this.attach_BundleHeaders), new Action< BundleHeader>(this.detach_BundleHeaders)); 
			
			this._Contributions = new EntitySet<Contribution>(new Action< Contribution>(this.attach_Contributions), new Action< Contribution>(this.detach_Contributions)); 
			
			this._RecurringAmounts = new EntitySet<RecurringAmount>(new Action< RecurringAmount>(this.attach_RecurringAmounts), new Action< RecurringAmount>(this.detach_RecurringAmounts)); 
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="FundId", UpdateCheck=UpdateCheck.Never, Storage="_FundId", DbType="int NOT NULL", IsPrimaryKey=true)]
		public int FundId
		{
			get { return this._FundId; }

			set
			{
				if (this._FundId != value)
				{
				
                    this.OnFundIdChanging(value);
					this.SendPropertyChanging();
					this._FundId = value;
					this.SendPropertyChanged("FundId");
					this.OnFundIdChanged();
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

		
		[Column(Name="FundName", UpdateCheck=UpdateCheck.Never, Storage="_FundName", DbType="nvarchar(256) NOT NULL")]
		public string FundName
		{
			get { return this._FundName; }

			set
			{
				if (this._FundName != value)
				{
				
                    this.OnFundNameChanging(value);
					this.SendPropertyChanging();
					this._FundName = value;
					this.SendPropertyChanged("FundName");
					this.OnFundNameChanged();
				}

			}

		}

		
		[Column(Name="FundDescription", UpdateCheck=UpdateCheck.Never, Storage="_FundDescription", DbType="nvarchar(256)")]
		public string FundDescription
		{
			get { return this._FundDescription; }

			set
			{
				if (this._FundDescription != value)
				{
				
                    this.OnFundDescriptionChanging(value);
					this.SendPropertyChanging();
					this._FundDescription = value;
					this.SendPropertyChanged("FundDescription");
					this.OnFundDescriptionChanged();
				}

			}

		}

		
		[Column(Name="FundStatusId", UpdateCheck=UpdateCheck.Never, Storage="_FundStatusId", DbType="int NOT NULL")]
		public int FundStatusId
		{
			get { return this._FundStatusId; }

			set
			{
				if (this._FundStatusId != value)
				{
				
                    this.OnFundStatusIdChanging(value);
					this.SendPropertyChanging();
					this._FundStatusId = value;
					this.SendPropertyChanged("FundStatusId");
					this.OnFundStatusIdChanged();
				}

			}

		}

		
		[Column(Name="FundTypeId", UpdateCheck=UpdateCheck.Never, Storage="_FundTypeId", DbType="int NOT NULL")]
		public int FundTypeId
		{
			get { return this._FundTypeId; }

			set
			{
				if (this._FundTypeId != value)
				{
				
                    this.OnFundTypeIdChanging(value);
					this.SendPropertyChanging();
					this._FundTypeId = value;
					this.SendPropertyChanged("FundTypeId");
					this.OnFundTypeIdChanged();
				}

			}

		}

		
		[Column(Name="FundPledgeFlag", UpdateCheck=UpdateCheck.Never, Storage="_FundPledgeFlag", DbType="bit NOT NULL")]
		public bool FundPledgeFlag
		{
			get { return this._FundPledgeFlag; }

			set
			{
				if (this._FundPledgeFlag != value)
				{
				
                    this.OnFundPledgeFlagChanging(value);
					this.SendPropertyChanging();
					this._FundPledgeFlag = value;
					this.SendPropertyChanged("FundPledgeFlag");
					this.OnFundPledgeFlagChanged();
				}

			}

		}

		
		[Column(Name="FundAccountCode", UpdateCheck=UpdateCheck.Never, Storage="_FundAccountCode", DbType="int")]
		public int? FundAccountCode
		{
			get { return this._FundAccountCode; }

			set
			{
				if (this._FundAccountCode != value)
				{
				
                    this.OnFundAccountCodeChanging(value);
					this.SendPropertyChanging();
					this._FundAccountCode = value;
					this.SendPropertyChanged("FundAccountCode");
					this.OnFundAccountCodeChanged();
				}

			}

		}

		
		[Column(Name="FundIncomeDept", UpdateCheck=UpdateCheck.Never, Storage="_FundIncomeDept", DbType="nvarchar(25)")]
		public string FundIncomeDept
		{
			get { return this._FundIncomeDept; }

			set
			{
				if (this._FundIncomeDept != value)
				{
				
                    this.OnFundIncomeDeptChanging(value);
					this.SendPropertyChanging();
					this._FundIncomeDept = value;
					this.SendPropertyChanged("FundIncomeDept");
					this.OnFundIncomeDeptChanged();
				}

			}

		}

		
		[Column(Name="FundIncomeAccount", UpdateCheck=UpdateCheck.Never, Storage="_FundIncomeAccount", DbType="nvarchar(25)")]
		public string FundIncomeAccount
		{
			get { return this._FundIncomeAccount; }

			set
			{
				if (this._FundIncomeAccount != value)
				{
				
                    this.OnFundIncomeAccountChanging(value);
					this.SendPropertyChanging();
					this._FundIncomeAccount = value;
					this.SendPropertyChanged("FundIncomeAccount");
					this.OnFundIncomeAccountChanged();
				}

			}

		}

		
		[Column(Name="FundIncomeFund", UpdateCheck=UpdateCheck.Never, Storage="_FundIncomeFund", DbType="nvarchar(25)")]
		public string FundIncomeFund
		{
			get { return this._FundIncomeFund; }

			set
			{
				if (this._FundIncomeFund != value)
				{
				
                    this.OnFundIncomeFundChanging(value);
					this.SendPropertyChanging();
					this._FundIncomeFund = value;
					this.SendPropertyChanged("FundIncomeFund");
					this.OnFundIncomeFundChanged();
				}

			}

		}

		
		[Column(Name="FundCashDept", UpdateCheck=UpdateCheck.Never, Storage="_FundCashDept", DbType="nvarchar(25)")]
		public string FundCashDept
		{
			get { return this._FundCashDept; }

			set
			{
				if (this._FundCashDept != value)
				{
				
                    this.OnFundCashDeptChanging(value);
					this.SendPropertyChanging();
					this._FundCashDept = value;
					this.SendPropertyChanged("FundCashDept");
					this.OnFundCashDeptChanged();
				}

			}

		}

		
		[Column(Name="FundCashAccount", UpdateCheck=UpdateCheck.Never, Storage="_FundCashAccount", DbType="nvarchar(25)")]
		public string FundCashAccount
		{
			get { return this._FundCashAccount; }

			set
			{
				if (this._FundCashAccount != value)
				{
				
                    this.OnFundCashAccountChanging(value);
					this.SendPropertyChanging();
					this._FundCashAccount = value;
					this.SendPropertyChanged("FundCashAccount");
					this.OnFundCashAccountChanged();
				}

			}

		}

		
		[Column(Name="FundCashFund", UpdateCheck=UpdateCheck.Never, Storage="_FundCashFund", DbType="nvarchar(25)")]
		public string FundCashFund
		{
			get { return this._FundCashFund; }

			set
			{
				if (this._FundCashFund != value)
				{
				
                    this.OnFundCashFundChanging(value);
					this.SendPropertyChanging();
					this._FundCashFund = value;
					this.SendPropertyChanged("FundCashFund");
					this.OnFundCashFundChanged();
				}

			}

		}

		
		[Column(Name="OnlineSort", UpdateCheck=UpdateCheck.Never, Storage="_OnlineSort", DbType="int")]
		public int? OnlineSort
		{
			get { return this._OnlineSort; }

			set
			{
				if (this._OnlineSort != value)
				{
				
                    this.OnOnlineSortChanging(value);
					this.SendPropertyChanging();
					this._OnlineSort = value;
					this.SendPropertyChanged("OnlineSort");
					this.OnOnlineSortChanged();
				}

			}

		}

		
		[Column(Name="NonTaxDeductible", UpdateCheck=UpdateCheck.Never, Storage="_NonTaxDeductible", DbType="bit")]
		public bool? NonTaxDeductible
		{
			get { return this._NonTaxDeductible; }

			set
			{
				if (this._NonTaxDeductible != value)
				{
				
                    this.OnNonTaxDeductibleChanging(value);
					this.SendPropertyChanging();
					this._NonTaxDeductible = value;
					this.SendPropertyChanged("NonTaxDeductible");
					this.OnNonTaxDeductibleChanged();
				}

			}

		}

		
		[Column(Name="QBIncomeAccount", UpdateCheck=UpdateCheck.Never, Storage="_QBIncomeAccount", DbType="int NOT NULL")]
		public int QBIncomeAccount
		{
			get { return this._QBIncomeAccount; }

			set
			{
				if (this._QBIncomeAccount != value)
				{
				
                    this.OnQBIncomeAccountChanging(value);
					this.SendPropertyChanging();
					this._QBIncomeAccount = value;
					this.SendPropertyChanged("QBIncomeAccount");
					this.OnQBIncomeAccountChanged();
				}

			}

		}

		
		[Column(Name="QBAssetAccount", UpdateCheck=UpdateCheck.Never, Storage="_QBAssetAccount", DbType="int NOT NULL")]
		public int QBAssetAccount
		{
			get { return this._QBAssetAccount; }

			set
			{
				if (this._QBAssetAccount != value)
				{
				
                    this.OnQBAssetAccountChanging(value);
					this.SendPropertyChanging();
					this._QBAssetAccount = value;
					this.SendPropertyChanged("QBAssetAccount");
					this.OnQBAssetAccountChanged();
				}

			}

		}

		
		[Column(Name="FundManagerRoleId", UpdateCheck=UpdateCheck.Never, Storage="_FundManagerRoleId", DbType="int NOT NULL")]
		public int FundManagerRoleId
		{
			get { return this._FundManagerRoleId; }

			set
			{
				if (this._FundManagerRoleId != value)
				{
				
                    this.OnFundManagerRoleIdChanging(value);
					this.SendPropertyChanging();
					this._FundManagerRoleId = value;
					this.SendPropertyChanged("FundManagerRoleId");
					this.OnFundManagerRoleIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="BundleHeaders__Fund", Storage="_BundleHeaders", OtherKey="FundId")]
   		public EntitySet<BundleHeader> BundleHeaders
   		{
   		    get { return this._BundleHeaders; }

			set	{ this._BundleHeaders.Assign(value); }

   		}

		
   		[Association(Name="FK_Contribution_ContributionFund", Storage="_Contributions", OtherKey="FundId")]
   		public EntitySet<Contribution> Contributions
   		{
   		    get { return this._Contributions; }

			set	{ this._Contributions.Assign(value); }

   		}

		
   		[Association(Name="FK_RecurringAmounts_ContributionFund", Storage="_RecurringAmounts", OtherKey="FundId")]
   		public EntitySet<RecurringAmount> RecurringAmounts
   		{
   		    get { return this._RecurringAmounts; }

			set	{ this._RecurringAmounts.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
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

   		
		private void attach_BundleHeaders(BundleHeader entity)
		{
			this.SendPropertyChanging();
			entity.Fund = this;
		}

		private void detach_BundleHeaders(BundleHeader entity)
		{
			this.SendPropertyChanging();
			entity.Fund = null;
		}

		
		private void attach_Contributions(Contribution entity)
		{
			this.SendPropertyChanging();
			entity.ContributionFund = this;
		}

		private void detach_Contributions(Contribution entity)
		{
			this.SendPropertyChanging();
			entity.ContributionFund = null;
		}

		
		private void attach_RecurringAmounts(RecurringAmount entity)
		{
			this.SendPropertyChanging();
			entity.ContributionFund = this;
		}

		private void detach_RecurringAmounts(RecurringAmount entity)
		{
			this.SendPropertyChanging();
			entity.ContributionFund = null;
		}

		
	}

}

