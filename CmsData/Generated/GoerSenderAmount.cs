using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData
{
	[Table(Name="dbo.GoerSenderAmounts")]
	public partial class GoerSenderAmount : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _OrgId;
		
		private int _SupporterId;
		
		private int? _GoerId;
		
		private decimal? _Amount;
		
		private DateTime? _Created;
		
		private bool? _InActive;
		
		private bool? _NoNoticeToGoer;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnOrgIdChanging(int value);
		partial void OnOrgIdChanged();
		
		partial void OnSupporterIdChanging(int value);
		partial void OnSupporterIdChanged();
		
		partial void OnGoerIdChanging(int? value);
		partial void OnGoerIdChanged();
		
		partial void OnAmountChanging(decimal? value);
		partial void OnAmountChanged();
		
		partial void OnCreatedChanging(DateTime? value);
		partial void OnCreatedChanged();
		
		partial void OnInActiveChanging(bool? value);
		partial void OnInActiveChanged();
		
		partial void OnNoNoticeToGoerChanging(bool? value);
		partial void OnNoNoticeToGoerChanged();
		
    #endregion
		public GoerSenderAmount()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
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

		
		[Column(Name="OrgId", UpdateCheck=UpdateCheck.Never, Storage="_OrgId", DbType="int NOT NULL")]
		public int OrgId
		{
			get { return this._OrgId; }

			set
			{
				if (this._OrgId != value)
				{
				
                    this.OnOrgIdChanging(value);
					this.SendPropertyChanging();
					this._OrgId = value;
					this.SendPropertyChanged("OrgId");
					this.OnOrgIdChanged();
				}

			}

		}

		
		[Column(Name="SupporterId", UpdateCheck=UpdateCheck.Never, Storage="_SupporterId", DbType="int NOT NULL")]
		public int SupporterId
		{
			get { return this._SupporterId; }

			set
			{
				if (this._SupporterId != value)
				{
				
                    this.OnSupporterIdChanging(value);
					this.SendPropertyChanging();
					this._SupporterId = value;
					this.SendPropertyChanged("SupporterId");
					this.OnSupporterIdChanged();
				}

			}

		}

		
		[Column(Name="GoerId", UpdateCheck=UpdateCheck.Never, Storage="_GoerId", DbType="int")]
		public int? GoerId
		{
			get { return this._GoerId; }

			set
			{
				if (this._GoerId != value)
				{
				
                    this.OnGoerIdChanging(value);
					this.SendPropertyChanging();
					this._GoerId = value;
					this.SendPropertyChanged("GoerId");
					this.OnGoerIdChanged();
				}

			}

		}

		
		[Column(Name="Amount", UpdateCheck=UpdateCheck.Never, Storage="_Amount", DbType="money")]
		public decimal? Amount
		{
			get { return this._Amount; }

			set
			{
				if (this._Amount != value)
				{
				
                    this.OnAmountChanging(value);
					this.SendPropertyChanging();
					this._Amount = value;
					this.SendPropertyChanged("Amount");
					this.OnAmountChanged();
				}

			}

		}

		
		[Column(Name="Created", UpdateCheck=UpdateCheck.Never, Storage="_Created", DbType="datetime")]
		public DateTime? Created
		{
			get { return this._Created; }

			set
			{
				if (this._Created != value)
				{
				
                    this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}

			}

		}

		
		[Column(Name="InActive", UpdateCheck=UpdateCheck.Never, Storage="_InActive", DbType="bit")]
		public bool? InActive
		{
			get { return this._InActive; }

			set
			{
				if (this._InActive != value)
				{
				
                    this.OnInActiveChanging(value);
					this.SendPropertyChanging();
					this._InActive = value;
					this.SendPropertyChanged("InActive");
					this.OnInActiveChanged();
				}

			}

		}

		
		[Column(Name="NoNoticeToGoer", UpdateCheck=UpdateCheck.Never, Storage="_NoNoticeToGoer", DbType="bit")]
		public bool? NoNoticeToGoer
		{
			get { return this._NoNoticeToGoer; }

			set
			{
				if (this._NoNoticeToGoer != value)
				{
				
                    this.OnNoNoticeToGoerChanging(value);
					this.SendPropertyChanging();
					this._NoNoticeToGoer = value;
					this.SendPropertyChanged("NoNoticeToGoer");
					this.OnNoNoticeToGoerChanged();
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

