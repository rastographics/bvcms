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
	[Table(Name="dbo.Division")]
	public partial class Division : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Name;
		
		private int? _ProgId;
		
		private int? _SortOrder;
		
		private string _EmailMessage;
		
		private string _EmailSubject;
		
		private string _Instructions;
		
		private string _Terms;
		
		private int? _ReportLine;
		
		private bool? _NoDisplayZero;
		
   		
   		private EntitySet<Coupon> _Coupons;
		
   		private EntitySet<DivOrg> _DivOrgs;
		
   		private EntitySet<Organization> _Organizations;
		
   		private EntitySet<ProgDiv> _ProgDivs;
		
   		private EntitySet<Resource> _Resources;
		
   		private EntitySet<Promotion> _FromPromotions;
		
   		private EntitySet<Promotion> _ToPromotions;
		
    	
		private EntityRef<Program> _Program;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnProgIdChanging(int? value);
		partial void OnProgIdChanged();
		
		partial void OnSortOrderChanging(int? value);
		partial void OnSortOrderChanged();
		
		partial void OnEmailMessageChanging(string value);
		partial void OnEmailMessageChanged();
		
		partial void OnEmailSubjectChanging(string value);
		partial void OnEmailSubjectChanged();
		
		partial void OnInstructionsChanging(string value);
		partial void OnInstructionsChanged();
		
		partial void OnTermsChanging(string value);
		partial void OnTermsChanged();
		
		partial void OnReportLineChanging(int? value);
		partial void OnReportLineChanged();
		
		partial void OnNoDisplayZeroChanging(bool? value);
		partial void OnNoDisplayZeroChanged();
		
    #endregion
		public Division()
		{
			
			this._Coupons = new EntitySet<Coupon>(new Action< Coupon>(this.attach_Coupons), new Action< Coupon>(this.detach_Coupons)); 
			
			this._DivOrgs = new EntitySet<DivOrg>(new Action< DivOrg>(this.attach_DivOrgs), new Action< DivOrg>(this.detach_DivOrgs)); 
			
			this._Organizations = new EntitySet<Organization>(new Action< Organization>(this.attach_Organizations), new Action< Organization>(this.detach_Organizations)); 
			
			this._ProgDivs = new EntitySet<ProgDiv>(new Action< ProgDiv>(this.attach_ProgDivs), new Action< ProgDiv>(this.detach_ProgDivs)); 
			
			this._Resources = new EntitySet<Resource>(new Action< Resource>(this.attach_Resources), new Action< Resource>(this.detach_Resources)); 
			
			this._FromPromotions = new EntitySet<Promotion>(new Action< Promotion>(this.attach_FromPromotions), new Action< Promotion>(this.detach_FromPromotions)); 
			
			this._ToPromotions = new EntitySet<Promotion>(new Action< Promotion>(this.attach_ToPromotions), new Action< Promotion>(this.detach_ToPromotions)); 
			
			
			this._Program = default(EntityRef<Program>); 
			
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

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(50)")]
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

		
		[Column(Name="ProgId", UpdateCheck=UpdateCheck.Never, Storage="_ProgId", DbType="int")]
		[IsForeignKey]
		public int? ProgId
		{
			get { return this._ProgId; }

			set
			{
				if (this._ProgId != value)
				{
				
					if (this._Program.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnProgIdChanging(value);
					this.SendPropertyChanging();
					this._ProgId = value;
					this.SendPropertyChanged("ProgId");
					this.OnProgIdChanged();
				}

			}

		}

		
		[Column(Name="SortOrder", UpdateCheck=UpdateCheck.Never, Storage="_SortOrder", DbType="int")]
		public int? SortOrder
		{
			get { return this._SortOrder; }

			set
			{
				if (this._SortOrder != value)
				{
				
                    this.OnSortOrderChanging(value);
					this.SendPropertyChanging();
					this._SortOrder = value;
					this.SendPropertyChanged("SortOrder");
					this.OnSortOrderChanged();
				}

			}

		}

		
		[Column(Name="EmailMessage", UpdateCheck=UpdateCheck.Never, Storage="_EmailMessage", DbType="nvarchar")]
		public string EmailMessage
		{
			get { return this._EmailMessage; }

			set
			{
				if (this._EmailMessage != value)
				{
				
                    this.OnEmailMessageChanging(value);
					this.SendPropertyChanging();
					this._EmailMessage = value;
					this.SendPropertyChanged("EmailMessage");
					this.OnEmailMessageChanged();
				}

			}

		}

		
		[Column(Name="EmailSubject", UpdateCheck=UpdateCheck.Never, Storage="_EmailSubject", DbType="nvarchar(100)")]
		public string EmailSubject
		{
			get { return this._EmailSubject; }

			set
			{
				if (this._EmailSubject != value)
				{
				
                    this.OnEmailSubjectChanging(value);
					this.SendPropertyChanging();
					this._EmailSubject = value;
					this.SendPropertyChanged("EmailSubject");
					this.OnEmailSubjectChanged();
				}

			}

		}

		
		[Column(Name="Instructions", UpdateCheck=UpdateCheck.Never, Storage="_Instructions", DbType="nvarchar")]
		public string Instructions
		{
			get { return this._Instructions; }

			set
			{
				if (this._Instructions != value)
				{
				
                    this.OnInstructionsChanging(value);
					this.SendPropertyChanging();
					this._Instructions = value;
					this.SendPropertyChanged("Instructions");
					this.OnInstructionsChanged();
				}

			}

		}

		
		[Column(Name="Terms", UpdateCheck=UpdateCheck.Never, Storage="_Terms", DbType="nvarchar")]
		public string Terms
		{
			get { return this._Terms; }

			set
			{
				if (this._Terms != value)
				{
				
                    this.OnTermsChanging(value);
					this.SendPropertyChanging();
					this._Terms = value;
					this.SendPropertyChanged("Terms");
					this.OnTermsChanged();
				}

			}

		}

		
		[Column(Name="ReportLine", UpdateCheck=UpdateCheck.Never, Storage="_ReportLine", DbType="int")]
		public int? ReportLine
		{
			get { return this._ReportLine; }

			set
			{
				if (this._ReportLine != value)
				{
				
                    this.OnReportLineChanging(value);
					this.SendPropertyChanging();
					this._ReportLine = value;
					this.SendPropertyChanged("ReportLine");
					this.OnReportLineChanged();
				}

			}

		}

		
		[Column(Name="NoDisplayZero", UpdateCheck=UpdateCheck.Never, Storage="_NoDisplayZero", DbType="bit")]
		public bool? NoDisplayZero
		{
			get { return this._NoDisplayZero; }

			set
			{
				if (this._NoDisplayZero != value)
				{
				
                    this.OnNoDisplayZeroChanging(value);
					this.SendPropertyChanging();
					this._NoDisplayZero = value;
					this.SendPropertyChanged("NoDisplayZero");
					this.OnNoDisplayZeroChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_Coupons_Division", Storage="_Coupons", OtherKey="DivId")]
   		public EntitySet<Coupon> Coupons
   		{
   		    get { return this._Coupons; }

			set	{ this._Coupons.Assign(value); }

   		}

		
   		[Association(Name="FK_DivOrg_Division", Storage="_DivOrgs", OtherKey="DivId")]
   		public EntitySet<DivOrg> DivOrgs
   		{
   		    get { return this._DivOrgs; }

			set	{ this._DivOrgs.Assign(value); }

   		}

		
   		[Association(Name="FK_Organizations_Division", Storage="_Organizations", OtherKey="DivisionId")]
   		public EntitySet<Organization> Organizations
   		{
   		    get { return this._Organizations; }

			set	{ this._Organizations.Assign(value); }

   		}

		
   		[Association(Name="FK_ProgDiv_Division", Storage="_ProgDivs", OtherKey="DivId")]
   		public EntitySet<ProgDiv> ProgDivs
   		{
   		    get { return this._ProgDivs; }

			set	{ this._ProgDivs.Assign(value); }

   		}

		
   		[Association(Name="FK_Resource_Division", Storage="_Resources", OtherKey="DivisionId")]
   		public EntitySet<Resource> Resources
   		{
   		    get { return this._Resources; }

			set	{ this._Resources.Assign(value); }

   		}

		
   		[Association(Name="FromPromotions__FromDivision", Storage="_FromPromotions", OtherKey="FromDivId")]
   		public EntitySet<Promotion> FromPromotions
   		{
   		    get { return this._FromPromotions; }

			set	{ this._FromPromotions.Assign(value); }

   		}

		
   		[Association(Name="ToPromotions__ToDivision", Storage="_ToPromotions", OtherKey="ToDivId")]
   		public EntitySet<Promotion> ToPromotions
   		{
   		    get { return this._ToPromotions; }

			set	{ this._ToPromotions.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_Division_Program", Storage="_Program", ThisKey="ProgId", IsForeignKey=true)]
		public Program Program
		{
			get { return this._Program.Entity; }

			set
			{
				Program previousValue = this._Program.Entity;
				if (((previousValue != value) 
							|| (this._Program.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Program.Entity = null;
						previousValue.Divisions.Remove(this);
					}

					this._Program.Entity = value;
					if (value != null)
					{
						value.Divisions.Add(this);
						
						this._ProgId = value.Id;
						
					}

					else
					{
						
						this._ProgId = default(int?);
						
					}

					this.SendPropertyChanged("Program");
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

   		
		private void attach_Coupons(Coupon entity)
		{
			this.SendPropertyChanging();
			entity.Division = this;
		}

		private void detach_Coupons(Coupon entity)
		{
			this.SendPropertyChanging();
			entity.Division = null;
		}

		
		private void attach_DivOrgs(DivOrg entity)
		{
			this.SendPropertyChanging();
			entity.Division = this;
		}

		private void detach_DivOrgs(DivOrg entity)
		{
			this.SendPropertyChanging();
			entity.Division = null;
		}

		
		private void attach_Organizations(Organization entity)
		{
			this.SendPropertyChanging();
			entity.Division = this;
		}

		private void detach_Organizations(Organization entity)
		{
			this.SendPropertyChanging();
			entity.Division = null;
		}

		
		private void attach_ProgDivs(ProgDiv entity)
		{
			this.SendPropertyChanging();
			entity.Division = this;
		}

		private void detach_ProgDivs(ProgDiv entity)
		{
			this.SendPropertyChanging();
			entity.Division = null;
		}

		
		private void attach_Resources(Resource entity)
		{
			this.SendPropertyChanging();
			entity.Division = this;
		}

		private void detach_Resources(Resource entity)
		{
			this.SendPropertyChanging();
			entity.Division = null;
		}

		
		private void attach_FromPromotions(Promotion entity)
		{
			this.SendPropertyChanging();
			entity.FromDivision = this;
		}

		private void detach_FromPromotions(Promotion entity)
		{
			this.SendPropertyChanging();
			entity.FromDivision = null;
		}

		
		private void attach_ToPromotions(Promotion entity)
		{
			this.SendPropertyChanging();
			entity.ToDivision = this;
		}

		private void detach_ToPromotions(Promotion entity)
		{
			this.SendPropertyChanging();
			entity.ToDivision = null;
		}

		
	}

}

