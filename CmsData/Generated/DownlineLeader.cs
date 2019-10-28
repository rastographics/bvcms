using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.DownlineLeaders")]
    public partial class DownlineLeader : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int? _CategoryId;

        private int? _PeopleId;

        private string _Name;

        private int? _Cnt;

        private int? _Levels;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnCategoryIdChanging(int? value);
        partial void OnCategoryIdChanged();

        partial void OnPeopleIdChanging(int? value);
        partial void OnPeopleIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnCntChanging(int? value);
        partial void OnCntChanged();

        partial void OnLevelsChanging(int? value);
        partial void OnLevelsChanged();

        #endregion

        public DownlineLeader()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "CategoryId", UpdateCheck = UpdateCheck.Never, Storage = "_CategoryId", DbType = "int")]
        public int? CategoryId
        {
            get => _CategoryId;

            set
            {
                if (_CategoryId != value)
                {
                    OnCategoryIdChanging(value);
                    SendPropertyChanging();
                    _CategoryId = value;
                    SendPropertyChanged("CategoryId");
                    OnCategoryIdChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(100)")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    OnNameChanging(value);
                    SendPropertyChanging();
                    _Name = value;
                    SendPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }

        [Column(Name = "Cnt", UpdateCheck = UpdateCheck.Never, Storage = "_Cnt", DbType = "int")]
        public int? Cnt
        {
            get => _Cnt;

            set
            {
                if (_Cnt != value)
                {
                    OnCntChanging(value);
                    SendPropertyChanging();
                    _Cnt = value;
                    SendPropertyChanged("Cnt");
                    OnCntChanged();
                }
            }
        }

        [Column(Name = "Levels", UpdateCheck = UpdateCheck.Never, Storage = "_Levels", DbType = "int")]
        public int? Levels
        {
            get => _Levels;

            set
            {
                if (_Levels != value)
                {
                    OnLevelsChanging(value);
                    SendPropertyChanging();
                    _Levels = value;
                    SendPropertyChanged("Levels");
                    OnLevelsChanged();
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
