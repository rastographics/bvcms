using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "FailedEmails")]
    public partial class FailedEmail
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private DateTime? _Time;

        private int _Id;

        private int _PeopleId;

        private string _Fail;

        public FailedEmail()
        {
        }

        [Column(Name = "time", Storage = "_Time", DbType = "datetime")]
        public DateTime? Time
        {
            get => _Time;

            set
            {
                if (_Time != value)
                {
                    _Time = value;
                }
            }
        }

        [Column(Name = "Id", Storage = "_Id", DbType = "int NOT NULL")]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    _Id = value;
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

        [Column(Name = "Fail", Storage = "_Fail", DbType = "nvarchar(20)")]
        public string Fail
        {
            get => _Fail;

            set
            {
                if (_Fail != value)
                {
                    _Fail = value;
                }
            }
        }
    }
}
