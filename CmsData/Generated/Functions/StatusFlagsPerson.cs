using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "StatusFlagsPerson")]
    public partial class StatusFlagsPerson
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Flag;

        private string _Name;

        private string _RoleName;

        private int _TokenID;

        public StatusFlagsPerson()
        {
        }

        [Column(Name = "Flag", Storage = "_Flag", DbType = "nvarchar(200) NOT NULL")]
        public string Flag
        {
            get => _Flag;

            set
            {
                if (_Flag != value)
                {
                    _Flag = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "varchar(100)")]
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

        [Column(Name = "RoleName", Storage = "_RoleName", DbType = "nvarchar(50)")]
        public string RoleName
        {
            get => _RoleName;

            set
            {
                if (_RoleName != value)
                {
                    _RoleName = value;
                }
            }
        }

        [Column(Name = "TokenID", Storage = "_TokenID", DbType = "int NOT NULL")]
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
    }
}
