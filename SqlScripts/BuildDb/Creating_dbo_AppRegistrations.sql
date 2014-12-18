CREATE VIEW [dbo].[AppRegistrations]
AS
(
	SELECT 
		o.OrganizationId
		,NULLIF((SELECT dbo.RegexMatch(rr.RegSetting, '(?<=^Title:\s)(.*)$')
			FROM dbo.Organizations rr WHERE rr.OrganizationId = o.OrganizationId),'') Title
		,o.OrganizationName
		,o.[Description]
		,o.PublicSortOrder
		,o.UseRegisterLink2
	FROM dbo.Organizations o
	LEFT JOIN dbo.MasterOrgs mo ON mo.PickListOrgId = o.OrganizationId
	WHERE mo.OrganizationId IS NULL
	AND RegistrationTypeId > 0
	AND ISNULL(RegistrationClosed, 0) = 0
	AND ISNULL(ClassFilled, 0) = 0
	AND (RegEND IS NULL OR RegEND > GETDATE())
	AND LEN(ISNULL(PublicSortOrder, '')) > 0
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
