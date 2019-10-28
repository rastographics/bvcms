using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "OnlineRegQA")]
    public partial class OnlineRegQA
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private int _PeopleId;

        private string _Type;

        private int? _SetX;

        private string _Question;

        private string _Answer;

        public OnlineRegQA()
        {
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int NOT NULL")]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
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

        [Column(Name = "Question", Storage = "_Question", DbType = "nvarchar(500)")]
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

        [Column(Name = "Answer", Storage = "_Answer", DbType = "nvarchar")]
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
