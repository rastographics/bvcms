CREATE FUNCTION [dbo].[StatusFlag] 
(
	@pid INT
)

RETURNS NVARCHAR(200)
AS
BEGIN
	DECLARE @Result NVARCHAR(200)

DECLARE @flags NVARCHAR(200)
SELECT @flags = Setting FROM dbo.Setting WHERE Id = 'StatusFlags'
DECLARE @show TABLE (Value nvarchar(100))
INSERT INTO @show SELECT Value FROM dbo.Split(@flags, ',')
SELECT @Result = COALESCE(@Result + ', ', '') + SUBSTRING(c.Name, 5, 50)
FROM dbo.People p
JOIN dbo.TagPerson tp ON p.PeopleId = tp.PeopleId
JOIN dbo.Tag t ON tp.Id = t.Id
JOIN dbo.Query c ON c.Name LIKE (t.Name + ':%')
WHERE t.Name IN (SELECT Value FROM @show)
AND p.PeopleId = @pid

RETURN @Result
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
