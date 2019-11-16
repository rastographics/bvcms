using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.VolApplicationStatus")]
    public partial class VolApplicationStatus : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Code;

        private string _Description;

        private bool? _Hardwired;

        private EntitySet<Volunteer> _Volunteers;

        private EntitySet<Volunteer> _StatusMvrId;

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

        public VolApplicationStatus()
        {
            _Volunteers = new EntitySet<Volunteer>(new Action<Volunteer>(attach_Volunteers), new Action<Volunteer>(detach_Volunteers));

            _StatusMvrId = new EntitySet<Volunteer>(new Action<Volunteer>(attach_StatusMvrId), new Action<Volunteer>(detach_StatusMvrId));

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

        [Column(Name = "Code", UpdateCheck = UpdateCheck.Never, Storage = "_Code", DbType = "nvarchar(10)")]
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

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(50)")]
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

        [Association(Name = "FK_Volunteer_VolApplicationStatus", Storage = "_Volunteers", OtherKey = "StatusId")]
        public EntitySet<Volunteer> Volunteers
           {
               get => _Volunteers;

            set => _Volunteers.Assign(value);

           }

        [Association(Name = "StatusMvrId__StatusMvr", Storage = "_StatusMvrId", OtherKey = "MVRStatusId")]
        public EntitySet<Volunteer> StatusMvrId
           {
               get => _StatusMvrId;

            set => _StatusMvrId.Assign(value);

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

        private void attach_Volunteers(Volunteer entity)
        {
            SendPropertyChanging();
            entity.VolApplicationStatus = this;
        }

        private void detach_Volunteers(Volunteer entity)
        {
            SendPropertyChanging();
            entity.VolApplicationStatus = null;
        }

        private void attach_StatusMvrId(Volunteer entity)
        {
            SendPropertyChanging();
            entity.StatusMvr = this;
        }

        private void detach_StatusMvrId(Volunteer entity)
        {
            SendPropertyChanging();
            entity.StatusMvr = null;
        }
    }
}
