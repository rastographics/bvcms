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
	[Table(Name="dbo.PushPayLog")]
	public partial class PushPayLog : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private long _Id;
		
		private DateTime? _TransactionDate;
		
		private string _BatchKey;
		
		private string _TransactionId;
		
		private DateTime? _ImportDate;
		
		private int? _BundleHeaderId;
		
		private int? _ContributionId;
		
		private string _SettlementKey;
		
		private string _OrganizationKey;
		
		private string _MerchantKey;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(long value);
		partial void OnIdChanged();
		
		partial void OnTransactionDateChanging(DateTime? value);
		partial void OnTransactionDateChanged();
		
		partial void OnBatchKeyChanging(string value);
		partial void OnBatchKeyChanged();
		
		partial void OnTransactionIdChanging(string value);
		partial void OnTransactionIdChanged();
		
		partial void OnImportDateChanging(DateTime? value);
		partial void OnImportDateChanged();
		
		partial void OnBundleHeaderIdChanging(int? value);
		partial void OnBundleHeaderIdChanged();
		
		partial void OnContributionIdChanging(int? value);
		partial void OnContributionIdChanged();
		
		partial void OnSettlementKeyChanging(string value);
		partial void OnSettlementKeyChanged();
		
		partial void OnOrganizationKeyChanging(string value);
		partial void OnOrganizationKeyChanged();
		
		partial void OnMerchantKeyChanging(string value);
		partial void OnMerchantKeyChanged();
		
    #endregion
		public PushPayLog()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="bigint NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public long Id
		{
			get { return this._Id; }

			set
			{
				if (this._Id != value)
				{
				
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}

			}

		}

		
		[Column(Name="TransactionDate", UpdateCheck=UpdateCheck.Never, Storage="_TransactionDate", DbType="datetime")]
		public DateTime? TransactionDate
		{
			get { return this._TransactionDate; }

			set
			{
				if (this._TransactionDate != value)
				{
				
                    this.OnTransactionDateChanging(value);
					this.SendPropertyChanging();
					this._TransactionDate = value;
					this.SendPropertyChanged("TransactionDate");
					this.OnTransactionDateChanged();
				}

			}

		}

		
		[Column(Name="BatchKey", UpdateCheck=UpdateCheck.Never, Storage="_BatchKey", DbType="nvarchar(100)")]
		public string BatchKey
		{
			get { return this._BatchKey; }

			set
			{
				if (this._BatchKey != value)
				{
				
                    this.OnBatchKeyChanging(value);
					this.SendPropertyChanging();
					this._BatchKey = value;
					this.SendPropertyChanged("BatchKey");
					this.OnBatchKeyChanged();
				}

			}

		}

		
		[Column(Name="TransactionId", UpdateCheck=UpdateCheck.Never, Storage="_TransactionId", DbType="nvarchar(100)")]
		public string TransactionId
		{
			get { return this._TransactionId; }

			set
			{
				if (this._TransactionId != value)
				{
				
                    this.OnTransactionIdChanging(value);
					this.SendPropertyChanging();
					this._TransactionId = value;
					this.SendPropertyChanged("TransactionId");
					this.OnTransactionIdChanged();
				}

			}

		}

		
		[Column(Name="ImportDate", UpdateCheck=UpdateCheck.Never, Storage="_ImportDate", DbType="datetime")]
		public DateTime? ImportDate
		{
			get { return this._ImportDate; }

			set
			{
				if (this._ImportDate != value)
				{
				
                    this.OnImportDateChanging(value);
					this.SendPropertyChanging();
					this._ImportDate = value;
					this.SendPropertyChanged("ImportDate");
					this.OnImportDateChanged();
				}

			}

		}

		
		[Column(Name="BundleHeaderId", UpdateCheck=UpdateCheck.Never, Storage="_BundleHeaderId", DbType="int")]
		public int? BundleHeaderId
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

		
		[Column(Name="ContributionId", UpdateCheck=UpdateCheck.Never, Storage="_ContributionId", DbType="int")]
		public int? ContributionId
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

		
		[Column(Name="SettlementKey", UpdateCheck=UpdateCheck.Never, Storage="_SettlementKey", DbType="nvarchar(100)")]
		public string SettlementKey
		{
			get { return this._SettlementKey; }

			set
			{
				if (this._SettlementKey != value)
				{
				
                    this.OnSettlementKeyChanging(value);
					this.SendPropertyChanging();
					this._SettlementKey = value;
					this.SendPropertyChanged("SettlementKey");
					this.OnSettlementKeyChanged();
				}

			}

		}

		
		[Column(Name="OrganizationKey", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationKey", DbType="nvarchar(100)")]
		public string OrganizationKey
		{
			get { return this._OrganizationKey; }

			set
			{
				if (this._OrganizationKey != value)
				{
				
                    this.OnOrganizationKeyChanging(value);
					this.SendPropertyChanging();
					this._OrganizationKey = value;
					this.SendPropertyChanged("OrganizationKey");
					this.OnOrganizationKeyChanged();
				}

			}

		}

		
		[Column(Name="MerchantKey", UpdateCheck=UpdateCheck.Never, Storage="_MerchantKey", DbType="nvarchar(100)")]
		public string MerchantKey
		{
			get { return this._MerchantKey; }

			set
			{
				if (this._MerchantKey != value)
				{
				
                    this.OnMerchantKeyChanging(value);
					this.SendPropertyChanging();
					this._MerchantKey = value;
					this.SendPropertyChanged("MerchantKey");
					this.OnMerchantKeyChanged();
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
				this.PropertyChanging(this, emptyChangingEventArgs);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

   		
	}

}

