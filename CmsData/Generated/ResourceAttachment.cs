using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ResourceAttachment")]
    public partial class ResourceAttachment : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ResourceAttachmentId;

        private int _ResourceId;

        private string _FilePath;

        private int? _FileTypeId;

        private string _Name;

        private DateTime? _CreationDate;

        private DateTime? _UpdateDate;

        private int? _DisplayOrder;

        private EntityRef<Resource> _Resource;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnResourceAttachmentIdChanging(int value);
        partial void OnResourceAttachmentIdChanged();

        partial void OnResourceIdChanging(int value);
        partial void OnResourceIdChanged();

        partial void OnFilePathChanging(string value);
        partial void OnFilePathChanged();

        partial void OnFileTypeIdChanging(int? value);
        partial void OnFileTypeIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnCreationDateChanging(DateTime? value);
        partial void OnCreationDateChanged();

        partial void OnUpdateDateChanging(DateTime? value);
        partial void OnUpdateDateChanged();

        partial void OnDisplayOrderChanging(int? value);
        partial void OnDisplayOrderChanged();

        #endregion

        public ResourceAttachment()
        {
            _Resource = default(EntityRef<Resource>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ResourceAttachmentId", UpdateCheck = UpdateCheck.Never, Storage = "_ResourceAttachmentId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ResourceAttachmentId
        {
            get => _ResourceAttachmentId;

            set
            {
                if (_ResourceAttachmentId != value)
                {
                    OnResourceAttachmentIdChanging(value);
                    SendPropertyChanging();
                    _ResourceAttachmentId = value;
                    SendPropertyChanged("ResourceAttachmentId");
                    OnResourceAttachmentIdChanged();
                }
            }
        }

        [Column(Name = "ResourceId", UpdateCheck = UpdateCheck.Never, Storage = "_ResourceId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int ResourceId
        {
            get => _ResourceId;

            set
            {
                if (_ResourceId != value)
                {
                    if (_Resource.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnResourceIdChanging(value);
                    SendPropertyChanging();
                    _ResourceId = value;
                    SendPropertyChanged("ResourceId");
                    OnResourceIdChanged();
                }
            }
        }

        [Column(Name = "FilePath", UpdateCheck = UpdateCheck.Never, Storage = "_FilePath", DbType = "nvarchar")]
        public string FilePath
        {
            get => _FilePath;

            set
            {
                if (_FilePath != value)
                {
                    OnFilePathChanging(value);
                    SendPropertyChanging();
                    _FilePath = value;
                    SendPropertyChanged("FilePath");
                    OnFilePathChanged();
                }
            }
        }

        [Column(Name = "FileTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_FileTypeId", DbType = "int")]
        public int? FileTypeId
        {
            get => _FileTypeId;

            set
            {
                if (_FileTypeId != value)
                {
                    OnFileTypeIdChanging(value);
                    SendPropertyChanging();
                    _FileTypeId = value;
                    SendPropertyChanged("FileTypeId");
                    OnFileTypeIdChanged();
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

        [Column(Name = "CreationDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreationDate", DbType = "datetime")]
        public DateTime? CreationDate
        {
            get => _CreationDate;

            set
            {
                if (_CreationDate != value)
                {
                    OnCreationDateChanging(value);
                    SendPropertyChanging();
                    _CreationDate = value;
                    SendPropertyChanged("CreationDate");
                    OnCreationDateChanged();
                }
            }
        }

        [Column(Name = "UpdateDate", UpdateCheck = UpdateCheck.Never, Storage = "_UpdateDate", DbType = "datetime")]
        public DateTime? UpdateDate
        {
            get => _UpdateDate;

            set
            {
                if (_UpdateDate != value)
                {
                    OnUpdateDateChanging(value);
                    SendPropertyChanging();
                    _UpdateDate = value;
                    SendPropertyChanged("UpdateDate");
                    OnUpdateDateChanged();
                }
            }
        }

        [Column(Name = "DisplayOrder", UpdateCheck = UpdateCheck.Never, Storage = "_DisplayOrder", DbType = "int")]
        public int? DisplayOrder
        {
            get => _DisplayOrder;

            set
            {
                if (_DisplayOrder != value)
                {
                    OnDisplayOrderChanging(value);
                    SendPropertyChanging();
                    _DisplayOrder = value;
                    SendPropertyChanged("DisplayOrder");
                    OnDisplayOrderChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_ResourceAttachment_Resource", Storage = "_Resource", ThisKey = "ResourceId", IsForeignKey = true)]
        public Resource Resource
        {
            get => _Resource.Entity;

            set
            {
                Resource previousValue = _Resource.Entity;
                if (((previousValue != value)
                            || (_Resource.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Resource.Entity = null;
                        previousValue.ResourceAttachments.Remove(this);
                    }

                    _Resource.Entity = value;
                    if (value != null)
                    {
                        value.ResourceAttachments.Add(this);

                        _ResourceId = value.ResourceId;

                    }

                    else
                    {
                        _ResourceId = default(int);

                    }

                    SendPropertyChanged("Resource");
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
