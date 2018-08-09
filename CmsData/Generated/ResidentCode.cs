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
	[Table(Name="lookup.ResidentCode")]
	public partial class ResidentCode : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Code;
		
		private string _Description;
		
		private bool? _Hardwired;
		
   		
   		private EntitySet<Zip> _Zips;
		
   		private EntitySet<Family> _ResCodeFamilies;
		
   		private EntitySet<Person> _ResCodePeople;
		
    	
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
		public ResidentCode()
		{
			
			this._Zips = new EntitySet<Zip>(new Action< Zip>(this.attach_Zips), new Action< Zip>(this.detach_Zips)); 
			
			this._ResCodeFamilies = new EntitySet<Family>(new Action< Family>(this.attach_ResCodeFamilies), new Action< Family>(this.detach_ResCodeFamilies)); 
			
			this._ResCodePeople = new EntitySet<Person>(new Action< Person>(this.attach_ResCodePeople), new Action< Person>(this.detach_ResCodePeople)); 
			
			
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
   		
   		[Association(Name="FK_Zips_ResidentCode", Storage="_Zips", OtherKey="MetroMarginalCode")]
   		public EntitySet<Zip> Zips
   		{
   		    get { return this._Zips; }

			set	{ this._Zips.Assign(value); }

   		}

		
   		[Association(Name="ResCodeFamilies__ResidentCode", Storage="_ResCodeFamilies", OtherKey="ResCodeId")]
   		public EntitySet<Family> ResCodeFamilies
   		{
   		    get { return this._ResCodeFamilies; }

			set	{ this._ResCodeFamilies.Assign(value); }

   		}

		
   		[Association(Name="ResCodePeople__ResidentCode", Storage="_ResCodePeople", OtherKey="ResCodeId")]
   		public EntitySet<Person> ResCodePeople
   		{
   		    get { return this._ResCodePeople; }

			set	{ this._ResCodePeople.Assign(value); }

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

   		
		private void attach_Zips(Zip entity)
		{
			this.SendPropertyChanging();
			entity.ResidentCode = this;
		}

		private void detach_Zips(Zip entity)
		{
			this.SendPropertyChanging();
			entity.ResidentCode = null;
		}

		
		private void attach_ResCodeFamilies(Family entity)
		{
			this.SendPropertyChanging();
			entity.ResidentCode = this;
		}

		private void detach_ResCodeFamilies(Family entity)
		{
			this.SendPropertyChanging();
			entity.ResidentCode = null;
		}

		
		private void attach_ResCodePeople(Person entity)
		{
			this.SendPropertyChanging();
			entity.ResidentCode = this;
		}

		private void detach_ResCodePeople(Person entity)
		{
			this.SendPropertyChanging();
			entity.ResidentCode = null;
		}

		
	}

}

