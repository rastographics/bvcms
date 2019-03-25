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
	[Table(Name="dbo.MemberDocForm")]
	public partial class MemberDocForm : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int _PeopleId;
		
		private DateTime? _DocDate;
		
		private int? _UploaderId;
		
		private bool? _IsDocument;
		
		private string _Purpose;
		
		private int? _LargeId;
		
		private int? _MediumId;
		
		private int? _SmallId;
		
		private string _Name;
		
   		
    	
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnDocDateChanging(DateTime? value);
		partial void OnDocDateChanged();
		
		partial void OnUploaderIdChanging(int? value);
		partial void OnUploaderIdChanged();
		
		partial void OnIsDocumentChanging(bool? value);
		partial void OnIsDocumentChanged();
		
		partial void OnPurposeChanging(string value);
		partial void OnPurposeChanged();
		
		partial void OnLargeIdChanging(int? value);
		partial void OnLargeIdChanged();
		
		partial void OnMediumIdChanging(int? value);
		partial void OnMediumIdChanged();
		
		partial void OnSmallIdChanging(int? value);
		partial void OnSmallIdChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
    #endregion
		public MemberDocForm()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
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

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="DocDate", UpdateCheck=UpdateCheck.Never, Storage="_DocDate", DbType="datetime")]
		public DateTime? DocDate
		{
			get { return this._DocDate; }

			set
			{
				if (this._DocDate != value)
				{
				
                    this.OnDocDateChanging(value);
					this.SendPropertyChanging();
					this._DocDate = value;
					this.SendPropertyChanged("DocDate");
					this.OnDocDateChanged();
				}

			}

		}

		
		[Column(Name="UploaderId", UpdateCheck=UpdateCheck.Never, Storage="_UploaderId", DbType="int")]
		public int? UploaderId
		{
			get { return this._UploaderId; }

			set
			{
				if (this._UploaderId != value)
				{
				
                    this.OnUploaderIdChanging(value);
					this.SendPropertyChanging();
					this._UploaderId = value;
					this.SendPropertyChanged("UploaderId");
					this.OnUploaderIdChanged();
				}

			}

		}

		
		[Column(Name="IsDocument", UpdateCheck=UpdateCheck.Never, Storage="_IsDocument", DbType="bit")]
		public bool? IsDocument
		{
			get { return this._IsDocument; }

			set
			{
				if (this._IsDocument != value)
				{
				
                    this.OnIsDocumentChanging(value);
					this.SendPropertyChanging();
					this._IsDocument = value;
					this.SendPropertyChanged("IsDocument");
					this.OnIsDocumentChanged();
				}

			}

		}

		
		[Column(Name="Purpose", UpdateCheck=UpdateCheck.Never, Storage="_Purpose", DbType="nvarchar(30)")]
		public string Purpose
		{
			get { return this._Purpose; }

			set
			{
				if (this._Purpose != value)
				{
				
                    this.OnPurposeChanging(value);
					this.SendPropertyChanging();
					this._Purpose = value;
					this.SendPropertyChanged("Purpose");
					this.OnPurposeChanged();
				}

			}

		}

		
		[Column(Name="LargeId", UpdateCheck=UpdateCheck.Never, Storage="_LargeId", DbType="int")]
		public int? LargeId
		{
			get { return this._LargeId; }

			set
			{
				if (this._LargeId != value)
				{
				
                    this.OnLargeIdChanging(value);
					this.SendPropertyChanging();
					this._LargeId = value;
					this.SendPropertyChanged("LargeId");
					this.OnLargeIdChanged();
				}

			}

		}

		
		[Column(Name="MediumId", UpdateCheck=UpdateCheck.Never, Storage="_MediumId", DbType="int")]
		public int? MediumId
		{
			get { return this._MediumId; }

			set
			{
				if (this._MediumId != value)
				{
				
                    this.OnMediumIdChanging(value);
					this.SendPropertyChanging();
					this._MediumId = value;
					this.SendPropertyChanged("MediumId");
					this.OnMediumIdChanged();
				}

			}

		}

		
		[Column(Name="SmallId", UpdateCheck=UpdateCheck.Never, Storage="_SmallId", DbType="int")]
		public int? SmallId
		{
			get { return this._SmallId; }

			set
			{
				if (this._SmallId != value)
				{
				
                    this.OnSmallIdChanging(value);
					this.SendPropertyChanging();
					this._SmallId = value;
					this.SendPropertyChanged("SmallId");
					this.OnSmallIdChanged();
				}

			}

		}

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(100)")]
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

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_MemberDocForm_PEOPLE_TBL", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.MemberDocForms.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.MemberDocForms.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int);
						
					}

					this.SendPropertyChanged("Person");
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

