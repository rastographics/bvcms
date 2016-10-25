CREATE FUNCTION [dbo].[RecentNewVisitCount]( 
	@progid INT,
	@divid INT,
	@org INT,
	@orgtype INT,
	@days0 INT,
	@days INT)
RETURNS 
@t TABLE (PeopleId INT, Cnt INT)
AS
BEGIN
	DECLARE @orgs TABLE (OrgId INT)
	INSERT @orgs
	SELECT OrganizationId 
	FROM dbo.Organizations
	WHERE (ISNULL(@orgtype, 0) = 0 OR OrganizationTypeId = @orgtype)
	AND (ISNULL(@org, 0) = 0 OR OrganizationId = @org)
	AND (ISNULL(@divid, 0) = 0 
			OR EXISTS(SELECT NULL FROM dbo.DivOrg WHERE OrgId = OrganizationId AND DivId = @divid))
	AND (ISNULL(@progid, 0) = 0 
			OR EXISTS(SELECT NULL FROM dbo.DivOrg dd WHERE dd.OrgId = OrganizationId
				AND EXISTS(SELECT NULL FROM dbo.ProgDiv pp WHERE pp.DivId = dd.DivId AND pp.ProgId = @progid)))

	INSERT @t
	SELECT 
		p.PeopleId, 
		(SELECT COUNT(*)
			FROM dbo.Attend a
			JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
			WHERE AttendanceFlag = 1
			AND a.PeopleId = p.PeopleId
			AND a.MeetingDate >= DATEADD(dd, -@days, GETDATE())
			AND m.OrganizationId IN (SELECT OrgId FROM @orgs)
		) Cnt
	FROM dbo.People p
	WHERE NOT EXISTS
	(
		SELECT NULL FROM dbo.Attend aa
		JOIN dbo.Meetings m ON aa.MeetingId = m.MeetingId
		WHERE AttendanceFlag = 1
		AND aa.PeopleId = p.PeopleId
		AND aa.MeetingDate > DATEADD(dd, -ISNULL(@days0, 365), GETDATE())
		AND aa.MeetingDate < DATEADD(dd, -@days, GETDATE())
		AND m.OrganizationId IN (SELECT OrgId FROM @orgs)
	)
	
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
