using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.EmailLinks")]
    public partial class EmailLink : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private DateTime? _Created;

        private int? _EmailID;

        private string _Hash;

        private string _Link;

        private int _Count;

        private EntityRef<EmailQueue> _EmailQueue;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnCreatedChanging(DateTime? value);
        partial void OnCreatedChanged();

        partial void OnEmailIDChanging(int? value);
        partial void OnEmailIDChanged();

        partial void OnHashChanging(string value);
        partial void OnHashChanged();

        partial void OnLinkChanging(string value);
        partial void OnLinkChanged();

        partial void OnCountChanging(int value);
        partial void OnCountChanged();

        #endregion

        public EmailLink()
        {
            _EmailQueue = default(EntityRef<EmailQueue>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ID", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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

        [Column(Name = "Created", UpdateCheck = UpdateCheck.Never, Storage = "_Created", DbType = "datetime")]
        public DateTime? Created
        {
            get => _Created;

            set
            {
                if (_Created != value)
                {
                    OnCreatedChanging(value);
                    SendPropertyChanging();
                    _Created = value;
                    SendPropertyChanged("Created");
                    OnCreatedChanged();
                }
            }
        }

        [Column(Name = "EmailID", UpdateCheck = UpdateCheck.Never, Storage = "_EmailID", DbType = "int")]
        [IsForeignKey]
        public int? EmailID
        {
            get => _EmailID;

            set
            {
                if (_EmailID != value)
                {
                    if (_EmailQueue.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnEmailIDChanging(value);
                    SendPropertyChanging();
                    _EmailID = value;
                    SendPropertyChanged("EmailID");
                    OnEmailIDChanged();
                }
            }
        }

        [Column(Name = "Hash", UpdateCheck = UpdateCheck.Never, Storage = "_Hash", DbType = "nvarchar(50)")]
        public string Hash
        {
            get => _Hash;

            set
            {
                if (_Hash != value)
                {
                    OnHashChanging(value);
                    SendPropertyChanging();
                    _Hash = value;
                    SendPropertyChanged("Hash");
                    OnHashChanged();
                }
            }
        }

        [Column(Name = "Link", UpdateCheck = UpdateCheck.Never, Storage = "_Link", DbType = "nvarchar(2000)")]
        public string Link
        {
            get => _Link;

            set
            {
                if (_Link != value)
                {
                    OnLinkChanging(value);
                    SendPropertyChanging();
                    _Link = value;
                    SendPropertyChanged("Link");
                    OnLinkChanged();
                }
            }
        }

        [Column(Name = "Count", UpdateCheck = UpdateCheck.Never, Storage = "_Count", DbType = "int NOT NULL")]
        public int Count
        {
            get => _Count;

            set
            {
                if (_Count != value)
                {
                    OnCountChanging(value);
                    SendPropertyChanging();
                    _Count = value;
                    SendPropertyChanged("Count");
                    OnCountChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_EmailLinks_EmailQueue", Storage = "_EmailQueue", ThisKey = "EmailID", IsForeignKey = true)]
        public EmailQueue EmailQueue
        {
            get => _EmailQueue.Entity;

            set
            {
                EmailQueue previousValue = _EmailQueue.Entity;
                if (((previousValue != value)
                            || (_EmailQueue.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _EmailQueue.Entity = null;
                        previousValue.EmailLinks.Remove(this);
                    }

                    _EmailQueue.Entity = value;
                    if (value != null)
                    {
                        value.EmailLinks.Add(this);

                        _EmailID = value.Id;

                    }

                    else
                    {
                        _EmailID = default(int?);

                    }

                    SendPropertyChanged("EmailQueue");
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
