using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.AttendType")]
    public partial class AttendType : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Code;

        private string _Description;

        private bool? _Hardwired;

        private bool _Worker;

        private bool _Guest;

        private EntitySet<Attend> _Attends;

        private EntitySet<MemberType> _MemberTypes;

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

        partial void OnWorkerChanging(bool value);
        partial void OnWorkerChanged();

        partial void OnGuestChanging(bool value);
        partial void OnGuestChanged();

        #endregion

        public AttendType()
        {
            _Attends = new EntitySet<Attend>(new Action<Attend>(attach_Attends), new Action<Attend>(detach_Attends));

            _MemberTypes = new EntitySet<MemberType>(new Action<MemberType>(attach_MemberTypes), new Action<MemberType>(detach_MemberTypes));

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

        [Column(Name = "Worker", UpdateCheck = UpdateCheck.Never, Storage = "_Worker", DbType = "bit NOT NULL")]
        public bool Worker
        {
            get => _Worker;

            set
            {
                if (_Worker != value)
                {
                    OnWorkerChanging(value);
                    SendPropertyChanging();
                    _Worker = value;
                    SendPropertyChanged("Worker");
                    OnWorkerChanged();
                }
            }
        }

        [Column(Name = "Guest", UpdateCheck = UpdateCheck.Never, Storage = "_Guest", DbType = "bit NOT NULL")]
        public bool Guest
        {
            get => _Guest;

            set
            {
                if (_Guest != value)
                {
                    OnGuestChanging(value);
                    SendPropertyChanging();
                    _Guest = value;
                    SendPropertyChanged("Guest");
                    OnGuestChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_AttendWithAbsents_TBL_AttendType", Storage = "_Attends", OtherKey = "AttendanceTypeId")]
        public EntitySet<Attend> Attends
           {
               get => _Attends;

            set => _Attends.Assign(value);

           }

        [Association(Name = "FK_MemberType_AttendType", Storage = "_MemberTypes", OtherKey = "AttendanceTypeId")]
        public EntitySet<MemberType> MemberTypes
           {
               get => _MemberTypes;

            set => _MemberTypes.Assign(value);

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

        private void attach_Attends(Attend entity)
        {
            SendPropertyChanging();
            entity.AttendType = this;
        }

        private void detach_Attends(Attend entity)
        {
            SendPropertyChanging();
            entity.AttendType = null;
        }

        private void attach_MemberTypes(MemberType entity)
        {
            SendPropertyChanging();
            entity.AttendType = this;
        }

        private void detach_MemberTypes(MemberType entity)
        {
            SendPropertyChanging();
            entity.AttendType = null;
        }
    }
}
