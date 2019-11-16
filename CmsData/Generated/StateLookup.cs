using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.StateLookup")]
    public partial class StateLookup : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _StateCode;

        private string _StateName;

        private bool? _Hardwired;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnStateCodeChanging(string value);
        partial void OnStateCodeChanged();

        partial void OnStateNameChanging(string value);
        partial void OnStateNameChanged();

        partial void OnHardwiredChanging(bool? value);
        partial void OnHardwiredChanged();

        #endregion

        public StateLookup()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "StateCode", UpdateCheck = UpdateCheck.Never, Storage = "_StateCode", DbType = "nvarchar(10) NOT NULL", IsPrimaryKey = true)]
        public string StateCode
        {
            get => _StateCode;

            set
            {
                if (_StateCode != value)
                {
                    OnStateCodeChanging(value);
                    SendPropertyChanging();
                    _StateCode = value;
                    SendPropertyChanged("StateCode");
                    OnStateCodeChanged();
                }
            }
        }

        [Column(Name = "StateName", UpdateCheck = UpdateCheck.Never, Storage = "_StateName", DbType = "nvarchar(30)")]
        public string StateName
        {
            get => _StateName;

            set
            {
                if (_StateName != value)
                {
                    OnStateNameChanging(value);
                    SendPropertyChanging();
                    _StateName = value;
                    SendPropertyChanged("StateName");
                    OnStateNameChanged();
                }
            }
        }

        [Column(Name = "Hardwired", UpdateCheck = UpdateCheck.Never, Storage = "_Hardwired", DbType = "bit")]
        public bool? Hardwired
        {
            get => _Hardwired;

            set
            {
                if (_Hardwired != value)
                {
                    OnHardwiredChanging(value);
                    SendPropertyChanging();
                    _Hardwired = value;
                    SendPropertyChanged("Hardwired");
                    OnHardwiredChanged();
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
