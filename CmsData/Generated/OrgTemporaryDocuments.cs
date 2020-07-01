using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.OrgTemporaryDocuments")]
    public partial class OrgTemporaryDocuments
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs("");

        #region Private Fields

        private int _TemporaryDocumentId;
        private string _DocumentName;
        private string _LastName;
        private string _FirstName;
        private string _EmailAddress;
        private int _ImageId;
        private int _OrganizationId;
        private DateTime _CreatedDate;

        private EntityRef<Organization> _Organization;
        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnTemporaryDocumentIdChanging(int value);
        partial void OnTemporaryDocumentIdChanged();

        partial void OnDocumentNameChanging(string value);
        partial void OnDocumentNameChanged();

        partial void OnLastNameChanging(string value);
        partial void OnLastNameChanged();

        partial void OnFirstNameChanging(string value);
        partial void OnFirstNameChanged();

        partial void OnEmailAddressChanging(string value);
        partial void OnEmailAddressChanged();

        partial void OnImageIdChanging(int value);
        partial void OnImageIdChanged();

        partial void OnOrganizationIdChanging(int value);
        partial void OnOrganizationIdChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();
        #endregion

        public OrgTemporaryDocuments()
        {
            _Organization = default(EntityRef<Organization>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "TemporaryDocumentId", UpdateCheck = UpdateCheck.Never, Storage = "_TemporaryDocumentId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int DocumentId
        {
            get => _TemporaryDocumentId;

            set
            {
                if (_TemporaryDocumentId != value)
                {
                    OnTemporaryDocumentIdChanging(value);
                    SendPropertyChanging();
                    _TemporaryDocumentId = value;
                    SendPropertyChanged("TemporaryDocumentId");
                    OnTemporaryDocumentIdChanged();
                }
            }
        }

        [Column(Name = "DocumentName", UpdateCheck = UpdateCheck.Never, Storage = "_DocumentName", DbType = "nvarchar(max)")]
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

        [Column(Name = "LastName", UpdateCheck = UpdateCheck.Never, Storage = "_LastName", DbType = "nvarchar(max)")]
        public string LastName
        {
            get => _LastName;

            set
            {
                if (_LastName != value)
                {
                    OnLastNameChanging(value);
                    SendPropertyChanging();
                    _LastName = value;
                    SendPropertyChanged("LastName");
                    OnLastNameChanged();
                }
            }
        }

        [Column(Name = "FirstName", UpdateCheck = UpdateCheck.Never, Storage = "_FirstName", DbType = "nvarchar(max)")]
        public string FirstName
        {
            get => _FirstName;

            set
            {
                if (_FirstName != value)
                {
                    OnFirstNameChanging(value);
                    SendPropertyChanging();
                    _FirstName = value;
                    SendPropertyChanged("FirstName");
                    OnFirstNameChanged();
                }
            }
        }

        [Column(Name = "EmailAddress", UpdateCheck = UpdateCheck.Never, Storage = "_EmailAddress", DbType = "nvarchar(max)")]
        public string EmailAddress
        {
            get => _EmailAddress;

            set
            {
                if (_EmailAddress != value)
                {
                    OnEmailAddressChanging(value);
                    SendPropertyChanging();
                    _EmailAddress = value;
                    SendPropertyChanged("EmailAddress");
                    OnEmailAddressChanged();
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

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime CreatedDate
        {
            get => _CreatedDate;

            set
            {
                if (_CreatedDate != value)
                {
                    OnCreatedDateChanging(value);
                    SendPropertyChanging();
                    _CreatedDate = value;
                    SendPropertyChanged("CreatedDate");
                    OnCreatedDateChanged();
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
                    SendPropertyChanged("OrganizationId");
                    OnOrganizationIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Keys

        [Association(Name = "Org_Temporary_Documents_ORG_FK", Storage = "_Organization", ThisKey = "OrganizationId", IsForeignKey = true)]
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
                        previousValue.OrgTemporaryDocuments.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.OrgTemporaryDocuments.Add(this);
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
