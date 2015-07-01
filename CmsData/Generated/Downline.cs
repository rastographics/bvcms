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
	[Table(Name="dbo.Downline")]
	public partial class Downline : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _OrgId;
		
		private int _PeopleId;
		
		private DateTime? _StartDt;
		
		private DateTime? _EndDt;
		
		private bool? _Leader;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnOrgIdChanging(int value);
		partial void OnOrgIdChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnStartDtChanging(DateTime? value);
		partial void OnStartDtChanged();
		
		partial void OnEndDtChanging(DateTime? value);
		partial void OnEndDtChanged();
		
		partial void OnLeaderChanging(bool? value);
		partial void OnLeaderChanged();
		
    #endregion
		public Downline()
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

		
		[Column(Name="OrgId", UpdateCheck=UpdateCheck.Never, Storage="_OrgId", DbType="int NOT NULL", IsPrimaryKey=true)]
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

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL", IsPrimaryKey=true)]
		public int PeopleId
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

		
		[Column(Name="StartDt", UpdateCheck=UpdateCheck.Never, Storage="_StartDt", DbType="datetime")]
		public DateTime? StartDt
		{
			get { return this._StartDt; }

			set
			{
				if (this._StartDt != value)
				{
				
                    this.OnStartDtChanging(value);
					this.SendPropertyChanging();
					this._StartDt = value;
					this.SendPropertyChanged("StartDt");
					this.OnStartDtChanged();
				}

			}

		}

		
		[Column(Name="EndDt", UpdateCheck=UpdateCheck.Never, Storage="_EndDt", DbType="datetime")]
		public DateTime? EndDt
		{
			get { return this._EndDt; }

			set
			{
				if (this._EndDt != value)
				{
				
                    this.OnEndDtChanging(value);
					this.SendPropertyChanging();
					this._EndDt = value;
					this.SendPropertyChanged("EndDt");
					this.OnEndDtChanged();
				}

			}

		}

		
		[Column(Name="Leader", UpdateCheck=UpdateCheck.Never, Storage="_Leader", DbType="bit")]
		public bool? Leader
		{
			get { return this._Leader; }

			set
			{
				if (this._Leader != value)
				{
				
                    this.OnLeaderChanging(value);
					this.SendPropertyChanging();
					this._Leader = value;
					this.SendPropertyChanged("Leader");
					this.OnLeaderChanged();
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

