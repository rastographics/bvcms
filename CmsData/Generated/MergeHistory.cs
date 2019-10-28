using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MergeHistory")]
    public partial class MergeHistory : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _FromId;

        private int _ToId;

        private string _FromName;

        private string _ToName;

        private DateTime _Dt;

        private string _WhoName;

        private int? _WhoId;

        private string _Action;



        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnFromIdChanging(int value);
        partial void OnFromIdChanged();

        partial void OnToIdChanging(int value);
        partial void OnToIdChanged();

        partial void OnFromNameChanging(string value);
        partial void OnFromNameChanged();

        partial void OnToNameChanging(string value);
        partial void OnToNameChanged();

        partial void OnDtChanging(DateTime value);
        partial void OnDtChanged();

        partial void OnWhoNameChanging(string value);
        partial void OnWhoNameChanged();

        partial void OnWhoIdChanging(int? value);
        partial void OnWhoIdChanged();

        partial void OnActionChanging(string value);
        partial void OnActionChanged();

        #endregion
        public MergeHistory()
        {


            OnCreated();
        }


        #region Columns

        [Column(Name = "FromId", UpdateCheck = UpdateCheck.Never, Storage = "_FromId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int FromId
        {
            get => this._FromId;

            set
            {
                if (this._FromId != value)
                {

                    this.OnFromIdChanging(value);
                    this.SendPropertyChanging();
                    this._FromId = value;
                    this.SendPropertyChanged("FromId");
                    this.OnFromIdChanged();
                }

            }

        }


        [Column(Name = "ToId", UpdateCheck = UpdateCheck.Never, Storage = "_ToId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int ToId
        {
            get => this._ToId;

            set
            {
                if (this._ToId != value)
                {

                    this.OnToIdChanging(value);
                    this.SendPropertyChanging();
                    this._ToId = value;
                    this.SendPropertyChanged("ToId");
                    this.OnToIdChanged();
                }

            }

        }


        [Column(Name = "FromName", UpdateCheck = UpdateCheck.Never, Storage = "_FromName", DbType = "nvarchar(150)")]
        public string FromName
        {
            get => this._FromName;

            set
            {
                if (this._FromName != value)
                {

                    this.OnFromNameChanging(value);
                    this.SendPropertyChanging();
                    this._FromName = value;
                    this.SendPropertyChanged("FromName");
                    this.OnFromNameChanged();
                }

            }

        }


        [Column(Name = "ToName", UpdateCheck = UpdateCheck.Never, Storage = "_ToName", DbType = "nvarchar(150)")]
        public string ToName
        {
            get => this._ToName;

            set
            {
                if (this._ToName != value)
                {

                    this.OnToNameChanging(value);
                    this.SendPropertyChanging();
                    this._ToName = value;
                    this.SendPropertyChanged("ToName");
                    this.OnToNameChanged();
                }

            }

        }


        [Column(Name = "Dt", UpdateCheck = UpdateCheck.Never, Storage = "_Dt", DbType = "datetime NOT NULL", IsPrimaryKey = true)]
        public DateTime Dt
        {
            get => this._Dt;

            set
            {
                if (this._Dt != value)
                {

                    this.OnDtChanging(value);
                    this.SendPropertyChanging();
                    this._Dt = value;
                    this.SendPropertyChanged("Dt");
                    this.OnDtChanged();
                }

            }

        }


        [Column(Name = "WhoName", UpdateCheck = UpdateCheck.Never, Storage = "_WhoName", DbType = "nvarchar(150)")]
        public string WhoName
        {
            get => this._WhoName;

            set
            {
                if (this._WhoName != value)
                {

                    this.OnWhoNameChanging(value);
                    this.SendPropertyChanging();
                    this._WhoName = value;
                    this.SendPropertyChanged("WhoName");
                    this.OnWhoNameChanged();
                }

            }

        }


        [Column(Name = "WhoId", UpdateCheck = UpdateCheck.Never, Storage = "_WhoId", DbType = "int")]
        public int? WhoId
        {
            get => this._WhoId;

            set
            {
                if (this._WhoId != value)
                {

                    this.OnWhoIdChanging(value);
                    this.SendPropertyChanging();
                    this._WhoId = value;
                    this.SendPropertyChanged("WhoId");
                    this.OnWhoIdChanged();
                }

            }

        }


        [Column(Name = "Action", UpdateCheck = UpdateCheck.Never, Storage = "_Action", DbType = "varchar(50)")]
        public string Action
        {
            get => this._Action;

            set
            {
                if (this._Action != value)
                {

                    this.OnActionChanging(value);
                    this.SendPropertyChanging();
                    this._Action = value;
                    this.SendPropertyChanged("Action");
                    this.OnActionChanged();
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

