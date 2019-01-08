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
	[Table(Name="lookup.OrganizationType")]
	public partial class OrganizationType : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Code;
		
		private string _Description;
		
		private bool? _Hardwired;
		
   		
   		private EntitySet<Organization> _Organizations;
		
   		private EntitySet<Resource> _Resources;
		
   		private EntitySet<ResourceOrganizationType> _ResourceOrganizationTypes;
		
    	
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
		
		partial void OnHardwiredChanging(bool? value);
		partial void OnHardwiredChanged();
		
    #endregion
		public OrganizationType()
		{
			
			this._Organizations = new EntitySet<Organization>(new Action< Organization>(this.attach_Organizations), new Action< Organization>(this.detach_Organizations)); 
			
			this._Resources = new EntitySet<Resource>(new Action< Resource>(this.attach_Resources), new Action< Resource>(this.detach_Resources)); 
			
			this._ResourceOrganizationTypes = new EntitySet<ResourceOrganizationType>(new Action< ResourceOrganizationType>(this.attach_ResourceOrganizationTypes), new Action< ResourceOrganizationType>(this.detach_ResourceOrganizationTypes)); 
			
			
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

		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar(50)")]
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

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_Organizations_OrganizationType", Storage="_Organizations", OtherKey="OrganizationTypeId")]
   		public EntitySet<Organization> Organizations
   		{
   		    get { return this._Organizations; }

			set	{ this._Organizations.Assign(value); }

   		}

		
   		[Association(Name="FK_Resource_OrganizationType", Storage="_Resources", OtherKey="OrganizationTypeId")]
   		public EntitySet<Resource> Resources
   		{
   		    get { return this._Resources; }

			set	{ this._Resources.Assign(value); }

   		}

		
   		[Association(Name="FK_ResourceOrganizationType_OrganizationType", Storage="_ResourceOrganizationTypes", OtherKey="OrganizationTypeId")]
   		public EntitySet<ResourceOrganizationType> ResourceOrganizationTypes
   		{
   		    get { return this._ResourceOrganizationTypes; }

			set	{ this._ResourceOrganizationTypes.Assign(value); }

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

   		
		private void attach_Organizations(Organization entity)
		{
			this.SendPropertyChanging();
			entity.OrganizationType = this;
		}

		private void detach_Organizations(Organization entity)
		{
			this.SendPropertyChanging();
			entity.OrganizationType = null;
		}

		
		private void attach_Resources(Resource entity)
		{
			this.SendPropertyChanging();
			entity.OrganizationType = this;
		}

		private void detach_Resources(Resource entity)
		{
			this.SendPropertyChanging();
			entity.OrganizationType = null;
		}

		
		private void attach_ResourceOrganizationTypes(ResourceOrganizationType entity)
		{
			this.SendPropertyChanging();
			entity.OrganizationType = this;
		}

		private void detach_ResourceOrganizationTypes(ResourceOrganizationType entity)
		{
			this.SendPropertyChanging();
			entity.OrganizationType = null;
		}

		
	}

}

