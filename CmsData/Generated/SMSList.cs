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
	[Table(Name="dbo.SMSList")]
	public partial class SMSList : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private DateTime _Created;
		
		private int _SenderID;
		
		private DateTime _SendAt;
		
		private int _SendGroupID;
		
		private string _Message;
		
		private int _SentSMS;
		
		private int _SentNone;
		
		private string _Title;
		
   		
   		private EntitySet<SMSItem> _SMSItems;
		
    	
		private EntityRef<Person> _Person;
		
		private EntityRef<SMSGroup> _SMSGroup;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnCreatedChanging(DateTime value);
		partial void OnCreatedChanged();
		
		partial void OnSenderIDChanging(int value);
		partial void OnSenderIDChanged();
		
		partial void OnSendAtChanging(DateTime value);
		partial void OnSendAtChanged();
		
		partial void OnSendGroupIDChanging(int value);
		partial void OnSendGroupIDChanged();
		
		partial void OnMessageChanging(string value);
		partial void OnMessageChanged();
		
		partial void OnSentSMSChanging(int value);
		partial void OnSentSMSChanged();
		
		partial void OnSentNoneChanging(int value);
		partial void OnSentNoneChanged();
		
		partial void OnTitleChanging(string value);
		partial void OnTitleChanged();
		
    #endregion
		public SMSList()
		{
			
			this._SMSItems = new EntitySet<SMSItem>(new Action< SMSItem>(this.attach_SMSItems), new Action< SMSItem>(this.detach_SMSItems)); 
			
			
			this._Person = default(EntityRef<Person>); 
			
			this._SMSGroup = default(EntityRef<SMSGroup>); 
			
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

		
		[Column(Name="Created", UpdateCheck=UpdateCheck.Never, Storage="_Created", DbType="datetime NOT NULL")]
		public DateTime Created
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

		
		[Column(Name="SenderID", UpdateCheck=UpdateCheck.Never, Storage="_SenderID", DbType="int NOT NULL")]
		[IsForeignKey]
		public int SenderID
		{
			get { return this._SenderID; }

			set
			{
				if (this._SenderID != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnSenderIDChanging(value);
					this.SendPropertyChanging();
					this._SenderID = value;
					this.SendPropertyChanged("SenderID");
					this.OnSenderIDChanged();
				}

			}

		}

		
		[Column(Name="SendAt", UpdateCheck=UpdateCheck.Never, Storage="_SendAt", DbType="datetime NOT NULL")]
		public DateTime SendAt
		{
			get { return this._SendAt; }

			set
			{
				if (this._SendAt != value)
				{
				
                    this.OnSendAtChanging(value);
					this.SendPropertyChanging();
					this._SendAt = value;
					this.SendPropertyChanged("SendAt");
					this.OnSendAtChanged();
				}

			}

		}

		
		[Column(Name="SendGroupID", UpdateCheck=UpdateCheck.Never, Storage="_SendGroupID", DbType="int NOT NULL")]
		[IsForeignKey]
		public int SendGroupID
		{
			get { return this._SendGroupID; }

			set
			{
				if (this._SendGroupID != value)
				{
				
					if (this._SMSGroup.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnSendGroupIDChanging(value);
					this.SendPropertyChanging();
					this._SendGroupID = value;
					this.SendPropertyChanged("SendGroupID");
					this.OnSendGroupIDChanged();
				}

			}

		}

		
		[Column(Name="Message", UpdateCheck=UpdateCheck.Never, Storage="_Message", DbType="nvarchar(1600) NOT NULL")]
		public string Message
		{
			get { return this._Message; }

			set
			{
				if (this._Message != value)
				{
				
                    this.OnMessageChanging(value);
					this.SendPropertyChanging();
					this._Message = value;
					this.SendPropertyChanged("Message");
					this.OnMessageChanged();
				}

			}

		}

		
		[Column(Name="SentSMS", UpdateCheck=UpdateCheck.Never, Storage="_SentSMS", DbType="int NOT NULL")]
		public int SentSMS
		{
			get { return this._SentSMS; }

			set
			{
				if (this._SentSMS != value)
				{
				
                    this.OnSentSMSChanging(value);
					this.SendPropertyChanging();
					this._SentSMS = value;
					this.SendPropertyChanged("SentSMS");
					this.OnSentSMSChanged();
				}

			}

		}

		
		[Column(Name="SentNone", UpdateCheck=UpdateCheck.Never, Storage="_SentNone", DbType="int NOT NULL")]
		public int SentNone
		{
			get { return this._SentNone; }

			set
			{
				if (this._SentNone != value)
				{
				
                    this.OnSentNoneChanging(value);
					this.SendPropertyChanging();
					this._SentNone = value;
					this.SendPropertyChanged("SentNone");
					this.OnSentNoneChanged();
				}

			}

		}

		
		[Column(Name="Title", UpdateCheck=UpdateCheck.Never, Storage="_Title", DbType="nvarchar(150) NOT NULL")]
		public string Title
		{
			get { return this._Title; }

			set
			{
				if (this._Title != value)
				{
				
                    this.OnTitleChanging(value);
					this.SendPropertyChanging();
					this._Title = value;
					this.SendPropertyChanged("Title");
					this.OnTitleChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_SMSItems_SMSList", Storage="_SMSItems", OtherKey="ListID")]
   		public EntitySet<SMSItem> SMSItems
   		{
   		    get { return this._SMSItems; }

			set	{ this._SMSItems.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_SMSList_People", Storage="_Person", ThisKey="SenderID", IsForeignKey=true)]
		public Person Person
		{
			get { return this._Person.Entity; }

			set
			{
				Person previousValue = this._Person.Entity;
				if (((previousValue != value) 
							|| (this._Person.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Person.Entity = null;
						previousValue.SMSLists.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.SMSLists.Add(this);
						
						this._SenderID = value.PeopleId;
						
					}

					else
					{
						
						this._SenderID = default(int);
						
					}

					this.SendPropertyChanged("Person");
				}

			}

		}

		
		[Association(Name="FK_SMSList_SMSGroups", Storage="_SMSGroup", ThisKey="SendGroupID", IsForeignKey=true)]
		public SMSGroup SMSGroup
		{
			get { return this._SMSGroup.Entity; }

			set
			{
				SMSGroup previousValue = this._SMSGroup.Entity;
				if (((previousValue != value) 
							|| (this._SMSGroup.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._SMSGroup.Entity = null;
						previousValue.SMSLists.Remove(this);
					}

					this._SMSGroup.Entity = value;
					if (value != null)
					{
						value.SMSLists.Add(this);
						
						this._SendGroupID = value.Id;
						
					}

					else
					{
						
						this._SendGroupID = default(int);
						
					}

					this.SendPropertyChanged("SMSGroup");
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

   		
		private void attach_SMSItems(SMSItem entity)
		{
			this.SendPropertyChanging();
			entity.SMSList = this;
		}

		private void detach_SMSItems(SMSItem entity)
		{
			this.SendPropertyChanging();
			entity.SMSList = null;
		}

		
	}

}

