using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "Split")]
    public partial class Split
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _TokenID;

        private string _ValueX;

        public Split()
        {
        }

        [Column(Name = "TokenID", Storage = "_TokenID", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
        public int TokenID
        {
            get => _TokenID;

            set
            {
                if (_TokenID != value)
                {
                    _TokenID = value;
                }
            }
        }

        [Column(Name = "Value", Storage = "_ValueX", DbType = "nvarchar(4000)")]
        public string ValueX
        {
            get => _ValueX;

            set
            {
                if (_ValueX != value)
                {
                    _ValueX = value;
                }
            }
        }
    }
}
