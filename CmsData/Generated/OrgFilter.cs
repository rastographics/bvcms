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
	[Table(Name="dbo.OrgFilter")]
	public partial class OrgFilter : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private Guid _QueryId;
		
		private int _Id;
		
		private string _GroupSelect;
		
		private string _FirstName;
		
		private string _LastName;
		
		private string _SgFilter;
		
		private bool? _ShowHidden;
		
		private bool? _FilterIndividuals;
		
		private bool? _FilterTag;
		
		private int? _TagId;
		
		private DateTime _LastUpdated;
		
		private int? _UserId;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnQueryIdChanging(Guid value);
		partial void OnQueryIdChanged();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnGroupSelectChanging(string value);
		partial void OnGroupSelectChanged();
		
		partial void OnFirstNameChanging(string value);
		partial void OnFirstNameChanged();
		
		partial void OnLastNameChanging(string value);
		partial void OnLastNameChanged();
		
		partial void OnSgFilterChanging(string value);
		partial void OnSgFilterChanged();
		
		partial void OnShowHiddenChanging(bool? value);
		partial void OnShowHiddenChanged();
		
		partial void OnFilterIndividualsChanging(bool? value);
		partial void OnFilterIndividualsChanged();
		
		partial void OnFilterTagChanging(bool? value);
		partial void OnFilterTagChanged();
		
		partial void OnTagIdChanging(int? value);
		partial void OnTagIdChanged();
		
		partial void OnLastUpdatedChanging(DateTime value);
		partial void OnLastUpdatedChanged();
		
		partial void OnUserIdChanging(int? value);
		partial void OnUserIdChanged();
		
    #endregion
		public OrgFilter()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="QueryId", UpdateCheck=UpdateCheck.Never, Storage="_QueryId", DbType="uniqueidentifier NOT NULL", IsPrimaryKey=true)]
		public Guid QueryId
		{
			get { return this._QueryId; }

			set
			{
				if (this._QueryId != value)
				{
				
                    this.OnQueryIdChanging(value);
					this.SendPropertyChanging();
					this._QueryId = value;
					this.SendPropertyChanged("QueryId");
					this.OnQueryIdChanged();
				}

			}

		}

		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="int NOT NULL")]
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

		
		[Column(Name="GroupSelect", UpdateCheck=UpdateCheck.Never, Storage="_GroupSelect", DbType="varchar(50)")]
		public string GroupSelect
		{
			get { return this._GroupSelect; }

			set
			{
				if (this._GroupSelect != value)
				{
				
                    this.OnGroupSelectChanging(value);
					this.SendPropertyChanging();
					this._GroupSelect = value;
					this.SendPropertyChanged("GroupSelect");
					this.OnGroupSelectChanged();
				}

			}

		}

		
		[Column(Name="FirstName", UpdateCheck=UpdateCheck.Never, Storage="_FirstName", DbType="varchar(50)")]
		public string FirstName
		{
			get { return this._FirstName; }

			set
			{
				if (this._FirstName != value)
				{
				
                    this.OnFirstNameChanging(value);
					this.SendPropertyChanging();
					this._FirstName = value;
					this.SendPropertyChanged("FirstName");
					this.OnFirstNameChanged();
				}

			}

		}

		
		[Column(Name="LastName", UpdateCheck=UpdateCheck.Never, Storage="_LastName", DbType="varchar(50)")]
		public string LastName
		{
			get { return this._LastName; }

			set
			{
				if (this._LastName != value)
				{
				
                    this.OnLastNameChanging(value);
					this.SendPropertyChanging();
					this._LastName = value;
					this.SendPropertyChanged("LastName");
					this.OnLastNameChanged();
				}

			}

		}

		
		[Column(Name="SgFilter", UpdateCheck=UpdateCheck.Never, Storage="_SgFilter", DbType="varchar(900)")]
		public string SgFilter
		{
			get { return this._SgFilter; }

			set
			{
				if (this._SgFilter != value)
				{
				
                    this.OnSgFilterChanging(value);
					this.SendPropertyChanging();
					this._SgFilter = value;
					this.SendPropertyChanged("SgFilter");
					this.OnSgFilterChanged();
				}

			}

		}

		
		[Column(Name="ShowHidden", UpdateCheck=UpdateCheck.Never, Storage="_ShowHidden", DbType="bit")]
		public bool? ShowHidden
		{
			get { return this._ShowHidden; }

			set
			{
				if (this._ShowHidden != value)
				{
				
                    this.OnShowHiddenChanging(value);
					this.SendPropertyChanging();
					this._ShowHidden = value;
					this.SendPropertyChanged("ShowHidden");
					this.OnShowHiddenChanged();
				}

			}

		}

		
		[Column(Name="FilterIndividuals", UpdateCheck=UpdateCheck.Never, Storage="_FilterIndividuals", DbType="bit")]
		public bool? FilterIndividuals
		{
			get { return this._FilterIndividuals; }

			set
			{
				if (this._FilterIndividuals != value)
				{
				
                    this.OnFilterIndividualsChanging(value);
					this.SendPropertyChanging();
					this._FilterIndividuals = value;
					this.SendPropertyChanged("FilterIndividuals");
					this.OnFilterIndividualsChanged();
				}

			}

		}

		
		[Column(Name="FilterTag", UpdateCheck=UpdateCheck.Never, Storage="_FilterTag", DbType="bit")]
		public bool? FilterTag
		{
			get { return this._FilterTag; }

			set
			{
				if (this._FilterTag != value)
				{
				
                    this.OnFilterTagChanging(value);
					this.SendPropertyChanging();
					this._FilterTag = value;
					this.SendPropertyChanged("FilterTag");
					this.OnFilterTagChanged();
				}

			}

		}

		
		[Column(Name="TagId", UpdateCheck=UpdateCheck.Never, Storage="_TagId", DbType="int")]
		public int? TagId
		{
			get { return this._TagId; }

			set
			{
				if (this._TagId != value)
				{
				
                    this.OnTagIdChanging(value);
					this.SendPropertyChanging();
					this._TagId = value;
					this.SendPropertyChanged("TagId");
					this.OnTagIdChanged();
				}

			}

		}

		
		[Column(Name="LastUpdated", UpdateCheck=UpdateCheck.Never, Storage="_LastUpdated", DbType="datetime NOT NULL")]
		public DateTime LastUpdated
		{
			get { return this._LastUpdated; }

			set
			{
				if (this._LastUpdated != value)
				{
				
                    this.OnLastUpdatedChanging(value);
					this.SendPropertyChanging();
					this._LastUpdated = value;
					this.SendPropertyChanged("LastUpdated");
					this.OnLastUpdatedChanged();
				}

			}

		}

		
		[Column(Name="UserId", UpdateCheck=UpdateCheck.Never, Storage="_UserId", DbType="int")]
		public int? UserId
		{
			get { return this._UserId; }

			set
			{
				if (this._UserId != value)
				{
				
                    this.OnUserIdChanging(value);
					this.SendPropertyChanging();
					this._UserId = value;
					this.SendPropertyChanged("UserId");
					this.OnUserIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
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

   		
	}

}

