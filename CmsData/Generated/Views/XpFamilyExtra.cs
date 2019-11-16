using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpFamilyExtra")]
    public partial class XpFamilyExtra
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _FamilyId;

        private string _Field;

        private string _CodeValue;

        private DateTime? _DateValue;

        private string _TextValue;

        private int? _IntValue;

        private bool? _BitValue;

        private string _Type;

        public XpFamilyExtra()
        {
        }

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int NOT NULL")]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    _FamilyId = value;
                }
            }
        }

        [Column(Name = "Field", Storage = "_Field", DbType = "nvarchar(50) NOT NULL")]
        public string Field
        {
            get => _Field;

            set
            {
                if (_Field != value)
                {
                    _Field = value;
                }
            }
        }

        [Column(Name = "CodeValue", Storage = "_CodeValue", DbType = "nvarchar(200)")]
        public string CodeValue
        {
            get => _CodeValue;

            set
            {
                if (_CodeValue != value)
                {
                    _CodeValue = value;
                }
            }
        }

        [Column(Name = "DateValue", Storage = "_DateValue", DbType = "datetime")]
        public DateTime? DateValue
        {
            get => _DateValue;

            set
            {
                if (_DateValue != value)
                {
                    _DateValue = value;
                }
            }
        }

        [Column(Name = "TextValue", Storage = "_TextValue", DbType = "nvarchar")]
        public string TextValue
        {
            get => _TextValue;

            set
            {
                if (_TextValue != value)
                {
                    _TextValue = value;
                }
            }
        }

        [Column(Name = "IntValue", Storage = "_IntValue", DbType = "int")]
        public int? IntValue
        {
            get => _IntValue;

            set
            {
                if (_IntValue != value)
                {
                    _IntValue = value;
                }
            }
        }

        [Column(Name = "BitValue", Storage = "_BitValue", DbType = "bit")]
        public bool? BitValue
        {
            get => _BitValue;

            set
            {
                if (_BitValue != value)
                {
                    _BitValue = value;
                }
            }
        }

        [Column(Name = "Type", Storage = "_Type", DbType = "varchar(22) NOT NULL")]
        public string Type
        {
            get => _Type;

            set
            {
                if (_Type != value)
                {
                    _Type = value;
                }
            }
        }
    }
}
