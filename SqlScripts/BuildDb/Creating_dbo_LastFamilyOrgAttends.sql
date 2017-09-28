CREATE FUNCTION [dbo].[LastFamilyOrgAttends](@progid INT, @divid INT, @orgid INT, @position INT)
RETURNS TABLE 
AS
RETURN 
(
	WITH orgs AS (
		SELECT OrganizationId
		FROM dbo.Organizations o
		WHERE (o.OrganizationId = @orgid OR @orgid = 0)
		AND (EXISTS(SELECT NULL FROM DivOrg d1
				WHERE d1.OrgId = o.OrganizationId
				AND d1.DivId = @divid) OR @divid = 0)
		AND (EXISTS(SELECT NULL FROM DivOrg d2
				WHERE d2.OrgId = o.OrganizationId
				AND EXISTS(SELECT NULL FROM ProgDiv pd
						WHERE d2.DivId = pd.DivId
						AND pd.ProgId = @progid)) OR @progid = 0)
	),
	lastattends AS (
		SELECT p.FamilyId, MAX(a.MeetingDate) lastattend
					FROM dbo.Attend a
					JOIN orgs oo ON oo.OrganizationId = a.OrganizationId
					JOIN dbo.People p ON p.PeopleId = a.PeopleId 
					WHERE a.AttendanceFlag = 1
					AND (@position IS NULL OR p.PositionInFamilyId = @position)
		GROUP BY p.FamilyId
	)
	SELECT la.FamilyId, p.PeopleId, la.lastattend 
	FROM dbo.People p
	JOIN lastattends la ON la.FamilyId = p.FamilyId
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
