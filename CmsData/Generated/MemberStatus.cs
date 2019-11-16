using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "lookup.MemberStatus")]
    public partial class MemberStatus : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Code;

        private string _Description;

        private bool? _Hardwired;

        private bool _Member;

        private bool _Previous;

        private bool _Pending;

        private EntitySet<Person> _People;

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

        partial void OnMemberChanging(bool value);
        partial void OnMemberChanged();

        partial void OnPreviousChanging(bool value);
        partial void OnPreviousChanged();

        partial void OnPendingChanging(bool value);
        partial void OnPendingChanged();

        #endregion

        public MemberStatus()
        {
            _People = new EntitySet<Person>(new Action<Person>(attach_People), new Action<Person>(detach_People));

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

        [Column(Name = "Member", UpdateCheck = UpdateCheck.Never, Storage = "_Member", DbType = "bit NOT NULL")]
        public bool Member
        {
            get => _Member;

            set
            {
                if (_Member != value)
                {
                    OnMemberChanging(value);
                    SendPropertyChanging();
                    _Member = value;
                    SendPropertyChanged("Member");
                    OnMemberChanged();
                }
            }
        }

        [Column(Name = "Previous", UpdateCheck = UpdateCheck.Never, Storage = "_Previous", DbType = "bit NOT NULL")]
        public bool Previous
        {
            get => _Previous;

            set
            {
                if (_Previous != value)
                {
                    OnPreviousChanging(value);
                    SendPropertyChanging();
                    _Previous = value;
                    SendPropertyChanged("Previous");
                    OnPreviousChanged();
                }
            }
        }

        [Column(Name = "Pending", UpdateCheck = UpdateCheck.Never, Storage = "_Pending", DbType = "bit NOT NULL")]
        public bool Pending
        {
            get => _Pending;

            set
            {
                if (_Pending != value)
                {
                    OnPendingChanging(value);
                    SendPropertyChanging();
                    _Pending = value;
                    SendPropertyChanged("Pending");
                    OnPendingChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_People_MemberStatus", Storage = "_People", OtherKey = "MemberStatusId")]
        public EntitySet<Person> People
           {
               get => _People;

            set => _People.Assign(value);

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

        private void attach_People(Person entity)
        {
            SendPropertyChanging();
            entity.MemberStatus = this;
        }

        private void detach_People(Person entity)
        {
            SendPropertyChanging();
            entity.MemberStatus = null;
        }
    }
}
