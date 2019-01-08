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
	[Table(Name="lookup.MemberType")]
	public partial class MemberType : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Code;
		
		private string _Description;
		
		private int? _AttendanceTypeId;
		
		private bool? _Hardwired;
		
		private bool _Pending;
		
		private bool _Inactive;
		
   		
   		private EntitySet<Attend> _Attends;
		
   		private EntitySet<EnrollmentTransaction> _EnrollmentTransactions;
		
   		private EntitySet<OrganizationMember> _OrganizationMembers;
		
    	
		private EntityRef<AttendType> _AttendType;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnCodeChanging(string value);
		partial void OnCodeChanged();
		
		partial void OnDescriptionChanging(string value);
		partial void OnDescriptionChanged();
		
		partial void OnAttendanceTypeIdChanging(int? value);
		partial void OnAttendanceTypeIdChanged();
		
		partial void OnHardwiredChanging(bool? value);
		partial void OnHardwiredChanged();
		
		partial void OnPendingChanging(bool value);
		partial void OnPendingChanged();
		
		partial void OnInactiveChanging(bool value);
		partial void OnInactiveChanged();
		
    #endregion
		public MemberType()
		{
			
			this._Attends = new EntitySet<Attend>(new Action< Attend>(this.attach_Attends), new Action< Attend>(this.detach_Attends)); 
			
			this._EnrollmentTransactions = new EntitySet<EnrollmentTransaction>(new Action< EnrollmentTransaction>(this.attach_EnrollmentTransactions), new Action< EnrollmentTransaction>(this.detach_EnrollmentTransactions)); 
			
			this._OrganizationMembers = new EntitySet<OrganizationMember>(new Action< OrganizationMember>(this.attach_OrganizationMembers), new Action< OrganizationMember>(this.detach_OrganizationMembers)); 
			
			
			this._AttendType = default(EntityRef<AttendType>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="int NOT NULL", IsPrimaryKey=true)]
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

		
		[Column(Name="Code", UpdateCheck=UpdateCheck.Never, Storage="_Code", DbType="nvarchar(20)")]
		public string Code
		{
			get { return this._Code; }

			set
			{
				if (this._Code != value)
				{
				
                    this.OnCodeChanging(value);
					this.SendPropertyChanging();
					this._Code = value;
					this.SendPropertyChanged("Code");
					this.OnCodeChanged();
				}

			}

		}

		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar(100)")]
		public string Description
		{
			get { return this._Description; }

			set
			{
				if (this._Description != value)
				{
				
                    this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}

			}

		}

		
		[Column(Name="AttendanceTypeId", UpdateCheck=UpdateCheck.Never, Storage="_AttendanceTypeId", DbType="int")]
		[IsForeignKey]
		public int? AttendanceTypeId
		{
			get { return this._AttendanceTypeId; }

			set
			{
				if (this._AttendanceTypeId != value)
				{
				
					if (this._AttendType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnAttendanceTypeIdChanging(value);
					this.SendPropertyChanging();
					this._AttendanceTypeId = value;
					this.SendPropertyChanged("AttendanceTypeId");
					this.OnAttendanceTypeIdChanged();
				}

			}

		}

		
		[Column(Name="Hardwired", UpdateCheck=UpdateCheck.Never, Storage="_Hardwired", DbType="bit")]
		public bool? Hardwired
		{
			get { return this._Hardwired; }

			set
			{
				if (this._Hardwired != value)
				{
				
                    this.OnHardwiredChanging(value);
					this.SendPropertyChanging();
					this._Hardwired = value;
					this.SendPropertyChanged("Hardwired");
					this.OnHardwiredChanged();
				}

			}

		}

		
		[Column(Name="Pending", UpdateCheck=UpdateCheck.Never, Storage="_Pending", DbType="bit NOT NULL")]
		public bool Pending
		{
			get { return this._Pending; }

			set
			{
				if (this._Pending != value)
				{
				
                    this.OnPendingChanging(value);
					this.SendPropertyChanging();
					this._Pending = value;
					this.SendPropertyChanged("Pending");
					this.OnPendingChanged();
				}

			}

		}

		
		[Column(Name="Inactive", UpdateCheck=UpdateCheck.Never, Storage="_Inactive", DbType="bit NOT NULL")]
		public bool Inactive
		{
			get { return this._Inactive; }

			set
			{
				if (this._Inactive != value)
				{
				
                    this.OnInactiveChanging(value);
					this.SendPropertyChanging();
					this._Inactive = value;
					this.SendPropertyChanged("Inactive");
					this.OnInactiveChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_Attend_MemberType", Storage="_Attends", OtherKey="MemberTypeId")]
   		public EntitySet<Attend> Attends
   		{
   		    get { return this._Attends; }

			set	{ this._Attends.Assign(value); }

   		}

		
   		[Association(Name="FK_ENROLLMENT_TRANSACTION_TBL_MemberType", Storage="_EnrollmentTransactions", OtherKey="MemberTypeId")]
   		public EntitySet<EnrollmentTransaction> EnrollmentTransactions
   		{
   		    get { return this._EnrollmentTransactions; }

			set	{ this._EnrollmentTransactions.Assign(value); }

   		}

		
   		[Association(Name="FK_ORGANIZATION_MEMBERS_TBL_MemberType", Storage="_OrganizationMembers", OtherKey="MemberTypeId")]
   		public EntitySet<OrganizationMember> OrganizationMembers
   		{
   		    get { return this._OrganizationMembers; }

			set	{ this._OrganizationMembers.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_MemberType_AttendType", Storage="_AttendType", ThisKey="AttendanceTypeId", IsForeignKey=true)]
		public AttendType AttendType
		{
			get { return this._AttendType.Entity; }

			set
			{
				AttendType previousValue = this._AttendType.Entity;
				if (((previousValue != value) 
							|| (this._AttendType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._AttendType.Entity = null;
						previousValue.MemberTypes.Remove(this);
					}

					this._AttendType.Entity = value;
					if (value != null)
					{
						value.MemberTypes.Add(this);
						
						this._AttendanceTypeId = value.Id;
						
					}

					else
					{
						
						this._AttendanceTypeId = default(int?);
						
					}

					this.SendPropertyChanged("AttendType");
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

   		
		private void attach_Attends(Attend entity)
		{
			this.SendPropertyChanging();
			entity.MemberType = this;
		}

		private void detach_Attends(Attend entity)
		{
			this.SendPropertyChanging();
			entity.MemberType = null;
		}

		
		private void attach_EnrollmentTransactions(EnrollmentTransaction entity)
		{
			this.SendPropertyChanging();
			entity.MemberType = this;
		}

		private void detach_EnrollmentTransactions(EnrollmentTransaction entity)
		{
			this.SendPropertyChanging();
			entity.MemberType = null;
		}

		
		private void attach_OrganizationMembers(OrganizationMember entity)
		{
			this.SendPropertyChanging();
			entity.MemberType = this;
		}

		private void detach_OrganizationMembers(OrganizationMember entity)
		{
			this.SendPropertyChanging();
			entity.MemberType = null;
		}

		
	}

}

