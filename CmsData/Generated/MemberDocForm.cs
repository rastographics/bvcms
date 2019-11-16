using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MemberDocForm")]
    public partial class MemberDocForm : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

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

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
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

        [Column(Name = "DocDate", UpdateCheck = UpdateCheck.Never, Storage = "_DocDate", DbType = "datetime")]
        public DateTime? DocDate
        {
            get => _DocDate;

            set
            {
                if (_DocDate != value)
                {
                    OnDocDateChanging(value);
                    SendPropertyChanging();
                    _DocDate = value;
                    SendPropertyChanged("DocDate");
                    OnDocDateChanged();
                }
            }
        }

        [Column(Name = "UploaderId", UpdateCheck = UpdateCheck.Never, Storage = "_UploaderId", DbType = "int")]
        public int? UploaderId
        {
            get => _UploaderId;

            set
            {
                if (_UploaderId != value)
                {
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

        [Column(Name = "Purpose", UpdateCheck = UpdateCheck.Never, Storage = "_Purpose", DbType = "nvarchar(30)")]
        public string Purpose
        {
            get => _Purpose;

            set
            {
                if (_Purpose != value)
                {
                    OnPurposeChanging(value);
                    SendPropertyChanging();
                    _Purpose = value;
                    SendPropertyChanged("Purpose");
                    OnPurposeChanged();
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

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(100)")]
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

        [Association(Name = "FK_MemberDocForm_PEOPLE_TBL", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.MemberDocForms.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.MemberDocForms.Add(this);

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
