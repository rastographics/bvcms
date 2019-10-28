using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.GoerSupporter")]
    public partial class GoerSupporter : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _GoerId;

        private int? _SupporterId;

        private string _NoDbEmail;

        private bool? _Active;

        private bool? _Unsubscribe;

        private DateTime _Created;

        private string _Salutation;

        private EntityRef<Person> _Supporter;

        private EntityRef<Person> _Goer;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnGoerIdChanging(int value);
        partial void OnGoerIdChanged();

        partial void OnSupporterIdChanging(int? value);
        partial void OnSupporterIdChanged();

        partial void OnNoDbEmailChanging(string value);
        partial void OnNoDbEmailChanged();

        partial void OnActiveChanging(bool? value);
        partial void OnActiveChanged();

        partial void OnUnsubscribeChanging(bool? value);
        partial void OnUnsubscribeChanged();

        partial void OnCreatedChanging(DateTime value);
        partial void OnCreatedChanged();

        partial void OnSalutationChanging(string value);
        partial void OnSalutationChanged();

        #endregion

        public GoerSupporter()
        {
            _Supporter = default(EntityRef<Person>);

            _Goer = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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

        [Column(Name = "GoerId", UpdateCheck = UpdateCheck.Never, Storage = "_GoerId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int GoerId
        {
            get => _GoerId;

            set
            {
                if (_GoerId != value)
                {
                    if (_Goer.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnGoerIdChanging(value);
                    SendPropertyChanging();
                    _GoerId = value;
                    SendPropertyChanged("GoerId");
                    OnGoerIdChanged();
                }
            }
        }

        [Column(Name = "SupporterId", UpdateCheck = UpdateCheck.Never, Storage = "_SupporterId", DbType = "int")]
        [IsForeignKey]
        public int? SupporterId
        {
            get => _SupporterId;

            set
            {
                if (_SupporterId != value)
                {
                    if (_Supporter.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnSupporterIdChanging(value);
                    SendPropertyChanging();
                    _SupporterId = value;
                    SendPropertyChanged("SupporterId");
                    OnSupporterIdChanged();
                }
            }
        }

        [Column(Name = "NoDbEmail", UpdateCheck = UpdateCheck.Never, Storage = "_NoDbEmail", DbType = "varchar(80)")]
        public string NoDbEmail
        {
            get => _NoDbEmail;

            set
            {
                if (_NoDbEmail != value)
                {
                    OnNoDbEmailChanging(value);
                    SendPropertyChanging();
                    _NoDbEmail = value;
                    SendPropertyChanged("NoDbEmail");
                    OnNoDbEmailChanged();
                }
            }
        }

        [Column(Name = "Active", UpdateCheck = UpdateCheck.Never, Storage = "_Active", DbType = "bit")]
        public bool? Active
        {
            get => _Active;

            set
            {
                if (_Active != value)
                {
                    OnActiveChanging(value);
                    SendPropertyChanging();
                    _Active = value;
                    SendPropertyChanged("Active");
                    OnActiveChanged();
                }
            }
        }

        [Column(Name = "Unsubscribe", UpdateCheck = UpdateCheck.Never, Storage = "_Unsubscribe", DbType = "bit")]
        public bool? Unsubscribe
        {
            get => _Unsubscribe;

            set
            {
                if (_Unsubscribe != value)
                {
                    OnUnsubscribeChanging(value);
                    SendPropertyChanging();
                    _Unsubscribe = value;
                    SendPropertyChanged("Unsubscribe");
                    OnUnsubscribeChanged();
                }
            }
        }

        [Column(Name = "Created", UpdateCheck = UpdateCheck.Never, Storage = "_Created", DbType = "datetime NOT NULL")]
        public DateTime Created
        {
            get => _Created;

            set
            {
                if (_Created != value)
                {
                    OnCreatedChanging(value);
                    SendPropertyChanging();
                    _Created = value;
                    SendPropertyChanged("Created");
                    OnCreatedChanged();
                }
            }
        }

        [Column(Name = "Salutation", UpdateCheck = UpdateCheck.Never, Storage = "_Salutation", DbType = "nvarchar(80)")]
        public string Salutation
        {
            get => _Salutation;

            set
            {
                if (_Salutation != value)
                {
                    OnSalutationChanging(value);
                    SendPropertyChanging();
                    _Salutation = value;
                    SendPropertyChanged("Salutation");
                    OnSalutationChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Goers__Supporter", Storage = "_Supporter", ThisKey = "SupporterId", IsForeignKey = true)]
        public Person Supporter
        {
            get => _Supporter.Entity;

            set
            {
                Person previousValue = _Supporter.Entity;
                if (((previousValue != value)
                            || (_Supporter.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Supporter.Entity = null;
                        previousValue.FK_Goers.Remove(this);
                    }

                    _Supporter.Entity = value;
                    if (value != null)
                    {
                        value.FK_Goers.Add(this);

                        _SupporterId = value.PeopleId;

                    }

                    else
                    {
                        _SupporterId = default(int?);

                    }

                    SendPropertyChanged("Supporter");
                }
            }
        }

        [Association(Name = "FK_Supporters__Goer", Storage = "_Goer", ThisKey = "GoerId", IsForeignKey = true)]
        public Person Goer
        {
            get => _Goer.Entity;

            set
            {
                Person previousValue = _Goer.Entity;
                if (((previousValue != value)
                            || (_Goer.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Goer.Entity = null;
                        previousValue.FK_Supporters.Remove(this);
                    }

                    _Goer.Entity = value;
                    if (value != null)
                    {
                        value.FK_Supporters.Add(this);

                        _GoerId = value.PeopleId;

                    }

                    else
                    {
                        _GoerId = default(int);

                    }

                    SendPropertyChanged("Goer");
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
