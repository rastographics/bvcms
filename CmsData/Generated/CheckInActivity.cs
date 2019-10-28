using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CheckInActivity")]
    public partial class CheckInActivity : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Activity;

        private EntityRef<CheckInTime> _CheckInTime;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnActivityChanging(string value);
        partial void OnActivityChanged();

        #endregion

        public CheckInActivity()
        {
            _CheckInTime = default(EntityRef<CheckInTime>);

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
                    if (_CheckInTime.HasLoadedOrAssignedValue)
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

        [Column(Name = "Activity", UpdateCheck = UpdateCheck.Never, Storage = "_Activity", DbType = "nvarchar(50) NOT NULL", IsPrimaryKey = true)]
        public string Activity
        {
            get => _Activity;

            set
            {
                if (_Activity != value)
                {
                    OnActivityChanging(value);
                    SendPropertyChanging();
                    _Activity = value;
                    SendPropertyChanged("Activity");
                    OnActivityChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_CheckInActivity_CheckInTimes", Storage = "_CheckInTime", ThisKey = "Id", IsForeignKey = true)]
        public CheckInTime CheckInTime
        {
            get => _CheckInTime.Entity;

            set
            {
                CheckInTime previousValue = _CheckInTime.Entity;
                if (((previousValue != value)
                            || (_CheckInTime.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _CheckInTime.Entity = null;
                        previousValue.CheckInActivities.Remove(this);
                    }

                    _CheckInTime.Entity = value;
                    if (value != null)
                    {
                        value.CheckInActivities.Add(this);

                        _Id = value.Id;

                    }

                    else
                    {
                        _Id = default(int);

                    }

                    SendPropertyChanged("CheckInTime");
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
