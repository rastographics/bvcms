CREATE VIEW [dbo].[MissionTripTotals] AS
WITH funding AS
(
	SELECT 
		o.OrganizationId,
		o.OrganizationName Trip,
		p.PeopleId,
		p.Name,
		p.Name2 SortOrder,
		ts.IndAmt TripCost,
		dbo.TotalPaid(om.OrganizationId, om.PeopleId) Raised
	FROM dbo.Organizations o
	JOIN dbo.OrganizationMembers om ON om.OrganizationId = o.OrganizationId
	JOIN dbo.OrgMemMemTags omm ON omm.OrgId = om.OrganizationId AND omm.PeopleId = om.PeopleId
	JOIN dbo.MemberTags mt ON mt.Id = omm.MemberTagId AND mt.Name = 'Goer'
	JOIN dbo.People p ON p.PeopleId = om.PeopleId
	LEFT JOIN dbo.TransactionSummary ts ON ts.OrganizationId = om.OrganizationId AND ts.PeopleId = om.PeopleId
	WHERE o.IsMissionTrip = 1 AND o.OrganizationStatusId = 30
),
undesig AS (
	SELECT 
		o.OrganizationId,
		o.OrganizationName Trip,
		NULL AS PeopleId,
		'Undesignated' AS Name,
		'YZZZZ' AS SortOrder,
		NULL TripCost,
		(	SELECT SUM(gsa.Amount)
			FROM dbo.GoerSenderAmounts gsa
			WHERE GoerId IS NULL AND OrgId = o.OrganizationId AND ISNULL(InActive, 0) = 0
		) Raised
	FROM dbo.Organizations o
	WHERE o.IsMissionTrip = 1 AND o.OrganizationStatusId = 30
),
total AS (
	SELECT 
		o.OrganizationId,
		o.OrganizationName Trip,
		NULL AS PeopleId,
		'Total' AS Name,
		'ZZZZ' AS SortOrder,
		(SELECT SUM(funding.TripCost) FROM funding WHERE funding.OrganizationId = o.OrganizationId) TripCost,
		(SELECT SUM(funding.Raised) FROM funding WHERE funding.OrganizationId = o.OrganizationId) Raised
	FROM dbo.Organizations o
	WHERE o.IsMissionTrip = 1 AND o.OrganizationStatusId = 30
),
final AS (
SELECT f.OrganizationId, f.Trip, f.PeopleId, f.Name, f.SortOrder, f.TripCost, f.Raised, ISNULL(f.TripCost, 0) - ISNULL(f.Raised, 0) Due FROM funding f
UNION ALL
SELECT u.OrganizationId, u.Trip, u.PeopleId, u.Name, u.SortOrder, u.TripCost, u.Raised, ISNULL(u.TripCost, 0) - ISNULL(u.Raised, 0) Due FROM undesig u
UNION ALL
SELECT t.OrganizationId, t.Trip, t.PeopleId, t.Name, t.SortOrder, t.TripCost, t.Raised, ISNULL(t.TripCost, 0) - ISNULL(t.Raised, 0) Due FROM total t
)
SELECT * FROM final f
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
