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
	[Table(Name="dbo.DivOrg")]
	public partial class DivOrg : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _DivId;
		
		private int _OrgId;
		
		private int? _Id;
		
   		
    	
		private EntityRef<Division> _Division;
		
		private EntityRef<Organization> _Organization;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnDivIdChanging(int value);
		partial void OnDivIdChanged();
		
		partial void OnOrgIdChanging(int value);
		partial void OnOrgIdChanged();
		
		partial void OnIdChanging(int? value);
		partial void OnIdChanged();
		
    #endregion
		public DivOrg()
		{
			
			
			this._Division = default(EntityRef<Division>); 
			
			this._Organization = default(EntityRef<Organization>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="DivId", UpdateCheck=UpdateCheck.Never, Storage="_DivId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int DivId
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

		
		[Column(Name="OrgId", UpdateCheck=UpdateCheck.Never, Storage="_OrgId", DbType="int NOT NULL", IsPrimaryKey=true)]
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

		
		[Column(Name="id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="int")]
		public int? Id
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

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_DivOrg_Division", Storage="_Division", ThisKey="DivId", IsForeignKey=true)]
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
						previousValue.DivOrgs.Remove(this);
					}

					this._Division.Entity = value;
					if (value != null)
					{
						value.DivOrgs.Add(this);
						
						this._DivId = value.Id;
						
					}

					else
					{
						
						this._DivId = default(int);
						
					}

					this.SendPropertyChanged("Division");
				}

			}

		}

		
		[Association(Name="FK_DivOrg_Organizations", Storage="_Organization", ThisKey="OrgId", IsForeignKey=true)]
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
						previousValue.DivOrgs.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.DivOrgs.Add(this);
						
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

