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
	[Table(Name="dbo.RelatedFamilies")]
	public partial class RelatedFamily : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _FamilyId;
		
		private int _RelatedFamilyId;
		
		private int _CreatedBy;
		
		private DateTime _CreatedDate;
		
		private string _FamilyRelationshipDesc;
		
		private int? _ModifiedBy;
		
		private DateTime? _ModifiedDate;
		
   		
    	
		private EntityRef<Family> _RelatedFamily1;
		
		private EntityRef<Family> _RelatedFamily2;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnFamilyIdChanging(int value);
		partial void OnFamilyIdChanged();
		
		partial void OnRelatedFamilyIdChanging(int value);
		partial void OnRelatedFamilyIdChanged();
		
		partial void OnCreatedByChanging(int value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime value);
		partial void OnCreatedDateChanged();
		
		partial void OnFamilyRelationshipDescChanging(string value);
		partial void OnFamilyRelationshipDescChanged();
		
		partial void OnModifiedByChanging(int? value);
		partial void OnModifiedByChanged();
		
		partial void OnModifiedDateChanging(DateTime? value);
		partial void OnModifiedDateChanged();
		
    #endregion
		public RelatedFamily()
		{
			
			
			this._RelatedFamily1 = default(EntityRef<Family>); 
			
			this._RelatedFamily2 = default(EntityRef<Family>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="FamilyId", UpdateCheck=UpdateCheck.Never, Storage="_FamilyId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int FamilyId
		{
			get { return this._FamilyId; }

			set
			{
				if (this._FamilyId != value)
				{
				
					if (this._RelatedFamily1.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnFamilyIdChanging(value);
					this.SendPropertyChanging();
					this._FamilyId = value;
					this.SendPropertyChanged("FamilyId");
					this.OnFamilyIdChanged();
				}

			}

		}

		
		[Column(Name="RelatedFamilyId", UpdateCheck=UpdateCheck.Never, Storage="_RelatedFamilyId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int RelatedFamilyId
		{
			get { return this._RelatedFamilyId; }

			set
			{
				if (this._RelatedFamilyId != value)
				{
				
					if (this._RelatedFamily2.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnRelatedFamilyIdChanging(value);
					this.SendPropertyChanging();
					this._RelatedFamilyId = value;
					this.SendPropertyChanged("RelatedFamilyId");
					this.OnRelatedFamilyIdChanged();
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

		
		[Column(Name="FamilyRelationshipDesc", UpdateCheck=UpdateCheck.Never, Storage="_FamilyRelationshipDesc", DbType="nvarchar(256) NOT NULL")]
		public string FamilyRelationshipDesc
		{
			get { return this._FamilyRelationshipDesc; }

			set
			{
				if (this._FamilyRelationshipDesc != value)
				{
				
                    this.OnFamilyRelationshipDescChanging(value);
					this.SendPropertyChanging();
					this._FamilyRelationshipDesc = value;
					this.SendPropertyChanged("FamilyRelationshipDesc");
					this.OnFamilyRelationshipDescChanged();
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

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="RelatedFamilies1__RelatedFamily1", Storage="_RelatedFamily1", ThisKey="FamilyId", IsForeignKey=true)]
		public Family RelatedFamily1
		{
			get { return this._RelatedFamily1.Entity; }

			set
			{
				Family previousValue = this._RelatedFamily1.Entity;
				if (((previousValue != value) 
							|| (this._RelatedFamily1.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._RelatedFamily1.Entity = null;
						previousValue.RelatedFamilies1.Remove(this);
					}

					this._RelatedFamily1.Entity = value;
					if (value != null)
					{
						value.RelatedFamilies1.Add(this);
						
						this._FamilyId = value.FamilyId;
						
					}

					else
					{
						
						this._FamilyId = default(int);
						
					}

					this.SendPropertyChanged("RelatedFamily1");
				}

			}

		}

		
		[Association(Name="RelatedFamilies2__RelatedFamily2", Storage="_RelatedFamily2", ThisKey="RelatedFamilyId", IsForeignKey=true)]
		public Family RelatedFamily2
		{
			get { return this._RelatedFamily2.Entity; }

			set
			{
				Family previousValue = this._RelatedFamily2.Entity;
				if (((previousValue != value) 
							|| (this._RelatedFamily2.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._RelatedFamily2.Entity = null;
						previousValue.RelatedFamilies2.Remove(this);
					}

					this._RelatedFamily2.Entity = value;
					if (value != null)
					{
						value.RelatedFamilies2.Add(this);
						
						this._RelatedFamilyId = value.FamilyId;
						
					}

					else
					{
						
						this._RelatedFamilyId = default(int);
						
					}

					this.SendPropertyChanged("RelatedFamily2");
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

