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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
