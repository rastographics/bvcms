using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData.View
{
    [Table(Name = "PaymentProcessDetails")]
    public partial class PaymentProcessDetails
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

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
            get
            {
                return this._ProcessId;
            }

            set
            {
                if (this._ProcessId != value)
                    this._ProcessId = value;
            }
        }

        [Column(Name = "ProcessName", Storage = "_ProcessName", DbType = "nvarchar")]
        public string ProcessName
        {
            get
            {
                return this._ProcessName;
            }

            set
            {
                if (this._ProcessName != value)
                    this._ProcessName = value;
            }
        }

        [Column(Name = "GatewayAccountId", Storage = "_GatewayAccountId", DbType = "int")]
        public int? GatewayAccountId
        {
            get
            {
                return this._GatewayAccountId;
            }

            set
            {
                if (this._GatewayAccountId != value)
                    this._GatewayAccountId = value;
            }
        }

        [Column(Name = "GatewayAccountName", Storage = "_GatewayAccountName", DbType = "nvarchar")]
        public string GatewayAccountName
        {
            get
            {
                return this._GatewayAccountName;
            }

            set
            {
                if (this._GatewayAccountName != value)
                    this._GatewayAccountName = value;
            }
        }

        [Column(Name = "GatewayId", Storage = "_GatewayId", DbType = "int")]
        public int? GatewayId
        {
            get
            {
                return this._GatewayId;
            }

            set
            {
                if (this._GatewayId != value)
                    this._GatewayId = value;
            }
        }

        [Column(Name = "GatewayDetailName", Storage = "_GatewayDetailName", DbType = "nvarchar NOT NULL")]
        public string GatewayDetailName
        {
            get
            {
                return this._GatewayDetailName;
            }

            set
            {
                if (this._GatewayDetailName != value)
                    this._GatewayDetailName = value;
            }
        }

        [Column(Name = "GatewayDetailValue", Storage = "_GatewayDetailValue", DbType = "nvarchar NOT NULL")]
        public string GatewayDetailValue
        {
            get
            {
                return this._GatewayDetailValue;
            }

            set
            {
                if (this._GatewayDetailValue != value)
                    this._GatewayDetailValue = value;
            }
        }

        [Column(Name = "IsDefault", Storage = "_IsDefault", DbType = "bit NOT NULL")]
        public bool? IsDefault
        {
            get
            {
                return this._IsDefault;
            }

            set
            {
                if (this._IsDefault != value)
                    this._IsDefault = value;
            }
        }

        [Column(Name = "IsBoolean", Storage = "_IsBoolean", DbType = "bit NOT NULL")]
        public bool? IsBoolean
        {
            get
            {
                return this._IsBoolean;
            }

            set
            {
                if (this._IsBoolean != value)
                    this._IsBoolean = value;
            }
        }
    }
}
