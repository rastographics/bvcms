
CREATE PROCEDURE [dbo].[DeleteSpecialTags](@pid INT = null)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	EXEC SetupNumbers

DELETE dbo.TagPerson
FROM dbo.TagPerson tp
JOIN dbo.Tag t ON tp.Id = t.Id
WHERE t.TypeId >= 3 AND t.TypeId < 100 
AND (t.PeopleId = @pid OR @pid IS NULL)

DELETE FROM dbo.Tag
WHERE TypeId >= 3 AND TypeId < 100 
AND (PeopleId = @pid OR @pid IS NULL)

DELETE dbo.TagPerson
FROM dbo.TagPerson tp
JOIN dbo.Tag t ON tp.Id = t.Id
WHERE t.Name LIKE '.temp email tag%' 
AND (t.PeopleId = @pid OR @pid IS NULL)

DELETE FROM dbo.Tag
WHERE Name LIKE '.temp email tag%'
AND (PeopleId = @pid OR @pid IS NULL)

DELETE dbo.TagPerson
FROM dbo.TagPerson tp
JOIN dbo.Tag t ON tp.Id = t.Id
WHERE t.TypeId = 1 AND t.PeopleId IS NULL

DELETE dbo.TagShare
FROM dbo.TagShare s
JOIN dbo.Tag t ON s.TagId = t.Id
WHERE TypeId = 1 AND t.PeopleId IS NULL

DELETE FROM dbo.Tag
WHERE TypeId = 1 AND PeopleId IS NULL

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
