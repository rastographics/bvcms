using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PrevAddress")]
    public partial class PrevAddress
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _PrevAddr;

        private string _PrevAddr2;

        private string _PrevCity;

        private string _PrevState;

        private string _PrevZip;

        public PrevAddress()
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

        [Column(Name = "PrevAddr", Storage = "_PrevAddr", DbType = "nvarchar")]
        public string PrevAddr
        {
            get => _PrevAddr;

            set
            {
                if (_PrevAddr != value)
                {
                    _PrevAddr = value;
                }
            }
        }

        [Column(Name = "PrevAddr2", Storage = "_PrevAddr2", DbType = "nvarchar")]
        public string PrevAddr2
        {
            get => _PrevAddr2;

            set
            {
                if (_PrevAddr2 != value)
                {
                    _PrevAddr2 = value;
                }
            }
        }

        [Column(Name = "PrevCity", Storage = "_PrevCity", DbType = "nvarchar")]
        public string PrevCity
        {
            get => _PrevCity;

            set
            {
                if (_PrevCity != value)
                {
                    _PrevCity = value;
                }
            }
        }

        [Column(Name = "PrevState", Storage = "_PrevState", DbType = "nvarchar")]
        public string PrevState
        {
            get => _PrevState;

            set
            {
                if (_PrevState != value)
                {
                    _PrevState = value;
                }
            }
        }

        [Column(Name = "PrevZip", Storage = "_PrevZip", DbType = "nvarchar")]
        public string PrevZip
        {
            get => _PrevZip;

            set
            {
                if (_PrevZip != value)
                {
                    _PrevZip = value;
                }
            }
        }
    }
}
