using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "SenderGifts")]
    public partial class SenderGift
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrgId;

        private string _Trip;

        private int _SenderId;

        private string _Sender;

        private int? _GoerId;

        private string _Goer;

        private DateTime? _DateGiven;

        private decimal? _Amt;

        private string _NoticeSent;

        public SenderGift()
        {
        }

        [Column(Name = "OrgId", Storage = "_OrgId", DbType = "int NOT NULL")]
        public int OrgId
        {
            get => _OrgId;

            set
            {
                if (_OrgId != value)
                {
                    _OrgId = value;
                }
            }
        }

        [Column(Name = "Trip", Storage = "_Trip", DbType = "nvarchar(100) NOT NULL")]
        public string Trip
        {
            get => _Trip;

            set
            {
                if (_Trip != value)
                {
                    _Trip = value;
                }
            }
        }

        [Column(Name = "SenderId", Storage = "_SenderId", DbType = "int NOT NULL")]
        public int SenderId
        {
            get => _SenderId;

            set
            {
                if (_SenderId != value)
                {
                    _SenderId = value;
                }
            }
        }

        [Column(Name = "Sender", Storage = "_Sender", DbType = "nvarchar(139)")]
        public string Sender
        {
            get => _Sender;

            set
            {
                if (_Sender != value)
                {
                    _Sender = value;
                }
            }
        }

        [Column(Name = "GoerId", Storage = "_GoerId", DbType = "int")]
        public int? GoerId
        {
            get => _GoerId;

            set
            {
                if (_GoerId != value)
                {
                    _GoerId = value;
                }
            }
        }

        [Column(Name = "Goer", Storage = "_Goer", DbType = "nvarchar(139)")]
        public string Goer
        {
            get => _Goer;

            set
            {
                if (_Goer != value)
                {
                    _Goer = value;
                }
            }
        }

        [Column(Name = "DateGiven", Storage = "_DateGiven", DbType = "datetime")]
        public DateTime? DateGiven
        {
            get => _DateGiven;

            set
            {
                if (_DateGiven != value)
                {
                    _DateGiven = value;
                }
            }
        }

        [Column(Name = "Amt", Storage = "_Amt", DbType = "money")]
        public decimal? Amt
        {
            get => _Amt;

            set
            {
                if (_Amt != value)
                {
                    _Amt = value;
                }
            }
        }

        [Column(Name = "NoticeSent", Storage = "_NoticeSent", DbType = "varchar(9) NOT NULL")]
        public string NoticeSent
        {
            get => _NoticeSent;

            set
            {
                if (_NoticeSent != value)
                {
                    _NoticeSent = value;
                }
            }
        }
    }
}
