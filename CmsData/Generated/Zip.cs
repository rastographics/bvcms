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
	[Table(Name="dbo.Zips")]
	public partial class Zip : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private string _ZipCode;
		
		private int? _MetroMarginalCode;
		
   		
    	
		private EntityRef<ResidentCode> _ResidentCode;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnZipCodeChanging(string value);
		partial void OnZipCodeChanged();
		
		partial void OnMetroMarginalCodeChanging(int? value);
		partial void OnMetroMarginalCodeChanged();
		
    #endregion
		public Zip()
		{
			
			
			this._ResidentCode = default(EntityRef<ResidentCode>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ZipCode", UpdateCheck=UpdateCheck.Never, Storage="_ZipCode", DbType="nvarchar(10) NOT NULL", IsPrimaryKey=true)]
		public string ZipCode
		{
			get { return this._ZipCode; }

			set
			{
				if (this._ZipCode != value)
				{
				
                    this.OnZipCodeChanging(value);
					this.SendPropertyChanging();
					this._ZipCode = value;
					this.SendPropertyChanged("ZipCode");
					this.OnZipCodeChanged();
				}

			}

		}

		
		[Column(Name="MetroMarginalCode", UpdateCheck=UpdateCheck.Never, Storage="_MetroMarginalCode", DbType="int")]
		[IsForeignKey]
		public int? MetroMarginalCode
		{
			get { return this._MetroMarginalCode; }

			set
			{
				if (this._MetroMarginalCode != value)
				{
				
					if (this._ResidentCode.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnMetroMarginalCodeChanging(value);
					this.SendPropertyChanging();
					this._MetroMarginalCode = value;
					this.SendPropertyChanged("MetroMarginalCode");
					this.OnMetroMarginalCodeChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_Zips_ResidentCode", Storage="_ResidentCode", ThisKey="MetroMarginalCode", IsForeignKey=true)]
		public ResidentCode ResidentCode
		{
			get { return this._ResidentCode.Entity; }

			set
			{
				ResidentCode previousValue = this._ResidentCode.Entity;
				if (((previousValue != value) 
							|| (this._ResidentCode.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ResidentCode.Entity = null;
						previousValue.Zips.Remove(this);
					}

					this._ResidentCode.Entity = value;
					if (value != null)
					{
						value.Zips.Add(this);
						
						this._MetroMarginalCode = value.Id;
						
					}

					else
					{
						
						this._MetroMarginalCode = default(int?);
						
					}

					this.SendPropertyChanged("ResidentCode");
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

