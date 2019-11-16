using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "AllStatusFlags")]
    public partial class AllStatusFlag
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Flag;

        private string _Name;

        private string _Role;

        public AllStatusFlag()
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

        [Column(Name = "Flag", Storage = "_Flag", DbType = "nvarchar(200) NOT NULL")]
        public string Flag
        {
            get => _Flag;

            set
            {
                if (_Flag != value)
                {
                    _Flag = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(4000)")]
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

        [Column(Name = "Role", Storage = "_Role", DbType = "nvarchar(50)")]
        public string Role
        {
            get => _Role;

            set
            {
                if (_Role != value)
                {
                    _Role = value;
                }
            }
        }
    }
}
