using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CustomColumns")]
    public partial class CustomColumn : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Ord;

        private string _Column;

        private string _Select;

        private string _JoinTable;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnOrdChanging(int value);
        partial void OnOrdChanged();

        partial void OnColumnChanging(string value);
        partial void OnColumnChanged();

        partial void OnSelectChanging(string value);
        partial void OnSelectChanged();

        partial void OnJoinTableChanging(string value);
        partial void OnJoinTableChanged();

        #endregion

        public CustomColumn()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "Ord", UpdateCheck = UpdateCheck.Never, Storage = "_Ord", DbType = "int NOT NULL")]
        public int Ord
        {
            get => _Ord;

            set
            {
                if (_Ord != value)
                {
                    OnOrdChanging(value);
                    SendPropertyChanging();
                    _Ord = value;
                    SendPropertyChanged("Ord");
                    OnOrdChanged();
                }
            }
        }

        [Column(Name = "Column", UpdateCheck = UpdateCheck.Never, Storage = "_Column", DbType = "varchar(50) NOT NULL", IsPrimaryKey = true)]
        public string Column
        {
            get => _Column;

            set
            {
                if (_Column != value)
                {
                    OnColumnChanging(value);
                    SendPropertyChanging();
                    _Column = value;
                    SendPropertyChanged("Column");
                    OnColumnChanged();
                }
            }
        }

        [Column(Name = "Select", UpdateCheck = UpdateCheck.Never, Storage = "_Select", DbType = "varchar(300)")]
        public string Select
        {
            get => _Select;

            set
            {
                if (_Select != value)
                {
                    OnSelectChanging(value);
                    SendPropertyChanging();
                    _Select = value;
                    SendPropertyChanged("Select");
                    OnSelectChanged();
                }
            }
        }

        [Column(Name = "JoinTable", UpdateCheck = UpdateCheck.Never, Storage = "_JoinTable", DbType = "varchar(200)")]
        public string JoinTable
        {
            get => _JoinTable;

            set
            {
                if (_JoinTable != value)
                {
                    OnJoinTableChanging(value);
                    SendPropertyChanging();
                    _JoinTable = value;
                    SendPropertyChanged("JoinTable");
                    OnJoinTableChanged();
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
