using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MergeHistory")]
    public partial class MergeHistory : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _FromId;

        private int _ToId;

        private string _FromName;

        private string _ToName;

        private DateTime _Dt;

        private string _WhoName;

        private int? _WhoId;

        private string _Action;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnFromIdChanging(int value);
        partial void OnFromIdChanged();

        partial void OnToIdChanging(int value);
        partial void OnToIdChanged();

        partial void OnFromNameChanging(string value);
        partial void OnFromNameChanged();

        partial void OnToNameChanging(string value);
        partial void OnToNameChanged();

        partial void OnDtChanging(DateTime value);
        partial void OnDtChanged();

        partial void OnWhoNameChanging(string value);
        partial void OnWhoNameChanged();

        partial void OnWhoIdChanging(int? value);
        partial void OnWhoIdChanged();

        partial void OnActionChanging(string value);
        partial void OnActionChanged();

        #endregion

        public MergeHistory()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "FromId", UpdateCheck = UpdateCheck.Never, Storage = "_FromId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int FromId
        {
            get => _FromId;

            set
            {
                if (_FromId != value)
                {
                    OnFromIdChanging(value);
                    SendPropertyChanging();
                    _FromId = value;
                    SendPropertyChanged("FromId");
                    OnFromIdChanged();
                }
            }
        }

        [Column(Name = "ToId", UpdateCheck = UpdateCheck.Never, Storage = "_ToId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int ToId
        {
            get => _ToId;

            set
            {
                if (_ToId != value)
                {
                    OnToIdChanging(value);
                    SendPropertyChanging();
                    _ToId = value;
                    SendPropertyChanged("ToId");
                    OnToIdChanged();
                }
            }
        }

        [Column(Name = "FromName", UpdateCheck = UpdateCheck.Never, Storage = "_FromName", DbType = "nvarchar(150)")]
        public string FromName
        {
            get => _FromName;

            set
            {
                if (_FromName != value)
                {
                    OnFromNameChanging(value);
                    SendPropertyChanging();
                    _FromName = value;
                    SendPropertyChanged("FromName");
                    OnFromNameChanged();
                }
            }
        }

        [Column(Name = "ToName", UpdateCheck = UpdateCheck.Never, Storage = "_ToName", DbType = "nvarchar(150)")]
        public string ToName
        {
            get => _ToName;

            set
            {
                if (_ToName != value)
                {
                    OnToNameChanging(value);
                    SendPropertyChanging();
                    _ToName = value;
                    SendPropertyChanged("ToName");
                    OnToNameChanged();
                }
            }
        }

        [Column(Name = "Dt", UpdateCheck = UpdateCheck.Never, Storage = "_Dt", DbType = "datetime NOT NULL", IsPrimaryKey = true)]
        public DateTime Dt
        {
            get => _Dt;

            set
            {
                if (_Dt != value)
                {
                    OnDtChanging(value);
                    SendPropertyChanging();
                    _Dt = value;
                    SendPropertyChanged("Dt");
                    OnDtChanged();
                }
            }
        }

        [Column(Name = "WhoName", UpdateCheck = UpdateCheck.Never, Storage = "_WhoName", DbType = "nvarchar(150)")]
        public string WhoName
        {
            get => _WhoName;

            set
            {
                if (_WhoName != value)
                {
                    OnWhoNameChanging(value);
                    SendPropertyChanging();
                    _WhoName = value;
                    SendPropertyChanged("WhoName");
                    OnWhoNameChanged();
                }
            }
        }

        [Column(Name = "WhoId", UpdateCheck = UpdateCheck.Never, Storage = "_WhoId", DbType = "int")]
        public int? WhoId
        {
            get => _WhoId;

            set
            {
                if (_WhoId != value)
                {
                    OnWhoIdChanging(value);
                    SendPropertyChanging();
                    _WhoId = value;
                    SendPropertyChanged("WhoId");
                    OnWhoIdChanged();
                }
            }
        }

        [Column(Name = "Action", UpdateCheck = UpdateCheck.Never, Storage = "_Action", DbType = "varchar(50)")]
        public string Action
        {
            get => _Action;

            set
            {
                if (_Action != value)
                {
                    OnActionChanging(value);
                    SendPropertyChanging();
                    _Action = value;
                    SendPropertyChanged("Action");
                    OnActionChanged();
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
