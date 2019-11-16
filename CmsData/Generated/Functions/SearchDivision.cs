using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "SearchDivisions")]
    public partial class SearchDivision
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _DivId;

        private string _Division;

        private string _Program;

        private string _Programs;

        private bool? _IsChecked;

        private bool? _IsMain;

        public SearchDivision()
        {
        }

        [Column(Name = "DivId", Storage = "_DivId", DbType = "int NOT NULL")]
        public int DivId
        {
            get => _DivId;

            set
            {
                if (_DivId != value)
                {
                    _DivId = value;
                }
            }
        }

        [Column(Name = "Division", Storage = "_Division", DbType = "nvarchar(50)")]
        public string Division
        {
            get => _Division;

            set
            {
                if (_Division != value)
                {
                    _Division = value;
                }
            }
        }

        [Column(Name = "Program", Storage = "_Program", DbType = "nvarchar(50)")]
        public string Program
        {
            get => _Program;

            set
            {
                if (_Program != value)
                {
                    _Program = value;
                }
            }
        }

        [Column(Name = "Programs", Storage = "_Programs", DbType = "nvarchar")]
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

        [Column(Name = "IsChecked", Storage = "_IsChecked", DbType = "bit")]
        public bool? IsChecked
        {
            get => _IsChecked;

            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                }
            }
        }

        [Column(Name = "IsMain", Storage = "_IsMain", DbType = "bit")]
        public bool? IsMain
        {
            get => _IsMain;

            set
            {
                if (_IsMain != value)
                {
                    _IsMain = value;
                }
            }
        }
    }
}
