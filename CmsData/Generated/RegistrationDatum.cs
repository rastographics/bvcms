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
	[Table(Name="dbo.RegistrationData")]
	public partial class RegistrationDatum : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Data;
		
		private DateTime? _Stamp;
		
		private bool? _Completed;
		
		private int? _OrganizationId;
		
		private int? _UserPeopleId;
		
		private bool? _Abandoned;
		
   		
   		private EntitySet<OrganizationMember> _OrganizationMembers;
		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnDataChanging(string value);
		partial void OnDataChanged();
		
		partial void OnStampChanging(DateTime? value);
		partial void OnStampChanged();
		
		partial void OnCompletedChanging(bool? value);
		partial void OnCompletedChanged();
		
		partial void OnOrganizationIdChanging(int? value);
		partial void OnOrganizationIdChanged();
		
		partial void OnUserPeopleIdChanging(int? value);
		partial void OnUserPeopleIdChanged();
		
		partial void OnAbandonedChanging(bool? value);
		partial void OnAbandonedChanged();
		
    #endregion
		public RegistrationDatum()
		{
			
			this._OrganizationMembers = new EntitySet<OrganizationMember>(new Action< OrganizationMember>(this.attach_OrganizationMembers), new Action< OrganizationMember>(this.detach_OrganizationMembers)); 
			
			
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

		
		[Column(Name="Data", UpdateCheck=UpdateCheck.Never, Storage="_Data", DbType="xml")]
		public string Data
		{
			get { return this._Data; }

			set
			{
				if (this._Data != value)
				{
				
                    this.OnDataChanging(value);
					this.SendPropertyChanging();
					this._Data = value;
					this.SendPropertyChanged("Data");
					this.OnDataChanged();
				}

			}

		}

		
		[Column(Name="Stamp", UpdateCheck=UpdateCheck.Never, Storage="_Stamp", DbType="datetime")]
		public DateTime? Stamp
		{
			get { return this._Stamp; }

			set
			{
				if (this._Stamp != value)
				{
				
                    this.OnStampChanging(value);
					this.SendPropertyChanging();
					this._Stamp = value;
					this.SendPropertyChanged("Stamp");
					this.OnStampChanged();
				}

			}

		}

		
		[Column(Name="completed", UpdateCheck=UpdateCheck.Never, Storage="_Completed", DbType="bit")]
		public bool? Completed
		{
			get { return this._Completed; }

			set
			{
				if (this._Completed != value)
				{
				
                    this.OnCompletedChanging(value);
					this.SendPropertyChanging();
					this._Completed = value;
					this.SendPropertyChanged("Completed");
					this.OnCompletedChanged();
				}

			}

		}

		
		[Column(Name="OrganizationId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationId", DbType="int")]
		public int? OrganizationId
		{
			get { return this._OrganizationId; }

			set
			{
				if (this._OrganizationId != value)
				{
				
                    this.OnOrganizationIdChanging(value);
					this.SendPropertyChanging();
					this._OrganizationId = value;
					this.SendPropertyChanged("OrganizationId");
					this.OnOrganizationIdChanged();
				}

			}

		}

		
		[Column(Name="UserPeopleId", UpdateCheck=UpdateCheck.Never, Storage="_UserPeopleId", DbType="int")]
		public int? UserPeopleId
		{
			get { return this._UserPeopleId; }

			set
			{
				if (this._UserPeopleId != value)
				{
				
                    this.OnUserPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._UserPeopleId = value;
					this.SendPropertyChanged("UserPeopleId");
					this.OnUserPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="abandoned", UpdateCheck=UpdateCheck.Never, Storage="_Abandoned", DbType="bit")]
		public bool? Abandoned
		{
			get { return this._Abandoned; }

			set
			{
				if (this._Abandoned != value)
				{
				
                    this.OnAbandonedChanging(value);
					this.SendPropertyChanging();
					this._Abandoned = value;
					this.SendPropertyChanged("Abandoned");
					this.OnAbandonedChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_OrganizationMembers_RegistrationData", Storage="_OrganizationMembers", OtherKey="RegistrationDataId")]
   		public EntitySet<OrganizationMember> OrganizationMembers
   		{
   		    get { return this._OrganizationMembers; }

			set	{ this._OrganizationMembers.Assign(value); }

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

   		
		private void attach_OrganizationMembers(OrganizationMember entity)
		{
			this.SendPropertyChanging();
			entity.RegistrationDatum = this;
		}

		private void detach_OrganizationMembers(OrganizationMember entity)
		{
			this.SendPropertyChanging();
			entity.RegistrationDatum = null;
		}

		
	}

}

