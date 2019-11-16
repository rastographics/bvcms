using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "StatusFlagList")]
    public partial class StatusFlagList
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Flag;

        private string _Name;

        private string _RoleName;

        private bool? _Visible;

        public StatusFlagList()
        {
        }

        [Column(Name = "Flag", Storage = "_Flag", DbType = "nvarchar(3)")]
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(100)")]
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

        [Column(Name = "Visible", Storage = "_Visible", DbType = "bit")]
        public bool? Visible
        {
            get => _Visible;

            set
            {
                if (_Visible != value)
                {
                    _Visible = value;
                }
            }
        }
    }
}
