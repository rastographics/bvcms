using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "FamilyGiver")]
    public partial class FamilyGiver
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _FamilyId;

        private int _PeopleId;

        private bool? _FamGive;

        private bool? _FamPledge;

        public FamilyGiver()
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

        [Column(Name = "FamGive", Storage = "_FamGive", DbType = "bit")]
        public bool? FamGive
        {
            get => _FamGive;

            set
            {
                if (_FamGive != value)
                {
                    _FamGive = value;
                }
            }
        }

        [Column(Name = "FamPledge", Storage = "_FamPledge", DbType = "bit")]
        public bool? FamPledge
        {
            get => _FamPledge;

            set
            {
                if (_FamPledge != value)
                {
                    _FamPledge = value;
                }
            }
        }
    }
}
