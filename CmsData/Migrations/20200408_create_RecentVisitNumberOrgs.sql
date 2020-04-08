DROP FUNCTION IF EXISTS dbo.RecentVisitNumberOrgs
GO
CREATE FUNCTION dbo.RecentVisitNumberOrgs
(	
	@progid INT,
	@divid INT,
	@org INT,
	@mindate DATETIME, 
	@minpersoncreatedt DATETIME
)
RETURNS TABLE 
AS
RETURN 
(
	WITH attendnumbers AS (
		SELECT a.PeopleId, MeetingDate, OrganizationId,
			ROW_NUMBER() OVER (PARTITION BY a.PeopleId ORDER BY MeetingDate) AS SeqNo
		FROM dbo.Attend a
		JOIN dbo.People p ON p.PeopleId = a.PeopleId
		WHERE AttendanceFlag = 1
			AND (ISNULL(@org, 0) = 0 OR OrganizationId = @org)
			AND (ISNULL(@divid, 0) = 0 
					OR EXISTS(SELECT NULL FROM dbo.DivOrg WHERE OrgId = OrganizationId AND DivId = @divid))
			AND (ISNULL(@progid, 0) = 0 
					OR EXISTS(SELECT NULL FROM dbo.DivOrg dd WHERE dd.OrgId = OrganizationId
						AND EXISTS(SELECT NULL FROM dbo.ProgDiv pp WHERE pp.DivId = dd.DivId AND pp.ProgId = @progid)))
		AND p.CreatedDate > @minpersoncreatedt
	)
	SELECT
		PeopleId, 
		SeqNo, 
		OrganizationId OrgId
	FROM attendnumbers
	WHERE SeqNo <= 5
	AND MeetingDate > @mindate
)
GO
