
CREATE VIEW [dbo].[OrganizationStructure]
AS
SELECT 
	 p.Name Program 
	,d.Name Division 
	,os.Description OrgStatus
	,o.OrganizationName Organization
	,(SELECT COUNT(*) FROM dbo.OrganizationMembers om WHERE om.OrganizationId = o.OrganizationId) Members
	,(SELECT COUNT(*) FROM dbo.EnrollmentTransaction et 
		WHERE et.OrganizationId = o.OrganizationId 
		AND et.TransactionTypeId = 5 
		AND NOT EXISTS (SELECT NULL FROM dbo.OrganizationMembers 
			WHERE OrganizationId = o.OrganizationId AND PeopleId = et.PeopleId) 
	 ) Previous
	,(SELECT COUNT(*) FROM dbo.Attend a WHERE a.OrganizationId = o.OrganizationId AND a.AttendanceFlag = 1 AND AttendanceTypeId IN (40,50,60)) Vistors
	,(SELECT COUNT(*) FROM dbo.Meetings m WHERE m.OrganizationId = o.OrganizationId) Meetings
	,p.Id ProgId
	,d.Id DivId
	,o.OrganizationId OrgId
FROM dbo.Program p
JOIN dbo.ProgDiv pd ON p.Id = pd.ProgId
JOIN dbo.Division  d ON d.Id = pd.DivId
JOIN dbo.DivOrg do ON pd.DivId = do.DivId
JOIN dbo.Organizations o ON do.OrgId = o.OrganizationId
JOIN lookup.OrganizationStatus os ON o.OrganizationStatusId = os.Id

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
