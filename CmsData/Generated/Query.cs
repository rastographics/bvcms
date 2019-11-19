using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Query")]
    public partial class Query : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private Guid _QueryId;

        private string _Text;

        private string _Owner;

        private DateTime? _Created;

        private DateTime? _LastRun;

        private string _Name;

        private bool _Ispublic;

        private int _RunCount;

        private Guid? _CopiedFrom;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnQueryIdChanging(Guid value);
        partial void OnQueryIdChanged();

        partial void OnTextChanging(string value);
        partial void OnTextChanged();

        partial void OnOwnerChanging(string value);
        partial void OnOwnerChanged();

        partial void OnCreatedChanging(DateTime? value);
        partial void OnCreatedChanged();

        partial void OnLastRunChanging(DateTime? value);
        partial void OnLastRunChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnIspublicChanging(bool value);
        partial void OnIspublicChanged();

        partial void OnRunCountChanging(int value);
        partial void OnRunCountChanged();

        partial void OnCopiedFromChanging(Guid? value);
        partial void OnCopiedFromChanged();

        #endregion

        public Query()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "QueryId", UpdateCheck = UpdateCheck.Never, Storage = "_QueryId", DbType = "uniqueidentifier NOT NULL", IsPrimaryKey = true)]
        public Guid QueryId
        {
            get => _QueryId;

            set
            {
                if (_QueryId != value)
                {
                    OnQueryIdChanging(value);
                    SendPropertyChanging();
                    _QueryId = value;
                    SendPropertyChanged("QueryId");
                    OnQueryIdChanged();
                }
            }
        }

        [Column(Name = "text", UpdateCheck = UpdateCheck.Never, Storage = "_Text", DbType = "nvarchar")]
        public string Text
        {
            get => _Text;

            set
            {
                if (_Text != value)
                {
                    OnTextChanging(value);
                    SendPropertyChanging();
                    _Text = value;
                    SendPropertyChanged("Text");
                    OnTextChanged();
                }
            }
        }

        [Column(Name = "owner", UpdateCheck = UpdateCheck.Never, Storage = "_Owner", DbType = "nvarchar(50)")]
        public string Owner
        {
            get => _Owner;

            set
            {
                if (_Owner != value)
                {
                    OnOwnerChanging(value);
                    SendPropertyChanging();
                    _Owner = value;
                    SendPropertyChanged("Owner");
                    OnOwnerChanged();
                }
            }
        }

        [Column(Name = "created", UpdateCheck = UpdateCheck.Never, Storage = "_Created", DbType = "datetime")]
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

        [Column(Name = "lastRun", UpdateCheck = UpdateCheck.Never, Storage = "_LastRun", DbType = "datetime")]
        public DateTime? LastRun
        {
            get => _LastRun;

            set
            {
                if (_LastRun != value)
                {
                    OnLastRunChanging(value);
                    SendPropertyChanging();
                    _LastRun = value;
                    SendPropertyChanged("LastRun");
                    OnLastRunChanged();
                }
            }
        }

        [Column(Name = "name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(100)")]
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

        [Column(Name = "ispublic", UpdateCheck = UpdateCheck.Never, Storage = "_Ispublic", DbType = "bit NOT NULL")]
        public bool Ispublic
        {
            get => _Ispublic;

            set
            {
                if (_Ispublic != value)
                {
                    OnIspublicChanging(value);
                    SendPropertyChanging();
                    _Ispublic = value;
                    SendPropertyChanged("Ispublic");
                    OnIspublicChanged();
                }
            }
        }

        [Column(Name = "runCount", UpdateCheck = UpdateCheck.Never, Storage = "_RunCount", DbType = "int NOT NULL")]
        public int RunCount
        {
            get => _RunCount;

            set
            {
                if (_RunCount != value)
                {
                    OnRunCountChanging(value);
                    SendPropertyChanging();
                    _RunCount = value;
                    SendPropertyChanged("RunCount");
                    OnRunCountChanged();
                }
            }
        }

        [Column(Name = "CopiedFrom", UpdateCheck = UpdateCheck.Never, Storage = "_CopiedFrom", DbType = "uniqueidentifier")]
        public Guid? CopiedFrom
        {
            get => _CopiedFrom;

            set
            {
                if (_CopiedFrom != value)
                {
                    OnCopiedFromChanging(value);
                    SendPropertyChanging();
                    _CopiedFrom = value;
                    SendPropertyChanged("CopiedFrom");
                    OnCopiedFromChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

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
    }
}
