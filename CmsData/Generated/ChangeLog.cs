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
	[Table(Name="dbo.ChangeLog")]
	public partial class ChangeLog : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private int? _FamilyId;
		
		private int _UserPeopleId;
		
		private DateTime _Created;
		
		private string _Field;
		
		private string _Data;
		
		private int _Id;
		
		private string _Before;
		
		private string _After;
		
   		
   		private EntitySet<ChangeDetail> _ChangeDetails;
		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnFamilyIdChanging(int? value);
		partial void OnFamilyIdChanged();
		
		partial void OnUserPeopleIdChanging(int value);
		partial void OnUserPeopleIdChanged();
		
		partial void OnCreatedChanging(DateTime value);
		partial void OnCreatedChanged();
		
		partial void OnFieldChanging(string value);
		partial void OnFieldChanged();
		
		partial void OnDataChanging(string value);
		partial void OnDataChanged();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnBeforeChanging(string value);
		partial void OnBeforeChanged();
		
		partial void OnAfterChanging(string value);
		partial void OnAfterChanged();
		
    #endregion
		public ChangeLog()
		{
			
			this._ChangeDetails = new EntitySet<ChangeDetail>(new Action< ChangeDetail>(this.attach_ChangeDetails), new Action< ChangeDetail>(this.detach_ChangeDetails)); 
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL")]
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

		
		[Column(Name="FamilyId", UpdateCheck=UpdateCheck.Never, Storage="_FamilyId", DbType="int")]
		public int? FamilyId
		{
			get { return this._FamilyId; }

			set
			{
				if (this._FamilyId != value)
				{
				
                    this.OnFamilyIdChanging(value);
					this.SendPropertyChanging();
					this._FamilyId = value;
					this.SendPropertyChanged("FamilyId");
					this.OnFamilyIdChanged();
				}

			}

		}

		
		[Column(Name="UserPeopleId", UpdateCheck=UpdateCheck.Never, Storage="_UserPeopleId", DbType="int NOT NULL")]
		public int UserPeopleId
		{
			get { return this._UserPeopleId; }

			set
			{
				if (this._UserPeopleId != value)
				{
				
                    this.OnUserPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._UserPeopleId = value;
					this.SendPropertyChanged("UserPeopleId");
					this.OnUserPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="Created", UpdateCheck=UpdateCheck.Never, Storage="_Created", DbType="datetime NOT NULL")]
		public DateTime Created
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

		
		[Column(Name="Field", UpdateCheck=UpdateCheck.Never, Storage="_Field", DbType="nvarchar(50)")]
		public string Field
		{
			get { return this._Field; }

			set
			{
				if (this._Field != value)
				{
				
                    this.OnFieldChanging(value);
					this.SendPropertyChanging();
					this._Field = value;
					this.SendPropertyChanged("Field");
					this.OnFieldChanged();
				}

			}

		}

		
		[Column(Name="Data", UpdateCheck=UpdateCheck.Never, Storage="_Data", DbType="nvarchar")]
		public string Data
		{
			get { return this._Data; }

			set
			{
				if (this._Data != value)
				{
				
                    this.OnDataChanging(value);
					this.SendPropertyChanging();
					this._Data = value;
					this.SendPropertyChanged("Data");
					this.OnDataChanged();
				}

			}

		}

		
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

		
		[Column(Name="Before", UpdateCheck=UpdateCheck.Never, Storage="_Before", DbType="nvarchar")]
		public string Before
		{
			get { return this._Before; }

			set
			{
				if (this._Before != value)
				{
				
                    this.OnBeforeChanging(value);
					this.SendPropertyChanging();
					this._Before = value;
					this.SendPropertyChanged("Before");
					this.OnBeforeChanged();
				}

			}

		}

		
		[Column(Name="After", UpdateCheck=UpdateCheck.Never, Storage="_After", DbType="nvarchar")]
		public string After
		{
			get { return this._After; }

			set
			{
				if (this._After != value)
				{
				
                    this.OnAfterChanging(value);
					this.SendPropertyChanging();
					this._After = value;
					this.SendPropertyChanged("After");
					this.OnAfterChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_ChangeDetails_ChangeLog", Storage="_ChangeDetails", OtherKey="Id")]
   		public EntitySet<ChangeDetail> ChangeDetails
   		{
   		    get { return this._ChangeDetails; }

			set	{ this._ChangeDetails.Assign(value); }

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

   		
		private void attach_ChangeDetails(ChangeDetail entity)
		{
			this.SendPropertyChanging();
			entity.ChangeLog = this;
		}

		private void detach_ChangeDetails(ChangeDetail entity)
		{
			this.SendPropertyChanging();
			entity.ChangeLog = null;
		}

		
	}

}

