using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.StreetTypes")]
    public partial class StreetType : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private string _Type;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnTypeChanging(string value);
        partial void OnTypeChanged();

        #endregion
        public StreetType()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "Type", UpdateCheck = UpdateCheck.Never, Storage = "_Type", DbType = "nvarchar(50) NOT NULL", IsPrimaryKey = true)]
        public string Type
        {
            get => this._Type;

            set
            {
                if (this._Type != value)
                {

                    this.OnTypeChanging(value);
                    this.SendPropertyChanging();
                    this._Type = value;
                    this.SendPropertyChanged("Type");
                    this.OnTypeChanged();
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

