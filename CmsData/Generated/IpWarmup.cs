using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.IpWarmup")]
    public partial class IpWarmup : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private DateTime? _Epoch;

        private int? _Sentsince;

        private DateTime? _Since;

        private int? _Totalsent;

        private int? _Totaltries;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnEpochChanging(DateTime? value);
        partial void OnEpochChanged();

        partial void OnSentsinceChanging(int? value);
        partial void OnSentsinceChanged();

        partial void OnSinceChanging(DateTime? value);
        partial void OnSinceChanged();

        partial void OnTotalsentChanging(int? value);
        partial void OnTotalsentChanged();

        partial void OnTotaltriesChanging(int? value);
        partial void OnTotaltriesChanged();

        #endregion

        public IpWarmup()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "epoch", UpdateCheck = UpdateCheck.Never, Storage = "_Epoch", DbType = "datetime")]
        public DateTime? Epoch
        {
            get => _Epoch;

            set
            {
                if (_Epoch != value)
                {
                    OnEpochChanging(value);
                    SendPropertyChanging();
                    _Epoch = value;
                    SendPropertyChanged("Epoch");
                    OnEpochChanged();
                }
            }
        }

        [Column(Name = "sentsince", UpdateCheck = UpdateCheck.Never, Storage = "_Sentsince", DbType = "int")]
        public int? Sentsince
        {
            get => _Sentsince;

            set
            {
                if (_Sentsince != value)
                {
                    OnSentsinceChanging(value);
                    SendPropertyChanging();
                    _Sentsince = value;
                    SendPropertyChanged("Sentsince");
                    OnSentsinceChanged();
                }
            }
        }

        [Column(Name = "since", UpdateCheck = UpdateCheck.Never, Storage = "_Since", DbType = "datetime")]
        public DateTime? Since
        {
            get => _Since;

            set
            {
                if (_Since != value)
                {
                    OnSinceChanging(value);
                    SendPropertyChanging();
                    _Since = value;
                    SendPropertyChanged("Since");
                    OnSinceChanged();
                }
            }
        }

        [Column(Name = "totalsent", UpdateCheck = UpdateCheck.Never, Storage = "_Totalsent", DbType = "int")]
        public int? Totalsent
        {
            get => _Totalsent;

            set
            {
                if (_Totalsent != value)
                {
                    OnTotalsentChanging(value);
                    SendPropertyChanging();
                    _Totalsent = value;
                    SendPropertyChanged("Totalsent");
                    OnTotalsentChanged();
                }
            }
        }

        [Column(Name = "totaltries", UpdateCheck = UpdateCheck.Never, Storage = "_Totaltries", DbType = "int")]
        public int? Totaltries
        {
            get => _Totaltries;

            set
            {
                if (_Totaltries != value)
                {
                    OnTotaltriesChanging(value);
                    SendPropertyChanging();
                    _Totaltries = value;
                    SendPropertyChanged("Totaltries");
                    OnTotaltriesChanged();
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
