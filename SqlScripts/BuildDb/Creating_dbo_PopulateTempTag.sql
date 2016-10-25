CREATE PROCEDURE [dbo].[PopulateTempTag] (@id INT, @List VARCHAR(MAX))
AS
BEGIN
    SET NOCOUNT ON;

	INSERT INTO TagPerson(Id, PeopleId) SELECT @id, Value FROM dbo.SplitInts(@List)
	        
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
