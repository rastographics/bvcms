-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[updFamily] 
   ON  [dbo].[Families] 
   FOR UPDATE, INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF UPDATE(HomePhone)
	BEGIN
		UPDATE dbo.Families SET HomePhone = dbo.GetDigits(HomePhone)
		WHERE FamilyId IN (SELECT FamilyId FROM INSERTED)

		UPDATE dbo.People
		SET HomePhone = f.HomePhone
		FROM dbo.People p
		JOIN dbo.Families f ON p.FamilyId = f.FamilyId
		WHERE f.FamilyId IN (SELECT FamilyId FROM INSERTED)
		
		UPDATE dbo.Families
		SET HomePhoneLU = RIGHT(HomePhone, 7),
			HomePhoneAC = LEFT(RIGHT(REPLICATE('0',10) + HomePhone, 10), 3)
		WHERE FamilyId IN (SELECT FamilyId FROM INSERTED)
	END
	
	IF UPDATE(CityName) 
	OR UPDATE(AddressLineOne) 
	OR UPDATE(AddressLineTwo) 
	OR UPDATE(StateCode) 
	OR UPDATE(ZipCode)
	OR UPDATE(CountryName)
	OR UPDATE(BadAddressFlag) 
	OR UPDATE(ResCodeId)
	BEGIN		
		UPDATE dbo.Families
		SET ResCodeId = dbo.FindResCode(ZipCode, CountryName)
		WHERE FamilyId IN (SELECT FamilyId FROM INSERTED)

		UPDATE dbo.People
		SET PrimaryCity = dbo.PrimaryCity(PeopleId),
		PrimaryAddress = dbo.PrimaryAddress(PeopleId),
		PrimaryAddress2 = dbo.PrimaryAddress2(PeopleId),
		PrimaryState = dbo.PrimaryState(PeopleId),
		PrimaryBadAddrFlag = dbo.PrimaryBadAddressFlag(PeopleId),
		PrimaryResCode = dbo.PrimaryResCode(PeopleId),
		PrimaryZip = dbo.PrimaryZip(PeopleId),
		PrimaryCountry = dbo.PrimaryCountry(PeopleId)
		WHERE FamilyId IN (SELECT FamilyId FROM inserted)
	END

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
