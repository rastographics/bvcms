using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "VerseSummaryForCategory2")]
    public partial class VerseSummaryForCategory2
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Id;

        private string _Reference;

        private string _ShortText;

        private int? _CategoryCount;

        private int? _Book;

        private int? _Chapter;

        private int? _VerseNum;

        private bool? _InCategory;

        public VerseSummaryForCategory2()
        {
        }

        [Column(Name = "id", Storage = "_Id", DbType = "int NOT NULL")]
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

        [Column(Name = "Reference", Storage = "_Reference", DbType = "nvarchar(203)")]
        public string Reference
        {
            get => _Reference;

            set
            {
                if (_Reference != value)
                {
                    _Reference = value;
                }
            }
        }

        [Column(Name = "ShortText", Storage = "_ShortText", DbType = "nvarchar")]
        public string ShortText
        {
            get => _ShortText;

            set
            {
                if (_ShortText != value)
                {
                    _ShortText = value;
                }
            }
        }

        [Column(Name = "CategoryCount", Storage = "_CategoryCount", DbType = "int")]
        public int? CategoryCount
        {
            get => _CategoryCount;

            set
            {
                if (_CategoryCount != value)
                {
                    _CategoryCount = value;
                }
            }
        }

        [Column(Name = "Book", Storage = "_Book", DbType = "int")]
        public int? Book
        {
            get => _Book;

            set
            {
                if (_Book != value)
                {
                    _Book = value;
                }
            }
        }

        [Column(Name = "Chapter", Storage = "_Chapter", DbType = "int")]
        public int? Chapter
        {
            get => _Chapter;

            set
            {
                if (_Chapter != value)
                {
                    _Chapter = value;
                }
            }
        }

        [Column(Name = "VerseNum", Storage = "_VerseNum", DbType = "int")]
        public int? VerseNum
        {
            get => _VerseNum;

            set
            {
                if (_VerseNum != value)
                {
                    _VerseNum = value;
                }
            }
        }

        [Column(Name = "InCategory", Storage = "_InCategory", DbType = "bit")]
        public bool? InCategory
        {
            get => _InCategory;

            set
            {
                if (_InCategory != value)
                {
                    _InCategory = value;
                }
            }
        }
    }
}
