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
	[Table(Name="dbo.AuditValues")]
	public partial class AuditValue : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _AuditId;
		
		private string _MemberName;
		
		private string _OldValue;
		
		private string _NewValue;
		
   		
    	
		private EntityRef<Audit> _Audit;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnAuditIdChanging(int value);
		partial void OnAuditIdChanged();
		
		partial void OnMemberNameChanging(string value);
		partial void OnMemberNameChanged();
		
		partial void OnOldValueChanging(string value);
		partial void OnOldValueChanged();
		
		partial void OnNewValueChanging(string value);
		partial void OnNewValueChanged();
		
    #endregion
		public AuditValue()
		{
			
			
			this._Audit = default(EntityRef<Audit>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="AuditId", UpdateCheck=UpdateCheck.Never, Storage="_AuditId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int AuditId
		{
			get { return this._AuditId; }

			set
			{
				if (this._AuditId != value)
				{
				
					if (this._Audit.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnAuditIdChanging(value);
					this.SendPropertyChanging();
					this._AuditId = value;
					this.SendPropertyChanged("AuditId");
					this.OnAuditIdChanged();
				}

			}

		}

		
		[Column(Name="MemberName", UpdateCheck=UpdateCheck.Never, Storage="_MemberName", DbType="nvarchar(50) NOT NULL", IsPrimaryKey=true)]
		public string MemberName
		{
			get { return this._MemberName; }

			set
			{
				if (this._MemberName != value)
				{
				
                    this.OnMemberNameChanging(value);
					this.SendPropertyChanging();
					this._MemberName = value;
					this.SendPropertyChanged("MemberName");
					this.OnMemberNameChanged();
				}

			}

		}

		
		[Column(Name="OldValue", UpdateCheck=UpdateCheck.Never, Storage="_OldValue", DbType="nvarchar")]
		public string OldValue
		{
			get { return this._OldValue; }

			set
			{
				if (this._OldValue != value)
				{
				
                    this.OnOldValueChanging(value);
					this.SendPropertyChanging();
					this._OldValue = value;
					this.SendPropertyChanged("OldValue");
					this.OnOldValueChanged();
				}

			}

		}

		
		[Column(Name="NewValue", UpdateCheck=UpdateCheck.Never, Storage="_NewValue", DbType="nvarchar")]
		public string NewValue
		{
			get { return this._NewValue; }

			set
			{
				if (this._NewValue != value)
				{
				
                    this.OnNewValueChanging(value);
					this.SendPropertyChanging();
					this._NewValue = value;
					this.SendPropertyChanged("NewValue");
					this.OnNewValueChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_AuditValues_Audits", Storage="_Audit", ThisKey="AuditId", IsForeignKey=true)]
		public Audit Audit
		{
			get { return this._Audit.Entity; }

			set
			{
				Audit previousValue = this._Audit.Entity;
				if (((previousValue != value) 
							|| (this._Audit.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Audit.Entity = null;
						previousValue.AuditValues.Remove(this);
					}

					this._Audit.Entity = value;
					if (value != null)
					{
						value.AuditValues.Add(this);
						
						this._AuditId = value.Id;
						
					}

					else
					{
						
						this._AuditId = default(int);
						
					}

					this.SendPropertyChanged("Audit");
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

   		
	}

}

