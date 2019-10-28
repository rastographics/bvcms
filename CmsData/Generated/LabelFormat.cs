using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.LabelFormats")]
    public partial class LabelFormat : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Name;

        private int _Size;

        private string _Format;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnSizeChanging(int value);
        partial void OnSizeChanged();

        partial void OnFormatChanging(string value);
        partial void OnFormatChanged();

        #endregion

        public LabelFormat()
        {
            OnCreated();
        }

        #region Columns

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

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(30) NOT NULL")]
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

        [Column(Name = "Size", UpdateCheck = UpdateCheck.Never, Storage = "_Size", DbType = "int NOT NULL")]
        public int Size
        {
            get => _Size;

            set
            {
                if (_Size != value)
                {
                    OnSizeChanging(value);
                    SendPropertyChanging();
                    _Size = value;
                    SendPropertyChanged("Size");
                    OnSizeChanged();
                }
            }
        }

        [Column(Name = "Format", UpdateCheck = UpdateCheck.Never, Storage = "_Format", DbType = "text(2147483647) NOT NULL")]
        public string Format
        {
            get => _Format;

            set
            {
                if (_Format != value)
                {
                    OnFormatChanging(value);
                    SendPropertyChanging();
                    _Format = value;
                    SendPropertyChanged("Format");
                    OnFormatChanged();
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
