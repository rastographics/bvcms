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
	[Table(Name="lookup.EnvelopeOption")]
	public partial class EnvelopeOption : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Code;
		
		private string _Description;
		
		private bool? _Hardwired;
		
   		
   		private EntitySet<Person> _EnvPeople;
		
   		private EntitySet<Person> _StmtPeople;
		
    	
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
		public EnvelopeOption()
		{
			
			this._EnvPeople = new EntitySet<Person>(new Action< Person>(this.attach_EnvPeople), new Action< Person>(this.detach_EnvPeople)); 
			
			this._StmtPeople = new EntitySet<Person>(new Action< Person>(this.attach_StmtPeople), new Action< Person>(this.detach_StmtPeople)); 
			
			
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
   		
   		[Association(Name="EnvPeople__EnvelopeOption", Storage="_EnvPeople", OtherKey="EnvelopeOptionsId")]
   		public EntitySet<Person> EnvPeople
   		{
   		    get { return this._EnvPeople; }

			set	{ this._EnvPeople.Assign(value); }

   		}

		
   		[Association(Name="StmtPeople__ContributionStatementOption", Storage="_StmtPeople", OtherKey="ContributionOptionsId")]
   		public EntitySet<Person> StmtPeople
   		{
   		    get { return this._StmtPeople; }

			set	{ this._StmtPeople.Assign(value); }

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

   		
		private void attach_EnvPeople(Person entity)
		{
			this.SendPropertyChanging();
			entity.EnvelopeOption = this;
		}

		private void detach_EnvPeople(Person entity)
		{
			this.SendPropertyChanging();
			entity.EnvelopeOption = null;
		}

		
		private void attach_StmtPeople(Person entity)
		{
			this.SendPropertyChanging();
			entity.ContributionStatementOption = this;
		}

		private void detach_StmtPeople(Person entity)
		{
			this.SendPropertyChanging();
			entity.ContributionStatementOption = null;
		}

		
	}

}

