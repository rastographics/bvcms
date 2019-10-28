using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MobileAppIcons")]
    public partial class MobileAppIcon : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _SetID;

        private int _ActionID;

        private string _Url;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnSetIDChanging(int value);
        partial void OnSetIDChanged();

        partial void OnActionIDChanging(int value);
        partial void OnActionIDChanged();

        partial void OnUrlChanging(string value);
        partial void OnUrlChanged();

        #endregion

        public MobileAppIcon()
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

        [Column(Name = "setID", UpdateCheck = UpdateCheck.Never, Storage = "_SetID", DbType = "int NOT NULL")]
        public int SetID
        {
            get => _SetID;

            set
            {
                if (_SetID != value)
                {
                    OnSetIDChanging(value);
                    SendPropertyChanging();
                    _SetID = value;
                    SendPropertyChanged("SetID");
                    OnSetIDChanged();
                }
            }
        }

        [Column(Name = "actionID", UpdateCheck = UpdateCheck.Never, Storage = "_ActionID", DbType = "int NOT NULL")]
        public int ActionID
        {
            get => _ActionID;

            set
            {
                if (_ActionID != value)
                {
                    OnActionIDChanging(value);
                    SendPropertyChanging();
                    _ActionID = value;
                    SendPropertyChanged("ActionID");
                    OnActionIDChanged();
                }
            }
        }

        [Column(Name = "url", UpdateCheck = UpdateCheck.Never, Storage = "_Url", DbType = "nvarchar(200) NOT NULL")]
        public string Url
        {
            get => _Url;

            set
            {
                if (_Url != value)
                {
                    OnUrlChanging(value);
                    SendPropertyChanging();
                    _Url = value;
                    SendPropertyChanged("Url");
                    OnUrlChanged();
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
