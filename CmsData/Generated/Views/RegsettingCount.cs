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
	[Table(Name="RegsettingCounts")]
	public partial class RegsettingCount
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private string _OrganizationName;
		
		private int? _DropdownItemCount;
		
		private int? _CheckboxItemCount;
		
		private int? _ExtraQuestionCount;
		
		private int? _AskTextCount;
		
		private int? _GradeOptionCount;
		
		private int? _HeaderCount;
		
		private int? _AskInstructionCount;
		
		private int? _MenuItemCount;
		
		private int? _AskSizeCount;
		
		private int? _YesNoQuestionCount;
		
		private int? _NotRequiredCount;
		
		private int? _OrgFeesCount;
		
		private int? _AgeGroupsCount;
		
		private long? _BodyLen;
		
		private long? _SenderBodyLen;
		
		private long? _SupportBodyLen;
		
		private long? _ReminderBodyLen;
		
		private long? _InstructionsLoginLen;
		
		private long? _InstructionsOptionsLen;
		
		private long? _InstructionsSubmitLen;
		
		private long? _InstructionsFindLen;
		
		private long? _InstructionsSelectLen;
		
		private long? _InstructionsSorryLen;
		
		private long? _InstructionsSpecialLen;
		
		private long? _InstructionsThanksLen;
		
		private long? _InstructionsTermsLen;
		
		
		public RegsettingCount()
		{
		}

		
		
		[Column(Name="OrganizationId", Storage="_OrganizationId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsDbGenerated=true)]
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

		
		[Column(Name="DropdownItemCount", Storage="_DropdownItemCount", DbType="int")]
		public int? DropdownItemCount
		{
			get
			{
				return this._DropdownItemCount;
			}

			set
			{
				if (this._DropdownItemCount != value)
					this._DropdownItemCount = value;
			}

		}

		
		[Column(Name="CheckboxItemCount", Storage="_CheckboxItemCount", DbType="int")]
		public int? CheckboxItemCount
		{
			get
			{
				return this._CheckboxItemCount;
			}

			set
			{
				if (this._CheckboxItemCount != value)
					this._CheckboxItemCount = value;
			}

		}

		
		[Column(Name="ExtraQuestionCount", Storage="_ExtraQuestionCount", DbType="int")]
		public int? ExtraQuestionCount
		{
			get
			{
				return this._ExtraQuestionCount;
			}

			set
			{
				if (this._ExtraQuestionCount != value)
					this._ExtraQuestionCount = value;
			}

		}

		
		[Column(Name="AskTextCount", Storage="_AskTextCount", DbType="int")]
		public int? AskTextCount
		{
			get
			{
				return this._AskTextCount;
			}

			set
			{
				if (this._AskTextCount != value)
					this._AskTextCount = value;
			}

		}

		
		[Column(Name="GradeOptionCount", Storage="_GradeOptionCount", DbType="int")]
		public int? GradeOptionCount
		{
			get
			{
				return this._GradeOptionCount;
			}

			set
			{
				if (this._GradeOptionCount != value)
					this._GradeOptionCount = value;
			}

		}

		
		[Column(Name="HeaderCount", Storage="_HeaderCount", DbType="int")]
		public int? HeaderCount
		{
			get
			{
				return this._HeaderCount;
			}

			set
			{
				if (this._HeaderCount != value)
					this._HeaderCount = value;
			}

		}

		
		[Column(Name="AskInstructionCount", Storage="_AskInstructionCount", DbType="int")]
		public int? AskInstructionCount
		{
			get
			{
				return this._AskInstructionCount;
			}

			set
			{
				if (this._AskInstructionCount != value)
					this._AskInstructionCount = value;
			}

		}

		
		[Column(Name="MenuItemCount", Storage="_MenuItemCount", DbType="int")]
		public int? MenuItemCount
		{
			get
			{
				return this._MenuItemCount;
			}

			set
			{
				if (this._MenuItemCount != value)
					this._MenuItemCount = value;
			}

		}

		
		[Column(Name="AskSizeCount", Storage="_AskSizeCount", DbType="int")]
		public int? AskSizeCount
		{
			get
			{
				return this._AskSizeCount;
			}

			set
			{
				if (this._AskSizeCount != value)
					this._AskSizeCount = value;
			}

		}

		
		[Column(Name="YesNoQuestionCount", Storage="_YesNoQuestionCount", DbType="int")]
		public int? YesNoQuestionCount
		{
			get
			{
				return this._YesNoQuestionCount;
			}

			set
			{
				if (this._YesNoQuestionCount != value)
					this._YesNoQuestionCount = value;
			}

		}

		
		[Column(Name="NotRequiredCount", Storage="_NotRequiredCount", DbType="int")]
		public int? NotRequiredCount
		{
			get
			{
				return this._NotRequiredCount;
			}

			set
			{
				if (this._NotRequiredCount != value)
					this._NotRequiredCount = value;
			}

		}

		
		[Column(Name="OrgFeesCount", Storage="_OrgFeesCount", DbType="int")]
		public int? OrgFeesCount
		{
			get
			{
				return this._OrgFeesCount;
			}

			set
			{
				if (this._OrgFeesCount != value)
					this._OrgFeesCount = value;
			}

		}

		
		[Column(Name="AgeGroupsCount", Storage="_AgeGroupsCount", DbType="int")]
		public int? AgeGroupsCount
		{
			get
			{
				return this._AgeGroupsCount;
			}

			set
			{
				if (this._AgeGroupsCount != value)
					this._AgeGroupsCount = value;
			}

		}

		
		[Column(Name="BodyLen", Storage="_BodyLen", DbType="bigint")]
		public long? BodyLen
		{
			get
			{
				return this._BodyLen;
			}

			set
			{
				if (this._BodyLen != value)
					this._BodyLen = value;
			}

		}

		
		[Column(Name="SenderBodyLen", Storage="_SenderBodyLen", DbType="bigint")]
		public long? SenderBodyLen
		{
			get
			{
				return this._SenderBodyLen;
			}

			set
			{
				if (this._SenderBodyLen != value)
					this._SenderBodyLen = value;
			}

		}

		
		[Column(Name="SupportBodyLen", Storage="_SupportBodyLen", DbType="bigint")]
		public long? SupportBodyLen
		{
			get
			{
				return this._SupportBodyLen;
			}

			set
			{
				if (this._SupportBodyLen != value)
					this._SupportBodyLen = value;
			}

		}

		
		[Column(Name="ReminderBodyLen", Storage="_ReminderBodyLen", DbType="bigint")]
		public long? ReminderBodyLen
		{
			get
			{
				return this._ReminderBodyLen;
			}

			set
			{
				if (this._ReminderBodyLen != value)
					this._ReminderBodyLen = value;
			}

		}

		
		[Column(Name="InstructionsLoginLen", Storage="_InstructionsLoginLen", DbType="bigint")]
		public long? InstructionsLoginLen
		{
			get
			{
				return this._InstructionsLoginLen;
			}

			set
			{
				if (this._InstructionsLoginLen != value)
					this._InstructionsLoginLen = value;
			}

		}

		
		[Column(Name="InstructionsOptionsLen", Storage="_InstructionsOptionsLen", DbType="bigint")]
		public long? InstructionsOptionsLen
		{
			get
			{
				return this._InstructionsOptionsLen;
			}

			set
			{
				if (this._InstructionsOptionsLen != value)
					this._InstructionsOptionsLen = value;
			}

		}

		
		[Column(Name="InstructionsSubmitLen", Storage="_InstructionsSubmitLen", DbType="bigint")]
		public long? InstructionsSubmitLen
		{
			get
			{
				return this._InstructionsSubmitLen;
			}

			set
			{
				if (this._InstructionsSubmitLen != value)
					this._InstructionsSubmitLen = value;
			}

		}

		
		[Column(Name="InstructionsFindLen", Storage="_InstructionsFindLen", DbType="bigint")]
		public long? InstructionsFindLen
		{
			get
			{
				return this._InstructionsFindLen;
			}

			set
			{
				if (this._InstructionsFindLen != value)
					this._InstructionsFindLen = value;
			}

		}

		
		[Column(Name="InstructionsSelectLen", Storage="_InstructionsSelectLen", DbType="bigint")]
		public long? InstructionsSelectLen
		{
			get
			{
				return this._InstructionsSelectLen;
			}

			set
			{
				if (this._InstructionsSelectLen != value)
					this._InstructionsSelectLen = value;
			}

		}

		
		[Column(Name="InstructionsSorryLen", Storage="_InstructionsSorryLen", DbType="bigint")]
		public long? InstructionsSorryLen
		{
			get
			{
				return this._InstructionsSorryLen;
			}

			set
			{
				if (this._InstructionsSorryLen != value)
					this._InstructionsSorryLen = value;
			}

		}

		
		[Column(Name="InstructionsSpecialLen", Storage="_InstructionsSpecialLen", DbType="bigint")]
		public long? InstructionsSpecialLen
		{
			get
			{
				return this._InstructionsSpecialLen;
			}

			set
			{
				if (this._InstructionsSpecialLen != value)
					this._InstructionsSpecialLen = value;
			}

		}

		
		[Column(Name="InstructionsThanksLen", Storage="_InstructionsThanksLen", DbType="bigint")]
		public long? InstructionsThanksLen
		{
			get
			{
				return this._InstructionsThanksLen;
			}

			set
			{
				if (this._InstructionsThanksLen != value)
					this._InstructionsThanksLen = value;
			}

		}

		
		[Column(Name="InstructionsTermsLen", Storage="_InstructionsTermsLen", DbType="bigint")]
		public long? InstructionsTermsLen
		{
			get
			{
				return this._InstructionsTermsLen;
			}

			set
			{
				if (this._InstructionsTermsLen != value)
					this._InstructionsTermsLen = value;
			}

		}

		
    }

}
