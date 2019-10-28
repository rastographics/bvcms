using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpDivision")]
    public partial class XpDivision
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Id;

        private string _Name;

        private int? _ProgId;

        public XpDivision()
        {
        }

        [Column(Name = "Id", Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(50)")]
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

        [Column(Name = "ProgId", Storage = "_ProgId", DbType = "int")]
        public int? ProgId
        {
            get => _ProgId;

            set
            {
                if (_ProgId != value)
                {
                    _ProgId = value;
                }
            }
        }
    }
}
