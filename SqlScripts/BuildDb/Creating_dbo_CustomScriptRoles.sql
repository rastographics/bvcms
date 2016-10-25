CREATE VIEW [dbo].[CustomScriptRoles] AS 
WITH body AS (
	SELECT c.Body, CONVERT(XML, c.Body) b 
	FROM dbo.Content c
	WHERE c.Name = 'CustomReports'
), tbl AS (
	SELECT x.value('(@name)[1]', 'varchar(100)') Name
			,x.value('(@type)[1]', 'varchar(100)') [Type]
			,x.value('(@role)[1]', 'varchar(100)') [Role]
			,x.value('(@showOnOrgId)[1]', 'int') [ShowOnOrgId]
	FROM body 
	CROSS APPLY b.nodes('//Report') tt (x)
), reports AS (
	SELECT tbl.Name
          ,ISNULL(tbl.[Type], 'Custom') [Type]
		  ,tbl.ShowOnOrgId
		  ,CASE WHEN tbl.Type IS NULL THEN tbl.Role ELSE REPLACE(REPLACE(dbo.RegexMatch(ISNULL(s.Body, ''), '(?<=^(#|--)roles=)(.*)$'),CHAR(10), ''),CHAR(13), '') END [Role]
		  ,dbo.RegexMatch(ISNULL(s.Body, ''), '(?<=^(#|--)class=)(.*)$') [class]
	FROM tbl
	LEFT JOIN dbo.Content s ON s.Name = tbl.name

)
SELECT r.Name
      ,r.[Type]
      ,r.[Role]
	  ,r.ShowOnOrgId
	  ,r.class
FROM reports r

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
