using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MobileAppActions")]
    public partial class MobileAppAction : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _Type;

        private string _Title;

        private int _Option;

        private string _Data;

        private int _Order;

        private int _LoginType;

        private bool _Enabled;

        private string _Roles;

        private int _Api;

        private DateTime _Active;

        private string _AltTitle;

        private int _Rebranded;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnTypeChanging(int value);
        partial void OnTypeChanged();

        partial void OnTitleChanging(string value);
        partial void OnTitleChanged();

        partial void OnOptionChanging(int value);
        partial void OnOptionChanged();

        partial void OnDataChanging(string value);
        partial void OnDataChanged();

        partial void OnOrderChanging(int value);
        partial void OnOrderChanged();

        partial void OnLoginTypeChanging(int value);
        partial void OnLoginTypeChanged();

        partial void OnEnabledChanging(bool value);
        partial void OnEnabledChanged();

        partial void OnRolesChanging(string value);
        partial void OnRolesChanged();

        partial void OnApiChanging(int value);
        partial void OnApiChanged();

        partial void OnActiveChanging(DateTime value);
        partial void OnActiveChanged();

        partial void OnAltTitleChanging(string value);
        partial void OnAltTitleChanged();

        partial void OnRebrandedChanging(int value);
        partial void OnRebrandedChanged();

        #endregion

        public MobileAppAction()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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

        [Column(Name = "type", UpdateCheck = UpdateCheck.Never, Storage = "_Type", DbType = "int NOT NULL")]
        public int Type
        {
            get => _Type;

            set
            {
                if (_Type != value)
                {
                    OnTypeChanging(value);
                    SendPropertyChanging();
                    _Type = value;
                    SendPropertyChanged("Type");
                    OnTypeChanged();
                }
            }
        }

        [Column(Name = "title", UpdateCheck = UpdateCheck.Never, Storage = "_Title", DbType = "nvarchar(50) NOT NULL")]
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

        [Column(Name = "option", UpdateCheck = UpdateCheck.Never, Storage = "_Option", DbType = "int NOT NULL")]
        public int Option
        {
            get => _Option;

            set
            {
                if (_Option != value)
                {
                    OnOptionChanging(value);
                    SendPropertyChanging();
                    _Option = value;
                    SendPropertyChanged("Option");
                    OnOptionChanged();
                }
            }
        }

        [Column(Name = "data", UpdateCheck = UpdateCheck.Never, Storage = "_Data", DbType = "nvarchar NOT NULL")]
        public string Data
        {
            get => _Data;

            set
            {
                if (_Data != value)
                {
                    OnDataChanging(value);
                    SendPropertyChanging();
                    _Data = value;
                    SendPropertyChanged("Data");
                    OnDataChanged();
                }
            }
        }

        [Column(Name = "order", UpdateCheck = UpdateCheck.Never, Storage = "_Order", DbType = "int NOT NULL")]
        public int Order
        {
            get => _Order;

            set
            {
                if (_Order != value)
                {
                    OnOrderChanging(value);
                    SendPropertyChanging();
                    _Order = value;
                    SendPropertyChanged("Order");
                    OnOrderChanged();
                }
            }
        }

        [Column(Name = "loginType", UpdateCheck = UpdateCheck.Never, Storage = "_LoginType", DbType = "int NOT NULL")]
        public int LoginType
        {
            get => _LoginType;

            set
            {
                if (_LoginType != value)
                {
                    OnLoginTypeChanging(value);
                    SendPropertyChanging();
                    _LoginType = value;
                    SendPropertyChanged("LoginType");
                    OnLoginTypeChanged();
                }
            }
        }

        [Column(Name = "enabled", UpdateCheck = UpdateCheck.Never, Storage = "_Enabled", DbType = "bit NOT NULL")]
        public bool Enabled
        {
            get => _Enabled;

            set
            {
                if (_Enabled != value)
                {
                    OnEnabledChanging(value);
                    SendPropertyChanging();
                    _Enabled = value;
                    SendPropertyChanged("Enabled");
                    OnEnabledChanged();
                }
            }
        }

        [Column(Name = "roles", UpdateCheck = UpdateCheck.Never, Storage = "_Roles", DbType = "nvarchar NOT NULL")]
        public string Roles
        {
            get => _Roles;

            set
            {
                if (_Roles != value)
                {
                    OnRolesChanging(value);
                    SendPropertyChanging();
                    _Roles = value;
                    SendPropertyChanged("Roles");
                    OnRolesChanged();
                }
            }
        }

        [Column(Name = "api", UpdateCheck = UpdateCheck.Never, Storage = "_Api", DbType = "int NOT NULL")]
        public int Api
        {
            get => _Api;

            set
            {
                if (_Api != value)
                {
                    OnApiChanging(value);
                    SendPropertyChanging();
                    _Api = value;
                    SendPropertyChanged("Api");
                    OnApiChanged();
                }
            }
        }

        [Column(Name = "active", UpdateCheck = UpdateCheck.Never, Storage = "_Active", DbType = "datetime NOT NULL")]
        public DateTime Active
        {
            get => _Active;

            set
            {
                if (_Active != value)
                {
                    OnActiveChanging(value);
                    SendPropertyChanging();
                    _Active = value;
                    SendPropertyChanged("Active");
                    OnActiveChanged();
                }
            }
        }

        [Column(Name = "altTitle", UpdateCheck = UpdateCheck.Never, Storage = "_AltTitle", DbType = "nvarchar(50) NOT NULL")]
        public string AltTitle
        {
            get => _AltTitle;

            set
            {
                if (_AltTitle != value)
                {
                    OnAltTitleChanging(value);
                    SendPropertyChanging();
                    _AltTitle = value;
                    SendPropertyChanged("AltTitle");
                    OnAltTitleChanged();
                }
            }
        }

        [Column(Name = "rebranded", UpdateCheck = UpdateCheck.Never, Storage = "_Rebranded", DbType = "int NOT NULL")]
        public int Rebranded
        {
            get => _Rebranded;

            set
            {
                if (_Rebranded != value)
                {
                    OnRebrandedChanging(value);
                    SendPropertyChanging();
                    _Rebranded = value;
                    SendPropertyChanged("Rebranded");
                    OnRebrandedChanged();
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
