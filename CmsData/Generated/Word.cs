using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.words")]
    public partial class Word : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _WordX;

        private int? _N;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnWordXChanging(string value);
        partial void OnWordXChanged();

        partial void OnNChanging(int? value);
        partial void OnNChanged();

        #endregion

        public Word()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "word", UpdateCheck = UpdateCheck.Never, Storage = "_WordX", DbType = "nvarchar(20) NOT NULL", IsPrimaryKey = true)]
        public string WordX
        {
            get => _WordX;

            set
            {
                if (_WordX != value)
                {
                    OnWordXChanging(value);
                    SendPropertyChanging();
                    _WordX = value;
                    SendPropertyChanged("WordX");
                    OnWordXChanged();
                }
            }
        }

        [Column(Name = "n", UpdateCheck = UpdateCheck.Never, Storage = "_N", DbType = "int")]
        public int? N
        {
            get => _N;

            set
            {
                if (_N != value)
                {
                    OnNChanging(value);
                    SendPropertyChanging();
                    _N = value;
                    SendPropertyChanged("N");
                    OnNChanged();
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
