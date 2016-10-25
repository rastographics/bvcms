CREATE FUNCTION [dbo].[CheckinByDate](@dt DATETIME)
RETURNS @tbl TABLE ( PeopleId INT, [Name] VARCHAR(100), OrgId INT, OrgName VARCHAR(100), [time] TIME, present BIT) 
AS
BEGIN
	DECLARE @t TABLE(dt DATETIME, pid INT, oid INT, att varchar(10))
	INSERT @t(dt, pid, oid, att)
		SELECT 
		CONVERT(TIME, ActivityDate),
		dbo.RegexMatch(Activity,'(?<=checkin\s)\d+') as pid,
		dbo.RegexMatch(Activity,'(?<=checkin\s\d+,\s)\d+') as oid,
		dbo.RegexMatch(Activity,'(?<=checkin\s\d+,\s\d+,\s)\w*') as att
	FROM dbo.ActivityLog
	WHERE ActivityDate >= @dt
	AND ActivityDate < DATEADD(HOUR, 24, @dt)
	AND Activity LIKE 'checkin %attend'

	INSERT @tbl(PeopleId, Name, OrgId, OrgName, [time], present)
	SELECT 
		p.PeopleId,
		p.Name, 
		o.OrganizationId,
		o.OrganizationName, 
		dt,
		CASE WHEN t.att = 'attend' THEN 1 ELSE 0 END present
	FROM @t t
	JOIN dbo.People p ON p.PeopleId = t.pid
	JOIN dbo.Organizations o ON o.OrganizationId = t.oid
	ORDER BY dt
	RETURN
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
