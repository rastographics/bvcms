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
	[Table(Name="lookup.MemberStatus")]
	public partial class MemberStatus : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Code;
		
		private string _Description;
		
		private bool? _Hardwired;
		
		private bool _Member;
		
		private bool _Previous;
		
		private bool _Pending;
		
   		
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
		
		partial void OnMemberChanging(bool value);
		partial void OnMemberChanged();
		
		partial void OnPreviousChanging(bool value);
		partial void OnPreviousChanged();
		
		partial void OnPendingChanging(bool value);
		partial void OnPendingChanged();
		
    #endregion
		public MemberStatus()
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

		
		[Column(Name="Member", UpdateCheck=UpdateCheck.Never, Storage="_Member", DbType="bit NOT NULL")]
		public bool Member
		{
			get { return this._Member; }

			set
			{
				if (this._Member != value)
				{
				
                    this.OnMemberChanging(value);
					this.SendPropertyChanging();
					this._Member = value;
					this.SendPropertyChanged("Member");
					this.OnMemberChanged();
				}

			}

		}

		
		[Column(Name="Previous", UpdateCheck=UpdateCheck.Never, Storage="_Previous", DbType="bit NOT NULL")]
		public bool Previous
		{
			get { return this._Previous; }

			set
			{
				if (this._Previous != value)
				{
				
                    this.OnPreviousChanging(value);
					this.SendPropertyChanging();
					this._Previous = value;
					this.SendPropertyChanged("Previous");
					this.OnPreviousChanged();
				}

			}

		}

		
		[Column(Name="Pending", UpdateCheck=UpdateCheck.Never, Storage="_Pending", DbType="bit NOT NULL")]
		public bool Pending
		{
			get { return this._Pending; }

			set
			{
				if (this._Pending != value)
				{
				
                    this.OnPendingChanging(value);
					this.SendPropertyChanging();
					this._Pending = value;
					this.SendPropertyChanged("Pending");
					this.OnPendingChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_People_MemberStatus", Storage="_People", OtherKey="MemberStatusId")]
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
			entity.MemberStatus = this;
		}

		private void detach_People(Person entity)
		{
			this.SendPropertyChanging();
			entity.MemberStatus = null;
		}

		
	}

}

