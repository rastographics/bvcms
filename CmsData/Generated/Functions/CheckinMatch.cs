using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "CheckinMatch")]
    public partial class CheckinMatch
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Familyid;

        private string _Areacode;

        private string _Name;

        private string _Phone;

        private bool? _Locked;

        public CheckinMatch()
        {
        }

        [Column(Name = "familyid", Storage = "_Familyid", DbType = "int")]
        public int? Familyid
        {
            get => _Familyid;

            set
            {
                if (_Familyid != value)
                {
                    _Familyid = value;
                }
            }
        }

        [Column(Name = "areacode", Storage = "_Areacode", DbType = "nvarchar(3)")]
        public string Areacode
        {
            get => _Areacode;

            set
            {
                if (_Areacode != value)
                {
                    _Areacode = value;
                }
            }
        }

        [Column(Name = "NAME", Storage = "_Name", DbType = "nvarchar(100)")]
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

        [Column(Name = "phone", Storage = "_Phone", DbType = "nvarchar(20)")]
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

        [Column(Name = "locked", Storage = "_Locked", DbType = "bit")]
        public bool? Locked
        {
            get => _Locked;

            set
            {
                if (_Locked != value)
                {
                    _Locked = value;
                }
            }
        }
    }
}
