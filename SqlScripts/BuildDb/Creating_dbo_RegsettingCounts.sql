CREATE VIEW [dbo].[RegsettingCounts]
AS
SELECT 
	OrganizationId
	,OrganizationName

	,RegSettingXml.value('count(/Settings/AskItems/AskDropdown/DropdownItem)', 'int') DropdownItemCount
	,RegSettingXml.value('count(/Settings/AskItems/AskDropdown/DropdownItem[@Fee])', 'int') DropdownItemFeeCount
	,RegSettingXml.value('count(/Settings/AskItems/AskCheckboxes/CheckboxItem)', 'int') CheckboxItemCount
	,RegSettingXml.value('count(/Settings/AskItems/AskCheckboxes/CheckboxItem[@Fee])', 'int') CheckboxItemFeeCount
	,RegSettingXml.value('count(/Settings/AskItems/AskExtraQuestions/Question)', 'int') ExtraQuestionCount
	,RegSettingXml.value('count(/Settings/AskItems/AskText/Question)', 'int') AskTextCount
	,RegSettingXml.value('count(/Settings/AskItems/AskGradeOptions/GradeOption)', 'int') GradeOptionCount
	,RegSettingXml.value('count(/Settings/AskItems/AskHeader)', 'int') HeaderCount
	,RegSettingXml.value('count(/Settings/AskItems/AskInstruction)', 'int') AskInstructionCount
	,RegSettingXml.value('count(/Settings/AskItems/AskMenu/MenuItem)', 'int') MenuItemCount
	,RegSettingXml.value('count(/Settings/AskItems/AskSize/Size)', 'int') AskSizeCount
	,RegSettingXml.value('count(/Settings/AskItems/AskYesNoQuestions/YesNoQuestion)[1]', 'int') YesNoQuestionCount
	,RegSettingXml.value('count(/Settings/NotRequired/*)', 'int') NotRequiredCount
	,RegSettingXml.value('count(/Settings/OrgFees/Fee)', 'int') OrgFeesCount
	,RegSettingXml.value('count(/Settings/AgeGroups/Group)', 'int') AgeGroupsCount
	,RegSettingXml.value('(/Settings/Fees/Fee)[1]', 'money') [Fee]

	,LEN(ISNULL(RegSettingXml.value('(/Settings/Confirmation/Body)[1]', 'varchar(max)'),'')) BodyLen
	,LEN(ISNULL(RegSettingXml.value('(/Settings/SenderEmail/Body)[1]', 'varchar(max)'),'')) SenderBodyLen
	,LEN(ISNULL(RegSettingXml.value('(/Settings/SupportEmail/Body)[1]', 'varchar(max)'),'')) SupportBodyLen
	,LEN(ISNULL(RegSettingXml.value('(/Settings/Reminder/Body)[1]', 'varchar(max)'),'')) ReminderBodyLen

	,LEN(ISNULL(RegSettingXml.value('(/Settings/Instructions/Login)[1]', 'varchar(max)'),'')) InstructionsLoginLen
	,LEN(ISNULL(RegSettingXml.value('(/Settings/Instructions/Options)[1]', 'varchar(max)'),'')) InstructionsOptionsLen
	,LEN(ISNULL(RegSettingXml.value('(/Settings/Instructions/Submit)[1]', 'varchar(max)'),'')) InstructionsSubmitLen
	,LEN(ISNULL(RegSettingXml.value('(/Settings/Instructions/Find)[1]', 'varchar(max)'),'')) InstructionsFindLen
	,LEN(ISNULL(RegSettingXml.value('(/Settings/Instructions/Select)[1]', 'varchar(max)'),'')) InstructionsSelectLen
	,LEN(ISNULL(RegSettingXml.value('(/Settings/Instructions/Sorry)[1]', 'varchar(max)'),'')) InstructionsSorryLen
	,LEN(ISNULL(RegSettingXml.value('(/Settings/Instructions/Special)[1]', 'varchar(max)'),'')) InstructionsSpecialLen
	,LEN(ISNULL(RegSettingXml.value('(/Settings/Instructions/Thanks)[1]', 'varchar(max)'),'')) InstructionsThanksLen
	,LEN(ISNULL(RegSettingXml.value('(/Settings/Instructions/Terms)[1]', 'varchar(max)'),'')) InstructionsTermsLen

FROM dbo.Organizations WHERE RegSettingXml IS NOT NULL





GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
