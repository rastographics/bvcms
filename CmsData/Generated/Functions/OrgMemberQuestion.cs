using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OrgMemberQuestions")]
    public partial class OrgMemberQuestion
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private long? _Row;

        private string _Type;

        private int? _SetX;

        private string _Question;

        private string _Answer;

        public OrgMemberQuestion()
        {
        }

        [Column(Name = "row", Storage = "_Row", DbType = "bigint")]
        public long? Row
        {
            get => _Row;

            set
            {
                if (_Row != value)
                {
                    _Row = value;
                }
            }
        }

        [Column(Name = "type", Storage = "_Type", DbType = "varchar(8) NOT NULL")]
        public string Type
        {
            get => _Type;

            set
            {
                if (_Type != value)
                {
                    _Type = value;
                }
            }
        }

        [Column(Name = "set", Storage = "_SetX", DbType = "int")]
        public int? SetX
        {
            get => _SetX;

            set
            {
                if (_SetX != value)
                {
                    _SetX = value;
                }
            }
        }

        [Column(Name = "Question", Storage = "_Question", DbType = "varchar(500)")]
        public string Question
        {
            get => _Question;

            set
            {
                if (_Question != value)
                {
                    _Question = value;
                }
            }
        }

        [Column(Name = "Answer", Storage = "_Answer", DbType = "varchar")]
        public string Answer
        {
            get => _Answer;

            set
            {
                if (_Answer != value)
                {
                    _Answer = value;
                }
            }
        }
    }
}
