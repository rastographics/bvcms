CREATE FUNCTION [dbo].[StatusFlagsAll] 
(
	@pid INT
)

RETURNS NVARCHAR(200)
AS
BEGIN
	DECLARE @Result NVARCHAR(200)

DECLARE @flags NVARCHAR(200)
DECLARE @show TABLE (Value nvarchar(10))

INSERT INTO @show SELECT substring(name,1,3)
FROM dbo.Query
WHERE name LIKE 'F[0-9][0-9]%'
GROUP BY name


SELECT @Result = COALESCE(@Result + ', ', '') + c.Name
FROM dbo.People p
JOIN dbo.TagPerson tp ON p.PeopleId = tp.PeopleId
JOIN dbo.Tag t ON tp.Id = t.Id
JOIN dbo.Query c ON c.Name LIKE (t.Name + ':%')
WHERE t.Name IN (SELECT Value FROM @show)
AND p.PeopleId = @pid

RETURN ISNULL(@Result, '')
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
