CREATE VIEW [dbo].[MoveSchedule] AS
WITH movedata AS (
	SELECT FromOrgId = om.OrganizationId
	      ,om.PeopleId
		  ,p.Name
		  ,BirthDate = CONVERT(DATE, dbo.Birthday(om.PeopleId))
		  ,MosMax = (SELECT IntValue FROM dbo.OrganizationExtra 
					 WHERE Field = 'MaxMonths' 
					 AND OrganizationId = om.OrganizationId)
		  ,ToOrgId = (SELECT IntValue FROM dbo.OrganizationExtra 
					 WHERE Field = 'MoveTo' 
					 AND OrganizationId = om.OrganizationId)
	FROM dbo.OrganizationMembers om
	JOIN dbo.People p ON p.PeopleId = om.PeopleId
	WHERE om.MemberTypeId = 220
)
SELECT 
       PeopleId
      ,Name
      ,BirthDate
	  ,FromOrgId
	  ,FromOrg = f.OrganizationName
	  ,LastSunday = CONVERT(DATE, dbo.SundayForDate(DATEADD(MONTH, MosMax, BirthDate)))
      ,MosMax
      ,ToOrgId
	  ,ToOrg = t.OrganizationName
	  ,t.Location ToLocation
FROM movedata
LEFT JOIN dbo.Organizations f ON f.OrganizationId = FromOrgId
LEFT JOIN dbo.Organizations t ON t.OrganizationId = ToOrgId
WHERE ToOrgId IS NOT NULL AND MosMax IS NOT NULL



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
