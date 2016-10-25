CREATE PROCEDURE [dbo].[UpdateResCodes] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-----------------------------------------------------------------
	
UPDATE dbo.People
SET ResCodeId = dbo.FindResCode(ZipCode, CountryName)

UPDATE dbo.Families
SET ResCodeId = dbo.FindResCode(ZipCode, CountryName)


END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
