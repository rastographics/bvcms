using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Numbers")]
    public partial class Number : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private long? _NumberX;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnNumberXChanging(long? value);
        partial void OnNumberXChanged();

        #endregion

        public Number()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Number", UpdateCheck = UpdateCheck.Never, Storage = "_NumberX", DbType = "bigint")]
        public long? NumberX
        {
            get => _NumberX;

            set
            {
                if (_NumberX != value)
                {
                    OnNumberXChanging(value);
                    SendPropertyChanging();
                    _NumberX = value;
                    SendPropertyChanged("NumberX");
                    OnNumberXChanged();
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
