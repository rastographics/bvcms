using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "FirstName2")]
    public partial class FirstName2
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _FirstName;

        private int _GenderId;

        private string _Ca;

        private int? _Expr1;

        public FirstName2()
        {
        }

        [Column(Name = "FirstName", Storage = "_FirstName", DbType = "nvarchar(25)")]
        public string FirstName
        {
            get => _FirstName;

            set
            {
                if (_FirstName != value)
                {
                    _FirstName = value;
                }
            }
        }

        [Column(Name = "GenderId", Storage = "_GenderId", DbType = "int NOT NULL")]
        public int GenderId
        {
            get => _GenderId;

            set
            {
                if (_GenderId != value)
                {
                    _GenderId = value;
                }
            }
        }

        [Column(Name = "CA", Storage = "_Ca", DbType = "varchar(1) NOT NULL")]
        public string Ca
        {
            get => _Ca;

            set
            {
                if (_Ca != value)
                {
                    _Ca = value;
                }
            }
        }

        [Column(Name = "Expr1", Storage = "_Expr1", DbType = "int")]
        public int? Expr1
        {
            get => _Expr1;

            set
            {
                if (_Expr1 != value)
                {
                    _Expr1 = value;
                }
            }
        }
    }
}
