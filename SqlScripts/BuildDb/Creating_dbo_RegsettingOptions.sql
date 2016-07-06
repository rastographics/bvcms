
CREATE VIEW [dbo].[RegsettingOptions]
AS
SELECT 
	OrganizationId
	,OrganizationName
	,CAST(RegSettingXml.value('(/Settings/AskItems/AskAllergies)[1]', 'bit') + 1 AS BIT) AskAllergies
	,CAST(RegSettingXml.value('(/Settings/AskItems/AnswersNotRequired)[1]', 'bit') + 1 AS BIT) AnswersNotRequired
	,CAST(RegSettingXml.value('(/Settings/AskItems/AskChurch)[1]', 'bit') + 1 AS BIT) AskChurch
	,CAST(RegSettingXml.value('(/Settings/AskItems/AskCoaching)[1]', 'bit') + 1 AS BIT) AskCoaching
	,CAST(RegSettingXml.value('(/Settings/AskItems/AskDoctor)[1]', 'bit') + 1 AS BIT) AskDoctor
	,CAST(RegSettingXml.value('(/Settings/AskItems/AskEmContact)[1]', 'bit') + 1 AS BIT) AskEmContact
	,CAST(RegSettingXml.value('(/Settings/AskItems/AskInsurance)[1]', 'bit') + 1 AS BIT) AskInsurance
	,CAST(RegSettingXml.value('(/Settings/AskItems/AskParents)[1]', 'bit') + 1 AS BIT) AskParents
	,CAST(RegSettingXml.value('(/Settings/AskItems/AskSMS)[1]', 'bit') + 1 AS BIT) AskSMS
	,CAST(RegSettingXml.value('(/Settings/AskItems/AskTylenolEtc)[1]', 'bit') + 1 AS BIT) AskTylenolEtc
	,CAST(RegSettingXml.value('(/Settings/AskItems/AskSuggestedFee)[1]', 'bit') + 1 AS BIT) AskSuggestedFee

	,RegSettingXml.value('(/Settings/AskItems/AskRequest)[1]', 'varchar(80)') AskRequest
	,RegSettingXml.value('(/Settings/AskItems/AskTickets)[1]', 'varchar(100)') AskTickets

	,RegSettingXml.value('(/Settings/NotRequired/NoReqBirthYear)[1]', 'bit') NoReqBirthYear
	,RegSettingXml.value('(/Settings/NotRequired/NotReqDOB)[1]', 'bit') NotReqDOB
	,RegSettingXml.value('(/Settings/NotRequired/NotReqAddr)[1]', 'bit') NotReqAddr
	,RegSettingXml.value('(/Settings/NotRequired/NotReqZip)[1]', 'bit') NotReqZip
	,RegSettingXml.value('(/Settings/NotRequired/NotReqPhone)[1]', 'bit') NotReqPhone
	,RegSettingXml.value('(/Settings/NotRequired/NotReqGender)[1]', 'bit') NotReqGender
	,RegSettingXml.value('(/Settings/NotRequired/NotReqMarital)[1]', 'bit') NotReqMarital

	,RegSettingXml.value('(/Settings/Options/ConfirmationTrackingCode)[1]', 'varchar(max)') ConfirmationTrackingCode
	,RegSettingXml.value('(/Settings/Options/ValidateOrgs)[1]', 'varchar(200)') ValidateOrgs
	,RegSettingXml.value('(/Settings/Options/Shell)[1]', 'varchar(50)') Shell
	,RegSettingXml.value('(/Settings/Options/ShellBs)[1]', 'varchar(50)') ShellBs
	,RegSettingXml.value('(/Settings/Options/FinishRegistrationButton)[1]', 'varchar(80)') FinishRegistrationButton
	,RegSettingXml.value('(/Settings/Options/SpecialScript)[1]', 'varchar(50)') SpecialScript
	,RegSettingXml.value('(/Settings/Options/GroupToJoin)[1]', 'varchar(50)') GroupToJoin
	,RegSettingXml.value('(/Settings/Options/TimeOut)[1]', 'int') [TimeOut]

	,RegSettingXml.value('(/Settings/Fees/AllowOnlyOne)[1]', 'bit') AllowOnlyOne
	,RegSettingXml.value('(/Settings/Fees/TargetExtraValues)[1]', 'bit') TargetExtraValues
	,RegSettingXml.value('(/Settings/Fees/AllowReRegister)[1]', 'bit') AllowReRegister
	,RegSettingXml.value('(/Settings/Fees/AllowSaveProgress)[1]', 'bit') AllowSaveProgress
	,RegSettingXml.value('(/Settings/Fees/MemberOnly)[1]', 'bit') MemberOnly
	,RegSettingXml.value('(/Settings/Fees/AddAsProspect)[1]', 'bit') AddAsProspect
	,RegSettingXml.value('(/Settings/Fees/DisallowAnonymous)[1]', 'bit') DisallowAnonymous

	,RegSettingXml.value('(/Settings/Fees/Fee)[1]', 'money') [Fee]
	,RegSettingXml.value('(/Settings/Fees/MaximumFee)[1]', 'money') [MaxFee]
	,RegSettingXml.value('(/Settings/Fees/ExtraFee)[1]', 'money') [ExtraFee]
	,RegSettingXml.value('(/Settings/Fees/Deposit)[1]', 'money') [Deposit]
	,RegSettingXml.value('(/Settings/Fees/AccountingCode)[1]', 'varchar(50)') [AccountingCode]
	,RegSettingXml.value('(/Settings/Fees/ExtraValueFeeName)[1]', 'varchar(50)') [ExtraValueFeeName]
	,RegSettingXml.value('(/Settings/Fees/ApplyMaxToOtherFees)[1]', 'bit') [ApplyMaxToOtheFees]
	,RegSettingXml.value('(/Settings/Fees/IncludeOtherFeesWithDeposit)[1]', 'bit') [IncludeOtherFeesWithDeposit]
	,RegSettingXml.value('(/Settings/Fees/OtherFeesAddedToOrgFee)[1]', 'bit') OtherFeesAddedToOrgFee
	,RegSettingXml.value('(/Settings/Fees/AskDonation)[1]', 'bit') AskDonation
	,RegSettingXml.value('(/Settings/Fees/DonationLabel)[1]', 'varchar(50)') DonationLabel
	,RegSettingXml.value('(/Settings/Fees/DonationFundId)[1]', 'int') DonationFundId

FROM dbo.Organizations WHERE RegSettingXml IS NOT NULL


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
