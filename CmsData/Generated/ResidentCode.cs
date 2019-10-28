using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.ResidentCode")]
    public partial class ResidentCode : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Code;

        private string _Description;

        private bool? _Hardwired;

        private EntitySet<Zip> _Zips;

        private EntitySet<Family> _ResCodeFamilies;

        private EntitySet<Person> _ResCodePeople;

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

        public ResidentCode()
        {
            _Zips = new EntitySet<Zip>(new Action<Zip>(attach_Zips), new Action<Zip>(detach_Zips));

            _ResCodeFamilies = new EntitySet<Family>(new Action<Family>(attach_ResCodeFamilies), new Action<Family>(detach_ResCodeFamilies));

            _ResCodePeople = new EntitySet<Person>(new Action<Person>(attach_ResCodePeople), new Action<Person>(detach_ResCodePeople));

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

        [Association(Name = "FK_Zips_ResidentCode", Storage = "_Zips", OtherKey = "MetroMarginalCode")]
        public EntitySet<Zip> Zips
           {
               get => _Zips;

            set => _Zips.Assign(value);

           }

        [Association(Name = "ResCodeFamilies__ResidentCode", Storage = "_ResCodeFamilies", OtherKey = "ResCodeId")]
        public EntitySet<Family> ResCodeFamilies
           {
               get => _ResCodeFamilies;

            set => _ResCodeFamilies.Assign(value);

           }

        [Association(Name = "ResCodePeople__ResidentCode", Storage = "_ResCodePeople", OtherKey = "ResCodeId")]
        public EntitySet<Person> ResCodePeople
           {
               get => _ResCodePeople;

            set => _ResCodePeople.Assign(value);

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

        private void attach_Zips(Zip entity)
        {
            SendPropertyChanging();
            entity.ResidentCode = this;
        }

        private void detach_Zips(Zip entity)
        {
            SendPropertyChanging();
            entity.ResidentCode = null;
        }

        private void attach_ResCodeFamilies(Family entity)
        {
            SendPropertyChanging();
            entity.ResidentCode = this;
        }

        private void detach_ResCodeFamilies(Family entity)
        {
            SendPropertyChanging();
            entity.ResidentCode = null;
        }

        private void attach_ResCodePeople(Person entity)
        {
            SendPropertyChanging();
            entity.ResidentCode = this;
        }

        private void detach_ResCodePeople(Person entity)
        {
            SendPropertyChanging();
            entity.ResidentCode = null;
        }
    }
}
