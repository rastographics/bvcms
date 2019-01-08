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
		
   		
    	
		private EntityRef<Organization> _Organization;
		
		private EntityRef<Person> _Goer;
		
		private EntityRef<Person> _Sender;
		
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
			
			
			this._Organization = default(EntityRef<Organization>); 
			
			this._Goer = default(EntityRef<Person>); 
			
			this._Sender = default(EntityRef<Person>); 
			
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
		[IsForeignKey]
		public int OrgId
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

		
		[Column(Name="SupporterId", UpdateCheck=UpdateCheck.Never, Storage="_SupporterId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int SupporterId
		{
			get { return this._SupporterId; }

			set
			{
				if (this._SupporterId != value)
				{
				
					if (this._Goer.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnSupporterIdChanging(value);
					this.SendPropertyChanging();
					this._SupporterId = value;
					this.SendPropertyChanged("SupporterId");
					this.OnSupporterIdChanged();
				}

			}

		}

		
		[Column(Name="GoerId", UpdateCheck=UpdateCheck.Never, Storage="_GoerId", DbType="int")]
		[IsForeignKey]
		public int? GoerId
		{
			get { return this._GoerId; }

			set
			{
				if (this._GoerId != value)
				{
				
					if (this._Sender.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
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
    	
		[Association(Name="FK_GoerSenderAmounts_Organizations", Storage="_Organization", ThisKey="OrgId", IsForeignKey=true)]
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
						previousValue.GoerSenderAmounts.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.GoerSenderAmounts.Add(this);
						
						this._OrgId = value.OrganizationId;
						
					}

					else
					{
						
						this._OrgId = default(int);
						
					}

					this.SendPropertyChanged("Organization");
				}

			}

		}

		
		[Association(Name="GoerAmounts__Goer", Storage="_Goer", ThisKey="SupporterId", IsForeignKey=true)]
		public Person Goer
		{
			get { return this._Goer.Entity; }

			set
			{
				Person previousValue = this._Goer.Entity;
				if (((previousValue != value) 
							|| (this._Goer.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Goer.Entity = null;
						previousValue.GoerAmounts.Remove(this);
					}

					this._Goer.Entity = value;
					if (value != null)
					{
						value.GoerAmounts.Add(this);
						
						this._SupporterId = value.PeopleId;
						
					}

					else
					{
						
						this._SupporterId = default(int);
						
					}

					this.SendPropertyChanged("Goer");
				}

			}

		}

		
		[Association(Name="SenderAmounts__Sender", Storage="_Sender", ThisKey="GoerId", IsForeignKey=true)]
		public Person Sender
		{
			get { return this._Sender.Entity; }

			set
			{
				Person previousValue = this._Sender.Entity;
				if (((previousValue != value) 
							|| (this._Sender.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Sender.Entity = null;
						previousValue.SenderAmounts.Remove(this);
					}

					this._Sender.Entity = value;
					if (value != null)
					{
						value.SenderAmounts.Add(this);
						
						this._GoerId = value.PeopleId;
						
					}

					else
					{
						
						this._GoerId = default(int?);
						
					}

					this.SendPropertyChanged("Sender");
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

