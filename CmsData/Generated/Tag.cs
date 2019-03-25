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
	[Table(Name="dbo.Tag")]
	public partial class Tag : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Name;
		
		private int _TypeId;
		
		private string _Owner;
		
		private bool? _Active;
		
		private int? _PeopleId;
		
		private string _OwnerName;
		
		private DateTime? _Created;
		
   		
   		private EntitySet<TagShare> _TagShares;
		
   		private EntitySet<TagPerson> _PersonTags;
		
    	
		private EntityRef<Person> _PersonOwner;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnTypeIdChanging(int value);
		partial void OnTypeIdChanged();
		
		partial void OnOwnerChanging(string value);
		partial void OnOwnerChanged();
		
		partial void OnActiveChanging(bool? value);
		partial void OnActiveChanged();
		
		partial void OnPeopleIdChanging(int? value);
		partial void OnPeopleIdChanged();
		
		partial void OnOwnerNameChanging(string value);
		partial void OnOwnerNameChanged();
		
		partial void OnCreatedChanging(DateTime? value);
		partial void OnCreatedChanged();
		
    #endregion
		public Tag()
		{
			
			this._TagShares = new EntitySet<TagShare>(new Action< TagShare>(this.attach_TagShares), new Action< TagShare>(this.detach_TagShares)); 
			
			this._PersonTags = new EntitySet<TagPerson>(new Action< TagPerson>(this.attach_PersonTags), new Action< TagPerson>(this.detach_PersonTags)); 
			
			
			this._PersonOwner = default(EntityRef<Person>); 
			
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

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(200) NOT NULL")]
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

		
		[Column(Name="TypeId", UpdateCheck=UpdateCheck.Never, Storage="_TypeId", DbType="int NOT NULL")]
		public int TypeId
		{
			get { return this._TypeId; }

			set
			{
				if (this._TypeId != value)
				{
				
                    this.OnTypeIdChanging(value);
					this.SendPropertyChanging();
					this._TypeId = value;
					this.SendPropertyChanged("TypeId");
					this.OnTypeIdChanged();
				}

			}

		}

		
		[Column(Name="Owner", UpdateCheck=UpdateCheck.Never, Storage="_Owner", DbType="nvarchar(50)")]
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

		
		[Column(Name="Active", UpdateCheck=UpdateCheck.Never, Storage="_Active", DbType="bit")]
		public bool? Active
		{
			get { return this._Active; }

			set
			{
				if (this._Active != value)
				{
				
                    this.OnActiveChanging(value);
					this.SendPropertyChanging();
					this._Active = value;
					this.SendPropertyChanged("Active");
					this.OnActiveChanged();
				}

			}

		}

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int")]
		[IsForeignKey]
		public int? PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._PersonOwner.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="OwnerName", UpdateCheck=UpdateCheck.Never, Storage="_OwnerName", DbType="nvarchar(100)", IsDbGenerated=true)]
		public string OwnerName
		{
			get { return this._OwnerName; }

			set
			{
				if (this._OwnerName != value)
				{
				
                    this.OnOwnerNameChanging(value);
					this.SendPropertyChanging();
					this._OwnerName = value;
					this.SendPropertyChanged("OwnerName");
					this.OnOwnerNameChanged();
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

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_TagShare_Tag", Storage="_TagShares", OtherKey="TagId")]
   		public EntitySet<TagShare> TagShares
   		{
   		    get { return this._TagShares; }

			set	{ this._TagShares.Assign(value); }

   		}

		
   		[Association(Name="PersonTags__Tag", Storage="_PersonTags", OtherKey="Id")]
   		public EntitySet<TagPerson> PersonTags
   		{
   		    get { return this._PersonTags; }

			set	{ this._PersonTags.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="TagsOwned__PersonOwner", Storage="_PersonOwner", ThisKey="PeopleId", IsForeignKey=true)]
		public Person PersonOwner
		{
			get { return this._PersonOwner.Entity; }

			set
			{
				Person previousValue = this._PersonOwner.Entity;
				if (((previousValue != value) 
							|| (this._PersonOwner.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._PersonOwner.Entity = null;
						previousValue.TagsOwned.Remove(this);
					}

					this._PersonOwner.Entity = value;
					if (value != null)
					{
						value.TagsOwned.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int?);
						
					}

					this.SendPropertyChanged("PersonOwner");
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

   		
		private void attach_TagShares(TagShare entity)
		{
			this.SendPropertyChanging();
			entity.Tag = this;
		}

		private void detach_TagShares(TagShare entity)
		{
			this.SendPropertyChanging();
			entity.Tag = null;
		}

		
		private void attach_PersonTags(TagPerson entity)
		{
			this.SendPropertyChanging();
			entity.Tag = this;
		}

		private void detach_PersonTags(TagPerson entity)
		{
			this.SendPropertyChanging();
			entity.Tag = null;
		}

		
	}

}

