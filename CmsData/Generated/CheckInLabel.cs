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
	[Table(Name="dbo.CheckInLabel")]
	public partial class CheckInLabel : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _TypeID;
		
		private string _Name;
		
		private int _Minimum;
		
		private int _Maximum;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnTypeIDChanging(int value);
		partial void OnTypeIDChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnMinimumChanging(int value);
		partial void OnMinimumChanged();
		
		partial void OnMaximumChanging(int value);
		partial void OnMaximumChanged();
		
    #endregion
		public CheckInLabel()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
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

		
		[Column(Name="typeID", UpdateCheck=UpdateCheck.Never, Storage="_TypeID", DbType="int NOT NULL")]
		public int TypeID
		{
			get { return this._TypeID; }

			set
			{
				if (this._TypeID != value)
				{
				
                    this.OnTypeIDChanging(value);
					this.SendPropertyChanging();
					this._TypeID = value;
					this.SendPropertyChanged("TypeID");
					this.OnTypeIDChanged();
				}

			}

		}

		
		[Column(Name="name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(50) NOT NULL")]
		public string Name
		{
			get { return this._Name; }

			set
			{
				if (this._Name != value)
				{
				
                    this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}

			}

		}

		
		[Column(Name="minimum", UpdateCheck=UpdateCheck.Never, Storage="_Minimum", DbType="int NOT NULL")]
		public int Minimum
		{
			get { return this._Minimum; }

			set
			{
				if (this._Minimum != value)
				{
				
                    this.OnMinimumChanging(value);
					this.SendPropertyChanging();
					this._Minimum = value;
					this.SendPropertyChanged("Minimum");
					this.OnMinimumChanged();
				}

			}

		}

		
		[Column(Name="maximum", UpdateCheck=UpdateCheck.Never, Storage="_Maximum", DbType="int NOT NULL")]
		public int Maximum
		{
			get { return this._Maximum; }

			set
			{
				if (this._Maximum != value)
				{
				
                    this.OnMaximumChanging(value);
					this.SendPropertyChanging();
					this._Maximum = value;
					this.SendPropertyChanged("Maximum");
					this.OnMaximumChanged();
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

