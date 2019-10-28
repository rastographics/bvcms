using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.SMSList")]
    public partial class SMSList : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private DateTime _Created;

        private int _SenderID;

        private DateTime _SendAt;

        private int _SendGroupID;

        private string _Message;

        private int _SentSMS;

        private int _SentNone;

        private string _Title;

        private EntitySet<SMSItem> _SMSItems;

        private EntityRef<Person> _Person;

        private EntityRef<SMSGroup> _SMSGroup;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnCreatedChanging(DateTime value);
        partial void OnCreatedChanged();

        partial void OnSenderIDChanging(int value);
        partial void OnSenderIDChanged();

        partial void OnSendAtChanging(DateTime value);
        partial void OnSendAtChanged();

        partial void OnSendGroupIDChanging(int value);
        partial void OnSendGroupIDChanged();

        partial void OnMessageChanging(string value);
        partial void OnMessageChanged();

        partial void OnSentSMSChanging(int value);
        partial void OnSentSMSChanged();

        partial void OnSentNoneChanging(int value);
        partial void OnSentNoneChanged();

        partial void OnTitleChanging(string value);
        partial void OnTitleChanged();

        #endregion

        public SMSList()
        {
            _SMSItems = new EntitySet<SMSItem>(new Action<SMSItem>(attach_SMSItems), new Action<SMSItem>(detach_SMSItems));

            _Person = default(EntityRef<Person>);

            _SMSGroup = default(EntityRef<SMSGroup>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ID", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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

        [Column(Name = "SenderID", UpdateCheck = UpdateCheck.Never, Storage = "_SenderID", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int SenderID
        {
            get => _SenderID;

            set
            {
                if (_SenderID != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnSenderIDChanging(value);
                    SendPropertyChanging();
                    _SenderID = value;
                    SendPropertyChanged("SenderID");
                    OnSenderIDChanged();
                }
            }
        }

        [Column(Name = "SendAt", UpdateCheck = UpdateCheck.Never, Storage = "_SendAt", DbType = "datetime NOT NULL")]
        public DateTime SendAt
        {
            get => _SendAt;

            set
            {
                if (_SendAt != value)
                {
                    OnSendAtChanging(value);
                    SendPropertyChanging();
                    _SendAt = value;
                    SendPropertyChanged("SendAt");
                    OnSendAtChanged();
                }
            }
        }

        [Column(Name = "SendGroupID", UpdateCheck = UpdateCheck.Never, Storage = "_SendGroupID", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int SendGroupID
        {
            get => _SendGroupID;

            set
            {
                if (_SendGroupID != value)
                {
                    if (_SMSGroup.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnSendGroupIDChanging(value);
                    SendPropertyChanging();
                    _SendGroupID = value;
                    SendPropertyChanged("SendGroupID");
                    OnSendGroupIDChanged();
                }
            }
        }

        [Column(Name = "Message", UpdateCheck = UpdateCheck.Never, Storage = "_Message", DbType = "nvarchar(1600) NOT NULL")]
        public string Message
        {
            get => _Message;

            set
            {
                if (_Message != value)
                {
                    OnMessageChanging(value);
                    SendPropertyChanging();
                    _Message = value;
                    SendPropertyChanged("Message");
                    OnMessageChanged();
                }
            }
        }

        [Column(Name = "SentSMS", UpdateCheck = UpdateCheck.Never, Storage = "_SentSMS", DbType = "int NOT NULL")]
        public int SentSMS
        {
            get => _SentSMS;

            set
            {
                if (_SentSMS != value)
                {
                    OnSentSMSChanging(value);
                    SendPropertyChanging();
                    _SentSMS = value;
                    SendPropertyChanged("SentSMS");
                    OnSentSMSChanged();
                }
            }
        }

        [Column(Name = "SentNone", UpdateCheck = UpdateCheck.Never, Storage = "_SentNone", DbType = "int NOT NULL")]
        public int SentNone
        {
            get => _SentNone;

            set
            {
                if (_SentNone != value)
                {
                    OnSentNoneChanging(value);
                    SendPropertyChanging();
                    _SentNone = value;
                    SendPropertyChanged("SentNone");
                    OnSentNoneChanged();
                }
            }
        }

        [Column(Name = "Title", UpdateCheck = UpdateCheck.Never, Storage = "_Title", DbType = "nvarchar(150) NOT NULL")]
        public string Title
        {
            get => _Title;

            set
            {
                if (_Title != value)
                {
                    OnTitleChanging(value);
                    SendPropertyChanging();
                    _Title = value;
                    SendPropertyChanged("Title");
                    OnTitleChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_SMSItems_SMSList", Storage = "_SMSItems", OtherKey = "ListID")]
        public EntitySet<SMSItem> SMSItems
           {
               get => _SMSItems;

            set => _SMSItems.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_SMSList_People", Storage = "_Person", ThisKey = "SenderID", IsForeignKey = true)]
        public Person Person
        {
            get => _Person.Entity;

            set
            {
                Person previousValue = _Person.Entity;
                if (((previousValue != value)
                            || (_Person.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Person.Entity = null;
                        previousValue.SMSLists.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.SMSLists.Add(this);

                        _SenderID = value.PeopleId;

                    }

                    else
                    {
                        _SenderID = default(int);

                    }

                    SendPropertyChanged("Person");
                }
            }
        }

        [Association(Name = "FK_SMSList_SMSGroups", Storage = "_SMSGroup", ThisKey = "SendGroupID", IsForeignKey = true)]
        public SMSGroup SMSGroup
        {
            get => _SMSGroup.Entity;

            set
            {
                SMSGroup previousValue = _SMSGroup.Entity;
                if (((previousValue != value)
                            || (_SMSGroup.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _SMSGroup.Entity = null;
                        previousValue.SMSLists.Remove(this);
                    }

                    _SMSGroup.Entity = value;
                    if (value != null)
                    {
                        value.SMSLists.Add(this);

                        _SendGroupID = value.Id;

                    }

                    else
                    {
                        _SendGroupID = default(int);

                    }

                    SendPropertyChanged("SMSGroup");
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

        private void attach_SMSItems(SMSItem entity)
        {
            SendPropertyChanging();
            entity.SMSList = this;
        }

        private void detach_SMSItems(SMSItem entity)
        {
            SendPropertyChanging();
            entity.SMSList = null;
        }
    }
}
