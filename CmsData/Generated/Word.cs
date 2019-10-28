using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.words")]
    public partial class Word : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private string _WordX;

        private int? _N;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnWordXChanging(string value);
        partial void OnWordXChanged();

        partial void OnNChanging(int? value);
        partial void OnNChanged();

        #endregion
        public Word()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "word", UpdateCheck = UpdateCheck.Never, Storage = "_WordX", DbType = "nvarchar(20) NOT NULL", IsPrimaryKey = true)]
        public string WordX
        {
            get => this._WordX;

            set
            {
                if (this._WordX != value)
                {

                    this.OnWordXChanging(value);
                    this.SendPropertyChanging();
                    this._WordX = value;
                    this.SendPropertyChanged("WordX");
                    this.OnWordXChanged();
                }

            }

        }


        [Column(Name = "n", UpdateCheck = UpdateCheck.Never, Storage = "_N", DbType = "int")]
        public int? N
        {
            get => this._N;

            set
            {
                if (this._N != value)
                {

                    this.OnNChanging(value);
                    this.SendPropertyChanging();
                    this._N = value;
                    this.SendPropertyChanged("N");
                    this.OnNChanged();
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

