using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PotentialSubstitutes")]
    public partial class PotentialSubstitute
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Name2;

        private string _EmailAddress;

        private string _SameSchedule;

        private string _Committed;

        private string _Groups;

        public PotentialSubstitute()
        {
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

        [Column(Name = "Name2", Storage = "_Name2", DbType = "nvarchar(139)")]
        public string Name2
        {
            get => _Name2;

            set
            {
                if (_Name2 != value)
                {
                    _Name2 = value;
                }
            }
        }

        [Column(Name = "EmailAddress", Storage = "_EmailAddress", DbType = "nvarchar(150)")]
        public string EmailAddress
        {
            get => _EmailAddress;

            set
            {
                if (_EmailAddress != value)
                {
                    _EmailAddress = value;
                }
            }
        }

        [Column(Name = "SameSchedule", Storage = "_SameSchedule", DbType = "varchar(13)")]
        public string SameSchedule
        {
            get => _SameSchedule;

            set
            {
                if (_SameSchedule != value)
                {
                    _SameSchedule = value;
                }
            }
        }

        [Column(Name = "Committed", Storage = "_Committed", DbType = "varchar(9)")]
        public string Committed
        {
            get => _Committed;

            set
            {
                if (_Committed != value)
                {
                    _Committed = value;
                }
            }
        }

        [Column(Name = "Groups", Storage = "_Groups", DbType = "nvarchar")]
        public string Groups
        {
            get => _Groups;

            set
            {
                if (_Groups != value)
                {
                    _Groups = value;
                }
            }
        }
    }
}
