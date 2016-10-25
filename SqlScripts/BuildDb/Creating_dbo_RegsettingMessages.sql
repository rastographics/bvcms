CREATE VIEW [dbo].[RegsettingMessages]
AS
SELECT 
	OrganizationId
	,OrganizationName
	,RegSettingXml.value('(/Settings/Confirmation/Subject)[1]', 'varchar(100)') [Subject]
	,RegSettingXml.value('(/Settings/Confirmation/Body)[1]', 'varchar(max)') Body
	,RegSettingXml.value('(/Settings/Confirmation/SenderSubject)[1]', 'varchar(100)') [SenderSubject]
	,RegSettingXml.value('(/Settings/Confirmation/SenderBody)[1]', 'varchar(max)') SenderBody
	,RegSettingXml.value('(/Settings/Confirmation/SupportSubject)[1]', 'varchar(100)') SupportSubject
	,RegSettingXml.value('(/Settings/Confirmation/SupportBody)[1]', 'varchar(max)') SupportBody
	,RegSettingXml.value('(/Settings/Confirmation/ReminderSubject)[1]', 'varchar(100)') ReminderSubject
	,RegSettingXml.value('(/Settings/Confirmation/ReminderBody)[1]', 'varchar(max)') ReminderBody

	,RegSettingXml.value('(/Settings/Instructions/Login)[1]', 'varchar(max)') InstructionsLogin
	,RegSettingXml.value('(/Settings/Instructions/Options)[1]', 'varchar(max)') InstructionsOptions
	,RegSettingXml.value('(/Settings/Instructions/Submit)[1]', 'varchar(max)') InstructionsSubmit
	,RegSettingXml.value('(/Settings/Instructions/Find)[1]', 'varchar(max)') InstructionsFind
	,RegSettingXml.value('(/Settings/Instructions/Select)[1]', 'varchar(max)') InstructionsSelect
	,RegSettingXml.value('(/Settings/Instructions/Sorry)[1]', 'varchar(max)') InstructionsSorry
	,RegSettingXml.value('(/Settings/Instructions/Special)[1]', 'varchar(max)') InstructionsSpecial
	,RegSettingXml.value('(/Settings/Instructions/Thanks)[1]', 'varchar(max)') InstructionsThanks
	,RegSettingXml.value('(/Settings/Instructions/Terms)[1]', 'varchar(max)') InstructionsTerms

FROM dbo.Organizations WHERE RegSettingXml IS NOT NULL
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
