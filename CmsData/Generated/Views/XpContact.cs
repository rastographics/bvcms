using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpContact")]
    public partial class XpContact
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _ContactId;

        private string _ContactType;

        private DateTime _ContactDate;

        private string _ContactReason;

        private string _Ministry;

        private bool? _NotAtHome;

        private bool? _LeftDoorHanger;

        private bool? _LeftMessage;

        private bool? _GospelShared;

        private bool? _PrayerRequest;

        private bool? _ContactMade;

        private bool? _GiftBagGiven;

        private string _Comments;

        private int? _OrganizationId;

        public XpContact()
        {
        }

        [Column(Name = "ContactId", Storage = "_ContactId", DbType = "int NOT NULL")]
        public int ContactId
        {
            get => _ContactId;

            set
            {
                if (_ContactId != value)
                {
                    _ContactId = value;
                }
            }
        }

        [Column(Name = "ContactType", Storage = "_ContactType", DbType = "nvarchar(100)")]
        public string ContactType
        {
            get => _ContactType;

            set
            {
                if (_ContactType != value)
                {
                    _ContactType = value;
                }
            }
        }

        [Column(Name = "ContactDate", Storage = "_ContactDate", DbType = "datetime NOT NULL")]
        public DateTime ContactDate
        {
            get => _ContactDate;

            set
            {
                if (_ContactDate != value)
                {
                    _ContactDate = value;
                }
            }
        }

        [Column(Name = "ContactReason", Storage = "_ContactReason", DbType = "nvarchar(100)")]
        public string ContactReason
        {
            get => _ContactReason;

            set
            {
                if (_ContactReason != value)
                {
                    _ContactReason = value;
                }
            }
        }

        [Column(Name = "Ministry", Storage = "_Ministry", DbType = "nvarchar(50)")]
        public string Ministry
        {
            get => _Ministry;

            set
            {
                if (_Ministry != value)
                {
                    _Ministry = value;
                }
            }
        }

        [Column(Name = "NotAtHome", Storage = "_NotAtHome", DbType = "bit")]
        public bool? NotAtHome
        {
            get => _NotAtHome;

            set
            {
                if (_NotAtHome != value)
                {
                    _NotAtHome = value;
                }
            }
        }

        [Column(Name = "LeftDoorHanger", Storage = "_LeftDoorHanger", DbType = "bit")]
        public bool? LeftDoorHanger
        {
            get => _LeftDoorHanger;

            set
            {
                if (_LeftDoorHanger != value)
                {
                    _LeftDoorHanger = value;
                }
            }
        }

        [Column(Name = "LeftMessage", Storage = "_LeftMessage", DbType = "bit")]
        public bool? LeftMessage
        {
            get => _LeftMessage;

            set
            {
                if (_LeftMessage != value)
                {
                    _LeftMessage = value;
                }
            }
        }

        [Column(Name = "GospelShared", Storage = "_GospelShared", DbType = "bit")]
        public bool? GospelShared
        {
            get => _GospelShared;

            set
            {
                if (_GospelShared != value)
                {
                    _GospelShared = value;
                }
            }
        }

        [Column(Name = "PrayerRequest", Storage = "_PrayerRequest", DbType = "bit")]
        public bool? PrayerRequest
        {
            get => _PrayerRequest;

            set
            {
                if (_PrayerRequest != value)
                {
                    _PrayerRequest = value;
                }
            }
        }

        [Column(Name = "ContactMade", Storage = "_ContactMade", DbType = "bit")]
        public bool? ContactMade
        {
            get => _ContactMade;

            set
            {
                if (_ContactMade != value)
                {
                    _ContactMade = value;
                }
            }
        }

        [Column(Name = "GiftBagGiven", Storage = "_GiftBagGiven", DbType = "bit")]
        public bool? GiftBagGiven
        {
            get => _GiftBagGiven;

            set
            {
                if (_GiftBagGiven != value)
                {
                    _GiftBagGiven = value;
                }
            }
        }

        [Column(Name = "Comments", Storage = "_Comments", DbType = "nvarchar")]
        public string Comments
        {
            get => _Comments;

            set
            {
                if (_Comments != value)
                {
                    _Comments = value;
                }
            }
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int")]
        public int? OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
                }
            }
        }
    }
}
