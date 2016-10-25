CREATE PROCEDURE [dbo].[StatusGrid](@tagid INT)
AS
BEGIN
DECLARE @cases nvarchar(MAX) = ''
SELECT @cases = @cases +
', MAX(CASE t.Name WHEN ''' + t.Name + ''' THEN ''X'' ELSE '''' END) AS [' 
+ REPLACE(
	+ REPLACE(SUBSTRING(c.Name, 5, 50), ' ', '_')
, '.', '_') + ']
'
FROM Tag t
JOIN dbo.Query c ON LEFT(c.Name,3) = t.Name
WHERE t.TypeId = 100
GROUP BY t.Name, c.Name
ORDER BY t.Name

DECLARE @sql nvarchar(MAX) = '
SELECT 
PreferredName First, 
LastName Last, 
ISNULL(CAST(Age AS nvarchar),'''') Age, 
ms.Description Marital,
REPLACE(ISNULL(CONVERT(nvarchar(10), dbo.FirstMeetingDateLastLear(p.PeopleId), 20), ''''), ''/'', ''-'') FirstAttend,
tt.*
FROM dbo.People p
JOIN lookup.MaritalStatus ms on p.MaritalStatusId = ms.Id
JOIN 
(
SELECT pp.PeopleId
' + @cases +
'FROM
dbo.People pp
JOIN dbo.TagPerson tp ON pp.PeopleId = tp.PeopleId
JOIN Tag t ON tp.Id = t.Id
WHERE t.Name LIKE ''F[0-9][0-9]''
GROUP BY pp.PeopleId
) tt
ON p.PeopleId = tt.PeopleId
WHERE EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.Id = ' + CAST(@tagid AS nvarchar)
+ ' AND tp.PeopleId = p.PeopleId)'
PRINT @sql
EXEC (@sql)
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
