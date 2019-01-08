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
	[Table(Name="dbo.VolInterestCodes")]
	public partial class VolInterestCode : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Description;
		
		private string _Code;
		
		private string _Org;
		
   		
   		private EntitySet<VolInterestInterestCode> _VolInterestInterestCodes;
		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnDescriptionChanging(string value);
		partial void OnDescriptionChanged();
		
		partial void OnCodeChanging(string value);
		partial void OnCodeChanged();
		
		partial void OnOrgChanging(string value);
		partial void OnOrgChanged();
		
    #endregion
		public VolInterestCode()
		{
			
			this._VolInterestInterestCodes = new EntitySet<VolInterestInterestCode>(new Action< VolInterestInterestCode>(this.attach_VolInterestInterestCodes), new Action< VolInterestInterestCode>(this.detach_VolInterestInterestCodes)); 
			
			
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

		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar(180)")]
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

		
		[Column(Name="Code", UpdateCheck=UpdateCheck.Never, Storage="_Code", DbType="nvarchar(100)")]
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

		
		[Column(Name="Org", UpdateCheck=UpdateCheck.Never, Storage="_Org", DbType="nvarchar(150)")]
		public string Org
		{
			get { return this._Org; }

			set
			{
				if (this._Org != value)
				{
				
                    this.OnOrgChanging(value);
					this.SendPropertyChanging();
					this._Org = value;
					this.SendPropertyChanged("Org");
					this.OnOrgChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_VolInterestInterestCodes_VolInterestCodes", Storage="_VolInterestInterestCodes", OtherKey="InterestCodeId")]
   		public EntitySet<VolInterestInterestCode> VolInterestInterestCodes
   		{
   		    get { return this._VolInterestInterestCodes; }

			set	{ this._VolInterestInterestCodes.Assign(value); }

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

   		
		private void attach_VolInterestInterestCodes(VolInterestInterestCode entity)
		{
			this.SendPropertyChanging();
			entity.VolInterestCode = this;
		}

		private void detach_VolInterestInterestCodes(VolInterestInterestCode entity)
		{
			this.SendPropertyChanging();
			entity.VolInterestCode = null;
		}

		
	}

}

