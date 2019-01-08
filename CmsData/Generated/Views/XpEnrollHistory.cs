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
	[Table(Name="XpEnrollHistory")]
	public partial class XpEnrollHistory
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _TransactionId;
		
		private int _OrganizationId;
		
		private int _PeopleId;
		
		private DateTime _TransactionDate;
		
		private string _OrganizationName;
		
		private int _MemberTypeId;
		
		private string _TransactionType;
		
		private int? _EnrollmentTransactionId;
		
		
		public XpEnrollHistory()
		{
		}

		
		
		[Column(Name="TransactionId", Storage="_TransactionId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsDbGenerated=true)]
		public int TransactionId
		{
			get
			{
				return this._TransactionId;
			}

			set
			{
				if (this._TransactionId != value)
					this._TransactionId = value;
			}

		}

		
		[Column(Name="OrganizationId", Storage="_OrganizationId", DbType="int NOT NULL")]
		public int OrganizationId
		{
			get
			{
				return this._OrganizationId;
			}

			set
			{
				if (this._OrganizationId != value)
					this._OrganizationId = value;
			}

		}

		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int NOT NULL")]
		public int PeopleId
		{
			get
			{
				return this._PeopleId;
			}

			set
			{
				if (this._PeopleId != value)
					this._PeopleId = value;
			}

		}

		
		[Column(Name="TransactionDate", Storage="_TransactionDate", DbType="datetime NOT NULL")]
		public DateTime TransactionDate
		{
			get
			{
				return this._TransactionDate;
			}

			set
			{
				if (this._TransactionDate != value)
					this._TransactionDate = value;
			}

		}

		
		[Column(Name="OrganizationName", Storage="_OrganizationName", DbType="nvarchar(100) NOT NULL")]
		public string OrganizationName
		{
			get
			{
				return this._OrganizationName;
			}

			set
			{
				if (this._OrganizationName != value)
					this._OrganizationName = value;
			}

		}

		
		[Column(Name="MemberTypeId", Storage="_MemberTypeId", DbType="int NOT NULL")]
		public int MemberTypeId
		{
			get
			{
				return this._MemberTypeId;
			}

			set
			{
				if (this._MemberTypeId != value)
					this._MemberTypeId = value;
			}

		}

		
		[Column(Name="TransactionType", Storage="_TransactionType", DbType="varchar(4)")]
		public string TransactionType
		{
			get
			{
				return this._TransactionType;
			}

			set
			{
				if (this._TransactionType != value)
					this._TransactionType = value;
			}

		}

		
		[Column(Name="EnrollmentTransactionId", Storage="_EnrollmentTransactionId", DbType="int")]
		public int? EnrollmentTransactionId
		{
			get
			{
				return this._EnrollmentTransactionId;
			}

			set
			{
				if (this._EnrollmentTransactionId != value)
					this._EnrollmentTransactionId = value;
			}

		}

		
    }

}
