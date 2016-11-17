-- ============================================= 
-- Author:		<Author,,Name> 
-- Create date: <Create Date,,> 
-- Description:	<Description,,> 
-- ============================================= 
CREATE TRIGGER [dbo].[delPeople]  
   ON  [dbo].[People] 
   AFTER DELETE 
AS  
BEGIN 
	-- SET NOCOUNT ON added to prevent extra result sets from 
	-- interfering with SELECT statements. 
	SET NOCOUNT ON; 
 
	DECLARE @Cinfo VARBINARY(128)   
	SELECT @Cinfo = CONTEXT_INFO()   
	IF @Cinfo = 0x55555   
		RETURN   
 
	DECLARE @fid INT 
	DECLARE c CURSOR FOR 
	SELECT FamilyId FROM deleted GROUP BY FamilyId 
	OPEN c; 
	FETCH NEXT FROM c INTO @fid; 
	WHILE @@FETCH_STATUS = 0 
	BEGIN 
		UPDATE dbo.Families SET HeadOfHouseHoldId = dbo.HeadOfHouseholdId(FamilyId), 
			HeadOfHouseHoldSpouseId = dbo.HeadOfHouseHoldSpouseId(FamilyId), 
			CoupleFlag = dbo.CoupleFlag(FamilyId) 
		WHERE FamilyId = @fid 
 
		UPDATE dbo.People 
		SET SpouseId = dbo.SpouseId(PeopleId) 
		WHERE FamilyId = @fid 
 
		DECLARE @n INT 
		SELECT @n = COUNT(*) FROM dbo.People WHERE FamilyId = @fid 
		IF @n = 0 
		BEGIN 
			DELETE dbo.RelatedFamilies WHERE @fid IN(FamilyId, RelatedFamilyId) 
			DELETE dbo.FamilyCheckinLock WHERE FamilyId = @fid 
			DELETE dbo.Families WHERE FamilyId = @fid 
		END 
		FETCH NEXT FROM c INTO @fid; 
	END; 
	CLOSE c; 
	DEALLOCATE c; 
 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
