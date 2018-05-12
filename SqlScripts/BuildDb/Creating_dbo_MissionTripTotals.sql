CREATE VIEW [dbo].[MissionTripTotals] AS
with trips as (
	select 
		o.OrganizationId,
		o.OrganizationName Trip,
		p.PeopleId,
		p.Name,
		p.Name2 SortOrder,
		ts.IndAmt TripCost,
		dbo.TotalPaid(om.OrganizationId, om.PeopleId) Raised
	from dbo.Organizations o
	join dbo.OrganizationMembers om on om.OrganizationId = o.OrganizationId
	join dbo.OrgMemMemTags omm on omm.OrgId = om.OrganizationId and omm.PeopleId = om.PeopleId
	join dbo.MemberTags mt on mt.Id = omm.MemberTagId and mt.Name = 'Goer'
	join dbo.People p on p.PeopleId = om.PeopleId
	left join dbo.TransactionSummary ts on ts.OrganizationId = om.OrganizationId and ts.PeopleId = om.PeopleId
	where o.IsMissionTrip = 1
	and o.OrganizationStatusId = 30

	union all

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
),
report as (
	select OrganizationId , Trip , PeopleId , Name , SortOrder , TripCost , Raised
	from trips

	union all 

	select g.OrganizationId, g.Trip, null as PeopleId, 'Total' as [Name], 'ZZZZZ' as SortOrder, sum(isnull(g.TripCost, 0)) as TripCost, sum(g.Raised) as Raised
	from trips g
	group by OrganizationId, Trip
)
select *, (ISNULL(TripCost,0) - ISNULL(Raised, 0)) Due
from report

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
