using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "disc.ReadPlan")]
    public partial class ReadPlan : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _Day;

        private int _Section;

        private int? _StartBook;

        private int? _StartChap;

        private int? _StartVerse;

        private int? _EndBook;

        private int? _EndChap;

        private int? _EndVerse;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnDayChanging(int value);
        partial void OnDayChanged();

        partial void OnSectionChanging(int value);
        partial void OnSectionChanged();

        partial void OnStartBookChanging(int? value);
        partial void OnStartBookChanged();

        partial void OnStartChapChanging(int? value);
        partial void OnStartChapChanged();

        partial void OnStartVerseChanging(int? value);
        partial void OnStartVerseChanged();

        partial void OnEndBookChanging(int? value);
        partial void OnEndBookChanged();

        partial void OnEndChapChanging(int? value);
        partial void OnEndChapChanged();

        partial void OnEndVerseChanging(int? value);
        partial void OnEndVerseChanged();

        #endregion
        public ReadPlan()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "Day", UpdateCheck = UpdateCheck.Never, Storage = "_Day", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int Day
        {
            get => this._Day;

            set
            {
                if (this._Day != value)
                {

                    this.OnDayChanging(value);
                    this.SendPropertyChanging();
                    this._Day = value;
                    this.SendPropertyChanged("Day");
                    this.OnDayChanged();
                }

            }

        }


        [Column(Name = "Section", UpdateCheck = UpdateCheck.Never, Storage = "_Section", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int Section
        {
            get => this._Section;

            set
            {
                if (this._Section != value)
                {

                    this.OnSectionChanging(value);
                    this.SendPropertyChanging();
                    this._Section = value;
                    this.SendPropertyChanged("Section");
                    this.OnSectionChanged();
                }

            }

        }


        [Column(Name = "StartBook", UpdateCheck = UpdateCheck.Never, Storage = "_StartBook", DbType = "int")]
        public int? StartBook
        {
            get => this._StartBook;

            set
            {
                if (this._StartBook != value)
                {

                    this.OnStartBookChanging(value);
                    this.SendPropertyChanging();
                    this._StartBook = value;
                    this.SendPropertyChanged("StartBook");
                    this.OnStartBookChanged();
                }

            }

        }


        [Column(Name = "StartChap", UpdateCheck = UpdateCheck.Never, Storage = "_StartChap", DbType = "int")]
        public int? StartChap
        {
            get => this._StartChap;

            set
            {
                if (this._StartChap != value)
                {

                    this.OnStartChapChanging(value);
                    this.SendPropertyChanging();
                    this._StartChap = value;
                    this.SendPropertyChanged("StartChap");
                    this.OnStartChapChanged();
                }

            }

        }


        [Column(Name = "StartVerse", UpdateCheck = UpdateCheck.Never, Storage = "_StartVerse", DbType = "int")]
        public int? StartVerse
        {
            get => this._StartVerse;

            set
            {
                if (this._StartVerse != value)
                {

                    this.OnStartVerseChanging(value);
                    this.SendPropertyChanging();
                    this._StartVerse = value;
                    this.SendPropertyChanged("StartVerse");
                    this.OnStartVerseChanged();
                }

            }

        }


        [Column(Name = "EndBook", UpdateCheck = UpdateCheck.Never, Storage = "_EndBook", DbType = "int")]
        public int? EndBook
        {
            get => this._EndBook;

            set
            {
                if (this._EndBook != value)
                {

                    this.OnEndBookChanging(value);
                    this.SendPropertyChanging();
                    this._EndBook = value;
                    this.SendPropertyChanged("EndBook");
                    this.OnEndBookChanged();
                }

            }

        }


        [Column(Name = "EndChap", UpdateCheck = UpdateCheck.Never, Storage = "_EndChap", DbType = "int")]
        public int? EndChap
        {
            get => this._EndChap;

            set
            {
                if (this._EndChap != value)
                {

                    this.OnEndChapChanging(value);
                    this.SendPropertyChanging();
                    this._EndChap = value;
                    this.SendPropertyChanged("EndChap");
                    this.OnEndChapChanged();
                }

            }

        }


        [Column(Name = "EndVerse", UpdateCheck = UpdateCheck.Never, Storage = "_EndVerse", DbType = "int")]
        public int? EndVerse
        {
            get => this._EndVerse;

            set
            {
                if (this._EndVerse != value)
                {

                    this.OnEndVerseChanging(value);
                    this.SendPropertyChanging();
                    this._EndVerse = value;
                    this.SendPropertyChanged("EndVerse");
                    this.OnEndVerseChanged();
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

