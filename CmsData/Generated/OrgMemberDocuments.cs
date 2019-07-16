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
        private int _ImageId;
        private int _PeopleId;
        private int _OrganizationId;

        private EntityRef<Person> _People;
        private EntityRef<Organization> _Organization;
        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnDocumentIdChanging(int value);
        partial void OnDocumentIdChanged();

        partial void OnImageIdChanging(int value);
        partial void OnImageIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnOrganizationIdChanging(int value);
        partial void OnOrganizationIdChanged();
        #endregion

        public OrgMemberDocuments()
        {
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

        [Column(Name = "ImageId", UpdateCheck = UpdateCheck.Never, Storage = "_ImageId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL UNIQUE", IsPrimaryKey = true, IsDbGenerated = true)]
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
                    if (this._People.HasLoadedOrAssignedValue)
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
