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
	[Table(Name="dbo.Ministries")]
	public partial class Ministry : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _MinistryId;
		
		private string _MinistryName;
		
		private int? _CreatedBy;
		
		private DateTime? _CreatedDate;
		
		private int? _ModifiedBy;
		
		private DateTime? _ModifiedDate;
		
		private bool? _RecordStatus;
		
		private int? _DepartmentId;
		
		private string _MinistryDescription;
		
		private int? _MinistryContactId;
		
		private int? _ChurchId;
		
   		
   		private EntitySet<Contact> _Contacts;
		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnMinistryIdChanging(int value);
		partial void OnMinistryIdChanged();
		
		partial void OnMinistryNameChanging(string value);
		partial void OnMinistryNameChanged();
		
		partial void OnCreatedByChanging(int? value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime? value);
		partial void OnCreatedDateChanged();
		
		partial void OnModifiedByChanging(int? value);
		partial void OnModifiedByChanged();
		
		partial void OnModifiedDateChanging(DateTime? value);
		partial void OnModifiedDateChanged();
		
		partial void OnRecordStatusChanging(bool? value);
		partial void OnRecordStatusChanged();
		
		partial void OnDepartmentIdChanging(int? value);
		partial void OnDepartmentIdChanged();
		
		partial void OnMinistryDescriptionChanging(string value);
		partial void OnMinistryDescriptionChanged();
		
		partial void OnMinistryContactIdChanging(int? value);
		partial void OnMinistryContactIdChanged();
		
		partial void OnChurchIdChanging(int? value);
		partial void OnChurchIdChanged();
		
    #endregion
		public Ministry()
		{
			
			this._Contacts = new EntitySet<Contact>(new Action< Contact>(this.attach_Contacts), new Action< Contact>(this.detach_Contacts)); 
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="MinistryId", UpdateCheck=UpdateCheck.Never, Storage="_MinistryId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int MinistryId
		{
			get { return this._MinistryId; }

			set
			{
				if (this._MinistryId != value)
				{
				
                    this.OnMinistryIdChanging(value);
					this.SendPropertyChanging();
					this._MinistryId = value;
					this.SendPropertyChanged("MinistryId");
					this.OnMinistryIdChanged();
				}

			}

		}

		
		[Column(Name="MinistryName", UpdateCheck=UpdateCheck.Never, Storage="_MinistryName", DbType="nvarchar(50)")]
		public string MinistryName
		{
			get { return this._MinistryName; }

			set
			{
				if (this._MinistryName != value)
				{
				
                    this.OnMinistryNameChanging(value);
					this.SendPropertyChanging();
					this._MinistryName = value;
					this.SendPropertyChanged("MinistryName");
					this.OnMinistryNameChanged();
				}

			}

		}

		
		[Column(Name="CreatedBy", UpdateCheck=UpdateCheck.Never, Storage="_CreatedBy", DbType="int")]
		public int? CreatedBy
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

		
		[Column(Name="CreatedDate", UpdateCheck=UpdateCheck.Never, Storage="_CreatedDate", DbType="datetime")]
		public DateTime? CreatedDate
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

		
		[Column(Name="RecordStatus", UpdateCheck=UpdateCheck.Never, Storage="_RecordStatus", DbType="bit")]
		public bool? RecordStatus
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

		
		[Column(Name="DepartmentId", UpdateCheck=UpdateCheck.Never, Storage="_DepartmentId", DbType="int")]
		public int? DepartmentId
		{
			get { return this._DepartmentId; }

			set
			{
				if (this._DepartmentId != value)
				{
				
                    this.OnDepartmentIdChanging(value);
					this.SendPropertyChanging();
					this._DepartmentId = value;
					this.SendPropertyChanged("DepartmentId");
					this.OnDepartmentIdChanged();
				}

			}

		}

		
		[Column(Name="MinistryDescription", UpdateCheck=UpdateCheck.Never, Storage="_MinistryDescription", DbType="nvarchar(512)")]
		public string MinistryDescription
		{
			get { return this._MinistryDescription; }

			set
			{
				if (this._MinistryDescription != value)
				{
				
                    this.OnMinistryDescriptionChanging(value);
					this.SendPropertyChanging();
					this._MinistryDescription = value;
					this.SendPropertyChanged("MinistryDescription");
					this.OnMinistryDescriptionChanged();
				}

			}

		}

		
		[Column(Name="MinistryContactId", UpdateCheck=UpdateCheck.Never, Storage="_MinistryContactId", DbType="int")]
		public int? MinistryContactId
		{
			get { return this._MinistryContactId; }

			set
			{
				if (this._MinistryContactId != value)
				{
				
                    this.OnMinistryContactIdChanging(value);
					this.SendPropertyChanging();
					this._MinistryContactId = value;
					this.SendPropertyChanged("MinistryContactId");
					this.OnMinistryContactIdChanged();
				}

			}

		}

		
		[Column(Name="ChurchId", UpdateCheck=UpdateCheck.Never, Storage="_ChurchId", DbType="int")]
		public int? ChurchId
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

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_Contacts_Ministries", Storage="_Contacts", OtherKey="MinistryId")]
   		public EntitySet<Contact> Contacts
   		{
   		    get { return this._Contacts; }

			set	{ this._Contacts.Assign(value); }

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

   		
		private void attach_Contacts(Contact entity)
		{
			this.SendPropertyChanging();
			entity.Ministry = this;
		}

		private void detach_Contacts(Contact entity)
		{
			this.SendPropertyChanging();
			entity.Ministry = null;
		}

		
	}

}

