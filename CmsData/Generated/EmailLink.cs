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
	[Table(Name="dbo.EmailLinks")]
	public partial class EmailLink : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private DateTime? _Created;
		
		private int? _EmailID;
		
		private string _Hash;
		
		private string _Link;
		
		private int _Count;
		
   		
    	
		private EntityRef<EmailQueue> _EmailQueue;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnCreatedChanging(DateTime? value);
		partial void OnCreatedChanged();
		
		partial void OnEmailIDChanging(int? value);
		partial void OnEmailIDChanged();
		
		partial void OnHashChanging(string value);
		partial void OnHashChanged();
		
		partial void OnLinkChanging(string value);
		partial void OnLinkChanged();
		
		partial void OnCountChanging(int value);
		partial void OnCountChanged();
		
    #endregion
		public EmailLink()
		{
			
			
			this._EmailQueue = default(EntityRef<EmailQueue>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ID", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
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

		
		[Column(Name="Created", UpdateCheck=UpdateCheck.Never, Storage="_Created", DbType="datetime")]
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

		
		[Column(Name="EmailID", UpdateCheck=UpdateCheck.Never, Storage="_EmailID", DbType="int")]
		[IsForeignKey]
		public int? EmailID
		{
			get { return this._EmailID; }

			set
			{
				if (this._EmailID != value)
				{
				
					if (this._EmailQueue.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnEmailIDChanging(value);
					this.SendPropertyChanging();
					this._EmailID = value;
					this.SendPropertyChanged("EmailID");
					this.OnEmailIDChanged();
				}

			}

		}

		
		[Column(Name="Hash", UpdateCheck=UpdateCheck.Never, Storage="_Hash", DbType="nvarchar(50)")]
		public string Hash
		{
			get { return this._Hash; }

			set
			{
				if (this._Hash != value)
				{
				
                    this.OnHashChanging(value);
					this.SendPropertyChanging();
					this._Hash = value;
					this.SendPropertyChanged("Hash");
					this.OnHashChanged();
				}

			}

		}

		
		[Column(Name="Link", UpdateCheck=UpdateCheck.Never, Storage="_Link", DbType="nvarchar(2000)")]
		public string Link
		{
			get { return this._Link; }

			set
			{
				if (this._Link != value)
				{
				
                    this.OnLinkChanging(value);
					this.SendPropertyChanging();
					this._Link = value;
					this.SendPropertyChanged("Link");
					this.OnLinkChanged();
				}

			}

		}

		
		[Column(Name="Count", UpdateCheck=UpdateCheck.Never, Storage="_Count", DbType="int NOT NULL")]
		public int Count
		{
			get { return this._Count; }

			set
			{
				if (this._Count != value)
				{
				
                    this.OnCountChanging(value);
					this.SendPropertyChanging();
					this._Count = value;
					this.SendPropertyChanged("Count");
					this.OnCountChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_EmailLinks_EmailQueue", Storage="_EmailQueue", ThisKey="EmailID", IsForeignKey=true)]
		public EmailQueue EmailQueue
		{
			get { return this._EmailQueue.Entity; }

			set
			{
				EmailQueue previousValue = this._EmailQueue.Entity;
				if (((previousValue != value) 
							|| (this._EmailQueue.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._EmailQueue.Entity = null;
						previousValue.EmailLinks.Remove(this);
					}

					this._EmailQueue.Entity = value;
					if (value != null)
					{
						value.EmailLinks.Add(this);
						
						this._EmailID = value.Id;
						
					}

					else
					{
						
						this._EmailID = default(int?);
						
					}

					this.SendPropertyChanged("EmailQueue");
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

