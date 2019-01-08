CREATE PROCEDURE [dbo].[OrgMembers](@oid INT, @sgFilter nvarchar(200))
AS
BEGIN

DECLARE @headers nvarchar(MAX)
SELECT @headers = 
  COALESCE(
    @headers + ',[' + cast(Name as nvarchar(200)) + ']',
    '[' + cast(Name as nvarchar(200))+ ']'
  )
FROM dbo.MemberTags mt 
JOIN dbo.OrgMemMemTags omt ON mt.Id = omt.MemberTagId 
WHERE omt.OrgId = @oid
GROUP BY mt.Name
SET @headers = ISNULL(@headers, '[No Groups]')
PRINT @headers

DECLARE @PivotTableSQL NVARCHAR(MAX)
SET @PivotTableSQL = N'
SELECT t1.*, t2.*
FROM (
  SELECT *
  FROM (
    SELECT isnull(omt.Number, 1) num, p.PeopleId, mt.Name TagName
    FROM dbo.OrganizationMembers om
    JOIN dbo.People p ON om.PeopleId = p.PeopleId
    LEFT JOIN dbo.OrgMemMemTags omt ON omt.OrgId = om.OrganizationId AND omt.PeopleId = om.PeopleId
    LEFT JOIN dbo.MemberTags mt on omt.MemberTagId = mt.Id
	WHERE OrganizationId = ' + CAST(@oid AS nvarchar(10)) + '
  ) AS pdata
  PIVOT 
  (
    sum(num)
    FOR TagName IN (
      ' + @headers + '
    )
  ) AS ptable
) t2
JOIN 
	(select p.PreferredName AS FirstName, 
			p.LastName, 
			g.Code AS Gender,
			CAST(p.Grade AS nvarchar(10)) AS Grade,
			om.ShirtSize,
			om.Request,
			ISNULL(ts.IndAmt, 0) AS Amount,
			ISNULL(ts.IndPaid, 0) AS AmountPaid,
			p.EmailAddress AS Email,
			mas.Description AS Marital,
			dbo.FmtPhone(p.HomePhone) AS HomePhone,
			dbo.FmtPhone(p.CellPhone) AS CellPhone,
			dbo.FmtPhone(p.WorkPhone) AS WorkPhone,
			CAST(p.Age AS nvarchar(10)) AS Age,
			CAST(dbo.Birthday(p.PeopleId) AS nvarchar(20)) AS Birthday,
			CONVERT(nvarchar(12), p.JoinDate, 101) AS JoinDate,
			ms.Description AS MemberStatus,
			p.SchoolOther AS School,
			CONVERT(nvarchar(12),om.LastAttended, 101) AS LastAttend,
			CAST(om.AttendPct AS nvarchar(12)) AS AttendPct,
			om.AttendStr,
			mt.Description AS MemberType,
			om.UserData AS MemberInfo, 

	(SELECT STUFF((
		SELECT CHAR(13) + ''    '' + Question + '': '' + Answer
		FROM dbo.OnlineRegQA
		WHERE type IN (''text'', ''question'')
		AND OrganizationId = om.OrganizationId
		AND PeopleId = om.PeopleId
		FOR XML PATH(''''), TYPE
	).value(''.'', ''varchar(MAX)''), 1, 1, '''')) Questions,

			CONVERT(nvarchar(12), om.EnrollmentDate, 101) AS EnrollDate,
			p.PeopleId
	from People p
	JOIN dbo.OrganizationMembers om ON p.PeopleId = om.PeopleId
	JOIN dbo.OrgMember(' + CAST(@oid AS nvarchar(10)) + ', 10, NULL, NULL, ''' + ISNULL(@sgFilter,'') + ''', 0) mm ON mm.PeopleId = p.PeopleId
	LEFT JOIN lookup.Gender g ON p.GenderId = g.Id
	LEFT JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
	LEFT JOIN lookup.MaritalStatus mas ON p.MaritalStatusId = mas.Id
	LEFT JOIN lookup.MemberType mt ON om.MemberTypeId = mt.Id
	LEFT JOIN dbo.TransactionSummary ts ON ts.OrganizationId = om.OrganizationId AND ts.PeopleId = om.PeopleId
	WHERE om.OrganizationId = ' + CAST(@oid AS nvarchar(10)) + '
	) t1 ON t1.PeopleId = t2.PeopleId
'
PRINT @PivotTableSQL

EXECUTE(@PivotTableSQL)

END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
