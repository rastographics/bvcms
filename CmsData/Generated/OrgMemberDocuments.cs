using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.OrgMemberDocuments")]
    public partial class OrgMemberDocuments
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _DocumentId;
        private string _DocumentName;
        private int _ImageId;
        private int _PeopleId;
        private int _OrganizationId;

        private EntityRef<Person> _Person;
        private EntityRef<Organization> _Organization;
        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnDocumentIdChanging(int value);
        partial void OnDocumentIdChanged();

        partial void OnDocumentNameChanging(string value);
        partial void OnDocumentNameChanged();

        partial void OnImageIdChanging(int value);
        partial void OnImageIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnOrganizationIdChanging(int value);
        partial void OnOrganizationIdChanged();
        #endregion

        public OrgMemberDocuments()
        {
            _Person = default(EntityRef<Person>);

            _Organization = default(EntityRef<Organization>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "DocumentId", UpdateCheck = UpdateCheck.Never, Storage = "_DocumentId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int DocumentId
        {
            get => _DocumentId;

            set
            {
                if (_DocumentId != value)
                {
                    OnDocumentIdChanging(value);
                    SendPropertyChanging();
                    _DocumentId = value;
                    SendPropertyChanged("DocumentId");
                    OnDocumentIdChanged();
                }
            }
        }

        [Column(Name = "DocumentName", UpdateCheck = UpdateCheck.Never, Storage = "_DocumentName", DbType = "nvarchar(100)")]
        public string DocumentName
        {
            get => _DocumentName;

            set
            {
                if (_DocumentName != value)
                {
                    OnDocumentNameChanging(value);
                    SendPropertyChanging();
                    _DocumentName = value;
                    SendPropertyChanged("DocumentName");
                    OnDocumentNameChanged();
                }
            }
        }

        [Column(Name = "ImageId", UpdateCheck = UpdateCheck.Never, Storage = "_ImageId", DbType = "int NOT NULL UNIQUE")]
        public int ImageId
        {
            get => _ImageId;

            set
            {
                if (_ImageId != value)
                {
                    OnImageIdChanging(value);
                    SendPropertyChanging();
                    _ImageId = value;
                    SendPropertyChanged("ImageId");
                    OnImageIdChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "OrganizationId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    if (_Organization.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOrganizationIdChanging(value);
                    SendPropertyChanging();
                    _OrganizationId = value;
                    SendPropertyChanged("PeopleId");
                    OnOrganizationIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Keys

        [Association(Name = "Org_Member_Documents_PPL_FK", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.OrgMemberDocuments.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.OrgMemberDocuments.Add(this);
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

        [Association(Name = "Org_Member_Documents_ORG_FK", Storage = "_Organization", ThisKey = "OrganizationId", IsForeignKey = true)]
        public Organization Organization
        {
            get => _Organization.Entity;

            set
            {
                Organization previousValue = _Organization.Entity;
                if (((previousValue != value)
                            || (_Organization.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Organization.Entity = null;
                        previousValue.OrgMemberDocuments.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.OrgMemberDocuments.Add(this);
                        _OrganizationId = value.OrganizationId;
                    }

                    else
                    {
                        _OrganizationId = default(int);
                    }

                    SendPropertyChanged("Organization");
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
