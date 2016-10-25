CREATE FUNCTION [dbo].[RecentIncompleteRegistrations2]
(	
	@orgs VARCHAR(MAX) 
	, @days INT
)
RETURNS 
@t TABLE 
(
	PeopleId INT,
	OrgId INT,
	DatumId INT,
	Stamp DATETIME,
	OrgName NVARCHAR(80),
	Name NVARCHAR(80)
)
AS
BEGIN
	DECLARE @tt TABLE (oid INT, pid1 INT, pid2 INT, pid3 INT, pid4 INT, stamp DATETIME, completed BIT, datumid INT)
	DECLARE @enddt DATETIME = GETDATE()
	DECLARE @begdt DATETIME = DATEADD(DAY, -ISNULL(@days, 90), @enddt)

	INSERT @tt ( oid , pid1 , pid2 , pid3 , pid4 , stamp , completed, datumid )
	SELECT r.OrganizationId, PeopleId1, PeopleId2, PeopleId3, PeopleId4, Stamp, completed, Id
	FROM dbo.RegistrationList r
	JOIN dbo.Organizations o ON o.OrganizationId = r.OrganizationId
	JOIN dbo.SplitInts(@orgs) s ON s.Value = o.OrganizationId
	WHERE expired = 0
	AND r.OrganizationId IS NOT NULL
	
	;WITH registrations AS (
		SELECT r.oid, r.pid1 pid, r.datumid, stamp, r.completed 
		FROM @tt r
		WHERE r.pid1 IS NOT NULL
		UNION
		SELECT r.oid, r.pid2, r.datumid, stamp, r.completed 
		FROM @tt r
		WHERE r.pid2 IS NOT NULL
		UNION
		SELECT r.oid, r.pid3, r.datumid, stamp, r.completed 
		FROM @tt r
		WHERE r.pid3 IS NOT NULL
		UNION
		SELECT r.oid, r.pid4, r.datumid, stamp, r.completed 
		FROM @tt r
		WHERE r.pid4 IS NOT NULL
	), 
	oidpids AS (
		SELECT oid, pid, MAX(r.datumid) datumid, MAX(stamp) stamp, o.OrganizationName, p.Name2
		FROM registrations r
		JOIN dbo.Organizations o ON o.OrganizationId = r.oid
		JOIN dbo.People p ON p.PeopleId = r.pid
		GROUP BY oid, pid, o.OrganizationName, p.Name2
		HAVING MAX(CAST(ISNULL(completed, 0) AS INT)) = 0
	)
	INSERT @t
	        ( PeopleId, OrgId, DatumId, Stamp, OrgName, Name )
	SELECT pid, oid, op.datumid, stamp, op.OrganizationName, op.Name2
	FROM oidpids op
	LEFT JOIN dbo.OrganizationMembers om ON om.PeopleId = op.pid AND om.OrganizationId = op.oid
	WHERE om.PeopleId IS NULL
	AND op.stamp >= @begdt AND op.stamp <= @enddt
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
