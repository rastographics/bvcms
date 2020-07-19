using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using CmsData.Infrastructure;

namespace CmsData
{
    [Table(Name = "dbo.TicketingOrder")]
    public partial class TicketingOrder : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _OrderId;

        private int _MeetingId;

        private string _Status;

        private string _SelectedSeats;

        private int _Count;

        private decimal _TotalPrice;

        private DateTime? _PurchaseDate;

        private int? _PeopleId;

        private DateTime _CreatedDate;

        private EntitySet<TicketingSeat> _TicketingSeats;

        private EntityRef<Meeting> _Meeting;

        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(ChangeAction action);
        partial void OnCreated();

        partial void OnOrderIdChanging(int value);
        partial void OnOrderIdChanged();

        partial void OnMeetingIdChanging(int value);
        partial void OnMeetingIdChanged();

        partial void OnStatusChanging(string value);
        partial void OnStatusChanged();

        partial void OnCountChanging(int? value);
        partial void OnCountChanged();

        partial void OnTotalPriceChanging(decimal? value);
        partial void OnTotalPriceChanged();

        partial void OnPurchaseDateChanging(DateTime? value);
        partial void OnPurchaseDateChanged();

        partial void OnPeopleIdChanging(int? value);
        partial void OnPeopleIdChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        #endregion
        public TicketingOrder()
        {
            _TicketingSeats = new EntitySet<TicketingSeat>(new Action<TicketingSeat>(attach_TicketingSeats), new Action<TicketingSeat>(detach_TicketingSeats));
            _Meeting = default(EntityRef<Meeting>);
            OnCreated();
        }

        #region Columns

        [Column(Name = "OrderId", UpdateCheck = UpdateCheck.Never, Storage = "_OrderId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int OrderId
        {
            get { return _OrderId; }

            set
            {
                if (_OrderId != value)
                {
                    OnOrderIdChanging(value);
                    SendPropertyChanging();
                    _OrderId = value;
                    SendPropertyChanged("OrderId");
                    OnOrderIdChanged();
                }
            }
        }

        [Column(Name = "MeetingId", UpdateCheck = UpdateCheck.Never, Storage = "_MeetingId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int MeetingId
        {
            get { return _MeetingId; }

            set
            {
                if (_MeetingId != value)
                {
                    if (_Meeting.HasLoadedOrAssignedValue)
                        throw new ForeignKeyReferenceAlreadyHasValueException();

                    OnMeetingIdChanging(value);
                    SendPropertyChanging();
                    _MeetingId = value;
                    SendPropertyChanged("MeetingId");
                    OnMeetingIdChanged();
                }
            }
        }

        [Column(Name = "Status", UpdateCheck = UpdateCheck.Never, Storage = "_Status", DbType = "varchar(25) NOT NULL")]
        public string Status
        {
            get { return _Status; }

            set
            {
                if (_Status != value)
                {
                    OnStatusChanging(value);
                    SendPropertyChanging();
                    _Status = value;
                    SendPropertyChanged("Status");
                    OnStatusChanged();
                }
            }
        }

        [Column(Name = "SelectedSeats", UpdateCheck = UpdateCheck.Never, Storage = "_SelectedSeats", DbType = "varchar(1000) NOT NULL")]
        public string SelectedSeats
        {
            get { return _SelectedSeats; }

            set
            {
                if (_SelectedSeats != value)
                {
                    OnStatusChanging(value);
                    SendPropertyChanging();
                    _SelectedSeats = value;
                    SendPropertyChanged("SelectedSeats");
                    OnStatusChanged();
                }
            }
        }

        [Column(Name = "Count", UpdateCheck = UpdateCheck.Never, Storage = "_Count", DbType = "int not null")]
        public int Count
        {
            get { return _Count; }

            set
            {
                if (_Count != value)
                {
                    OnCountChanging(value);
                    SendPropertyChanging();
                    _Count = value;
                    SendPropertyChanged("Count");
                    OnCountChanged();
                }
            }
        }

        [Column(Name = "TotalPrice", UpdateCheck = UpdateCheck.Never, Storage = "_TotalPrice", DbType = "money not null")]
        public decimal TotalPrice
        {
            get { return _TotalPrice; }

            set
            {
                if (_TotalPrice != value)
                {
                    OnTotalPriceChanging(value);
                    SendPropertyChanging();
                    _TotalPrice = value;
                    SendPropertyChanged("TotalPrice");
                    OnTotalPriceChanged();
                }
            }
        }

        [Column(Name = "PurchaseDate", UpdateCheck = UpdateCheck.Never, Storage = "_PurchaseDate", DbType = "datetime")]
        public DateTime? PurchaseDate
        {
            get { return _PurchaseDate; }

            set
            {
                if (_PurchaseDate != value)
                {
                    OnPurchaseDateChanging(value);
                    SendPropertyChanging();
                    _PurchaseDate = value;
                    SendPropertyChanged("PurchaseDate");
                    OnPurchaseDateChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
        {
            get { return _PeopleId; }

            set
            {
                if (_PeopleId != value)
                {
                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime NOT NULL")]
        public DateTime CreatedDate
        {
            get { return _CreatedDate; }

            set
            {
                if (_CreatedDate != value)
                {
                    OnCreatedDateChanging(value);
                    SendPropertyChanging();
                    _CreatedDate = value;
                    SendPropertyChanged("CreatedDate");
                    OnCreatedDateChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_TicketingSeats_TicketingOrder", Storage = "_TicketingSeats", OtherKey = "OrderId")]
        public EntitySet<TicketingSeat> TicketingSeats
        {
            get { return _TicketingSeats; }

            set { _TicketingSeats.Assign(value); }
        }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_TicketingOrder_Meetings", Storage = "_Meeting", ThisKey = "MeetingId", IsForeignKey = true)]
        public Meeting Meeting
        {
            get { return _Meeting.Entity; }

            set
            {
                Meeting previousValue = _Meeting.Entity;
                if (((previousValue != value)
                            || (_Meeting.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Meeting.Entity = null;
                        previousValue.TicketingOrders.Remove(this);
                    }

                    _Meeting.Entity = value;
                    if (value != null)
                    {
                        value.TicketingOrders.Add(this);
                        _MeetingId = value.MeetingId;
                    }
                    else
                    {
                        _MeetingId = default(int);
                    }
                    SendPropertyChanged("Meeting");
                }
            }
        }

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((PropertyChanging != null))
                PropertyChanging(this, emptyChangingEventArgs);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((PropertyChanged != null))
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void attach_TicketingSeats(TicketingSeat entity)
        {
            SendPropertyChanging();
            entity.TicketingOrder = this;
        }

        private void detach_TicketingSeats(TicketingSeat entity)
        {
            SendPropertyChanging();
            entity.TicketingOrder = null;
        }
    }
}
