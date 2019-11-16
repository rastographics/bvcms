using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RegsettingCounts")]
    public partial class RegsettingCount
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _OrganizationId;

        private string _OrganizationName;

        private int? _DropdownItemCount;

        private int? _DropdownItemFeeCount;

        private int? _CheckboxItemCount;

        private int? _CheckboxItemFeeCount;

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

        private decimal? _Fee;

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

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
                }
            }
        }

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "nvarchar(100) NOT NULL")]
        public string OrganizationName
        {
            get => _OrganizationName;

            set
            {
                if (_OrganizationName != value)
                {
                    _OrganizationName = value;
                }
            }
        }

        [Column(Name = "DropdownItemCount", Storage = "_DropdownItemCount", DbType = "int")]
        public int? DropdownItemCount
        {
            get => _DropdownItemCount;

            set
            {
                if (_DropdownItemCount != value)
                {
                    _DropdownItemCount = value;
                }
            }
        }

        [Column(Name = "DropdownItemFeeCount", Storage = "_DropdownItemFeeCount", DbType = "int")]
        public int? DropdownItemFeeCount
        {
            get => _DropdownItemFeeCount;

            set
            {
                if (_DropdownItemFeeCount != value)
                {
                    _DropdownItemFeeCount = value;
                }
            }
        }

        [Column(Name = "CheckboxItemCount", Storage = "_CheckboxItemCount", DbType = "int")]
        public int? CheckboxItemCount
        {
            get => _CheckboxItemCount;

            set
            {
                if (_CheckboxItemCount != value)
                {
                    _CheckboxItemCount = value;
                }
            }
        }

        [Column(Name = "CheckboxItemFeeCount", Storage = "_CheckboxItemFeeCount", DbType = "int")]
        public int? CheckboxItemFeeCount
        {
            get => _CheckboxItemFeeCount;

            set
            {
                if (_CheckboxItemFeeCount != value)
                {
                    _CheckboxItemFeeCount = value;
                }
            }
        }

        [Column(Name = "ExtraQuestionCount", Storage = "_ExtraQuestionCount", DbType = "int")]
        public int? ExtraQuestionCount
        {
            get => _ExtraQuestionCount;

            set
            {
                if (_ExtraQuestionCount != value)
                {
                    _ExtraQuestionCount = value;
                }
            }
        }

        [Column(Name = "AskTextCount", Storage = "_AskTextCount", DbType = "int")]
        public int? AskTextCount
        {
            get => _AskTextCount;

            set
            {
                if (_AskTextCount != value)
                {
                    _AskTextCount = value;
                }
            }
        }

        [Column(Name = "GradeOptionCount", Storage = "_GradeOptionCount", DbType = "int")]
        public int? GradeOptionCount
        {
            get => _GradeOptionCount;

            set
            {
                if (_GradeOptionCount != value)
                {
                    _GradeOptionCount = value;
                }
            }
        }

        [Column(Name = "HeaderCount", Storage = "_HeaderCount", DbType = "int")]
        public int? HeaderCount
        {
            get => _HeaderCount;

            set
            {
                if (_HeaderCount != value)
                {
                    _HeaderCount = value;
                }
            }
        }

        [Column(Name = "AskInstructionCount", Storage = "_AskInstructionCount", DbType = "int")]
        public int? AskInstructionCount
        {
            get => _AskInstructionCount;

            set
            {
                if (_AskInstructionCount != value)
                {
                    _AskInstructionCount = value;
                }
            }
        }

        [Column(Name = "MenuItemCount", Storage = "_MenuItemCount", DbType = "int")]
        public int? MenuItemCount
        {
            get => _MenuItemCount;

            set
            {
                if (_MenuItemCount != value)
                {
                    _MenuItemCount = value;
                }
            }
        }

        [Column(Name = "AskSizeCount", Storage = "_AskSizeCount", DbType = "int")]
        public int? AskSizeCount
        {
            get => _AskSizeCount;

            set
            {
                if (_AskSizeCount != value)
                {
                    _AskSizeCount = value;
                }
            }
        }

        [Column(Name = "YesNoQuestionCount", Storage = "_YesNoQuestionCount", DbType = "int")]
        public int? YesNoQuestionCount
        {
            get => _YesNoQuestionCount;

            set
            {
                if (_YesNoQuestionCount != value)
                {
                    _YesNoQuestionCount = value;
                }
            }
        }

        [Column(Name = "NotRequiredCount", Storage = "_NotRequiredCount", DbType = "int")]
        public int? NotRequiredCount
        {
            get => _NotRequiredCount;

            set
            {
                if (_NotRequiredCount != value)
                {
                    _NotRequiredCount = value;
                }
            }
        }

        [Column(Name = "OrgFeesCount", Storage = "_OrgFeesCount", DbType = "int")]
        public int? OrgFeesCount
        {
            get => _OrgFeesCount;

            set
            {
                if (_OrgFeesCount != value)
                {
                    _OrgFeesCount = value;
                }
            }
        }

        [Column(Name = "AgeGroupsCount", Storage = "_AgeGroupsCount", DbType = "int")]
        public int? AgeGroupsCount
        {
            get => _AgeGroupsCount;

            set
            {
                if (_AgeGroupsCount != value)
                {
                    _AgeGroupsCount = value;
                }
            }
        }

        [Column(Name = "Fee", Storage = "_Fee", DbType = "money")]
        public decimal? Fee
        {
            get => _Fee;

            set
            {
                if (_Fee != value)
                {
                    _Fee = value;
                }
            }
        }

        [Column(Name = "BodyLen", Storage = "_BodyLen", DbType = "bigint")]
        public long? BodyLen
        {
            get => _BodyLen;

            set
            {
                if (_BodyLen != value)
                {
                    _BodyLen = value;
                }
            }
        }

        [Column(Name = "SenderBodyLen", Storage = "_SenderBodyLen", DbType = "bigint")]
        public long? SenderBodyLen
        {
            get => _SenderBodyLen;

            set
            {
                if (_SenderBodyLen != value)
                {
                    _SenderBodyLen = value;
                }
            }
        }

        [Column(Name = "SupportBodyLen", Storage = "_SupportBodyLen", DbType = "bigint")]
        public long? SupportBodyLen
        {
            get => _SupportBodyLen;

            set
            {
                if (_SupportBodyLen != value)
                {
                    _SupportBodyLen = value;
                }
            }
        }

        [Column(Name = "ReminderBodyLen", Storage = "_ReminderBodyLen", DbType = "bigint")]
        public long? ReminderBodyLen
        {
            get => _ReminderBodyLen;

            set
            {
                if (_ReminderBodyLen != value)
                {
                    _ReminderBodyLen = value;
                }
            }
        }

        [Column(Name = "InstructionsLoginLen", Storage = "_InstructionsLoginLen", DbType = "bigint")]
        public long? InstructionsLoginLen
        {
            get => _InstructionsLoginLen;

            set
            {
                if (_InstructionsLoginLen != value)
                {
                    _InstructionsLoginLen = value;
                }
            }
        }

        [Column(Name = "InstructionsOptionsLen", Storage = "_InstructionsOptionsLen", DbType = "bigint")]
        public long? InstructionsOptionsLen
        {
            get => _InstructionsOptionsLen;

            set
            {
                if (_InstructionsOptionsLen != value)
                {
                    _InstructionsOptionsLen = value;
                }
            }
        }

        [Column(Name = "InstructionsSubmitLen", Storage = "_InstructionsSubmitLen", DbType = "bigint")]
        public long? InstructionsSubmitLen
        {
            get => _InstructionsSubmitLen;

            set
            {
                if (_InstructionsSubmitLen != value)
                {
                    _InstructionsSubmitLen = value;
                }
            }
        }

        [Column(Name = "InstructionsFindLen", Storage = "_InstructionsFindLen", DbType = "bigint")]
        public long? InstructionsFindLen
        {
            get => _InstructionsFindLen;

            set
            {
                if (_InstructionsFindLen != value)
                {
                    _InstructionsFindLen = value;
                }
            }
        }

        [Column(Name = "InstructionsSelectLen", Storage = "_InstructionsSelectLen", DbType = "bigint")]
        public long? InstructionsSelectLen
        {
            get => _InstructionsSelectLen;

            set
            {
                if (_InstructionsSelectLen != value)
                {
                    _InstructionsSelectLen = value;
                }
            }
        }

        [Column(Name = "InstructionsSorryLen", Storage = "_InstructionsSorryLen", DbType = "bigint")]
        public long? InstructionsSorryLen
        {
            get => _InstructionsSorryLen;

            set
            {
                if (_InstructionsSorryLen != value)
                {
                    _InstructionsSorryLen = value;
                }
            }
        }

        [Column(Name = "InstructionsSpecialLen", Storage = "_InstructionsSpecialLen", DbType = "bigint")]
        public long? InstructionsSpecialLen
        {
            get => _InstructionsSpecialLen;

            set
            {
                if (_InstructionsSpecialLen != value)
                {
                    _InstructionsSpecialLen = value;
                }
            }
        }

        [Column(Name = "InstructionsThanksLen", Storage = "_InstructionsThanksLen", DbType = "bigint")]
        public long? InstructionsThanksLen
        {
            get => _InstructionsThanksLen;

            set
            {
                if (_InstructionsThanksLen != value)
                {
                    _InstructionsThanksLen = value;
                }
            }
        }

        [Column(Name = "InstructionsTermsLen", Storage = "_InstructionsTermsLen", DbType = "bigint")]
        public long? InstructionsTermsLen
        {
            get => _InstructionsTermsLen;

            set
            {
                if (_InstructionsTermsLen != value)
                {
                    _InstructionsTermsLen = value;
                }
            }
        }
    }
}
