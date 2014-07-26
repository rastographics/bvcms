-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[insPeople] 
   ON  [dbo].[People]
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE dbo.Families 
	SET HeadOfHouseHoldId = dbo.HeadOfHouseholdId(FamilyId),
		HeadOfHouseHoldSpouseId = dbo.HeadOfHouseHoldSpouseId(FamilyId),
		CoupleFlag = dbo.CoupleFlag(FamilyId)
	WHERE FamilyId IN (SELECT FamilyId FROM INSERTED)
	
	UPDATE dbo.People
	SET SpouseId = dbo.SpouseId(PeopleId)
	WHERE FamilyId IN (SELECT FamilyId FROM INSERTED)
	
	UPDATE dbo.People
	SET ResCodeId = dbo.FindResCode(ZipCode, CountryName)
	WHERE PeopleId IN (SELECT PeopleId FROM INSERTED)

	UPDATE dbo.People
	SET HomePhone = f.HomePhone
	FROM dbo.People p JOIN dbo.Families f ON p.FamilyId = f.FamilyId
	WHERE p.PeopleId IN (SELECT PeopleId FROM INSERTED)

	UPDATE dbo.People
	SET CellPhoneLU = RIGHT(CellPhone, 7),
	CellPhoneAC = LEFT(RIGHT(REPLICATE('0',10) + CellPhone, 10), 3),
	PrimaryCity = dbo.PrimaryCity(PeopleId),
	PrimaryAddress = dbo.PrimaryAddress(PeopleId),
	PrimaryAddress2 = dbo.PrimaryAddress2(PeopleId),
	PrimaryState = dbo.PrimaryState(PeopleId),
	PrimaryBadAddrFlag = dbo.PrimaryBadAddressFlag(PeopleId),
	PrimaryResCode = dbo.PrimaryResCode(PeopleId),
	PrimaryZip = dbo.PrimaryZip(PeopleId),
	PrimaryCountry = dbo.PrimaryCountry(PeopleId),
	SpouseId = dbo.SpouseId(PeopleId),
	FirstName2 = REPLACE(FirstName + ISNULL(MiddleName, ''), ' ', '')
	WHERE PeopleId IN (SELECT PeopleId FROM INSERTED)
		
	UPDATE dbo.Families
	SET CoupleFlag = dbo.CoupleFlag(FamilyId)
	WHERE FamilyId IN (SELECT FamilyId FROM INSERTED)
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
