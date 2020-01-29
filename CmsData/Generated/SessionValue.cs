using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.SessionValues")]
    public partial class SessionValue : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs("");

        #region Private Fields

        private string _SessionId;
        private string _Name;
        private string _Value;
        private DateTime _CreatedDate = DateTime.Now;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnSessionIdChanging(string value);
        partial void OnSessionIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnValueChanging(string value);
        partial void OnValueChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        #endregion

        public SessionValue()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "SessionId", UpdateCheck = UpdateCheck.Never, Storage = "_SessionId", DbType = "nvarchar(88) NOT NULL", IsPrimaryKey = true)]
        public string SessionId
        {
            get => _SessionId;

            set
            {
                if (_SessionId != value)
                {
                    OnSessionIdChanging(value);
                    SendPropertyChanging();
                    _SessionId = value;
                    SendPropertyChanged("SessionId");
                    OnSessionIdChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(88) NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "Value", UpdateCheck = UpdateCheck.Never, Storage = "_Value", DbType = "nvarchar(MAX) NULL")]
        public string Value
        {
            get => _Value;

            set
            {
                if (_Value != value)
                {
                    OnValueChanging(value);
                    SendPropertyChanging();
                    _Value = value;
                    SendPropertyChanged("Value");
                    OnValueChanged();
                }
            }
        }

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime CreatedDate
        {
            get => _CreatedDate;

            set
            {
                if (_CreatedDate != value)
                {
                    OnCreatedDateChanging(value);
                    SendPropertyChanging();
                    _CreatedDate = value;
                    SendPropertyChanged("CreatedDate");
                    OnCreatedDateChanged();
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
