using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RecentVisitNumberOrgs")]
    public partial class RecentVisitNumberOrgs
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs("");

        private int _PeopleId;
        private int? _SeqNo;
        private int? _OrgId;

        public RecentVisitNumberOrgs()
        {
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int")]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    _PeopleId = value;
                }
            }
        }
        [Column(Name = "SeqNo", Storage = "_SeqNo", DbType = "int")]
        public int? SeqNo
        {
            get => _SeqNo;

            set
            {
                if (_SeqNo != value)
                {
                    _SeqNo = value;
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
    }
}
