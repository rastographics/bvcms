using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.EnvelopeOption")]
    public partial class EnvelopeOption : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Code;

        private string _Description;

        private bool? _Hardwired;

        private EntitySet<Person> _EnvPeople;

        private EntitySet<Person> _StmtPeople;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnCodeChanging(string value);
        partial void OnCodeChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnHardwiredChanging(bool? value);
        partial void OnHardwiredChanged();

        #endregion

        public EnvelopeOption()
        {
            _EnvPeople = new EntitySet<Person>(new Action<Person>(attach_EnvPeople), new Action<Person>(detach_EnvPeople));

            _StmtPeople = new EntitySet<Person>(new Action<Person>(attach_StmtPeople), new Action<Person>(detach_StmtPeople));

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

        [Column(Name = "Code", UpdateCheck = UpdateCheck.Never, Storage = "_Code", DbType = "nvarchar(20)")]
        public string Code
        {
            get => _Code;

            set
            {
                if (_Code != value)
                {
                    OnCodeChanging(value);
                    SendPropertyChanging();
                    _Code = value;
                    SendPropertyChanged("Code");
                    OnCodeChanged();
                }
            }
        }

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(100)")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    OnDescriptionChanging(value);
                    SendPropertyChanging();
                    _Description = value;
                    SendPropertyChanged("Description");
                    OnDescriptionChanged();
                }
            }
        }

        [Column(Name = "Hardwired", UpdateCheck = UpdateCheck.Never, Storage = "_Hardwired", DbType = "bit")]
        public bool? Hardwired
        {
            get => _Hardwired;

            set
            {
                if (_Hardwired != value)
                {
                    OnHardwiredChanging(value);
                    SendPropertyChanging();
                    _Hardwired = value;
                    SendPropertyChanged("Hardwired");
                    OnHardwiredChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "EnvPeople__EnvelopeOption", Storage = "_EnvPeople", OtherKey = "EnvelopeOptionsId")]
        public EntitySet<Person> EnvPeople
           {
               get => _EnvPeople;

            set => _EnvPeople.Assign(value);

           }

        [Association(Name = "StmtPeople__ContributionStatementOption", Storage = "_StmtPeople", OtherKey = "ContributionOptionsId")]
        public EntitySet<Person> StmtPeople
           {
               get => _StmtPeople;

            set => _StmtPeople.Assign(value);

           }

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

        private void attach_EnvPeople(Person entity)
        {
            SendPropertyChanging();
            entity.EnvelopeOption = this;
        }

        private void detach_EnvPeople(Person entity)
        {
            SendPropertyChanging();
            entity.EnvelopeOption = null;
        }

        private void attach_StmtPeople(Person entity)
        {
            SendPropertyChanging();
            entity.ContributionStatementOption = this;
        }

        private void detach_StmtPeople(Person entity)
        {
            SendPropertyChanging();
            entity.ContributionStatementOption = null;
        }
    }
}
