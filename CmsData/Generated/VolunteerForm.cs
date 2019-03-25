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
	[Table(Name="dbo.VolunteerForm")]
	public partial class VolunteerForm : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private DateTime? _AppDate;
		
		private int? _LargeId;
		
		private int? _MediumId;
		
		private int? _SmallId;
		
		private int _Id;
		
		private int? _UploaderId;
		
		private bool? _IsDocument;
		
		private string _Name;
		
   		
    	
		private EntityRef<Person> _Person;
		
		private EntityRef<Volunteer> _Volunteer;
		
		private EntityRef<User> _Uploader;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnAppDateChanging(DateTime? value);
		partial void OnAppDateChanged();
		
		partial void OnLargeIdChanging(int? value);
		partial void OnLargeIdChanged();
		
		partial void OnMediumIdChanging(int? value);
		partial void OnMediumIdChanged();
		
		partial void OnSmallIdChanging(int? value);
		partial void OnSmallIdChanged();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnUploaderIdChanging(int? value);
		partial void OnUploaderIdChanged();
		
		partial void OnIsDocumentChanging(bool? value);
		partial void OnIsDocumentChanged();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
    #endregion
		public VolunteerForm()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
			this._Volunteer = default(EntityRef<Volunteer>); 
			
			this._Uploader = default(EntityRef<User>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._Volunteer.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="AppDate", UpdateCheck=UpdateCheck.Never, Storage="_AppDate", DbType="datetime")]
		public DateTime? AppDate
		{
			get { return this._AppDate; }

			set
			{
				if (this._AppDate != value)
				{
				
                    this.OnAppDateChanging(value);
					this.SendPropertyChanging();
					this._AppDate = value;
					this.SendPropertyChanged("AppDate");
					this.OnAppDateChanged();
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

		
		[Column(Name="UploaderId", UpdateCheck=UpdateCheck.Never, Storage="_UploaderId", DbType="int")]
		[IsForeignKey]
		public int? UploaderId
		{
			get { return this._UploaderId; }

			set
			{
				if (this._UploaderId != value)
				{
				
					if (this._Uploader.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
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

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(50)")]
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
    	
		[Association(Name="FK_VolunteerForm_PEOPLE_TBL", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.VolunteerForms.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.VolunteerForms.Add(this);
						
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

		
		[Association(Name="FK_VolunteerForm_Volunteer1", Storage="_Volunteer", ThisKey="PeopleId", IsForeignKey=true)]
		public Volunteer Volunteer
		{
			get { return this._Volunteer.Entity; }

			set
			{
				Volunteer previousValue = this._Volunteer.Entity;
				if (((previousValue != value) 
							|| (this._Volunteer.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Volunteer.Entity = null;
						previousValue.VolunteerForms.Remove(this);
					}

					this._Volunteer.Entity = value;
					if (value != null)
					{
						value.VolunteerForms.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int);
						
					}

					this.SendPropertyChanged("Volunteer");
				}

			}

		}

		
		[Association(Name="VolunteerFormsUploaded__Uploader", Storage="_Uploader", ThisKey="UploaderId", IsForeignKey=true)]
		public User Uploader
		{
			get { return this._Uploader.Entity; }

			set
			{
				User previousValue = this._Uploader.Entity;
				if (((previousValue != value) 
							|| (this._Uploader.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Uploader.Entity = null;
						previousValue.VolunteerFormsUploaded.Remove(this);
					}

					this._Uploader.Entity = value;
					if (value != null)
					{
						value.VolunteerFormsUploaded.Add(this);
						
						this._UploaderId = value.UserId;
						
					}

					else
					{
						
						this._UploaderId = default(int?);
						
					}

					this.SendPropertyChanged("Uploader");
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

