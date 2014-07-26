CREATE FUNCTION [dbo].[StatusFlagsPerson]
(
	@pid int
)
RETURNS TABLE AS RETURN
(
	SELECT t.Name Flag, SUBSTRING(c.Name, 5, 100) Name, r.RoleName
	FROM dbo.People p1
	JOIN dbo.TagPerson tp ON p1.PeopleId = tp.PeopleId
	JOIN dbo.Tag t ON tp.Id = t.Id
	JOIN dbo.Query c ON c.Name LIKE (t.Name + ':%')
	JOIN dbo.Split((SELECT Setting FROM Setting WHERE Id = 'StatusFlags'), ',') ff ON t.Name = ff.Value
	LEFT JOIN dbo.Roles r ON r.RoleName = 'StatusFlag:' + t.Name
	WHERE p1.PeopleId = @pid
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
