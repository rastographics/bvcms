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
	[Table(Name="dbo.Promotion")]
	public partial class Promotion : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int? _FromDivId;
		
		private int? _ToDivId;
		
		private string _Description;
		
		private string _Sort;
		
   		
    	
		private EntityRef<Division> _FromDivision;
		
		private EntityRef<Division> _ToDivision;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnFromDivIdChanging(int? value);
		partial void OnFromDivIdChanged();
		
		partial void OnToDivIdChanging(int? value);
		partial void OnToDivIdChanged();
		
		partial void OnDescriptionChanging(string value);
		partial void OnDescriptionChanged();
		
		partial void OnSortChanging(string value);
		partial void OnSortChanged();
		
    #endregion
		public Promotion()
		{
			
			
			this._FromDivision = default(EntityRef<Division>); 
			
			this._ToDivision = default(EntityRef<Division>); 
			
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

		
		[Column(Name="FromDivId", UpdateCheck=UpdateCheck.Never, Storage="_FromDivId", DbType="int")]
		[IsForeignKey]
		public int? FromDivId
		{
			get { return this._FromDivId; }

			set
			{
				if (this._FromDivId != value)
				{
				
					if (this._FromDivision.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnFromDivIdChanging(value);
					this.SendPropertyChanging();
					this._FromDivId = value;
					this.SendPropertyChanged("FromDivId");
					this.OnFromDivIdChanged();
				}

			}

		}

		
		[Column(Name="ToDivId", UpdateCheck=UpdateCheck.Never, Storage="_ToDivId", DbType="int")]
		[IsForeignKey]
		public int? ToDivId
		{
			get { return this._ToDivId; }

			set
			{
				if (this._ToDivId != value)
				{
				
					if (this._ToDivision.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnToDivIdChanging(value);
					this.SendPropertyChanging();
					this._ToDivId = value;
					this.SendPropertyChanged("ToDivId");
					this.OnToDivIdChanged();
				}

			}

		}

		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar(200)")]
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

		
		[Column(Name="Sort", UpdateCheck=UpdateCheck.Never, Storage="_Sort", DbType="nvarchar(10)")]
		public string Sort
		{
			get { return this._Sort; }

			set
			{
				if (this._Sort != value)
				{
				
                    this.OnSortChanging(value);
					this.SendPropertyChanging();
					this._Sort = value;
					this.SendPropertyChanged("Sort");
					this.OnSortChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FromPromotions__FromDivision", Storage="_FromDivision", ThisKey="FromDivId", IsForeignKey=true)]
		public Division FromDivision
		{
			get { return this._FromDivision.Entity; }

			set
			{
				Division previousValue = this._FromDivision.Entity;
				if (((previousValue != value) 
							|| (this._FromDivision.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._FromDivision.Entity = null;
						previousValue.FromPromotions.Remove(this);
					}

					this._FromDivision.Entity = value;
					if (value != null)
					{
						value.FromPromotions.Add(this);
						
						this._FromDivId = value.Id;
						
					}

					else
					{
						
						this._FromDivId = default(int?);
						
					}

					this.SendPropertyChanged("FromDivision");
				}

			}

		}

		
		[Association(Name="ToPromotions__ToDivision", Storage="_ToDivision", ThisKey="ToDivId", IsForeignKey=true)]
		public Division ToDivision
		{
			get { return this._ToDivision.Entity; }

			set
			{
				Division previousValue = this._ToDivision.Entity;
				if (((previousValue != value) 
							|| (this._ToDivision.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ToDivision.Entity = null;
						previousValue.ToPromotions.Remove(this);
					}

					this._ToDivision.Entity = value;
					if (value != null)
					{
						value.ToPromotions.Add(this);
						
						this._ToDivId = value.Id;
						
					}

					else
					{
						
						this._ToDivId = default(int?);
						
					}

					this.SendPropertyChanged("ToDivision");
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

