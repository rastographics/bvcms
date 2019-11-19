using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "LastFamilyOrgAttends")]
    public partial class LastFamilyOrgAttend
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _FamilyId;

        private int _PeopleId;

        private DateTime? _Lastattend;

        public LastFamilyOrgAttend()
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

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int NOT NULL")]
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

        [Column(Name = "lastattend", Storage = "_Lastattend", DbType = "datetime")]
        public DateTime? Lastattend
        {
            get => _Lastattend;

            set
            {
                if (_Lastattend != value)
                {
                    _Lastattend = value;
                }
            }
        }
    }
}
