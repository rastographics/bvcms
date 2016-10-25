CREATE FUNCTION [dbo].[StatusFlags](@flags nvarchar(100))
RETURNS TABLE 
AS
RETURN 
(
SELECT PeopleId,
SUBSTRING((Select ', ' + SUBSTRING(c.Name, 5, 50) AS [text()]
FROM dbo.People p1
JOIN dbo.TagPerson tp ON p1.PeopleId = tp.PeopleId
JOIN dbo.Tag t ON tp.Id = t.Id
JOIN dbo.Query c ON c.Name LIKE (t.Name + ':%')
JOIN dbo.Split(@flags, ',') ff ON t.Name = ff.Value
WHERE p1.PeopleId = p2.PeopleId
ORDER BY ff.TokenID
FOR XML PATH ('')),3, 1000) StatusFlags
From dbo.People p2
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
