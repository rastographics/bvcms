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
	[Table(Name="dbo.Query")]
	public partial class Query : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private Guid _QueryId;
		
		private string _Text;
		
		private string _Owner;
		
		private DateTime? _Created;
		
		private DateTime? _LastRun;
		
		private string _Name;
		
		private bool _Ispublic;
		
		private int _RunCount;
		
		private Guid? _CopiedFrom;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnQueryIdChanging(Guid value);
		partial void OnQueryIdChanged();
		
		partial void OnTextChanging(string value);
		partial void OnTextChanged();
		
		partial void OnOwnerChanging(string value);
		partial void OnOwnerChanged();
		
		partial void OnCreatedChanging(DateTime? value);
		partial void OnCreatedChanged();
		
		partial void OnLastRunChanging(DateTime? value);
		partial void OnLastRunChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnIspublicChanging(bool value);
		partial void OnIspublicChanged();
		
		partial void OnRunCountChanging(int value);
		partial void OnRunCountChanged();
		
		partial void OnCopiedFromChanging(Guid? value);
		partial void OnCopiedFromChanged();
		
    #endregion
		public Query()
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

		
		[Column(Name="text", UpdateCheck=UpdateCheck.Never, Storage="_Text", DbType="nvarchar")]
		public string Text
		{
			get { return this._Text; }

			set
			{
				if (this._Text != value)
				{
				
                    this.OnTextChanging(value);
					this.SendPropertyChanging();
					this._Text = value;
					this.SendPropertyChanged("Text");
					this.OnTextChanged();
				}

			}

		}

		
		[Column(Name="owner", UpdateCheck=UpdateCheck.Never, Storage="_Owner", DbType="nvarchar(50)")]
		public string Owner
		{
			get { return this._Owner; }

			set
			{
				if (this._Owner != value)
				{
				
                    this.OnOwnerChanging(value);
					this.SendPropertyChanging();
					this._Owner = value;
					this.SendPropertyChanged("Owner");
					this.OnOwnerChanged();
				}

			}

		}

		
		[Column(Name="created", UpdateCheck=UpdateCheck.Never, Storage="_Created", DbType="datetime")]
		public DateTime? Created
		{
			get { return this._Created; }

			set
			{
				if (this._Created != value)
				{
				
                    this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}

			}

		}

		
		[Column(Name="lastRun", UpdateCheck=UpdateCheck.Never, Storage="_LastRun", DbType="datetime")]
		public DateTime? LastRun
		{
			get { return this._LastRun; }

			set
			{
				if (this._LastRun != value)
				{
				
                    this.OnLastRunChanging(value);
					this.SendPropertyChanging();
					this._LastRun = value;
					this.SendPropertyChanged("LastRun");
					this.OnLastRunChanged();
				}

			}

		}

		
		[Column(Name="name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(100)")]
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

		
		[Column(Name="ispublic", UpdateCheck=UpdateCheck.Never, Storage="_Ispublic", DbType="bit NOT NULL")]
		public bool Ispublic
		{
			get { return this._Ispublic; }

			set
			{
				if (this._Ispublic != value)
				{
				
                    this.OnIspublicChanging(value);
					this.SendPropertyChanging();
					this._Ispublic = value;
					this.SendPropertyChanged("Ispublic");
					this.OnIspublicChanged();
				}

			}

		}

		
		[Column(Name="runCount", UpdateCheck=UpdateCheck.Never, Storage="_RunCount", DbType="int NOT NULL")]
		public int RunCount
		{
			get { return this._RunCount; }

			set
			{
				if (this._RunCount != value)
				{
				
                    this.OnRunCountChanging(value);
					this.SendPropertyChanging();
					this._RunCount = value;
					this.SendPropertyChanged("RunCount");
					this.OnRunCountChanged();
				}

			}

		}

		
		[Column(Name="CopiedFrom", UpdateCheck=UpdateCheck.Never, Storage="_CopiedFrom", DbType="uniqueidentifier")]
		public Guid? CopiedFrom
		{
			get { return this._CopiedFrom; }

			set
			{
				if (this._CopiedFrom != value)
				{
				
                    this.OnCopiedFromChanging(value);
					this.SendPropertyChanging();
					this._CopiedFrom = value;
					this.SendPropertyChanged("CopiedFrom");
					this.OnCopiedFromChanged();
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

