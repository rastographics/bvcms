using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PaymentProcessDetails")]
    public partial class PaymentProcessDetails
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _ProcessId;
        private string _ProcessName;
        private int? _GatewayAccountId;
        private string _GatewayAccountName;
        private int? _GatewayId;
        private string _GatewayDetailName;
        private string _GatewayDetailValue;
        private bool? _IsDefault;
        private bool? _IsBoolean;

        public PaymentProcessDetails()
        {
        }

        [Column(Name = "ProcessId", Storage = "_ProcessId", DbType = "int")]
        public int ProcessId
        {
            get => _ProcessId;

            set
            {
                if (_ProcessId != value)
                {
                    _ProcessId = value;
                }
            }
        }

        [Column(Name = "ProcessName", Storage = "_ProcessName", DbType = "nvarchar")]
        public string ProcessName
        {
            get => _ProcessName;

            set
            {
                if (_ProcessName != value)
                {
                    _ProcessName = value;
                }
            }
        }

        [Column(Name = "GatewayAccountId", Storage = "_GatewayAccountId", DbType = "int")]
        public int? GatewayAccountId
        {
            get => _GatewayAccountId;

            set
            {
                if (_GatewayAccountId != value)
                {
                    _GatewayAccountId = value;
                }
            }
        }

        [Column(Name = "GatewayAccountName", Storage = "_GatewayAccountName", DbType = "nvarchar")]
        public string GatewayAccountName
        {
            get => _GatewayAccountName;

            set
            {
                if (_GatewayAccountName != value)
                {
                    _GatewayAccountName = value;
                }
            }
        }

        [Column(Name = "GatewayId", Storage = "_GatewayId", DbType = "int")]
        public int? GatewayId
        {
            get => _GatewayId;

            set
            {
                if (_GatewayId != value)
                {
                    _GatewayId = value;
                }
            }
        }

        [Column(Name = "GatewayDetailName", Storage = "_GatewayDetailName", DbType = "nvarchar NOT NULL")]
        public string GatewayDetailName
        {
            get => _GatewayDetailName;

            set
            {
                if (_GatewayDetailName != value)
                {
                    _GatewayDetailName = value;
                }
            }
        }

        [Column(Name = "GatewayDetailValue", Storage = "_GatewayDetailValue", DbType = "nvarchar NOT NULL")]
        public string GatewayDetailValue
        {
            get => _GatewayDetailValue;

            set
            {
                if (_GatewayDetailValue != value)
                {
                    _GatewayDetailValue = value;
                }
            }
        }

        [Column(Name = "IsDefault", Storage = "_IsDefault", DbType = "bit NOT NULL")]
        public bool? IsDefault
        {
            get => _IsDefault;

            set
            {
                if (_IsDefault != value)
                {
                    _IsDefault = value;
                }
            }
        }

        [Column(Name = "IsBoolean", Storage = "_IsBoolean", DbType = "bit NOT NULL")]
        public bool? IsBoolean
        {
            get => _IsBoolean;

            set
            {
                if (_IsBoolean != value)
                {
                    _IsBoolean = value;
                }
            }
        }
    }
}
