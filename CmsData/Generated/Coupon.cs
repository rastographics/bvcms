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
	[Table(Name="dbo.Coupons")]
	public partial class Coupon : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private string _Id;
		
		private DateTime _Created;
		
		private DateTime? _Used;
		
		private DateTime? _Canceled;
		
		private decimal? _Amount;
		
		private int? _DivId;
		
		private int? _OrgId;
		
		private int? _PeopleId;
		
		private string _Name;
		
		private int? _UserId;
		
		private decimal? _RegAmount;
		
		private string _DivOrg;
		
		private bool? _MultiUse;
		
		private bool? _Generated;
		
   		
    	
		private EntityRef<Division> _Division;
		
		private EntityRef<Organization> _Organization;
		
		private EntityRef<Person> _Person;
		
		private EntityRef<User> _User;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(string value);
		partial void OnIdChanged();
		
		partial void OnCreatedChanging(DateTime value);
		partial void OnCreatedChanged();
		
		partial void OnUsedChanging(DateTime? value);
		partial void OnUsedChanged();
		
		partial void OnCanceledChanging(DateTime? value);
		partial void OnCanceledChanged();
		
		partial void OnAmountChanging(decimal? value);
		partial void OnAmountChanged();
		
		partial void OnDivIdChanging(int? value);
		partial void OnDivIdChanged();
		
		partial void OnOrgIdChanging(int? value);
		partial void OnOrgIdChanged();
		
		partial void OnPeopleIdChanging(int? value);
		partial void OnPeopleIdChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnUserIdChanging(int? value);
		partial void OnUserIdChanged();
		
		partial void OnRegAmountChanging(decimal? value);
		partial void OnRegAmountChanged();
		
		partial void OnDivOrgChanging(string value);
		partial void OnDivOrgChanged();
		
		partial void OnMultiUseChanging(bool? value);
		partial void OnMultiUseChanged();
		
		partial void OnGeneratedChanging(bool? value);
		partial void OnGeneratedChanged();
		
    #endregion
		public Coupon()
		{
			
			
			this._Division = default(EntityRef<Division>); 
			
			this._Organization = default(EntityRef<Organization>); 
			
			this._Person = default(EntityRef<Person>); 
			
			this._User = default(EntityRef<User>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="nvarchar(50) NOT NULL", IsPrimaryKey=true)]
		public string Id
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

		
		[Column(Name="Used", UpdateCheck=UpdateCheck.Never, Storage="_Used", DbType="datetime")]
		public DateTime? Used
		{
			get { return this._Used; }

			set
			{
				if (this._Used != value)
				{
				
                    this.OnUsedChanging(value);
					this.SendPropertyChanging();
					this._Used = value;
					this.SendPropertyChanged("Used");
					this.OnUsedChanged();
				}

			}

		}

		
		[Column(Name="Canceled", UpdateCheck=UpdateCheck.Never, Storage="_Canceled", DbType="datetime")]
		public DateTime? Canceled
		{
			get { return this._Canceled; }

			set
			{
				if (this._Canceled != value)
				{
				
                    this.OnCanceledChanging(value);
					this.SendPropertyChanging();
					this._Canceled = value;
					this.SendPropertyChanged("Canceled");
					this.OnCanceledChanged();
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

		
		[Column(Name="DivId", UpdateCheck=UpdateCheck.Never, Storage="_DivId", DbType="int")]
		[IsForeignKey]
		public int? DivId
		{
			get { return this._DivId; }

			set
			{
				if (this._DivId != value)
				{
				
					if (this._Division.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnDivIdChanging(value);
					this.SendPropertyChanging();
					this._DivId = value;
					this.SendPropertyChanged("DivId");
					this.OnDivIdChanged();
				}

			}

		}

		
		[Column(Name="OrgId", UpdateCheck=UpdateCheck.Never, Storage="_OrgId", DbType="int")]
		[IsForeignKey]
		public int? OrgId
		{
			get { return this._OrgId; }

			set
			{
				if (this._OrgId != value)
				{
				
					if (this._Organization.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrgIdChanging(value);
					this.SendPropertyChanging();
					this._OrgId = value;
					this.SendPropertyChanged("OrgId");
					this.OnOrgIdChanged();
				}

			}

		}

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int")]
		[IsForeignKey]
		public int? PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(80)")]
		public string Name
		{
			get { return this._Name; }

			set
			{
				if (this._Name != value)
				{
				
                    this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}

			}

		}

		
		[Column(Name="UserId", UpdateCheck=UpdateCheck.Never, Storage="_UserId", DbType="int")]
		[IsForeignKey]
		public int? UserId
		{
			get { return this._UserId; }

			set
			{
				if (this._UserId != value)
				{
				
					if (this._User.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnUserIdChanging(value);
					this.SendPropertyChanging();
					this._UserId = value;
					this.SendPropertyChanged("UserId");
					this.OnUserIdChanged();
				}

			}

		}

		
		[Column(Name="RegAmount", UpdateCheck=UpdateCheck.Never, Storage="_RegAmount", DbType="money")]
		public decimal? RegAmount
		{
			get { return this._RegAmount; }

			set
			{
				if (this._RegAmount != value)
				{
				
                    this.OnRegAmountChanging(value);
					this.SendPropertyChanging();
					this._RegAmount = value;
					this.SendPropertyChanged("RegAmount");
					this.OnRegAmountChanged();
				}

			}

		}

		
		[Column(Name="DivOrg", UpdateCheck=UpdateCheck.Never, Storage="_DivOrg", DbType="nvarchar(34)", IsDbGenerated=true)]
		public string DivOrg
		{
			get { return this._DivOrg; }

			set
			{
				if (this._DivOrg != value)
				{
				
                    this.OnDivOrgChanging(value);
					this.SendPropertyChanging();
					this._DivOrg = value;
					this.SendPropertyChanged("DivOrg");
					this.OnDivOrgChanged();
				}

			}

		}

		
		[Column(Name="MultiUse", UpdateCheck=UpdateCheck.Never, Storage="_MultiUse", DbType="bit")]
		public bool? MultiUse
		{
			get { return this._MultiUse; }

			set
			{
				if (this._MultiUse != value)
				{
				
                    this.OnMultiUseChanging(value);
					this.SendPropertyChanging();
					this._MultiUse = value;
					this.SendPropertyChanged("MultiUse");
					this.OnMultiUseChanged();
				}

			}

		}

		
		[Column(Name="Generated", UpdateCheck=UpdateCheck.Never, Storage="_Generated", DbType="bit")]
		public bool? Generated
		{
			get { return this._Generated; }

			set
			{
				if (this._Generated != value)
				{
				
                    this.OnGeneratedChanging(value);
					this.SendPropertyChanging();
					this._Generated = value;
					this.SendPropertyChanged("Generated");
					this.OnGeneratedChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_Coupons_Division", Storage="_Division", ThisKey="DivId", IsForeignKey=true)]
		public Division Division
		{
			get { return this._Division.Entity; }

			set
			{
				Division previousValue = this._Division.Entity;
				if (((previousValue != value) 
							|| (this._Division.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Division.Entity = null;
						previousValue.Coupons.Remove(this);
					}

					this._Division.Entity = value;
					if (value != null)
					{
						value.Coupons.Add(this);
						
						this._DivId = value.Id;
						
					}

					else
					{
						
						this._DivId = default(int?);
						
					}

					this.SendPropertyChanged("Division");
				}

			}

		}

		
		[Association(Name="FK_Coupons_Organizations", Storage="_Organization", ThisKey="OrgId", IsForeignKey=true)]
		public Organization Organization
		{
			get { return this._Organization.Entity; }

			set
			{
				Organization previousValue = this._Organization.Entity;
				if (((previousValue != value) 
							|| (this._Organization.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Organization.Entity = null;
						previousValue.Coupons.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.Coupons.Add(this);
						
						this._OrgId = value.OrganizationId;
						
					}

					else
					{
						
						this._OrgId = default(int?);
						
					}

					this.SendPropertyChanged("Organization");
				}

			}

		}

		
		[Association(Name="FK_Coupons_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
		public Person Person
		{
			get { return this._Person.Entity; }

			set
			{
				Person previousValue = this._Person.Entity;
				if (((previousValue != value) 
							|| (this._Person.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Person.Entity = null;
						previousValue.Coupons.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.Coupons.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int?);
						
					}

					this.SendPropertyChanged("Person");
				}

			}

		}

		
		[Association(Name="FK_Coupons_Users", Storage="_User", ThisKey="UserId", IsForeignKey=true)]
		public User User
		{
			get { return this._User.Entity; }

			set
			{
				User previousValue = this._User.Entity;
				if (((previousValue != value) 
							|| (this._User.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._User.Entity = null;
						previousValue.Coupons.Remove(this);
					}

					this._User.Entity = value;
					if (value != null)
					{
						value.Coupons.Add(this);
						
						this._UserId = value.UserId;
						
					}

					else
					{
						
						this._UserId = default(int?);
						
					}

					this.SendPropertyChanged("User");
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

