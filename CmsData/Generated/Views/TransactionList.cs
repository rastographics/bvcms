using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "TransactionList")]
    public partial class TransactionList
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Id;

        private DateTime? _TransactionDate;

        private string _TransactionGateway;

        private int? _DatumId;

        private bool? _Testing;

        private decimal? _Amt;

        private string _ApprovalCode;

        private bool? _Approved;

        private string _TransactionId;

        private string _Message;

        private string _AuthCode;

        private decimal? _Amtdue;

        private string _Url;

        private string _Description;

        private string _Name;

        private string _Address;

        private string _City;

        private string _State;

        private string _Zip;

        private string _Phone;

        private string _Emails;

        private string _Participants;

        private int? _OrgId;

        private int? _OriginalId;

        private decimal? _Regfees;

        private decimal? _Donate;

        private string _Fund;

        private bool? _Financeonly;

        private bool? _Voided;

        private bool? _Credited;

        private bool? _Coupon;

        private bool? _Moneytran;

        private DateTime? _Settled;

        private DateTime? _Batch;

        private string _Batchref;

        private string _Batchtyp;

        private bool? _Fromsage;

        private int? _LoginPeopleId;

        private string _First;

        private string _MiddleInitial;

        private string _Last;

        private string _Suffix;

        private bool? _AdjustFee;

        private string _LastFourCC;

        private string _LastFourACH;

        private string _PaymentType;

        private string _Address2;

        private string _Country;

        private int _BalancesId;

        private decimal? _BegBal;

        private decimal? _Payment;

        private decimal? _TotDue;

        private int? _NumPeople;

        private string _People;

        private bool? _CanVoid;

        private bool? _CanCredit;

        private bool? _IsAdjustment;

        public TransactionList()
        {
        }

        [Column(Name = "Id", Storage = "_Id", DbType = "int NOT NULL")]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    _Id = value;
                }
            }
        }

        [Column(Name = "TransactionDate", Storage = "_TransactionDate", DbType = "datetime")]
        public DateTime? TransactionDate
        {
            get => _TransactionDate;

            set
            {
                if (_TransactionDate != value)
                {
                    _TransactionDate = value;
                }
            }
        }

        [Column(Name = "TransactionGateway", Storage = "_TransactionGateway", DbType = "nvarchar(50)")]
        public string TransactionGateway
        {
            get => _TransactionGateway;

            set
            {
                if (_TransactionGateway != value)
                {
                    _TransactionGateway = value;
                }
            }
        }

        [Column(Name = "DatumId", Storage = "_DatumId", DbType = "int")]
        public int? DatumId
        {
            get => _DatumId;

            set
            {
                if (_DatumId != value)
                {
                    _DatumId = value;
                }
            }
        }

        [Column(Name = "testing", Storage = "_Testing", DbType = "bit")]
        public bool? Testing
        {
            get => _Testing;

            set
            {
                if (_Testing != value)
                {
                    _Testing = value;
                }
            }
        }

        [Column(Name = "amt", Storage = "_Amt", DbType = "money")]
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

        [Column(Name = "ApprovalCode", Storage = "_ApprovalCode", DbType = "nvarchar(150)")]
        public string ApprovalCode
        {
            get => _ApprovalCode;

            set
            {
                if (_ApprovalCode != value)
                {
                    _ApprovalCode = value;
                }
            }
        }

        [Column(Name = "Approved", Storage = "_Approved", DbType = "bit")]
        public bool? Approved
        {
            get => _Approved;

            set
            {
                if (_Approved != value)
                {
                    _Approved = value;
                }
            }
        }

        [Column(Name = "TransactionId", Storage = "_TransactionId", DbType = "nvarchar(50)")]
        public string TransactionId
        {
            get => _TransactionId;

            set
            {
                if (_TransactionId != value)
                {
                    _TransactionId = value;
                }
            }
        }

        [Column(Name = "Message", Storage = "_Message", DbType = "nvarchar(150)")]
        public string Message
        {
            get => _Message;

            set
            {
                if (_Message != value)
                {
                    _Message = value;
                }
            }
        }

        [Column(Name = "AuthCode", Storage = "_AuthCode", DbType = "nvarchar(150)")]
        public string AuthCode
        {
            get => _AuthCode;

            set
            {
                if (_AuthCode != value)
                {
                    _AuthCode = value;
                }
            }
        }

        [Column(Name = "amtdue", Storage = "_Amtdue", DbType = "money")]
        public decimal? Amtdue
        {
            get => _Amtdue;

            set
            {
                if (_Amtdue != value)
                {
                    _Amtdue = value;
                }
            }
        }

        [Column(Name = "URL", Storage = "_Url", DbType = "nvarchar(300)")]
        public string Url
        {
            get => _Url;

            set
            {
                if (_Url != value)
                {
                    _Url = value;
                }
            }
        }

        [Column(Name = "Description", Storage = "_Description", DbType = "nvarchar(180)")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    _Description = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(100)")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    _Name = value;
                }
            }
        }

        [Column(Name = "Address", Storage = "_Address", DbType = "nvarchar(50)")]
        public string Address
        {
            get => _Address;

            set
            {
                if (_Address != value)
                {
                    _Address = value;
                }
            }
        }

        [Column(Name = "City", Storage = "_City", DbType = "nvarchar(50)")]
        public string City
        {
            get => _City;

            set
            {
                if (_City != value)
                {
                    _City = value;
                }
            }
        }

        [Column(Name = "State", Storage = "_State", DbType = "nvarchar(20)")]
        public string State
        {
            get => _State;

            set
            {
                if (_State != value)
                {
                    _State = value;
                }
            }
        }

        [Column(Name = "Zip", Storage = "_Zip", DbType = "nvarchar(15)")]
        public string Zip
        {
            get => _Zip;

            set
            {
                if (_Zip != value)
                {
                    _Zip = value;
                }
            }
        }

        [Column(Name = "Phone", Storage = "_Phone", DbType = "nvarchar(20)")]
        public string Phone
        {
            get => _Phone;

            set
            {
                if (_Phone != value)
                {
                    _Phone = value;
                }
            }
        }

        [Column(Name = "Emails", Storage = "_Emails", DbType = "nvarchar")]
        public string Emails
        {
            get => _Emails;

            set
            {
                if (_Emails != value)
                {
                    _Emails = value;
                }
            }
        }

        [Column(Name = "Participants", Storage = "_Participants", DbType = "nvarchar")]
        public string Participants
        {
            get => _Participants;

            set
            {
                if (_Participants != value)
                {
                    _Participants = value;
                }
            }
        }

        [Column(Name = "OrgId", Storage = "_OrgId", DbType = "int")]
        public int? OrgId
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

        [Column(Name = "OriginalId", Storage = "_OriginalId", DbType = "int")]
        public int? OriginalId
        {
            get => _OriginalId;

            set
            {
                if (_OriginalId != value)
                {
                    _OriginalId = value;
                }
            }
        }

        [Column(Name = "regfees", Storage = "_Regfees", DbType = "money")]
        public decimal? Regfees
        {
            get => _Regfees;

            set
            {
                if (_Regfees != value)
                {
                    _Regfees = value;
                }
            }
        }

        [Column(Name = "donate", Storage = "_Donate", DbType = "money")]
        public decimal? Donate
        {
            get => _Donate;

            set
            {
                if (_Donate != value)
                {
                    _Donate = value;
                }
            }
        }

        [Column(Name = "fund", Storage = "_Fund", DbType = "nvarchar(50)")]
        public string Fund
        {
            get => _Fund;

            set
            {
                if (_Fund != value)
                {
                    _Fund = value;
                }
            }
        }

        [Column(Name = "financeonly", Storage = "_Financeonly", DbType = "bit")]
        public bool? Financeonly
        {
            get => _Financeonly;

            set
            {
                if (_Financeonly != value)
                {
                    _Financeonly = value;
                }
            }
        }

        [Column(Name = "voided", Storage = "_Voided", DbType = "bit")]
        public bool? Voided
        {
            get => _Voided;

            set
            {
                if (_Voided != value)
                {
                    _Voided = value;
                }
            }
        }

        [Column(Name = "credited", Storage = "_Credited", DbType = "bit")]
        public bool? Credited
        {
            get => _Credited;

            set
            {
                if (_Credited != value)
                {
                    _Credited = value;
                }
            }
        }

        [Column(Name = "coupon", Storage = "_Coupon", DbType = "bit")]
        public bool? Coupon
        {
            get => _Coupon;

            set
            {
                if (_Coupon != value)
                {
                    _Coupon = value;
                }
            }
        }

        [Column(Name = "moneytran", Storage = "_Moneytran", DbType = "bit")]
        public bool? Moneytran
        {
            get => _Moneytran;

            set
            {
                if (_Moneytran != value)
                {
                    _Moneytran = value;
                }
            }
        }

        [Column(Name = "settled", Storage = "_Settled", DbType = "datetime")]
        public DateTime? Settled
        {
            get => _Settled;

            set
            {
                if (_Settled != value)
                {
                    _Settled = value;
                }
            }
        }

        [Column(Name = "batch", Storage = "_Batch", DbType = "datetime")]
        public DateTime? Batch
        {
            get => _Batch;

            set
            {
                if (_Batch != value)
                {
                    _Batch = value;
                }
            }
        }

        [Column(Name = "batchref", Storage = "_Batchref", DbType = "nvarchar(50)")]
        public string Batchref
        {
            get => _Batchref;

            set
            {
                if (_Batchref != value)
                {
                    _Batchref = value;
                }
            }
        }

        [Column(Name = "batchtyp", Storage = "_Batchtyp", DbType = "nvarchar(50)")]
        public string Batchtyp
        {
            get => _Batchtyp;

            set
            {
                if (_Batchtyp != value)
                {
                    _Batchtyp = value;
                }
            }
        }

        [Column(Name = "fromsage", Storage = "_Fromsage", DbType = "bit")]
        public bool? Fromsage
        {
            get => _Fromsage;

            set
            {
                if (_Fromsage != value)
                {
                    _Fromsage = value;
                }
            }
        }

        [Column(Name = "LoginPeopleId", Storage = "_LoginPeopleId", DbType = "int")]
        public int? LoginPeopleId
        {
            get => _LoginPeopleId;

            set
            {
                if (_LoginPeopleId != value)
                {
                    _LoginPeopleId = value;
                }
            }
        }

        [Column(Name = "First", Storage = "_First", DbType = "nvarchar(50)")]
        public string First
        {
            get => _First;

            set
            {
                if (_First != value)
                {
                    _First = value;
                }
            }
        }

        [Column(Name = "MiddleInitial", Storage = "_MiddleInitial", DbType = "nvarchar(1)")]
        public string MiddleInitial
        {
            get => _MiddleInitial;

            set
            {
                if (_MiddleInitial != value)
                {
                    _MiddleInitial = value;
                }
            }
        }

        [Column(Name = "Last", Storage = "_Last", DbType = "nvarchar(50)")]
        public string Last
        {
            get => _Last;

            set
            {
                if (_Last != value)
                {
                    _Last = value;
                }
            }
        }

        [Column(Name = "Suffix", Storage = "_Suffix", DbType = "nvarchar(10)")]
        public string Suffix
        {
            get => _Suffix;

            set
            {
                if (_Suffix != value)
                {
                    _Suffix = value;
                }
            }
        }

        [Column(Name = "AdjustFee", Storage = "_AdjustFee", DbType = "bit")]
        public bool? AdjustFee
        {
            get => _AdjustFee;

            set
            {
                if (_AdjustFee != value)
                {
                    _AdjustFee = value;
                }
            }
        }

        [Column(Name = "LastFourCC", Storage = "_LastFourCC", DbType = "nvarchar(4)")]
        public string LastFourCC
        {
            get => _LastFourCC;

            set
            {
                if (_LastFourCC != value)
                {
                    _LastFourCC = value;
                }
            }
        }

        [Column(Name = "LastFourACH", Storage = "_LastFourACH", DbType = "nvarchar(4)")]
        public string LastFourACH
        {
            get => _LastFourACH;

            set
            {
                if (_LastFourACH != value)
                {
                    _LastFourACH = value;
                }
            }
        }

        [Column(Name = "PaymentType", Storage = "_PaymentType", DbType = "nvarchar(1)")]
        public string PaymentType
        {
            get => _PaymentType;

            set
            {
                if (_PaymentType != value)
                {
                    _PaymentType = value;
                }
            }
        }

        [Column(Name = "Address2", Storage = "_Address2", DbType = "nvarchar(50)")]
        public string Address2
        {
            get => _Address2;

            set
            {
                if (_Address2 != value)
                {
                    _Address2 = value;
                }
            }
        }

        [Column(Name = "Country", Storage = "_Country", DbType = "nvarchar(50)")]
        public string Country
        {
            get => _Country;

            set
            {
                if (_Country != value)
                {
                    _Country = value;
                }
            }
        }

        [Column(Name = "BalancesId", Storage = "_BalancesId", DbType = "int NOT NULL")]
        public int BalancesId
        {
            get => _BalancesId;

            set
            {
                if (_BalancesId != value)
                {
                    _BalancesId = value;
                }
            }
        }

        [Column(Name = "BegBal", Storage = "_BegBal", DbType = "money")]
        public decimal? BegBal
        {
            get => _BegBal;

            set
            {
                if (_BegBal != value)
                {
                    _BegBal = value;
                }
            }
        }

        [Column(Name = "Payment", Storage = "_Payment", DbType = "money")]
        public decimal? Payment
        {
            get => _Payment;

            set
            {
                if (_Payment != value)
                {
                    _Payment = value;
                }
            }
        }

        [Column(Name = "TotDue", Storage = "_TotDue", DbType = "money")]
        public decimal? TotDue
        {
            get => _TotDue;

            set
            {
                if (_TotDue != value)
                {
                    _TotDue = value;
                }
            }
        }

        [Column(Name = "NumPeople", Storage = "_NumPeople", DbType = "int")]
        public int? NumPeople
        {
            get => _NumPeople;

            set
            {
                if (_NumPeople != value)
                {
                    _NumPeople = value;
                }
            }
        }

        [Column(Name = "People", Storage = "_People", DbType = "nvarchar")]
        public string People
        {
            get => _People;

            set
            {
                if (_People != value)
                {
                    _People = value;
                }
            }
        }

        [Column(Name = "CanVoid", Storage = "_CanVoid", DbType = "bit")]
        public bool? CanVoid
        {
            get => _CanVoid;

            set
            {
                if (_CanVoid != value)
                {
                    _CanVoid = value;
                }
            }
        }

        [Column(Name = "CanCredit", Storage = "_CanCredit", DbType = "bit")]
        public bool? CanCredit
        {
            get => _CanCredit;

            set
            {
                if (_CanCredit != value)
                {
                    _CanCredit = value;
                }
            }
        }

        [Column(Name = "IsAdjustment", Storage = "_IsAdjustment", DbType = "bit")]
        public bool? IsAdjustment
        {
            get => _IsAdjustment;

            set
            {
                if (_IsAdjustment != value)
                {
                    _IsAdjustment = value;
                }
            }
        }
    }
}
