using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CheckInLabelEntryAlignment")]
    public partial class CheckInLabelEntryAlignment : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _NameX;

        private string _NameY;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnNameXChanging(string value);
        partial void OnNameXChanged();

        partial void OnNameYChanging(string value);
        partial void OnNameYChanged();

        #endregion

        public CheckInLabelEntryAlignment()
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

        [Column(Name = "nameX", UpdateCheck = UpdateCheck.Never, Storage = "_NameX", DbType = "nvarchar(10) NOT NULL")]
        public string NameX
        {
            get => _NameX;

            set
            {
                if (_NameX != value)
                {
                    OnNameXChanging(value);
                    SendPropertyChanging();
                    _NameX = value;
                    SendPropertyChanged("NameX");
                    OnNameXChanged();
                }
            }
        }

        [Column(Name = "nameY", UpdateCheck = UpdateCheck.Never, Storage = "_NameY", DbType = "nvarchar(10) NOT NULL")]
        public string NameY
        {
            get => _NameY;

            set
            {
                if (_NameY != value)
                {
                    OnNameYChanging(value);
                    SendPropertyChanging();
                    _NameY = value;
                    SendPropertyChanged("NameY");
                    OnNameYChanged();
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
