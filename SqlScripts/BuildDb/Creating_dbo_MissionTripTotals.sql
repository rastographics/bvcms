

CREATE VIEW [dbo].[MissionTripTotals] AS
SELECT 
	tt.OrganizationId,
	tt.Trip,
	tt.PeopleId,
	tt.Name, 
	tt.SortOrder,
	tt.TripCost,
	tt.Raised,
	(ISNULL(TripCost,0) - ISNULL(Raised, 0)) Due
FROM
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
	WHERE o.IsMissionTrip = 1
	AND o.OrganizationStatusId = 30

	UNION ALL
	SELECT 
		o.OrganizationId,
		o.OrganizationName Trip,
		NULL AS PeopleId,
		'Undesignated' AS Name,
		'YZZZZ' AS SortOrder,
		NULL,
		(	SELECT SUM(gsa.Amount)
			FROM dbo.GoerSenderAmounts gsa
			WHERE GoerId IS NULL AND OrgId = o.OrganizationId AND ISNULL(InActive, 0) = 0
		) Raised
	FROM dbo.Organizations o
	WHERE o.IsMissionTrip = 1 AND o.OrganizationStatusId = 30

	UNION ALL
	SELECT 
		o.OrganizationId,
		o.OrganizationName Trip,
		NULL AS PeopleId,
		'Total' AS Name,
		'ZZZZ' AS SortOrder,
		(	SELECT SUM(ts.IndAmt)
			FROM dbo.OrganizationMembers om
			JOIN dbo.OrgMemMemTags omm ON omm.OrgId = om.OrganizationId AND omm.PeopleId = om.PeopleId
			JOIN dbo.MemberTags mt ON mt.Id = omm.MemberTagId AND mt.Name = 'Goer'
			LEFT JOIN dbo.TransactionSummary ts ON ts.OrganizationId = om.OrganizationId AND ts.PeopleId = om.PeopleId
			WHERE om.OrganizationId = o.OrganizationId
		) TripCost,
		(	SELECT SUM(gsa.Amount)
			FROM dbo.GoerSenderAmounts gsa
			WHERE OrgId = o.OrganizationId AND ISNULL(InActive, 0) = 0
		) Raised
	FROM dbo.Organizations o
	WHERE o.IsMissionTrip = 1 AND o.OrganizationStatusId = 30
) tt


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
