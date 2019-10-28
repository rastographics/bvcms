using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CheckInLabel")]
    public partial class CheckInLabel : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _TypeID;

        private string _Name;

        private int _Minimum;

        private int _Maximum;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnTypeIDChanging(int value);
        partial void OnTypeIDChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnMinimumChanging(int value);
        partial void OnMinimumChanged();

        partial void OnMaximumChanging(int value);
        partial void OnMaximumChanged();

        #endregion

        public CheckInLabel()
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

        [Column(Name = "typeID", UpdateCheck = UpdateCheck.Never, Storage = "_TypeID", DbType = "int NOT NULL")]
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

        [Column(Name = "name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(50) NOT NULL")]
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

        [Column(Name = "minimum", UpdateCheck = UpdateCheck.Never, Storage = "_Minimum", DbType = "int NOT NULL")]
        public int Minimum
        {
            get => _Minimum;

            set
            {
                if (_Minimum != value)
                {
                    OnMinimumChanging(value);
                    SendPropertyChanging();
                    _Minimum = value;
                    SendPropertyChanged("Minimum");
                    OnMinimumChanged();
                }
            }
        }

        [Column(Name = "maximum", UpdateCheck = UpdateCheck.Never, Storage = "_Maximum", DbType = "int NOT NULL")]
        public int Maximum
        {
            get => _Maximum;

            set
            {
                if (_Maximum != value)
                {
                    OnMaximumChanging(value);
                    SendPropertyChanging();
                    _Maximum = value;
                    SendPropertyChanged("Maximum");
                    OnMaximumChanged();
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
