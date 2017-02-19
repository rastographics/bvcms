CREATE VIEW [dbo].[CustomMenuRoles] AS 
WITH body AS (
	SELECT c.Body, CONVERT(XML, Replace(c.Body, 'encoding="utf-8', 'encoding="utf-16')) b 
	FROM dbo.Content c
	WHERE c.Name = 'CustomReportsMenu'
), tbl1 AS (
	SELECT x.value('(@link)[1]', 'varchar(100)') Link
			,x.value('(@role)[1]', 'varchar(100)') [Role]
	FROM body 
	CROSS APPLY b.nodes('/ReportsMenu/Column1/*') tt (x)
), tbl2 AS (
	SELECT x.value('(@link)[1]', 'varchar(100)') Link
			,x.value('(@role)[1]', 'varchar(100)') [Role]
	FROM body 
	CROSS APPLY b.nodes('/ReportsMenu/Column2/*') tt (x)
), tbl AS (
	SELECT Link, Role, 1 Col FROM tbl1
	UNION
	SELECT Link, Role, 2 Col FROM tbl2
), withroles AS (
	SELECT tbl.Link
		,tbl.Role
		,dbo.RegexMatch(tbl.Link, '/(RunScript|PyScript)/(?<group>[^/\r\n]*)') Name
		,tbl.Col
	FROM tbl
	WHERE tbl.Link IS NOT NULL
), reports AS (
	SELECT t.Link
          ,t.Name
		  ,t.Col
		  ,CASE WHEN t.Role IS NOT NULL THEN t.Role ELSE dbo.RegexMatch(s.Body, '^(--|#)roles=(?<group>[A-Z0-9,]*)') END [Role]
	FROM withroles t
	LEFT JOIN dbo.Content s ON s.Name = t.name
)
SELECT r.Link
      ,r.[Role]
	  ,r.Name
	  ,r.Col
FROM reports r
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
