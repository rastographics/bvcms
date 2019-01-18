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
		
		private DateTime? _BatchDate;
		
		private string _BatchKey;
		
		private string _TransactionId;
		
		private DateTime? _ImportDate;
		
		private int? _PeopleId;
		
		private int? _BundleHeaderId;
		
		private int? _ContributionId;
		
		private string _SettlementKey;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(long value);
		partial void OnIdChanged();
		
		partial void OnBatchDateChanging(DateTime? value);
		partial void OnBatchDateChanged();
		
		partial void OnBatchKeyChanging(string value);
		partial void OnBatchKeyChanged();
		
		partial void OnTransactionIdChanging(string value);
		partial void OnTransactionIdChanged();
		
		partial void OnImportDateChanging(DateTime? value);
		partial void OnImportDateChanged();
		
		partial void OnPeopleIdChanging(int? value);
		partial void OnPeopleIdChanged();
		
		partial void OnBundleHeaderIdChanging(int? value);
		partial void OnBundleHeaderIdChanged();
		
		partial void OnContributionIdChanging(int? value);
		partial void OnContributionIdChanged();
		
		partial void OnSettlementKeyChanging(string value);
		partial void OnSettlementKeyChanged();
		
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

		
		[Column(Name="BatchDate", UpdateCheck=UpdateCheck.Never, Storage="_BatchDate", DbType="datetime")]
		public DateTime? BatchDate
		{
			get { return this._BatchDate; }

			set
			{
				if (this._BatchDate != value)
				{
				
                    this.OnBatchDateChanging(value);
					this.SendPropertyChanging();
					this._BatchDate = value;
					this.SendPropertyChanged("BatchDate");
					this.OnBatchDateChanged();
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

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int")]
		public int? PeopleId
		{
			get { return this._PeopleId; }

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

