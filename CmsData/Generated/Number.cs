using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Numbers")]
    public partial class Number : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._NumberX;

            set
            {
                if (this._NumberX != value)
                {

                    this.OnNumberXChanging(value);
                    this.SendPropertyChanging();
                    this._NumberX = value;
                    this.SendPropertyChanged("NumberX");
                    this.OnNumberXChanged();
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

