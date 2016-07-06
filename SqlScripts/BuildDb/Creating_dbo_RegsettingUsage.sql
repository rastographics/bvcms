

CREATE VIEW [dbo].[RegsettingUsage]
AS
SELECT 
	OrganizationId
	,OrganizationName
	,Usage = 
		CASE WHEN RegSettingXml.value('count(//DropdownItem)', 'int') > 0 THEN ' Dropdown' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//DropdownItem[@Fee])', 'int') > 0 THEN ' DropdownFee' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//CheckboxItem)', 'int') > 0 THEN ' Checkbox' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//CheckboxItem[@Fee])', 'int') > 0 THEN ' CheckboxFee' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//AskExtraQuestions/Question)', 'int') > 0 THEN ' ExtraQuestion' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//AskText/Question)', 'int') > 0 THEN ' TextQuestion' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//GradeOption)', 'int') > 0 THEN ' GradeOption' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//AskHeader)', 'int') > 0 THEN ' Header' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//AskInstruction)', 'int') > 0 THEN ' Instruction' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//MenuItem)', 'int') > 0 THEN ' Menu' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//Size)', 'int') > 0 THEN ' AskSize' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//AskYesNoQuestion/Question)[1]', 'int') > 0 THEN ' YesNoQuestion' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//NotRequired)', 'int') > 0 THEN ' NotRequired' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//OrgFees/Fee)', 'int') > 0 THEN ' OrgFees' ELSE '' END
		+CASE WHEN RegSettingXml.value('count(//AgeGroups/Group)', 'int') > 0 THEN ' AgeGroups' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Confirmation/Body)[1]', 'varchar(max)'),'')) > 0 THEN ' Body' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Confirmation/SenderBody)[1]', 'varchar(max)'),'')) > 0 THEN ' SenderBody' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Confirmation/SupportBody)[1]', 'varchar(max)'),'')) > 0 THEN ' SupportBody' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Confirmation/ReminderBody)[1]', 'varchar(max)'),'')) > 0 THEN ' ReminderBody' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Instructions/Login)[1]', 'varchar(max)'),'')) > 0 THEN ' InstructionsLogin' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Instructions/Options)[1]', 'varchar(max)'),'')) > 0 THEN ' InstructionsOptions' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Instructions/Submit)[1]', 'varchar(max)'),'')) > 0 THEN ' InstructionsSubmit' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Instructions/Find)[1]', 'varchar(max)'),'')) > 0 THEN ' InstructionsFind' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Instructions/Select)[1]', 'varchar(max)'),'')) > 0 THEN ' InstructionsSelect' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Instructions/Sorry)[1]', 'varchar(max)'),'')) > 0 THEN ' InstructionsSorry' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Instructions/Special)[1]', 'varchar(max)'),'')) > 0 THEN ' InstructionsSpecial' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Instructions/Thanks)[1]', 'varchar(max)'),'')) > 0 THEN ' InstructionsThanks' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//Instructions/Terms)[1]', 'varchar(max)'),'')) > 0 THEN ' InstructionsTerms' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AskAllergies)[1]', 'varchar') IS NOT NULL THEN ' Allergies' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AnswersNotRequired)[1]', 'varchar') IS NOT NULL THEN ' AnswersNotRequired' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AskChurch)[1]', 'varchar') IS NOT NULL THEN ' Church' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AskCoaching)[1]', 'varchar') IS NOT NULL THEN ' Coaching' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AskDoctor)[1]', 'varchar') IS NOT NULL THEN ' Doctor' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AskEmContact)[1]', 'varchar') IS NOT NULL THEN ' EmContact' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AskInsurance)[1]', 'varchar') IS NOT NULL THEN ' Insurance' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AskParents)[1]', 'varchar') IS NOT NULL THEN ' Parents' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AskSMS)[1]', 'varchar') IS NOT NULL THEN ' SMS' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AskTylenolEtc)[1]', 'varchar') IS NOT NULL THEN ' TylenolEtc' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//AskSuggestedFee)[1]', 'varchar(80)'), '')) > 0 THEN ' AskSuggestedFee' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//AskRequest)[1]', 'varchar(80)'), '')) > 0 THEN ' Request' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//AskTickets)[1]', 'varchar(100)'), '')) > 0 THEN ' Tickets' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//NoReqBirthYear)[1]', 'bit') = 1 THEN ' NoReqBirthYear' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//NotReqDOB)[1]', 'bit') = 1 THEN ' NotReqDOB' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//NotReqAddr)[1]', 'bit') = 1 THEN ' NotReqAddr' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//NotReqZip)[1]', 'bit') = 1 THEN ' NotReqZip' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//NotReqPhone)[1]', 'bit') = 1 THEN ' NotReqPhone' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//NotReqGender)[1]', 'bit') = 1 THEN ' NotReqGender' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//NotReqMarital)[1]', 'bit') = 1 THEN ' NotReqMarital' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//ConfirmationTrackingCode)[1]', 'varchar(max)'), '')) > 0 THEN ' ConfirmationTrackingCode' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//ValidateOrgs)[1]', 'varchar(200)'), '')) > 0 THEN ' ValidateOrgs' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//ShellBs)[1]', 'varchar(50)'), '')) > 0 THEN ' Shell' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//FinishRegistrationButton)[1]', 'varchar(80)'), '')) > 0 THEN ' FinishRegistrationButton' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//SpecialScript)[1]', 'varchar(50)'), '')) > 0 THEN ' SpecialScript' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//GroupToJoin)[1]', 'varchar(50)'), '')) > 0 THEN ' GroupToJoin' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//TimeOut)[1]', 'int') > 0 THEN ' TimeOut' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AllowOnlyOne)[1]', 'bit') = 1 THEN ' AllowOnlyOne' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//TargetExtraValues)[1]', 'bit') = 1 THEN ' TargetExtraValues' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AllowReRegister)[1]', 'bit') = 1 THEN ' AllowReRegister' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AllowSaveProgress)[1]', 'bit') = 1 THEN ' AllowSaveProgress' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//MemberOnly)[1]', 'bit') = 1 THEN ' MemberOnly' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AddAsProspect)[1]', 'bit') = 1 THEN ' AddAsProspect' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//DisallowAnonymous)[1]', 'bit') = 1 THEN ' DisallowAnonymous' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//Fee)[1]', 'money') > 0 THEN ' Fee' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//MaximumFee)[1]', 'money') > 0 THEN ' MaxFee' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//ExtraFee)[1]', 'money') > 0 THEN ' ExtraFee' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//Deposit)[1]', 'money') > 0 THEN ' Deposit' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//AccountingCode)[1]', 'varchar(50)'), '')) > 0 THEN ' AccountingCode' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//ExtraValueFeeName)[1]', 'varchar(50)') , '')) > 0 THEN ' ExtraValueFeeName' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//ApplyMaxToOtherFees)[1]', 'bit') = 1 THEN ' ApplyMaxToOtheFees' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//IncludeOtherFeesWithDeposit)[1]', 'bit') = 1 THEN ' IncludeOtherFeesWithDeposit' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//OtherFeesAddedToOrgFee)[1]', 'bit') = 1 THEN ' OtherFeesAddedToOrgFee' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//AskDonation)[1]', 'bit') = 1 THEN ' AskDonation' ELSE '' END
		+CASE WHEN LEN(ISNULL(RegSettingXml.value('(//DonationLabel)[1]', 'varchar(50)'), '')) > 0 THEN ' DonationLabel' ELSE '' END
		+CASE WHEN RegSettingXml.value('(//DonationFundId)[1]', 'int') > 0 THEN ' DonationFundId' ELSE '' END
FROM dbo.Organizations WHERE RegSettingXml IS NOT NULL




GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
