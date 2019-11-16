using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.IpLog")]
    public partial class IpLog : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _Ip;

        private string _Id;

        private DateTime? _Tm;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIpChanging(string value);
        partial void OnIpChanged();

        partial void OnIdChanging(string value);
        partial void OnIdChanged();

        partial void OnTmChanging(DateTime? value);
        partial void OnTmChanged();

        #endregion

        public IpLog()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "ip", UpdateCheck = UpdateCheck.Never, Storage = "_Ip", DbType = "varchar(50) NOT NULL", IsPrimaryKey = true)]
        public string Ip
        {
            get => _Ip;

            set
            {
                if (_Ip != value)
                {
                    OnIpChanging(value);
                    SendPropertyChanging();
                    _Ip = value;
                    SendPropertyChanged("Ip");
                    OnIpChanged();
                }
            }
        }

        [Column(Name = "id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "varchar(300) NOT NULL", IsPrimaryKey = true)]
        public string Id
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

        [Column(Name = "tm", UpdateCheck = UpdateCheck.Never, Storage = "_Tm", DbType = "datetime")]
        public DateTime? Tm
        {
            get => _Tm;

            set
            {
                if (_Tm != value)
                {
                    OnTmChanging(value);
                    SendPropertyChanging();
                    _Tm = value;
                    SendPropertyChanged("Tm");
                    OnTmChanged();
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
