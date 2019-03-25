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
	[Table(Name="dbo.BundleDetail")]
	public partial class BundleDetail : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _BundleDetailId;
		
		private int _BundleHeaderId;
		
		private int _ContributionId;
		
		private int _CreatedBy;
		
		private DateTime _CreatedDate;
		
		private int? _BundleSort1;
		
		private int? _RefOrgId;
		
		private int? _RefPeopleId;
		
   		
    	
		private EntityRef<BundleHeader> _BundleHeader;
		
		private EntityRef<Contribution> _Contribution;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnBundleDetailIdChanging(int value);
		partial void OnBundleDetailIdChanged();
		
		partial void OnBundleHeaderIdChanging(int value);
		partial void OnBundleHeaderIdChanged();
		
		partial void OnContributionIdChanging(int value);
		partial void OnContributionIdChanged();
		
		partial void OnCreatedByChanging(int value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime value);
		partial void OnCreatedDateChanged();
		
		partial void OnBundleSort1Changing(int? value);
		partial void OnBundleSort1Changed();
		
		partial void OnRefOrgIdChanging(int? value);
		partial void OnRefOrgIdChanged();
		
		partial void OnRefPeopleIdChanging(int? value);
		partial void OnRefPeopleIdChanged();
		
    #endregion
		public BundleDetail()
		{
			
			
			this._BundleHeader = default(EntityRef<BundleHeader>); 
			
			this._Contribution = default(EntityRef<Contribution>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="BundleDetailId", UpdateCheck=UpdateCheck.Never, Storage="_BundleDetailId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int BundleDetailId
		{
			get { return this._BundleDetailId; }

			set
			{
				if (this._BundleDetailId != value)
				{
				
                    this.OnBundleDetailIdChanging(value);
					this.SendPropertyChanging();
					this._BundleDetailId = value;
					this.SendPropertyChanged("BundleDetailId");
					this.OnBundleDetailIdChanged();
				}

			}

		}

		
		[Column(Name="BundleHeaderId", UpdateCheck=UpdateCheck.Never, Storage="_BundleHeaderId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int BundleHeaderId
		{
			get { return this._BundleHeaderId; }

			set
			{
				if (this._BundleHeaderId != value)
				{
				
					if (this._BundleHeader.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnBundleHeaderIdChanging(value);
					this.SendPropertyChanging();
					this._BundleHeaderId = value;
					this.SendPropertyChanged("BundleHeaderId");
					this.OnBundleHeaderIdChanged();
				}

			}

		}

		
		[Column(Name="ContributionId", UpdateCheck=UpdateCheck.Never, Storage="_ContributionId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int ContributionId
		{
			get { return this._ContributionId; }

			set
			{
				if (this._ContributionId != value)
				{
				
					if (this._Contribution.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnContributionIdChanging(value);
					this.SendPropertyChanging();
					this._ContributionId = value;
					this.SendPropertyChanged("ContributionId");
					this.OnContributionIdChanged();
				}

			}

		}

		
		[Column(Name="CreatedBy", UpdateCheck=UpdateCheck.Never, Storage="_CreatedBy", DbType="int NOT NULL")]
		public int CreatedBy
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

		
		[Column(Name="CreatedDate", UpdateCheck=UpdateCheck.Never, Storage="_CreatedDate", DbType="datetime NOT NULL")]
		public DateTime CreatedDate
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

		
		[Column(Name="BundleSort1", UpdateCheck=UpdateCheck.Never, Storage="_BundleSort1", DbType="int")]
		public int? BundleSort1
		{
			get { return this._BundleSort1; }

			set
			{
				if (this._BundleSort1 != value)
				{
				
                    this.OnBundleSort1Changing(value);
					this.SendPropertyChanging();
					this._BundleSort1 = value;
					this.SendPropertyChanged("BundleSort1");
					this.OnBundleSort1Changed();
				}

			}

		}

		
		[Column(Name="RefOrgId", UpdateCheck=UpdateCheck.Never, Storage="_RefOrgId", DbType="int")]
		public int? RefOrgId
		{
			get { return this._RefOrgId; }

			set
			{
				if (this._RefOrgId != value)
				{
				
                    this.OnRefOrgIdChanging(value);
					this.SendPropertyChanging();
					this._RefOrgId = value;
					this.SendPropertyChanged("RefOrgId");
					this.OnRefOrgIdChanged();
				}

			}

		}

		
		[Column(Name="RefPeopleId", UpdateCheck=UpdateCheck.Never, Storage="_RefPeopleId", DbType="int")]
		public int? RefPeopleId
		{
			get { return this._RefPeopleId; }

			set
			{
				if (this._RefPeopleId != value)
				{
				
                    this.OnRefPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._RefPeopleId = value;
					this.SendPropertyChanged("RefPeopleId");
					this.OnRefPeopleIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="BUNDLE_DETAIL_BUNDLE_FK", Storage="_BundleHeader", ThisKey="BundleHeaderId", IsForeignKey=true)]
		public BundleHeader BundleHeader
		{
			get { return this._BundleHeader.Entity; }

			set
			{
				BundleHeader previousValue = this._BundleHeader.Entity;
				if (((previousValue != value) 
							|| (this._BundleHeader.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._BundleHeader.Entity = null;
						previousValue.BundleDetails.Remove(this);
					}

					this._BundleHeader.Entity = value;
					if (value != null)
					{
						value.BundleDetails.Add(this);
						
						this._BundleHeaderId = value.BundleHeaderId;
						
					}

					else
					{
						
						this._BundleHeaderId = default(int);
						
					}

					this.SendPropertyChanged("BundleHeader");
				}

			}

		}

		
		[Association(Name="BUNDLE_DETAIL_CONTR_FK", Storage="_Contribution", ThisKey="ContributionId", IsForeignKey=true)]
		public Contribution Contribution
		{
			get { return this._Contribution.Entity; }

			set
			{
				Contribution previousValue = this._Contribution.Entity;
				if (((previousValue != value) 
							|| (this._Contribution.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Contribution.Entity = null;
						previousValue.BundleDetails.Remove(this);
					}

					this._Contribution.Entity = value;
					if (value != null)
					{
						value.BundleDetails.Add(this);
						
						this._ContributionId = value.ContributionId;
						
					}

					else
					{
						
						this._ContributionId = default(int);
						
					}

					this.SendPropertyChanged("Contribution");
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

