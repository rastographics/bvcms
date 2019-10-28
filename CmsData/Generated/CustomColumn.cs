using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CustomColumns")]
    public partial class CustomColumn : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._Ord;

            set
            {
                if (this._Ord != value)
                {

                    this.OnOrdChanging(value);
                    this.SendPropertyChanging();
                    this._Ord = value;
                    this.SendPropertyChanged("Ord");
                    this.OnOrdChanged();
                }

            }

        }


        [Column(Name = "Column", UpdateCheck = UpdateCheck.Never, Storage = "_Column", DbType = "varchar(50) NOT NULL", IsPrimaryKey = true)]
        public string Column
        {
            get => this._Column;

            set
            {
                if (this._Column != value)
                {

                    this.OnColumnChanging(value);
                    this.SendPropertyChanging();
                    this._Column = value;
                    this.SendPropertyChanged("Column");
                    this.OnColumnChanged();
                }

            }

        }


        [Column(Name = "Select", UpdateCheck = UpdateCheck.Never, Storage = "_Select", DbType = "varchar(300)")]
        public string Select
        {
            get => this._Select;

            set
            {
                if (this._Select != value)
                {

                    this.OnSelectChanging(value);
                    this.SendPropertyChanging();
                    this._Select = value;
                    this.SendPropertyChanged("Select");
                    this.OnSelectChanged();
                }

            }

        }


        [Column(Name = "JoinTable", UpdateCheck = UpdateCheck.Never, Storage = "_JoinTable", DbType = "varchar(200)")]
        public string JoinTable
        {
            get => this._JoinTable;

            set
            {
                if (this._JoinTable != value)
                {

                    this.OnJoinTableChanging(value);
                    this.SendPropertyChanging();
                    this._JoinTable = value;
                    this.SendPropertyChanged("JoinTable");
                    this.OnJoinTableChanged();
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
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }

}

