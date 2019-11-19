using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.IpLog2")]
    public partial class IpLog2 : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _Ip;

        private DateTime? _Tm;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIpChanging(string value);
        partial void OnIpChanged();

        partial void OnTmChanging(DateTime? value);
        partial void OnTmChanged();

        #endregion

        public IpLog2()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "ip", UpdateCheck = UpdateCheck.Never, Storage = "_Ip", DbType = "varchar(50)")]
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
