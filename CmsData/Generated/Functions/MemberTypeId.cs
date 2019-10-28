using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "MemberTypeIds")]
    public partial class MemberTypeId
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _MemberTypeIdX;

        public MemberTypeId()
        {
        }

        [Column(Name = "member_type_id", Storage = "_MemberTypeIdX", DbType = "int NOT NULL")]
        public int MemberTypeIdX
        {
            get => _MemberTypeIdX;

            set
            {
                if (_MemberTypeIdX != value)
                {
                    _MemberTypeIdX = value;
                }
            }
        }
    }
}
