using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "TPStats")]
    public partial class TPStat
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Nrecs;

        private int? _Nmembers;

        private int? _Olgive;

        private int? _Give;

        private int _Wag;

        private int? _Checkin;

        private DateTime _Lastdt;

        private string _Lastp;

        private int? _Nlogs;

        private int? _Nlogs30;

        private int? _Norgs;

        private DateTime? _Created;

        private DateTime? _Converted;

        private int? _Nusers;

        private int? _Nmydata;

        private int? _Nadmins;

        private int? _Reg;

        private DateTime? _Firstactive;

        private int? _Notam;

        private int? _Campuses;

        public TPStat()
        {
        }

        [Column(Name = "nrecs", Storage = "_Nrecs", DbType = "int")]
        public int? Nrecs
        {
            get => _Nrecs;

            set
            {
                if (_Nrecs != value)
                {
                    _Nrecs = value;
                }
            }
        }

        [Column(Name = "nmembers", Storage = "_Nmembers", DbType = "int")]
        public int? Nmembers
        {
            get => _Nmembers;

            set
            {
                if (_Nmembers != value)
                {
                    _Nmembers = value;
                }
            }
        }

        [Column(Name = "olgive", Storage = "_Olgive", DbType = "int")]
        public int? Olgive
        {
            get => _Olgive;

            set
            {
                if (_Olgive != value)
                {
                    _Olgive = value;
                }
            }
        }

        [Column(Name = "give", Storage = "_Give", DbType = "int")]
        public int? Give
        {
            get => _Give;

            set
            {
                if (_Give != value)
                {
                    _Give = value;
                }
            }
        }

        [Column(Name = "wag", Storage = "_Wag", DbType = "int NOT NULL")]
        public int Wag
        {
            get => _Wag;

            set
            {
                if (_Wag != value)
                {
                    _Wag = value;
                }
            }
        }

        [Column(Name = "checkin", Storage = "_Checkin", DbType = "int")]
        public int? Checkin
        {
            get => _Checkin;

            set
            {
                if (_Checkin != value)
                {
                    _Checkin = value;
                }
            }
        }

        [Column(Name = "lastdt", Storage = "_Lastdt", DbType = "datetime NOT NULL")]
        public DateTime Lastdt
        {
            get => _Lastdt;

            set
            {
                if (_Lastdt != value)
                {
                    _Lastdt = value;
                }
            }
        }

        [Column(Name = "lastp", Storage = "_Lastp", DbType = "nvarchar(50) NOT NULL")]
        public string Lastp
        {
            get => _Lastp;

            set
            {
                if (_Lastp != value)
                {
                    _Lastp = value;
                }
            }
        }

        [Column(Name = "nlogs", Storage = "_Nlogs", DbType = "int")]
        public int? Nlogs
        {
            get => _Nlogs;

            set
            {
                if (_Nlogs != value)
                {
                    _Nlogs = value;
                }
            }
        }

        [Column(Name = "nlogs30", Storage = "_Nlogs30", DbType = "int")]
        public int? Nlogs30
        {
            get => _Nlogs30;

            set
            {
                if (_Nlogs30 != value)
                {
                    _Nlogs30 = value;
                }
            }
        }

        [Column(Name = "norgs", Storage = "_Norgs", DbType = "int")]
        public int? Norgs
        {
            get => _Norgs;

            set
            {
                if (_Norgs != value)
                {
                    _Norgs = value;
                }
            }
        }

        [Column(Name = "created", Storage = "_Created", DbType = "datetime")]
        public DateTime? Created
        {
            get => _Created;

            set
            {
                if (_Created != value)
                {
                    _Created = value;
                }
            }
        }

        [Column(Name = "converted", Storage = "_Converted", DbType = "datetime")]
        public DateTime? Converted
        {
            get => _Converted;

            set
            {
                if (_Converted != value)
                {
                    _Converted = value;
                }
            }
        }

        [Column(Name = "nusers", Storage = "_Nusers", DbType = "int")]
        public int? Nusers
        {
            get => _Nusers;

            set
            {
                if (_Nusers != value)
                {
                    _Nusers = value;
                }
            }
        }

        [Column(Name = "nmydata", Storage = "_Nmydata", DbType = "int")]
        public int? Nmydata
        {
            get => _Nmydata;

            set
            {
                if (_Nmydata != value)
                {
                    _Nmydata = value;
                }
            }
        }

        [Column(Name = "nadmins", Storage = "_Nadmins", DbType = "int")]
        public int? Nadmins
        {
            get => _Nadmins;

            set
            {
                if (_Nadmins != value)
                {
                    _Nadmins = value;
                }
            }
        }

        [Column(Name = "reg", Storage = "_Reg", DbType = "int")]
        public int? Reg
        {
            get => _Reg;

            set
            {
                if (_Reg != value)
                {
                    _Reg = value;
                }
            }
        }

        [Column(Name = "firstactive", Storage = "_Firstactive", DbType = "datetime")]
        public DateTime? Firstactive
        {
            get => _Firstactive;

            set
            {
                if (_Firstactive != value)
                {
                    _Firstactive = value;
                }
            }
        }

        [Column(Name = "notam", Storage = "_Notam", DbType = "int")]
        public int? Notam
        {
            get => _Notam;

            set
            {
                if (_Notam != value)
                {
                    _Notam = value;
                }
            }
        }

        [Column(Name = "campuses", Storage = "_Campuses", DbType = "int")]
        public int? Campuses
        {
            get => _Campuses;

            set
            {
                if (_Campuses != value)
                {
                    _Campuses = value;
                }
            }
        }
    }
}
