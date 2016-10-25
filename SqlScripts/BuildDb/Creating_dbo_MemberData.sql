CREATE VIEW [dbo].[MemberData]
AS
SELECT 
    p.PeopleId, 
	p.PreferredName [First],
	p.LastName [Last],
	p.Age,
	Marital.Description AS Marital,
	p.DecisionDate AS DecisionDt,
	p.JoinDate AS JoinDt,
	Decision.Code AS Decision,
	Baptism.Description AS Baptism
FROM dbo.People p 
JOIN lookup.MemberStatus Member ON Member.Id = p.MemberStatusId
left JOIN lookup.MaritalStatus Marital ON Marital.Id = p.MaritalStatusId
LEFT JOIN lookup.DecisionType Decision ON Decision.Id = p.DecisionTypeId
left JOIN lookup.BaptismStatus Baptism ON Baptism.Id = p.BaptismStatusId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
