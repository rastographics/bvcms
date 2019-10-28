using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ContentKeyWords")]
    public partial class ContentKeyWord : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Word;

        private EntityRef<Content> _Content;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnWordChanging(string value);
        partial void OnWordChanged();

        #endregion

        public ContentKeyWord()
        {
            _Content = default(EntityRef<Content>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    if (_Content.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

        [Column(Name = "Word", UpdateCheck = UpdateCheck.Never, Storage = "_Word", DbType = "nvarchar(50) NOT NULL", IsPrimaryKey = true)]
        public string Word
        {
            get => _Word;

            set
            {
                if (_Word != value)
                {
                    OnWordChanging(value);
                    SendPropertyChanging();
                    _Word = value;
                    SendPropertyChanged("Word");
                    OnWordChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_ContentKeyWords_Content", Storage = "_Content", ThisKey = "Id", IsForeignKey = true)]
        public Content Content
        {
            get => _Content.Entity;

            set
            {
                Content previousValue = _Content.Entity;
                if (((previousValue != value)
                            || (_Content.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Content.Entity = null;
                        previousValue.ContentKeyWords.Remove(this);
                    }

                    _Content.Entity = value;
                    if (value != null)
                    {
                        value.ContentKeyWords.Add(this);

                        _Id = value.Id;

                    }

                    else
                    {
                        _Id = default(int);

                    }

                    SendPropertyChanged("Content");
                }
            }
        }

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
