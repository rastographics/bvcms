using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Content")]
    public partial class Content : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _Name;

        private string _Title;

        private string _Body;

        private DateTime? _DateCreated;

        private int _Id;

        private bool? _TextOnly;

        private int _TypeID;

        private int _ThumbID;

        private int _RoleID;

        private int _OwnerID;

        private string _CreatedBy;

        private DateTime? _Archived;

        private int? _ArchivedFromId;

        private int? _UseTimes;

        private bool? _Snippet;

        private EntitySet<ContentKeyWord> _ContentKeyWords;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnTitleChanging(string value);
        partial void OnTitleChanged();

        partial void OnBodyChanging(string value);
        partial void OnBodyChanged();

        partial void OnDateCreatedChanging(DateTime? value);
        partial void OnDateCreatedChanged();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnTextOnlyChanging(bool? value);
        partial void OnTextOnlyChanged();

        partial void OnTypeIDChanging(int value);
        partial void OnTypeIDChanged();

        partial void OnThumbIDChanging(int value);
        partial void OnThumbIDChanged();

        partial void OnRoleIDChanging(int value);
        partial void OnRoleIDChanged();

        partial void OnOwnerIDChanging(int value);
        partial void OnOwnerIDChanged();

        partial void OnCreatedByChanging(string value);
        partial void OnCreatedByChanged();

        partial void OnArchivedChanging(DateTime? value);
        partial void OnArchivedChanged();

        partial void OnArchivedFromIdChanging(int? value);
        partial void OnArchivedFromIdChanged();

        partial void OnUseTimesChanging(int? value);
        partial void OnUseTimesChanged();

        partial void OnSnippetChanging(bool? value);
        partial void OnSnippetChanged();

        #endregion

        public Content()
        {
            _ContentKeyWords = new EntitySet<ContentKeyWord>(new Action<ContentKeyWord>(attach_ContentKeyWords), new Action<ContentKeyWord>(detach_ContentKeyWords));

            OnCreated();
        }

        #region Columns

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(400) NOT NULL")]
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

        [Column(Name = "Title", UpdateCheck = UpdateCheck.Never, Storage = "_Title", DbType = "nvarchar(500)")]
        public string Title
        {
            get => _Title;

            set
            {
                if (_Title != value)
                {
                    OnTitleChanging(value);
                    SendPropertyChanging();
                    _Title = value;
                    SendPropertyChanged("Title");
                    OnTitleChanged();
                }
            }
        }

        [Column(Name = "Body", UpdateCheck = UpdateCheck.Never, Storage = "_Body", DbType = "nvarchar")]
        public string Body
        {
            get => _Body;

            set
            {
                if (_Body != value)
                {
                    OnBodyChanging(value);
                    SendPropertyChanging();
                    _Body = value;
                    SendPropertyChanged("Body");
                    OnBodyChanged();
                }
            }
        }

        [Column(Name = "DateCreated", UpdateCheck = UpdateCheck.Never, Storage = "_DateCreated", DbType = "datetime")]
        public DateTime? DateCreated
        {
            get => _DateCreated;

            set
            {
                if (_DateCreated != value)
                {
                    OnDateCreatedChanging(value);
                    SendPropertyChanging();
                    _DateCreated = value;
                    SendPropertyChanged("DateCreated");
                    OnDateCreatedChanged();
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

        [Column(Name = "TextOnly", UpdateCheck = UpdateCheck.Never, Storage = "_TextOnly", DbType = "bit")]
        public bool? TextOnly
        {
            get => _TextOnly;

            set
            {
                if (_TextOnly != value)
                {
                    OnTextOnlyChanging(value);
                    SendPropertyChanging();
                    _TextOnly = value;
                    SendPropertyChanged("TextOnly");
                    OnTextOnlyChanged();
                }
            }
        }

        [Column(Name = "TypeID", UpdateCheck = UpdateCheck.Never, Storage = "_TypeID", DbType = "int NOT NULL")]
        public int TypeID
        {
            get => _TypeID;

            set
            {
                if (_TypeID != value)
                {
                    OnTypeIDChanging(value);
                    SendPropertyChanging();
                    _TypeID = value;
                    SendPropertyChanged("TypeID");
                    OnTypeIDChanged();
                }
            }
        }

        [Column(Name = "ThumbID", UpdateCheck = UpdateCheck.Never, Storage = "_ThumbID", DbType = "int NOT NULL")]
        public int ThumbID
        {
            get => _ThumbID;

            set
            {
                if (_ThumbID != value)
                {
                    OnThumbIDChanging(value);
                    SendPropertyChanging();
                    _ThumbID = value;
                    SendPropertyChanged("ThumbID");
                    OnThumbIDChanged();
                }
            }
        }

        [Column(Name = "RoleID", UpdateCheck = UpdateCheck.Never, Storage = "_RoleID", DbType = "int NOT NULL")]
        public int RoleID
        {
            get => _RoleID;

            set
            {
                if (_RoleID != value)
                {
                    OnRoleIDChanging(value);
                    SendPropertyChanging();
                    _RoleID = value;
                    SendPropertyChanged("RoleID");
                    OnRoleIDChanged();
                }
            }
        }

        [Column(Name = "OwnerID", UpdateCheck = UpdateCheck.Never, Storage = "_OwnerID", DbType = "int NOT NULL")]
        public int OwnerID
        {
            get => _OwnerID;

            set
            {
                if (_OwnerID != value)
                {
                    OnOwnerIDChanging(value);
                    SendPropertyChanging();
                    _OwnerID = value;
                    SendPropertyChanged("OwnerID");
                    OnOwnerIDChanged();
                }
            }
        }

        [Column(Name = "CreatedBy", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "nvarchar(50)")]
        public string CreatedBy
        {
            get => _CreatedBy;

            set
            {
                if (_CreatedBy != value)
                {
                    OnCreatedByChanging(value);
                    SendPropertyChanging();
                    _CreatedBy = value;
                    SendPropertyChanged("CreatedBy");
                    OnCreatedByChanged();
                }
            }
        }

        [Column(Name = "Archived", UpdateCheck = UpdateCheck.Never, Storage = "_Archived", DbType = "datetime")]
        public DateTime? Archived
        {
            get => _Archived;

            set
            {
                if (_Archived != value)
                {
                    OnArchivedChanging(value);
                    SendPropertyChanging();
                    _Archived = value;
                    SendPropertyChanged("Archived");
                    OnArchivedChanged();
                }
            }
        }

        [Column(Name = "ArchivedFromId", UpdateCheck = UpdateCheck.Never, Storage = "_ArchivedFromId", DbType = "int")]
        public int? ArchivedFromId
        {
            get => _ArchivedFromId;

            set
            {
                if (_ArchivedFromId != value)
                {
                    OnArchivedFromIdChanging(value);
                    SendPropertyChanging();
                    _ArchivedFromId = value;
                    SendPropertyChanged("ArchivedFromId");
                    OnArchivedFromIdChanged();
                }
            }
        }

        [Column(Name = "UseTimes", UpdateCheck = UpdateCheck.Never, Storage = "_UseTimes", DbType = "int")]
        public int? UseTimes
        {
            get => _UseTimes;

            set
            {
                if (_UseTimes != value)
                {
                    OnUseTimesChanging(value);
                    SendPropertyChanging();
                    _UseTimes = value;
                    SendPropertyChanged("UseTimes");
                    OnUseTimesChanged();
                }
            }
        }

        [Column(Name = "Snippet", UpdateCheck = UpdateCheck.Never, Storage = "_Snippet", DbType = "bit")]
        public bool? Snippet
        {
            get => _Snippet;

            set
            {
                if (_Snippet != value)
                {
                    OnSnippetChanging(value);
                    SendPropertyChanging();
                    _Snippet = value;
                    SendPropertyChanged("Snippet");
                    OnSnippetChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_ContentKeyWords_Content", Storage = "_ContentKeyWords", OtherKey = "Id")]
        public EntitySet<ContentKeyWord> ContentKeyWords
           {
               get => _ContentKeyWords;

            set => _ContentKeyWords.Assign(value);

           }

        #endregion

        #region Foreign Keys

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

        private void attach_ContentKeyWords(ContentKeyWord entity)
        {
            SendPropertyChanging();
            entity.Content = this;
        }

        private void detach_ContentKeyWords(ContentKeyWord entity)
        {
            SendPropertyChanging();
            entity.Content = null;
        }
    }
}
