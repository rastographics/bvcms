
CREATE VIEW [dbo].[RegsettingOptions]
AS
SELECT 
	OrganizationId
	,OrganizationName
	,AskAllergies = CAST(IIF(RegSettingXml.value('(/Settings/AskItems/AskAllergies)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AnswersNotRequired = CAST(IIF(RegSettingXml.value('(/Settings/AskItems/AnswersNotRequired)[1]', 'varchar(10)') IS NOT NULL , 1, NULL) AS BIT)
	,AskChurch = CAST(IIF(RegSettingXml.value('(/Settings/AskItems/AskChurch)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AskCoaching = CAST(IIF(RegSettingXml.value('(/Settings/AskItems/AskCoaching)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AskDoctor = CAST(IIF(RegSettingXml.value('(/Settings/AskItems/AskDoctor)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AskEmContact = CAST(IIF(RegSettingXml.value('(/Settings/AskItems/AskEmContact)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AskInsurance = CAST(IIF(RegSettingXml.value('(/Settings/AskItems/AskInsurance)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AskParents = CAST(IIF(RegSettingXml.value('(/Settings/AskItems/AskParents)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AskSMS = CAST(IIF(RegSettingXml.value('(/Settings/AskItems/AskSMS)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AskTylenolEtc = CAST(IIF(RegSettingXml.value('(/Settings/AskItems/AskTylenolEtc)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AskSuggestedFee = CAST(IIF(RegSettingXml.value('(/Settings/AskItems/AskSuggestedFee)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)

	,AskRequest = RegSettingXml.value('(/Settings/AskItems/AskRequest)[1]', 'varchar(80)')
	,AskTickets = RegSettingXml.value('(/Settings/AskItems/AskTickets)[1]', 'varchar(100)')

	,NoReqBirthYear = CAST(IIF(RegSettingXml.value('(//NoReqBirthYear)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,NotReqDOB = CAST(IIF(RegSettingXml.value('(//NotReqDOB)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,NotReqAddr = CAST(IIF(RegSettingXml.value('(//NotReqAddr)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,NotReqZip = CAST(IIF(RegSettingXml.value('(//NotReqZip)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,NotReqPhone = CAST(IIF(RegSettingXml.value('(//NotReqPhone)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,NotReqGender = CAST(IIF(RegSettingXml.value('(//NotReqGender)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,NotReqMarital = CAST(IIF(RegSettingXml.value('(//NotReqMarital)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)

	,ConfirmationTrackingCode = RegSettingXml.value('(/Settings/Options/ConfirmationTrackingCode)[1]', 'varchar(max)')
	,ValidateOrgs = RegSettingXml.value('(/Settings/Options/ValidateOrgs)[1]', 'varchar(200)')
	,Shell = RegSettingXml.value('(/Settings/Options/Shell)[1]', 'varchar(50)')
	,ShellBs = RegSettingXml.value('(/Settings/Options/ShellBs)[1]', 'varchar(50)')
	,FinishRegistrationButton = RegSettingXml.value('(/Settings/Options/FinishRegistrationButton)[1]', 'varchar(80)')
	,SpecialScript = RegSettingXml.value('(/Settings/Options/SpecialScript)[1]', 'varchar(50)')
	,GroupToJoin = RegSettingXml.value('(/Settings/Options/GroupToJoin)[1]', 'varchar(50)')
	,[TimeOut] = RegSettingXml.value('(/Settings/Options/TimeOut)[1]', 'int')

	,AllowOnlyOne = CAST(IIF(RegSettingXml.value('(//AllowOnlyOne)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,TargetExtraValues = CAST(IIF(RegSettingXml.value('(//TargetExtraValues)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AllowReRegister = CAST(IIF(RegSettingXml.value('(//AllowReRegister)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AllowSaveProgress = CAST(IIF(RegSettingXml.value('(//AllowSaveProgress)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,MemberOnly = CAST(IIF(RegSettingXml.value('(//MemberOnly)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AddAsProspect = CAST(IIF(RegSettingXml.value('(//AddAsProspect)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,DisallowAnonymous = CAST(IIF(RegSettingXml.value('(//DisallowAnonymous)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)

	,Fee = RegSettingXml.value('(/Settings/Fees/Fee)[1]', 'money')
	,MaxFee = RegSettingXml.value('(/Settings/Fees/MaximumFee)[1]', 'money')
	,ExtraFee = RegSettingXml.value('(/Settings/Fees/ExtraFee)[1]', 'money')
	,Deposit = RegSettingXml.value('(/Settings/Fees/Deposit)[1]', 'money')
	,AccountingCode = RegSettingXml.value('(/Settings/Fees/AccountingCode)[1]', 'varchar(50)')
	,ExtraValueFeeName = RegSettingXml.value('(/Settings/Fees/ExtraValueFeeName)[1]', 'varchar(50)')
	,ApplyMaxToOtheFees = CAST(IIF(RegSettingXml.value('(//ApplyMaxToOtherFees)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,IncludeOtherFeesWithDeposit = CAST(IIF(RegSettingXml.value('(//IncludeOtherFeesWithDeposit)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,OtherFeesAddedToOrgFee = CAST(IIF(RegSettingXml.value('(//OtherFeesAddedToOrgFee)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,AskDonation = CAST(IIF(RegSettingXml.value('(//AskDonation)[1]', 'varchar(10)') IS NOT NULL, 1, NULL) AS BIT)
	,DonationLabel = RegSettingXml.value('(/Settings/Fees/DonationLabel)[1]', 'varchar(50)')
	,DonationFundId = RegSettingXml.value('(/Settings/Fees/DonationFundId)[1]', 'int')

FROM dbo.Organizations WHERE RegSettingXml IS NOT NULL



GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
