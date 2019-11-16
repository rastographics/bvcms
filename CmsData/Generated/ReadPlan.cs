using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "disc.ReadPlan")]
    public partial class ReadPlan : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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
            get => _Day;

            set
            {
                if (_Day != value)
                {
                    OnDayChanging(value);
                    SendPropertyChanging();
                    _Day = value;
                    SendPropertyChanged("Day");
                    OnDayChanged();
                }
            }
        }

        [Column(Name = "Section", UpdateCheck = UpdateCheck.Never, Storage = "_Section", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int Section
        {
            get => _Section;

            set
            {
                if (_Section != value)
                {
                    OnSectionChanging(value);
                    SendPropertyChanging();
                    _Section = value;
                    SendPropertyChanged("Section");
                    OnSectionChanged();
                }
            }
        }

        [Column(Name = "StartBook", UpdateCheck = UpdateCheck.Never, Storage = "_StartBook", DbType = "int")]
        public int? StartBook
        {
            get => _StartBook;

            set
            {
                if (_StartBook != value)
                {
                    OnStartBookChanging(value);
                    SendPropertyChanging();
                    _StartBook = value;
                    SendPropertyChanged("StartBook");
                    OnStartBookChanged();
                }
            }
        }

        [Column(Name = "StartChap", UpdateCheck = UpdateCheck.Never, Storage = "_StartChap", DbType = "int")]
        public int? StartChap
        {
            get => _StartChap;

            set
            {
                if (_StartChap != value)
                {
                    OnStartChapChanging(value);
                    SendPropertyChanging();
                    _StartChap = value;
                    SendPropertyChanged("StartChap");
                    OnStartChapChanged();
                }
            }
        }

        [Column(Name = "StartVerse", UpdateCheck = UpdateCheck.Never, Storage = "_StartVerse", DbType = "int")]
        public int? StartVerse
        {
            get => _StartVerse;

            set
            {
                if (_StartVerse != value)
                {
                    OnStartVerseChanging(value);
                    SendPropertyChanging();
                    _StartVerse = value;
                    SendPropertyChanged("StartVerse");
                    OnStartVerseChanged();
                }
            }
        }

        [Column(Name = "EndBook", UpdateCheck = UpdateCheck.Never, Storage = "_EndBook", DbType = "int")]
        public int? EndBook
        {
            get => _EndBook;

            set
            {
                if (_EndBook != value)
                {
                    OnEndBookChanging(value);
                    SendPropertyChanging();
                    _EndBook = value;
                    SendPropertyChanged("EndBook");
                    OnEndBookChanged();
                }
            }
        }

        [Column(Name = "EndChap", UpdateCheck = UpdateCheck.Never, Storage = "_EndChap", DbType = "int")]
        public int? EndChap
        {
            get => _EndChap;

            set
            {
                if (_EndChap != value)
                {
                    OnEndChapChanging(value);
                    SendPropertyChanging();
                    _EndChap = value;
                    SendPropertyChanged("EndChap");
                    OnEndChapChanged();
                }
            }
        }

        [Column(Name = "EndVerse", UpdateCheck = UpdateCheck.Never, Storage = "_EndVerse", DbType = "int")]
        public int? EndVerse
        {
            get => _EndVerse;

            set
            {
                if (_EndVerse != value)
                {
                    OnEndVerseChanging(value);
                    SendPropertyChanging();
                    _EndVerse = value;
                    SendPropertyChanged("EndVerse");
                    OnEndVerseChanged();
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
