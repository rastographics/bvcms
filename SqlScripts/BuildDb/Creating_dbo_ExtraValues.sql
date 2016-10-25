CREATE PROCEDURE [dbo].[ExtraValues](@tagid INT, @sort nvarchar(100) = '', @nodisplaycols nvarchar(MAX))
AS
BEGIN
--DECLARE @nodisplaycols nvarchar(200) = 'Ted|PastoralCare'
DECLARE @Cols TABLE (Value nvarchar(100))
INSERT INTO @Cols SELECT Value FROM dbo.Split(@nodisplaycols, '|')

DECLARE @sql nvarchar(MAX) = 'select * from (SELECT PeopleId, (SELECT p2.Name FROM dbo.People p2 WHERE p2.PeopleId = p.PeopleId) FullName'

SELECT @sql = 
      @sql + ',ISNULL((SELECT TOP 1 pe.StrValue FROM dbo.PeopleExtra pe WHERE pe.PeopleId = p.PeopleId AND pe.Field = ''' +  REPLACE(CAST(Field as nvarchar(100)),'''','''''') + '''),'''') [' + REPLACE(CAST(Field as nvarchar(100)),'''','''''') + ']'
FROM dbo.PeopleExtra pe 
JOIN dbo.People p ON pe.PeopleId = p.PeopleId
WHERE EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.Id = @tagid AND tp.PeopleId = p.PeopleId)
AND pe.StrValue IS NOT NULL
AND pe.Field NOT IN (SELECT Value FROM @Cols)
GROUP BY pe.Field

SELECT @sql = 
      @sql + ',
ISNULL((SELECT TOP 1 pe.Data FROM dbo.PeopleExtra pe WHERE pe.PeopleId = p.PeopleId AND pe.Field = ''' +  REPLACE(CAST(Field as nvarchar(100)),'''','''''') + '''),'''') [' + REPLACE(CAST(Field as nvarchar(100)),'''','''''') + ']'
      FROM dbo.PeopleExtra pe 
JOIN dbo.People p ON pe.PeopleId = p.PeopleId
WHERE EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.Id = @tagid AND tp.PeopleId = p.PeopleId)
AND pe.Data IS NOT NULL
AND pe.Field NOT IN (SELECT Value FROM @Cols)
GROUP BY pe.Field

SELECT @sql = 
      @sql + ',
ISNULL((SELECT TOP 1 CONVERT(nvarchar, pe.DateValue, 111) FROM dbo.PeopleExtra pe WHERE pe.PeopleId = p.PeopleId AND pe.Field = ''' +  REPLACE(CAST(Field as nvarchar(100)),'''','''''') + '''),'''') [' + REPLACE(CAST(Field as nvarchar(100)),'''','''''') + ']'
            FROM dbo.PeopleExtra pe 
JOIN dbo.People p ON pe.PeopleId = p.PeopleId
WHERE EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.Id = @tagid AND tp.PeopleId = p.PeopleId)
AND pe.DateValue IS NOT NULL
AND pe.Field NOT IN (SELECT Value FROM @Cols)
GROUP BY pe.Field

SELECT @sql = 
      @sql + ',
ISNULL((SELECT TOP 1 CONVERT(nvarchar, pe.IntValue) FROM dbo.PeopleExtra pe WHERE pe.PeopleId = p.PeopleId AND pe.Field = ''' +  REPLACE(CAST(Field as nvarchar(100)),'''','''''') + '''),'''') [' + CAST(Field AS nvarchar(100)) + ']'
      FROM dbo.PeopleExtra pe 
JOIN dbo.People p ON pe.PeopleId = p.PeopleId
WHERE EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.Id = @tagid AND tp.PeopleId = p.PeopleId)
AND pe.IntValue IS NOT NULL
AND pe.Field NOT IN (SELECT Value FROM @Cols)
GROUP BY pe.Field

SELECT @sql = 
      @sql + ',
ISNULL((SELECT TOP 1 CONVERT(nvarchar, pe.BitValue) FROM dbo.PeopleExtra pe WHERE pe.PeopleId = p.PeopleId AND pe.Field = ''' +  REPLACE(CAST(Field as nvarchar(100)),'''', '''''') + '''),'''') [' + CAST(Field AS nvarchar(100)) + ']'
      FROM dbo.PeopleExtra pe 
JOIN dbo.People p ON pe.PeopleId = p.PeopleId
WHERE EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.Id = @tagid AND tp.PeopleId = p.PeopleId)
AND pe.BitValue IS NOT NULL
AND pe.Field NOT IN (SELECT Value FROM @Cols)
GROUP BY pe.Field

SELECT @sql = @sql + '
FROM dbo.People p
WHERE EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.Id = ' + CONVERT(nvarchar(100), @tagid) + ' AND tp.PeopleId = p.PeopleId)
GROUP By p.PeopleId) tt'

IF LEN(@sort) > 0 
	SELECT @sql = @sql + '
	ORDER BY CASE
				WHEN IsNumeric(' + @sort + ') = 1 THEN Replicate(Char(0), 100 - Len(' + @sort + ')) + ' + @sort + '
			    ELSE ' + @sort + '
		      END'PRINT @sql;

EXECUTE (@sql)

END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
