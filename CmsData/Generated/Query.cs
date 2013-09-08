using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData
{
	[Table(Name="dbo.Query")]
	public partial class Query : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Text;
		
		private string _Owner;
		
		private DateTime? _Created;
		
		private DateTime? _Modified;
		
		private string _Name;
		
		private bool? _Ispublic;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnTextChanging(string value);
		partial void OnTextChanged();
		
		partial void OnOwnerChanging(string value);
		partial void OnOwnerChanged();
		
		partial void OnCreatedChanging(DateTime? value);
		partial void OnCreatedChanged();
		
		partial void OnModifiedChanging(DateTime? value);
		partial void OnModifiedChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnIspublicChanging(bool? value);
		partial void OnIspublicChanged();
		
    #endregion
		public Query()
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

		
		[Column(Name="text", UpdateCheck=UpdateCheck.Never, Storage="_Text", DbType="varchar")]
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

		
		[Column(Name="owner", UpdateCheck=UpdateCheck.Never, Storage="_Owner", DbType="varchar(50)")]
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

		
		[Column(Name="modified", UpdateCheck=UpdateCheck.Never, Storage="_Modified", DbType="datetime")]
		public DateTime? Modified
		{
			get { return this._Modified; }

			set
			{
				if (this._Modified != value)
				{
				
                    this.OnModifiedChanging(value);
					this.SendPropertyChanging();
					this._Modified = value;
					this.SendPropertyChanged("Modified");
					this.OnModifiedChanged();
				}

			}

		}

		
		[Column(Name="name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="varchar(50)")]
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

		
		[Column(Name="ispublic", UpdateCheck=UpdateCheck.Never, Storage="_Ispublic", DbType="bit")]
		public bool? Ispublic
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

