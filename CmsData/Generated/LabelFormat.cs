using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.LabelFormats")]
    public partial class LabelFormat : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _Id;

        private string _Name;

        private int _Size;

        private string _Format;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnSizeChanging(int value);
        partial void OnSizeChanged();

        partial void OnFormatChanging(string value);
        partial void OnFormatChanged();

        #endregion
        public LabelFormat()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get => this._Id;

            set
            {
                if (this._Id != value)
                {

                    this.OnIdChanging(value);
                    this.SendPropertyChanging();
                    this._Id = value;
                    this.SendPropertyChanged("Id");
                    this.OnIdChanged();
                }

            }

        }


        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(30) NOT NULL")]
        public string Name
        {
            get => this._Name;

            set
            {
                if (this._Name != value)
                {

                    this.OnNameChanging(value);
                    this.SendPropertyChanging();
                    this._Name = value;
                    this.SendPropertyChanged("Name");
                    this.OnNameChanged();
                }

            }

        }


        [Column(Name = "Size", UpdateCheck = UpdateCheck.Never, Storage = "_Size", DbType = "int NOT NULL")]
        public int Size
        {
            get => this._Size;

            set
            {
                if (this._Size != value)
                {

                    this.OnSizeChanging(value);
                    this.SendPropertyChanging();
                    this._Size = value;
                    this.SendPropertyChanged("Size");
                    this.OnSizeChanged();
                }

            }

        }


        [Column(Name = "Format", UpdateCheck = UpdateCheck.Never, Storage = "_Format", DbType = "text(2147483647) NOT NULL")]
        public string Format
        {
            get => this._Format;

            set
            {
                if (this._Format != value)
                {

                    this.OnFormatChanging(value);
                    this.SendPropertyChanging();
                    this._Format = value;
                    this.SendPropertyChanged("Format");
                    this.OnFormatChanged();
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

