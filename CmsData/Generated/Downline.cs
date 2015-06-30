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
		
		private int? _OrgId;
		
		private int? _LeaderId;
		
		private int? _DiscipleId;
		
		private DateTime? _StartDt;
		
		private DateTime? _EndDt;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnOrgIdChanging(int? value);
		partial void OnOrgIdChanged();
		
		partial void OnLeaderIdChanging(int? value);
		partial void OnLeaderIdChanged();
		
		partial void OnDiscipleIdChanging(int? value);
		partial void OnDiscipleIdChanged();
		
		partial void OnStartDtChanging(DateTime? value);
		partial void OnStartDtChanged();
		
		partial void OnEndDtChanging(DateTime? value);
		partial void OnEndDtChanged();
		
    #endregion
		public Downline()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="OrgId", UpdateCheck=UpdateCheck.Never, Storage="_OrgId", DbType="int")]
		public int? OrgId
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

		
		[Column(Name="LeaderId", UpdateCheck=UpdateCheck.Never, Storage="_LeaderId", DbType="int")]
		public int? LeaderId
		{
			get { return this._LeaderId; }

			set
			{
				if (this._LeaderId != value)
				{
				
                    this.OnLeaderIdChanging(value);
					this.SendPropertyChanging();
					this._LeaderId = value;
					this.SendPropertyChanged("LeaderId");
					this.OnLeaderIdChanged();
				}

			}

		}

		
		[Column(Name="DiscipleId", UpdateCheck=UpdateCheck.Never, Storage="_DiscipleId", DbType="int")]
		public int? DiscipleId
		{
			get { return this._DiscipleId; }

			set
			{
				if (this._DiscipleId != value)
				{
				
                    this.OnDiscipleIdChanging(value);
					this.SendPropertyChanging();
					this._DiscipleId = value;
					this.SendPropertyChanged("DiscipleId");
					this.OnDiscipleIdChanged();
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

