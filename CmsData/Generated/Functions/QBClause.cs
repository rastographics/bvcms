using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "QBClauses")]
    public partial class QBClause
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _QueryId;

        private int? _TopId;

        private int? _GroupId;

        private string _SavedBy;

        private string _Description;

        private string _Field;

        private string _Comparison;

        private int? _Level;

        public QBClause()
        {
        }

        [Column(Name = "QueryId", Storage = "_QueryId", DbType = "int")]
        public int? QueryId
        {
            get => _QueryId;

            set
            {
                if (_QueryId != value)
                {
                    _QueryId = value;
                }
            }
        }

        [Column(Name = "TopId", Storage = "_TopId", DbType = "int")]
        public int? TopId
        {
            get => _TopId;

            set
            {
                if (_TopId != value)
                {
                    _TopId = value;
                }
            }
        }

        [Column(Name = "GroupId", Storage = "_GroupId", DbType = "int")]
        public int? GroupId
        {
            get => _GroupId;

            set
            {
                if (_GroupId != value)
                {
                    _GroupId = value;
                }
            }
        }

        [Column(Name = "SavedBy", Storage = "_SavedBy", DbType = "varchar(50)")]
        public string SavedBy
        {
            get => _SavedBy;

            set
            {
                if (_SavedBy != value)
                {
                    _SavedBy = value;
                }
            }
        }

        [Column(Name = "DESCRIPTION", Storage = "_Description", DbType = "varchar(150)")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    _Description = value;
                }
            }
        }

        [Column(Name = "Field", Storage = "_Field", DbType = "varchar(32)")]
        public string Field
        {
            get => _Field;

            set
            {
                if (_Field != value)
                {
                    _Field = value;
                }
            }
        }

        [Column(Name = "Comparison", Storage = "_Comparison", DbType = "varchar(20)")]
        public string Comparison
        {
            get => _Comparison;

            set
            {
                if (_Comparison != value)
                {
                    _Comparison = value;
                }
            }
        }

        [Column(Name = "Level", Storage = "_Level", DbType = "int")]
        public int? Level
        {
            get => _Level;

            set
            {
                if (_Level != value)
                {
                    _Level = value;
                }
            }
        }
    }
}
