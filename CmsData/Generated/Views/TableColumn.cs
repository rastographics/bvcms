using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "TableColumns")]
    public partial class TableColumn
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Database;

        private string _Owner;

        private string _TableName;

        private string _ColumnName;

        private int? _OrdinalPosition;

        private string _DefaultSetting;

        private string _IsNullable;

        private string _DataType;

        private int? _MaxLength;

        private short? _DatePrecision;

        private int? _IsIdentity;

        private int? _IsComputed;

        private int? _Scale;

        private byte? _Precision;

        private bool? _IsPrimaryKey;

        public TableColumn()
        {
        }

        [Column(Name = "Database", Storage = "_Database", DbType = "nvarchar(128)")]
        public string Database
        {
            get => _Database;

            set
            {
                if (_Database != value)
                {
                    _Database = value;
                }
            }
        }

        [Column(Name = "Owner", Storage = "_Owner", DbType = "nvarchar(128)")]
        public string Owner
        {
            get => _Owner;

            set
            {
                if (_Owner != value)
                {
                    _Owner = value;
                }
            }
        }

        [Column(Name = "TableName", Storage = "_TableName", DbType = "nvarchar(128) NOT NULL")]
        public string TableName
        {
            get => _TableName;

            set
            {
                if (_TableName != value)
                {
                    _TableName = value;
                }
            }
        }

        [Column(Name = "ColumnName", Storage = "_ColumnName", DbType = "nvarchar(128)")]
        public string ColumnName
        {
            get => _ColumnName;

            set
            {
                if (_ColumnName != value)
                {
                    _ColumnName = value;
                }
            }
        }

        [Column(Name = "OrdinalPosition", Storage = "_OrdinalPosition", DbType = "int")]
        public int? OrdinalPosition
        {
            get => _OrdinalPosition;

            set
            {
                if (_OrdinalPosition != value)
                {
                    _OrdinalPosition = value;
                }
            }
        }

        [Column(Name = "DefaultSetting", Storage = "_DefaultSetting", DbType = "nvarchar(4000)")]
        public string DefaultSetting
        {
            get => _DefaultSetting;

            set
            {
                if (_DefaultSetting != value)
                {
                    _DefaultSetting = value;
                }
            }
        }

        [Column(Name = "IsNullable", Storage = "_IsNullable", DbType = "varchar(3)")]
        public string IsNullable
        {
            get => _IsNullable;

            set
            {
                if (_IsNullable != value)
                {
                    _IsNullable = value;
                }
            }
        }

        [Column(Name = "DataType", Storage = "_DataType", DbType = "nvarchar(128)")]
        public string DataType
        {
            get => _DataType;

            set
            {
                if (_DataType != value)
                {
                    _DataType = value;
                }
            }
        }

        [Column(Name = "MaxLength", Storage = "_MaxLength", DbType = "int")]
        public int? MaxLength
        {
            get => _MaxLength;

            set
            {
                if (_MaxLength != value)
                {
                    _MaxLength = value;
                }
            }
        }

        [Column(Name = "DatePrecision", Storage = "_DatePrecision", DbType = "smallint")]
        public short? DatePrecision
        {
            get => _DatePrecision;

            set
            {
                if (_DatePrecision != value)
                {
                    _DatePrecision = value;
                }
            }
        }

        [Column(Name = "IsIdentity", Storage = "_IsIdentity", DbType = "int")]
        public int? IsIdentity
        {
            get => _IsIdentity;

            set
            {
                if (_IsIdentity != value)
                {
                    _IsIdentity = value;
                }
            }
        }

        [Column(Name = "IsComputed", Storage = "_IsComputed", DbType = "int")]
        public int? IsComputed
        {
            get => _IsComputed;

            set
            {
                if (_IsComputed != value)
                {
                    _IsComputed = value;
                }
            }
        }

        [Column(Name = "scale", Storage = "_Scale", DbType = "int")]
        public int? Scale
        {
            get => _Scale;

            set
            {
                if (_Scale != value)
                {
                    _Scale = value;
                }
            }
        }

        [Column(Name = "precision", Storage = "_Precision", DbType = "tinyint")]
        public byte? Precision
        {
            get => _Precision;

            set
            {
                if (_Precision != value)
                {
                    _Precision = value;
                }
            }
        }

        [Column(Name = "IsPrimaryKey", Storage = "_IsPrimaryKey", DbType = "bit")]
        public bool? IsPrimaryKey
        {
            get => _IsPrimaryKey;

            set
            {
                if (_IsPrimaryKey != value)
                {
                    _IsPrimaryKey = value;
                }
            }
        }
    }
}
