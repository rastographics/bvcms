using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData.View
{
	[Table(Name="PastAttendanceCredits")]
	public partial class PastAttendanceCredit
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private bool? _Attended;
		
		private int? _Year;
		
		private int? _Week;
		
		private int? _AttendCreditCode;
		
		private int? _AttendanceTypeId;
		
		
		public PastAttendanceCredit()
		{
		}

		
		
		[Column(Name="Attended", Storage="_Attended", DbType="bit")]
		public bool? Attended
		{
			get
			{
				return this._Attended;
			}

			set
			{
				if (this._Attended != value)
					this._Attended = value;
			}

		}

		
		[Column(Name="Year", Storage="_Year", DbType="int")]
		public int? Year
		{
			get
			{
				return this._Year;
			}

			set
			{
				if (this._Year != value)
					this._Year = value;
			}

		}

		
		[Column(Name="Week", Storage="_Week", DbType="int")]
		public int? Week
		{
			get
			{
				return this._Week;
			}

			set
			{
				if (this._Week != value)
					this._Week = value;
			}

		}

		
		[Column(Name="AttendCreditCode", Storage="_AttendCreditCode", DbType="int")]
		public int? AttendCreditCode
		{
			get
			{
				return this._AttendCreditCode;
			}

			set
			{
				if (this._AttendCreditCode != value)
					this._AttendCreditCode = value;
			}

		}

		
		[Column(Name="AttendanceTypeId", Storage="_AttendanceTypeId", DbType="int")]
		public int? AttendanceTypeId
		{
			get
			{
				return this._AttendanceTypeId;
			}

			set
			{
				if (this._AttendanceTypeId != value)
					this._AttendanceTypeId = value;
			}

		}

		
    }

}
