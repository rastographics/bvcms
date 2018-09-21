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
	[Table(Name="lookup.AttendType")]
	public partial class AttendType : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Code;
		
		private string _Description;
		
		private bool? _Hardwired;
		
		private bool _Worker;
		
		private bool _Guest;
		
   		
   		private EntitySet<Attend> _Attends;
		
   		private EntitySet<MemberType> _MemberTypes;
		
    	
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
		
		partial void OnWorkerChanging(bool value);
		partial void OnWorkerChanged();
		
		partial void OnGuestChanging(bool value);
		partial void OnGuestChanged();
		
    #endregion
		public AttendType()
		{
			
			this._Attends = new EntitySet<Attend>(new Action< Attend>(this.attach_Attends), new Action< Attend>(this.detach_Attends)); 
			
			this._MemberTypes = new EntitySet<MemberType>(new Action< MemberType>(this.attach_MemberTypes), new Action< MemberType>(this.detach_MemberTypes)); 
			
			
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

		
		[Column(Name="Worker", UpdateCheck=UpdateCheck.Never, Storage="_Worker", DbType="bit NOT NULL")]
		public bool Worker
		{
			get { return this._Worker; }

			set
			{
				if (this._Worker != value)
				{
				
                    this.OnWorkerChanging(value);
					this.SendPropertyChanging();
					this._Worker = value;
					this.SendPropertyChanged("Worker");
					this.OnWorkerChanged();
				}

			}

		}

		
		[Column(Name="Guest", UpdateCheck=UpdateCheck.Never, Storage="_Guest", DbType="bit NOT NULL")]
		public bool Guest
		{
			get { return this._Guest; }

			set
			{
				if (this._Guest != value)
				{
				
                    this.OnGuestChanging(value);
					this.SendPropertyChanging();
					this._Guest = value;
					this.SendPropertyChanged("Guest");
					this.OnGuestChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_AttendWithAbsents_TBL_AttendType", Storage="_Attends", OtherKey="AttendanceTypeId")]
   		public EntitySet<Attend> Attends
   		{
   		    get { return this._Attends; }

			set	{ this._Attends.Assign(value); }

   		}

		
   		[Association(Name="FK_MemberType_AttendType", Storage="_MemberTypes", OtherKey="AttendanceTypeId")]
   		public EntitySet<MemberType> MemberTypes
   		{
   		    get { return this._MemberTypes; }

			set	{ this._MemberTypes.Assign(value); }

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

   		
		private void attach_Attends(Attend entity)
		{
			this.SendPropertyChanging();
			entity.AttendType = this;
		}

		private void detach_Attends(Attend entity)
		{
			this.SendPropertyChanging();
			entity.AttendType = null;
		}

		
		private void attach_MemberTypes(MemberType entity)
		{
			this.SendPropertyChanging();
			entity.AttendType = this;
		}

		private void detach_MemberTypes(MemberType entity)
		{
			this.SendPropertyChanging();
			entity.AttendType = null;
		}

		
	}

}

