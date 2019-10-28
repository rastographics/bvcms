using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "MemberData")]
    public partial class MemberDatum
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _First;

        private string _Last;

        private int? _Age;

        private string _Marital;

        private DateTime? _DecisionDt;

        private DateTime? _JoinDt;

        private string _Decision;

        private string _Baptism;

        public MemberDatum()
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

        [Column(Name = "First", Storage = "_First", DbType = "nvarchar(25)")]
        public string First
        {
            get => _First;

            set
            {
                if (_First != value)
                {
                    _First = value;
                }
            }
        }

        [Column(Name = "Last", Storage = "_Last", DbType = "nvarchar(100) NOT NULL")]
        public string Last
        {
            get => _Last;

            set
            {
                if (_Last != value)
                {
                    _Last = value;
                }
            }
        }

        [Column(Name = "Age", Storage = "_Age", DbType = "int")]
        public int? Age
        {
            get => _Age;

            set
            {
                if (_Age != value)
                {
                    _Age = value;
                }
            }
        }

        [Column(Name = "Marital", Storage = "_Marital", DbType = "nvarchar(100)")]
        public string Marital
        {
            get => _Marital;

            set
            {
                if (_Marital != value)
                {
                    _Marital = value;
                }
            }
        }

        [Column(Name = "DecisionDt", Storage = "_DecisionDt", DbType = "datetime")]
        public DateTime? DecisionDt
        {
            get => _DecisionDt;

            set
            {
                if (_DecisionDt != value)
                {
                    _DecisionDt = value;
                }
            }
        }

        [Column(Name = "JoinDt", Storage = "_JoinDt", DbType = "datetime")]
        public DateTime? JoinDt
        {
            get => _JoinDt;

            set
            {
                if (_JoinDt != value)
                {
                    _JoinDt = value;
                }
            }
        }

        [Column(Name = "Decision", Storage = "_Decision", DbType = "nvarchar(20)")]
        public string Decision
        {
            get => _Decision;

            set
            {
                if (_Decision != value)
                {
                    _Decision = value;
                }
            }
        }

        [Column(Name = "Baptism", Storage = "_Baptism", DbType = "nvarchar(100)")]
        public string Baptism
        {
            get => _Baptism;

            set
            {
                if (_Baptism != value)
                {
                    _Baptism = value;
                }
            }
        }
    }
}
