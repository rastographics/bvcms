using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.VolunteerForm")]
    public partial class VolunteerForm : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            _Person = default(EntityRef<Person>);

            _Volunteer = default(EntityRef<Volunteer>);

            _Uploader = default(EntityRef<User>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    if (_Volunteer.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "AppDate", UpdateCheck = UpdateCheck.Never, Storage = "_AppDate", DbType = "datetime")]
        public DateTime? AppDate
        {
            get => _AppDate;

            set
            {
                if (_AppDate != value)
                {
                    OnAppDateChanging(value);
                    SendPropertyChanging();
                    _AppDate = value;
                    SendPropertyChanged("AppDate");
                    OnAppDateChanged();
                }
            }
        }

        [Column(Name = "LargeId", UpdateCheck = UpdateCheck.Never, Storage = "_LargeId", DbType = "int")]
        public int? LargeId
        {
            get => _LargeId;

            set
            {
                if (_LargeId != value)
                {
                    OnLargeIdChanging(value);
                    SendPropertyChanging();
                    _LargeId = value;
                    SendPropertyChanged("LargeId");
                    OnLargeIdChanged();
                }
            }
        }

        [Column(Name = "MediumId", UpdateCheck = UpdateCheck.Never, Storage = "_MediumId", DbType = "int")]
        public int? MediumId
        {
            get => _MediumId;

            set
            {
                if (_MediumId != value)
                {
                    OnMediumIdChanging(value);
                    SendPropertyChanging();
                    _MediumId = value;
                    SendPropertyChanged("MediumId");
                    OnMediumIdChanged();
                }
            }
        }

        [Column(Name = "SmallId", UpdateCheck = UpdateCheck.Never, Storage = "_SmallId", DbType = "int")]
        public int? SmallId
        {
            get => _SmallId;

            set
            {
                if (_SmallId != value)
                {
                    OnSmallIdChanging(value);
                    SendPropertyChanging();
                    _SmallId = value;
                    SendPropertyChanged("SmallId");
                    OnSmallIdChanged();
                }
            }
        }

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

        [Column(Name = "UploaderId", UpdateCheck = UpdateCheck.Never, Storage = "_UploaderId", DbType = "int")]
        [IsForeignKey]
        public int? UploaderId
        {
            get => _UploaderId;

            set
            {
                if (_UploaderId != value)
                {
                    if (_Uploader.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnUploaderIdChanging(value);
                    SendPropertyChanging();
                    _UploaderId = value;
                    SendPropertyChanged("UploaderId");
                    OnUploaderIdChanged();
                }
            }
        }

        [Column(Name = "IsDocument", UpdateCheck = UpdateCheck.Never, Storage = "_IsDocument", DbType = "bit")]
        public bool? IsDocument
        {
            get => _IsDocument;

            set
            {
                if (_IsDocument != value)
                {
                    OnIsDocumentChanging(value);
                    SendPropertyChanging();
                    _IsDocument = value;
                    SendPropertyChanged("IsDocument");
                    OnIsDocumentChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(50)")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    OnNameChanging(value);
                    SendPropertyChanging();
                    _Name = value;
                    SendPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_VolunteerForm_PEOPLE_TBL", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
        public Person Person
        {
            get => _Person.Entity;

            set
            {
                Person previousValue = _Person.Entity;
                if (((previousValue != value)
                            || (_Person.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Person.Entity = null;
                        previousValue.VolunteerForms.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.VolunteerForms.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int);

                    }

                    SendPropertyChanged("Person");
                }
            }
        }

        [Association(Name = "FK_VolunteerForm_Volunteer1", Storage = "_Volunteer", ThisKey = "PeopleId", IsForeignKey = true)]
        public Volunteer Volunteer
        {
            get => _Volunteer.Entity;

            set
            {
                Volunteer previousValue = _Volunteer.Entity;
                if (((previousValue != value)
                            || (_Volunteer.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Volunteer.Entity = null;
                        previousValue.VolunteerForms.Remove(this);
                    }

                    _Volunteer.Entity = value;
                    if (value != null)
                    {
                        value.VolunteerForms.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int);

                    }

                    SendPropertyChanged("Volunteer");
                }
            }
        }

        [Association(Name = "VolunteerFormsUploaded__Uploader", Storage = "_Uploader", ThisKey = "UploaderId", IsForeignKey = true)]
        public User Uploader
        {
            get => _Uploader.Entity;

            set
            {
                User previousValue = _Uploader.Entity;
                if (((previousValue != value)
                            || (_Uploader.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Uploader.Entity = null;
                        previousValue.VolunteerFormsUploaded.Remove(this);
                    }

                    _Uploader.Entity = value;
                    if (value != null)
                    {
                        value.VolunteerFormsUploaded.Add(this);

                        _UploaderId = value.UserId;

                    }

                    else
                    {
                        _UploaderId = default(int?);

                    }

                    SendPropertyChanged("Uploader");
                }
            }
        }

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((PropertyChanging != null))
            {
                PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
