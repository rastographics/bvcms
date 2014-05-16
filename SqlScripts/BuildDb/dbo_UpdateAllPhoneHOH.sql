-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateAllPhoneHOH]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		UPDATE dbo.Families
		SET HomePhoneLU = RIGHT(HomePhone, 7),
			HomePhoneAC = LEFT(RIGHT(REPLICATE('0',10) + HomePhone, 10), 3),
			HeadOfHouseholdId = dbo.HeadOfHouseholdId(FamilyId),
			HeadOfHouseholdSpouseId = dbo.HeadOfHouseHoldSpouseId(FamilyId)
			
		UPDATE dbo.People
		SET CellPhoneLU = RIGHT(CellPhone, 7),
			CellPhoneAC = LEFT(RIGHT(REPLICATE('0',10) + CellPhone, 10), 3)
		
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
