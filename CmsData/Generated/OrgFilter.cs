using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.OrgFilter")]
    public partial class OrgFilter : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private Guid _QueryId;

        private int _Id;

        private string _GroupSelect;

        private string _FirstName;

        private string _LastName;

        private string _SgFilter;

        private bool? _ShowHidden;

        private bool? _FilterIndividuals;

        private bool? _FilterTag;

        private int? _TagId;

        private DateTime _LastUpdated;

        private int? _UserId;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnQueryIdChanging(Guid value);
        partial void OnQueryIdChanged();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnGroupSelectChanging(string value);
        partial void OnGroupSelectChanged();

        partial void OnFirstNameChanging(string value);
        partial void OnFirstNameChanged();

        partial void OnLastNameChanging(string value);
        partial void OnLastNameChanged();

        partial void OnSgFilterChanging(string value);
        partial void OnSgFilterChanged();

        partial void OnShowHiddenChanging(bool? value);
        partial void OnShowHiddenChanged();

        partial void OnFilterIndividualsChanging(bool? value);
        partial void OnFilterIndividualsChanged();

        partial void OnFilterTagChanging(bool? value);
        partial void OnFilterTagChanged();

        partial void OnTagIdChanging(int? value);
        partial void OnTagIdChanged();

        partial void OnLastUpdatedChanging(DateTime value);
        partial void OnLastUpdatedChanged();

        partial void OnUserIdChanging(int? value);
        partial void OnUserIdChanged();

        #endregion

        public OrgFilter()
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

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int NOT NULL")]
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

        [Column(Name = "GroupSelect", UpdateCheck = UpdateCheck.Never, Storage = "_GroupSelect", DbType = "varchar(50)")]
        public string GroupSelect
        {
            get => _GroupSelect;

            set
            {
                if (_GroupSelect != value)
                {
                    OnGroupSelectChanging(value);
                    SendPropertyChanging();
                    _GroupSelect = value;
                    SendPropertyChanged("GroupSelect");
                    OnGroupSelectChanged();
                }
            }
        }

        [Column(Name = "FirstName", UpdateCheck = UpdateCheck.Never, Storage = "_FirstName", DbType = "varchar(50)")]
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

        [Column(Name = "LastName", UpdateCheck = UpdateCheck.Never, Storage = "_LastName", DbType = "varchar(50)")]
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

        [Column(Name = "SgFilter", UpdateCheck = UpdateCheck.Never, Storage = "_SgFilter", DbType = "varchar(900)")]
        public string SgFilter
        {
            get => _SgFilter;

            set
            {
                if (_SgFilter != value)
                {
                    OnSgFilterChanging(value);
                    SendPropertyChanging();
                    _SgFilter = value;
                    SendPropertyChanged("SgFilter");
                    OnSgFilterChanged();
                }
            }
        }

        [Column(Name = "ShowHidden", UpdateCheck = UpdateCheck.Never, Storage = "_ShowHidden", DbType = "bit")]
        public bool? ShowHidden
        {
            get => _ShowHidden;

            set
            {
                if (_ShowHidden != value)
                {
                    OnShowHiddenChanging(value);
                    SendPropertyChanging();
                    _ShowHidden = value;
                    SendPropertyChanged("ShowHidden");
                    OnShowHiddenChanged();
                }
            }
        }

        [Column(Name = "FilterIndividuals", UpdateCheck = UpdateCheck.Never, Storage = "_FilterIndividuals", DbType = "bit")]
        public bool? FilterIndividuals
        {
            get => _FilterIndividuals;

            set
            {
                if (_FilterIndividuals != value)
                {
                    OnFilterIndividualsChanging(value);
                    SendPropertyChanging();
                    _FilterIndividuals = value;
                    SendPropertyChanged("FilterIndividuals");
                    OnFilterIndividualsChanged();
                }
            }
        }

        [Column(Name = "FilterTag", UpdateCheck = UpdateCheck.Never, Storage = "_FilterTag", DbType = "bit")]
        public bool? FilterTag
        {
            get => _FilterTag;

            set
            {
                if (_FilterTag != value)
                {
                    OnFilterTagChanging(value);
                    SendPropertyChanging();
                    _FilterTag = value;
                    SendPropertyChanged("FilterTag");
                    OnFilterTagChanged();
                }
            }
        }

        [Column(Name = "TagId", UpdateCheck = UpdateCheck.Never, Storage = "_TagId", DbType = "int")]
        public int? TagId
        {
            get => _TagId;

            set
            {
                if (_TagId != value)
                {
                    OnTagIdChanging(value);
                    SendPropertyChanging();
                    _TagId = value;
                    SendPropertyChanged("TagId");
                    OnTagIdChanged();
                }
            }
        }

        [Column(Name = "LastUpdated", UpdateCheck = UpdateCheck.Never, Storage = "_LastUpdated", DbType = "datetime NOT NULL")]
        public DateTime LastUpdated
        {
            get => _LastUpdated;

            set
            {
                if (_LastUpdated != value)
                {
                    OnLastUpdatedChanging(value);
                    SendPropertyChanging();
                    _LastUpdated = value;
                    SendPropertyChanged("LastUpdated");
                    OnLastUpdatedChanged();
                }
            }
        }

        [Column(Name = "UserId", UpdateCheck = UpdateCheck.Never, Storage = "_UserId", DbType = "int")]
        public int? UserId
        {
            get => _UserId;

            set
            {
                if (_UserId != value)
                {
                    OnUserIdChanging(value);
                    SendPropertyChanging();
                    _UserId = value;
                    SendPropertyChanged("UserId");
                    OnUserIdChanged();
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
