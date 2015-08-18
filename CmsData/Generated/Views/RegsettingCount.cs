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
		
		private int? _OrgFeesCount;
		
		private int? _AgeGroupsCount;
		
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

		
    }

}
