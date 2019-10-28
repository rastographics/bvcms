using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CheckedBatches")]
    public partial class CheckedBatch : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private string _BatchRef;

        private DateTime? _CheckedX;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnBatchRefChanging(string value);
        partial void OnBatchRefChanged();

        partial void OnCheckedXChanging(DateTime? value);
        partial void OnCheckedXChanged();

        #endregion
        public CheckedBatch()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "BatchRef", UpdateCheck = UpdateCheck.Never, Storage = "_BatchRef", DbType = "nvarchar(50) NOT NULL", IsPrimaryKey = true)]
        public string BatchRef
        {
            get => this._BatchRef;

            set
            {
                if (this._BatchRef != value)
                {

                    this.OnBatchRefChanging(value);
                    this.SendPropertyChanging();
                    this._BatchRef = value;
                    this.SendPropertyChanged("BatchRef");
                    this.OnBatchRefChanged();
                }

            }

        }


        [Column(Name = "Checked", UpdateCheck = UpdateCheck.Never, Storage = "_CheckedX", DbType = "datetime")]
        public DateTime? CheckedX
        {
            get => this._CheckedX;

            set
            {
                if (this._CheckedX != value)
                {

                    this.OnCheckedXChanging(value);
                    this.SendPropertyChanging();
                    this._CheckedX = value;
                    this.SendPropertyChanged("CheckedX");
                    this.OnCheckedXChanged();
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

