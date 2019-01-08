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
	[Table(Name="lookup.FamilyPosition")]
	public partial class FamilyPosition : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Code;
		
		private string _Description;
		
		private bool? _Hardwired;
		
		private int _PrimaryAdult;
		
		private int _SecondaryAdult;
		
		private int _Child;
		
   		
   		private EntitySet<Person> _People;
		
    	
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
		
		partial void OnPrimaryAdultChanging(int value);
		partial void OnPrimaryAdultChanged();
		
		partial void OnSecondaryAdultChanging(int value);
		partial void OnSecondaryAdultChanged();
		
		partial void OnChildChanging(int value);
		partial void OnChildChanged();
		
    #endregion
		public FamilyPosition()
		{
			
			this._People = new EntitySet<Person>(new Action< Person>(this.attach_People), new Action< Person>(this.detach_People)); 
			
			
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

		
		[Column(Name="PrimaryAdult", UpdateCheck=UpdateCheck.Never, Storage="_PrimaryAdult", DbType="int NOT NULL")]
		public int PrimaryAdult
		{
			get { return this._PrimaryAdult; }

			set
			{
				if (this._PrimaryAdult != value)
				{
				
                    this.OnPrimaryAdultChanging(value);
					this.SendPropertyChanging();
					this._PrimaryAdult = value;
					this.SendPropertyChanged("PrimaryAdult");
					this.OnPrimaryAdultChanged();
				}

			}

		}

		
		[Column(Name="SecondaryAdult", UpdateCheck=UpdateCheck.Never, Storage="_SecondaryAdult", DbType="int NOT NULL")]
		public int SecondaryAdult
		{
			get { return this._SecondaryAdult; }

			set
			{
				if (this._SecondaryAdult != value)
				{
				
                    this.OnSecondaryAdultChanging(value);
					this.SendPropertyChanging();
					this._SecondaryAdult = value;
					this.SendPropertyChanged("SecondaryAdult");
					this.OnSecondaryAdultChanged();
				}

			}

		}

		
		[Column(Name="Child", UpdateCheck=UpdateCheck.Never, Storage="_Child", DbType="int NOT NULL")]
		public int Child
		{
			get { return this._Child; }

			set
			{
				if (this._Child != value)
				{
				
                    this.OnChildChanging(value);
					this.SendPropertyChanging();
					this._Child = value;
					this.SendPropertyChanged("Child");
					this.OnChildChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_People_FamilyPosition", Storage="_People", OtherKey="PositionInFamilyId")]
   		public EntitySet<Person> People
   		{
   		    get { return this._People; }

			set	{ this._People.Assign(value); }

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

   		
		private void attach_People(Person entity)
		{
			this.SendPropertyChanging();
			entity.FamilyPosition = this;
		}

		private void detach_People(Person entity)
		{
			this.SendPropertyChanging();
			entity.FamilyPosition = null;
		}

		
	}

}

