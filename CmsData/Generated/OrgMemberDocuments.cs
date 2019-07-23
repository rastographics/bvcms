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
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            this._Person = default(EntityRef<Person>);

            this._Organization = default(EntityRef<Organization>);

            OnCreated();
        }

        #region Columns
        [Column(Name = "DocumentId", UpdateCheck = UpdateCheck.Never, Storage = "_DocumentId", AutoSync = AutoSync.OnInsert, DbType = "int IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int DocumentId
        {
            get { return this._DocumentId; }

            set
            {
                if (this._DocumentId != value)
                {
                    this.OnDocumentIdChanging(value);
                    this.SendPropertyChanging();
                    this._DocumentId = value;
                    this.SendPropertyChanged("DocumentId");
                    this.OnDocumentIdChanged();
                }
            }
        }

        [Column(Name = "DocumentName", UpdateCheck = UpdateCheck.Never, Storage = "_DocumentName", DbType = "nvarchar(100)")]
        public string DocumentName
        {
            get { return this._DocumentName; }

            set
            {
                if (this._DocumentName != value)
                {

                    this.OnDocumentNameChanging(value);
                    this.SendPropertyChanging();
                    this._DocumentName = value;
                    this.SendPropertyChanged("DocumentName");
                    this.OnDocumentNameChanged();
                }
            }

        }

        [Column(Name = "ImageId", UpdateCheck = UpdateCheck.Never, Storage = "_ImageId", DbType = "int NOT NULL UNIQUE")]
        public int ImageId
        {
            get { return this._ImageId; }

            set
            {
                if (this._ImageId != value)
                {
                    this.OnImageIdChanging(value);
                    this.SendPropertyChanging();
                    this._ImageId = value;
                    this.SendPropertyChanged("ImageId");
                    this.OnImageIdChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "OrganizationId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int OrganizationId
        {
            get { return this._OrganizationId; }

            set
            {
                if (this._OrganizationId != value)
                {
                    if (this._Organization.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnOrganizationIdChanging(value);
                    this.SendPropertyChanging();
                    this._OrganizationId = value;
                    this.SendPropertyChanged("PeopleId");
                    this.OnOrganizationIdChanged();
                }
            }
        }
        #endregion

        #region Foreign Keys
        [Association(Name = "Org_Member_Documents_PPL_FK", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.OrgMemberDocuments.Remove(this);
                    }

                    this._Person.Entity = value;
                    if (value != null)
                    {
                        value.OrgMemberDocuments.Add(this);

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

        [Association(Name = "Org_Member_Documents_ORG_FK", Storage = "_Organization", ThisKey = "OrganizationId", IsForeignKey = true)]
        public Organization Organization
        {
            get { return this._Organization.Entity; }

            set
            {
                Organization previousValue = this._Organization.Entity;
                if (((previousValue != value)
                            || (this._Organization.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if (previousValue != null)
                    {
                        this._Organization.Entity = null;
                        previousValue.OrgMemberDocuments.Remove(this);
                    }

                    this._Organization.Entity = value;
                    if (value != null)
                    {
                        value.OrgMemberDocuments.Add(this);

                        this._OrganizationId = value.OrganizationId;

                    }

                    else
                    {

                        this._OrganizationId = default(int);

                    }

                    this.SendPropertyChanged("Organization");
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
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
