using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "CheckinMatchBarCodeOnly")]
    public partial class CheckinMatchBarCodeOnly
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Familyid;

        private string _Name;

        private bool? _Locked;

        public CheckinMatchBarCodeOnly()
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
