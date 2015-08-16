CREATE VIEW [dbo].[RegsettingCounts]
AS
SELECT 
	OrganizationId

	,RegSettingXml.value('count(/Settings/OrgFees/Fee)', 'int') OrgFeesCount
	,RegSettingXml.value('count(/Settings/AgeGroups/Group)', 'int') AgeGroupsCount

	,RegSettingXml.value('count(/Settings/AskItems/AskDropdown/DropdownItem)', 'int') DropdownItemCount
	,RegSettingXml.value('count(/Settings/AskItems/AskCheckboxes/CheckboxItem)', 'int') CheckboxItemCount
	,RegSettingXml.value('count(/Settings/AskItems/AskExtraQuestions/Question)', 'int') ExtraQuestionCount
	,RegSettingXml.value('count(/Settings/AskItems/AskText/Question)', 'int') AskTextCount
	,RegSettingXml.value('count(/Settings/AskItems/AskGradeOptions/GradeOption)', 'int') GradeOptionCount
	,RegSettingXml.value('count(/Settings/AskItems/AskHeader)', 'int') HeaderCount
	,RegSettingXml.value('count(/Settings/AskItems/AskInstruction)', 'int') AskInstructionCount
	,RegSettingXml.value('count(/Settings/AskItems/AskMenu/MenuItem)', 'int') MenuItemCount
	,RegSettingXml.value('count(/Settings/AskItems/AskSize/Size)', 'int') AskSizeCount
	,RegSettingXml.value('count(/Settings/AskItems/AskYesNoQuestions/YesNoQuestion)[1]', 'int') YesNoQuestionCount
	,RegSettingXml.value('count(/Settings/NotRequired)', 'int') NotRequiredCount

FROM dbo.Organizations WHERE RegSettingXml IS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
