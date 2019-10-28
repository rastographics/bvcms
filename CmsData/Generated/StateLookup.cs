using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.StateLookup")]
    public partial class StateLookup : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._StateCode;

            set
            {
                if (this._StateCode != value)
                {

                    this.OnStateCodeChanging(value);
                    this.SendPropertyChanging();
                    this._StateCode = value;
                    this.SendPropertyChanged("StateCode");
                    this.OnStateCodeChanged();
                }

            }

        }


        [Column(Name = "StateName", UpdateCheck = UpdateCheck.Never, Storage = "_StateName", DbType = "nvarchar(30)")]
        public string StateName
        {
            get => this._StateName;

            set
            {
                if (this._StateName != value)
                {

                    this.OnStateNameChanging(value);
                    this.SendPropertyChanging();
                    this._StateName = value;
                    this.SendPropertyChanged("StateName");
                    this.OnStateNameChanged();
                }

            }

        }


        [Column(Name = "Hardwired", UpdateCheck = UpdateCheck.Never, Storage = "_Hardwired", DbType = "bit")]
        public bool? Hardwired
        {
            get => this._Hardwired;

            set
            {
                if (this._Hardwired != value)
                {

                    this.OnHardwiredChanging(value);
                    this.SendPropertyChanging();
                    this._Hardwired = value;
                    this.SendPropertyChanged("Hardwired");
                    this.OnHardwiredChanged();
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

