using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MobileAppRoom")]
    public partial class MobileAppRoom : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _Floor;

        private string _Name;

        private string _Room;

        private int _X;

        private int _Y;

        private bool _Enabled;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnFloorChanging(int value);
        partial void OnFloorChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnRoomChanging(string value);
        partial void OnRoomChanged();

        partial void OnXChanging(int value);
        partial void OnXChanged();

        partial void OnYChanging(int value);
        partial void OnYChanged();

        partial void OnEnabledChanging(bool value);
        partial void OnEnabledChanged();

        #endregion

        public MobileAppRoom()
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

        [Column(Name = "floor", UpdateCheck = UpdateCheck.Never, Storage = "_Floor", DbType = "int NOT NULL")]
        public int Floor
        {
            get => _Floor;

            set
            {
                if (_Floor != value)
                {
                    OnFloorChanging(value);
                    SendPropertyChanging();
                    _Floor = value;
                    SendPropertyChanged("Floor");
                    OnFloorChanged();
                }
            }
        }

        [Column(Name = "name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(100) NOT NULL")]
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

        [Column(Name = "room", UpdateCheck = UpdateCheck.Never, Storage = "_Room", DbType = "nvarchar(50) NOT NULL")]
        public string Room
        {
            get => _Room;

            set
            {
                if (_Room != value)
                {
                    OnRoomChanging(value);
                    SendPropertyChanging();
                    _Room = value;
                    SendPropertyChanged("Room");
                    OnRoomChanged();
                }
            }
        }

        [Column(Name = "x", UpdateCheck = UpdateCheck.Never, Storage = "_X", DbType = "int NOT NULL")]
        public int X
        {
            get => _X;

            set
            {
                if (_X != value)
                {
                    OnXChanging(value);
                    SendPropertyChanging();
                    _X = value;
                    SendPropertyChanged("X");
                    OnXChanged();
                }
            }
        }

        [Column(Name = "y", UpdateCheck = UpdateCheck.Never, Storage = "_Y", DbType = "int NOT NULL")]
        public int Y
        {
            get => _Y;

            set
            {
                if (_Y != value)
                {
                    OnYChanging(value);
                    SendPropertyChanging();
                    _Y = value;
                    SendPropertyChanged("Y");
                    OnYChanged();
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
