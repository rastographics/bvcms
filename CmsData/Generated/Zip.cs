using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Zips")]
    public partial class Zip : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private string _ZipCode;

        private int? _MetroMarginalCode;

        private EntityRef<ResidentCode> _ResidentCode;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnZipCodeChanging(string value);
        partial void OnZipCodeChanged();

        partial void OnMetroMarginalCodeChanging(int? value);
        partial void OnMetroMarginalCodeChanged();

        #endregion

        public Zip()
        {
            _ResidentCode = default(EntityRef<ResidentCode>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ZipCode", UpdateCheck = UpdateCheck.Never, Storage = "_ZipCode", DbType = "nvarchar(10) NOT NULL", IsPrimaryKey = true)]
        public string ZipCode
        {
            get => _ZipCode;

            set
            {
                if (_ZipCode != value)
                {
                    OnZipCodeChanging(value);
                    SendPropertyChanging();
                    _ZipCode = value;
                    SendPropertyChanged("ZipCode");
                    OnZipCodeChanged();
                }
            }
        }

        [Column(Name = "MetroMarginalCode", UpdateCheck = UpdateCheck.Never, Storage = "_MetroMarginalCode", DbType = "int")]
        [IsForeignKey]
        public int? MetroMarginalCode
        {
            get => _MetroMarginalCode;

            set
            {
                if (_MetroMarginalCode != value)
                {
                    if (_ResidentCode.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnMetroMarginalCodeChanging(value);
                    SendPropertyChanging();
                    _MetroMarginalCode = value;
                    SendPropertyChanged("MetroMarginalCode");
                    OnMetroMarginalCodeChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Zips_ResidentCode", Storage = "_ResidentCode", ThisKey = "MetroMarginalCode", IsForeignKey = true)]
        public ResidentCode ResidentCode
        {
            get => _ResidentCode.Entity;

            set
            {
                ResidentCode previousValue = _ResidentCode.Entity;
                if (((previousValue != value)
                            || (_ResidentCode.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ResidentCode.Entity = null;
                        previousValue.Zips.Remove(this);
                    }

                    _ResidentCode.Entity = value;
                    if (value != null)
                    {
                        value.Zips.Add(this);

                        _MetroMarginalCode = value.Id;

                    }

                    else
                    {
                        _MetroMarginalCode = default(int?);

                    }

                    SendPropertyChanged("ResidentCode");
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
