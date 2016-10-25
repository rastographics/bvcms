-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[updPeople] 
   ON  [dbo].[People]
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    IF (
		UPDATE(PositionInFamilyId) 
		OR UPDATE(GenderId) 
		OR UPDATE(DeceasedDate) 
		OR UPDATE(FirstName)
		OR UPDATE(MaritalStatusId)
		OR UPDATE(FamilyId)
    )
	BEGIN
		UPDATE dbo.Families 
		SET HeadOfHouseHoldId = dbo.HeadOfHouseholdId(FamilyId),
			HeadOfHouseHoldSpouseId = dbo.HeadOfHouseHoldSpouseId(FamilyId),
			CoupleFlag = dbo.CoupleFlag(FamilyId)
		WHERE FamilyId IN (SELECT FamilyId FROM INSERTED) 
		OR FamilyId IN (SELECT FamilyId FROM DELETED)
		
		UPDATE dbo.People
		SET SpouseId = dbo.SpouseId(PeopleId)
		WHERE FamilyId IN (SELECT FamilyId FROM INSERTED)
		OR FamilyId IN (SELECT FamilyId FROM DELETED)

		IF (UPDATE(FamilyId))
		BEGIN
			DECLARE c CURSOR FOR
			SELECT FamilyId FROM deleted GROUP BY FamilyId
			OPEN c;
			DECLARE @fid INT
			FETCH NEXT FROM c INTO @fid;
			WHILE @@FETCH_STATUS = 0
			BEGIN
				DECLARE @n INT
				SELECT @n = COUNT(*) FROM dbo.People WHERE FamilyId = @fid
				IF @n = 0
				BEGIN
					DELETE dbo.RelatedFamilies WHERE @fid IN(FamilyId, RelatedFamilyId)
					DELETE dbo.FamilyCheckinLock WHERE @fid = FamilyId
					DELETE dbo.FamilyExtra WHERE @fid = FamilyId
					DELETE dbo.Families WHERE FamilyId = @fid
				END
				FETCH NEXT FROM c INTO @fid;
			END;
			CLOSE c;
			DEALLOCATE c;

			UPDATE dbo.People
			SET HomePhone = f.HomePhone
			FROM dbo.People p JOIN dbo.Families f ON p.FamilyId = f.FamilyId
			WHERE p.PeopleId IN (SELECT PeopleId FROM INSERTED)
		END

	END
	IF UPDATE(HomePhone)
	BEGIN
		UPDATE dbo.People
		SET 
			HomePhone = dbo.GetDigits(HomePhone)
		WHERE PeopleId IN (SELECT PeopleId FROM inserted)
	END
    IF UPDATE(CellPhone)
	BEGIN
		UPDATE dbo.People
		SET 
			CellPhone = dbo.GetDigits(CellPhone),
			CellPhoneLU = RIGHT(dbo.GetDigits(CellPhone), 7),
			CellPhoneAC = LEFT(RIGHT(REPLICATE('0',10) + dbo.GetDigits(CellPhone), 10), 3)
		WHERE PeopleId IN (SELECT PeopleId FROM inserted)
	END
    IF UPDATE(WorkPhone)
	BEGIN
		UPDATE dbo.People
		SET 
			WorkPhone = dbo.GetDigits(WorkPhone)
		WHERE PeopleId IN (SELECT PeopleId FROM inserted)
	END

	
	IF UPDATE(AddressTypeId)
	OR UPDATE(CityName) 
	OR UPDATE(AddressLineOne) 
	OR UPDATE(AddressLineTwo) 
	OR UPDATE(StateCode) 
	OR UPDATE(BadAddressFlag) 
	OR UPDATE(ZipCode)
	OR UPDATE(CountryName)
	OR UPDATE(FamilyId)
	OR UPDATE(ResCodeId)
	BEGIN
		UPDATE dbo.People
		SET 
		PrimaryCity = dbo.PrimaryCity(PeopleId),
		PrimaryAddress = dbo.PrimaryAddress(PeopleId),
		PrimaryAddress2 = dbo.PrimaryAddress2(PeopleId),
		PrimaryState = dbo.PrimaryState(PeopleId),
		PrimaryBadAddrFlag = dbo.PrimaryBadAddressFlag(PeopleId),
		PrimaryResCode = dbo.PrimaryResCode(PeopleId),
		PrimaryZip = dbo.PrimaryZip(PeopleId),
		PrimaryCountry = dbo.PrimaryCountry(PeopleId)
		WHERE PeopleId IN (SELECT PeopleId FROM inserted)
	END

	IF UPDATE(FirstName)
	OR UPDATE(LastName)
	OR UPDATE(NickName)
	OR UPDATE(MiddleName)
	BEGIN
		UPDATE Users
		SET Name = dbo.UName(PeopleId),
		Name2 = dbo.UName2(PeopleId)
		WHERE PeopleId IN (SELECT PeopleId FROM INSERTED)
		
		UPDATE dbo.People 
		SET FirstName2 = REPLACE(FirstName + ISNULL(MiddleName, ''), ' ', '')
		WHERE PeopleId IN (SELECT PeopleId FROM INSERTED)
	END
	
	IF UPDATE(WeddingDate)
	BEGIN
		UPDATE dbo.People
		SET WeddingDate = i.WeddingDate
		FROM dbo.People p 
		JOIN INSERTED i ON i.PeopleId = p.SpouseId
	END

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
