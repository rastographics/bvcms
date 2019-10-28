using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PastAttendanceCredits")]
    public partial class PastAttendanceCredit
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private bool? _Attended;

        private int? _Year;

        private int? _Week;

        private int? _AttendCreditCode;

        private int? _AttendanceTypeId;

        public PastAttendanceCredit()
        {
        }

        [Column(Name = "Attended", Storage = "_Attended", DbType = "bit")]
        public bool? Attended
        {
            get => _Attended;

            set
            {
                if (_Attended != value)
                {
                    _Attended = value;
                }
            }
        }

        [Column(Name = "Year", Storage = "_Year", DbType = "int")]
        public int? Year
        {
            get => _Year;

            set
            {
                if (_Year != value)
                {
                    _Year = value;
                }
            }
        }

        [Column(Name = "Week", Storage = "_Week", DbType = "int")]
        public int? Week
        {
            get => _Week;

            set
            {
                if (_Week != value)
                {
                    _Week = value;
                }
            }
        }

        [Column(Name = "AttendCreditCode", Storage = "_AttendCreditCode", DbType = "int")]
        public int? AttendCreditCode
        {
            get => _AttendCreditCode;

            set
            {
                if (_AttendCreditCode != value)
                {
                    _AttendCreditCode = value;
                }
            }
        }

        [Column(Name = "AttendanceTypeId", Storage = "_AttendanceTypeId", DbType = "int")]
        public int? AttendanceTypeId
        {
            get => _AttendanceTypeId;

            set
            {
                if (_AttendanceTypeId != value)
                {
                    _AttendanceTypeId = value;
                }
            }
        }
    }
}
