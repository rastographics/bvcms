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
		
		private int? _BatchPage;
		
		private DateTime? _BatchDate;
		
		private string _BatchKey;
		
		private string _MerchantKey;
		
		private string _TransactionId;
		
		private DateTime? _ImportDate;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(long value);
		partial void OnIdChanged();
		
		partial void OnBatchPageChanging(int? value);
		partial void OnBatchPageChanged();
		
		partial void OnBatchDateChanging(DateTime? value);
		partial void OnBatchDateChanged();
		
		partial void OnBatchKeyChanging(string value);
		partial void OnBatchKeyChanged();
		
		partial void OnMerchantKeyChanging(string value);
		partial void OnMerchantKeyChanged();
		
		partial void OnTransactionIdChanging(string value);
		partial void OnTransactionIdChanged();
		
		partial void OnImportDateChanging(DateTime? value);
		partial void OnImportDateChanged();
		
    #endregion
		public PushPayLog()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="bigint NOT NULL IDENTITY", IsDbGenerated=true)]
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

		
		[Column(Name="BatchPage", UpdateCheck=UpdateCheck.Never, Storage="_BatchPage", DbType="int")]
		public int? BatchPage
		{
			get { return this._BatchPage; }

			set
			{
				if (this._BatchPage != value)
				{
				
                    this.OnBatchPageChanging(value);
					this.SendPropertyChanging();
					this._BatchPage = value;
					this.SendPropertyChanged("BatchPage");
					this.OnBatchPageChanged();
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

