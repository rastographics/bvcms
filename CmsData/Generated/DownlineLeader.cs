using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.DownlineLeaders")]
    public partial class DownlineLeader : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get => this._CategoryId;

            set
            {
                if (this._CategoryId != value)
                {

                    this.OnCategoryIdChanging(value);
                    this.SendPropertyChanging();
                    this._CategoryId = value;
                    this.SendPropertyChanged("CategoryId");
                    this.OnCategoryIdChanged();
                }

            }

        }


        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
        {
            get => this._PeopleId;

            set
            {
                if (this._PeopleId != value)
                {

                    this.OnPeopleIdChanging(value);
                    this.SendPropertyChanging();
                    this._PeopleId = value;
                    this.SendPropertyChanged("PeopleId");
                    this.OnPeopleIdChanged();
                }

            }

        }


        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(100)")]
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


        [Column(Name = "Cnt", UpdateCheck = UpdateCheck.Never, Storage = "_Cnt", DbType = "int")]
        public int? Cnt
        {
            get => this._Cnt;

            set
            {
                if (this._Cnt != value)
                {

                    this.OnCntChanging(value);
                    this.SendPropertyChanging();
                    this._Cnt = value;
                    this.SendPropertyChanged("Cnt");
                    this.OnCntChanged();
                }

            }

        }


        [Column(Name = "Levels", UpdateCheck = UpdateCheck.Never, Storage = "_Levels", DbType = "int")]
        public int? Levels
        {
            get => this._Levels;

            set
            {
                if (this._Levels != value)
                {

                    this.OnLevelsChanging(value);
                    this.SendPropertyChanging();
                    this._Levels = value;
                    this.SendPropertyChanged("Levels");
                    this.OnLevelsChanged();
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

