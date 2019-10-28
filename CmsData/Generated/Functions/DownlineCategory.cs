using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "DownlineCategories")]
    public partial class DownlineCategory
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Rownum;

        private int? _Id;

        private string _Name;

        private bool? _Mainfellowship;

        private string _Programs;

        private string _Divisions;

        public DownlineCategory()
        {
        }

        [Column(Name = "rownum", Storage = "_Rownum", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
        public int Rownum
        {
            get => _Rownum;

            set
            {
                if (_Rownum != value)
                {
                    _Rownum = value;
                }
            }
        }

        [Column(Name = "id", Storage = "_Id", DbType = "int")]
        public int? Id
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

        [Column(Name = "name", Storage = "_Name", DbType = "varchar(50)")]
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

        [Column(Name = "mainfellowship", Storage = "_Mainfellowship", DbType = "bit")]
        public bool? Mainfellowship
        {
            get => _Mainfellowship;

            set
            {
                if (_Mainfellowship != value)
                {
                    _Mainfellowship = value;
                }
            }
        }

        [Column(Name = "programs", Storage = "_Programs", DbType = "varchar(100)")]
        public string Programs
        {
            get => _Programs;

            set
            {
                if (_Programs != value)
                {
                    _Programs = value;
                }
            }
        }

        [Column(Name = "divisions", Storage = "_Divisions", DbType = "varchar(100)")]
        public string Divisions
        {
            get => _Divisions;

            set
            {
                if (_Divisions != value)
                {
                    _Divisions = value;
                }
            }
        }
    }
}
