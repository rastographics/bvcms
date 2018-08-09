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
	[Table(Name="dbo.Audits")]
	public partial class Audit : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Action;
		
		private string _TableName;
		
		private int? _TableKey;
		
		private string _UserName;
		
		private DateTime _AuditDate;
		
   		
   		private EntitySet<AuditValue> _AuditValues;
		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnActionChanging(string value);
		partial void OnActionChanged();
		
		partial void OnTableNameChanging(string value);
		partial void OnTableNameChanged();
		
		partial void OnTableKeyChanging(int? value);
		partial void OnTableKeyChanged();
		
		partial void OnUserNameChanging(string value);
		partial void OnUserNameChanged();
		
		partial void OnAuditDateChanging(DateTime value);
		partial void OnAuditDateChanged();
		
    #endregion
		public Audit()
		{
			
			this._AuditValues = new EntitySet<AuditValue>(new Action< AuditValue>(this.attach_AuditValues), new Action< AuditValue>(this.detach_AuditValues)); 
			
			
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

		
		[Column(Name="Action", UpdateCheck=UpdateCheck.Never, Storage="_Action", DbType="nvarchar(20) NOT NULL")]
		public string Action
		{
			get { return this._Action; }

			set
			{
				if (this._Action != value)
				{
				
                    this.OnActionChanging(value);
					this.SendPropertyChanging();
					this._Action = value;
					this.SendPropertyChanged("Action");
					this.OnActionChanged();
				}

			}

		}

		
		[Column(Name="TableName", UpdateCheck=UpdateCheck.Never, Storage="_TableName", DbType="nvarchar(100) NOT NULL")]
		public string TableName
		{
			get { return this._TableName; }

			set
			{
				if (this._TableName != value)
				{
				
                    this.OnTableNameChanging(value);
					this.SendPropertyChanging();
					this._TableName = value;
					this.SendPropertyChanged("TableName");
					this.OnTableNameChanged();
				}

			}

		}

		
		[Column(Name="TableKey", UpdateCheck=UpdateCheck.Never, Storage="_TableKey", DbType="int")]
		public int? TableKey
		{
			get { return this._TableKey; }

			set
			{
				if (this._TableKey != value)
				{
				
                    this.OnTableKeyChanging(value);
					this.SendPropertyChanging();
					this._TableKey = value;
					this.SendPropertyChanged("TableKey");
					this.OnTableKeyChanged();
				}

			}

		}

		
		[Column(Name="UserName", UpdateCheck=UpdateCheck.Never, Storage="_UserName", DbType="nvarchar(50) NOT NULL")]
		public string UserName
		{
			get { return this._UserName; }

			set
			{
				if (this._UserName != value)
				{
				
                    this.OnUserNameChanging(value);
					this.SendPropertyChanging();
					this._UserName = value;
					this.SendPropertyChanged("UserName");
					this.OnUserNameChanged();
				}

			}

		}

		
		[Column(Name="AuditDate", UpdateCheck=UpdateCheck.Never, Storage="_AuditDate", DbType="smalldatetime NOT NULL")]
		public DateTime AuditDate
		{
			get { return this._AuditDate; }

			set
			{
				if (this._AuditDate != value)
				{
				
                    this.OnAuditDateChanging(value);
					this.SendPropertyChanging();
					this._AuditDate = value;
					this.SendPropertyChanged("AuditDate");
					this.OnAuditDateChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_AuditValues_Audits", Storage="_AuditValues", OtherKey="AuditId")]
   		public EntitySet<AuditValue> AuditValues
   		{
   		    get { return this._AuditValues; }

			set	{ this._AuditValues.Assign(value); }

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

   		
		private void attach_AuditValues(AuditValue entity)
		{
			this.SendPropertyChanging();
			entity.Audit = this;
		}

		private void detach_AuditValues(AuditValue entity)
		{
			this.SendPropertyChanging();
			entity.Audit = null;
		}

		
	}

}

