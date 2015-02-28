CREATE VIEW [dbo].[AppRegistrations]
AS
(
	SELECT 
		o.OrganizationId
		,NULLIF((SELECT dbo.RegexMatch(rr.RegSetting, '(?<=^Title:\s)(.*)$')
			FROM dbo.Organizations rr WHERE rr.OrganizationId = o.OrganizationId),'') Title
		,o.OrganizationName
		,o.[Description]
		,CASE WHEN ISNULL(o.AppCategory, '') <> '' THEN o.AppCategory ELSE 'Other' END AppCategory
		,o.PublicSortOrder
		,o.UseRegisterLink2
		,o.RegStart
		,o.RegEnd
	FROM dbo.Organizations o
	JOIN dbo.ActiveRegistrations ON ActiveRegistrations.OrganizationId = o.OrganizationId
	AND RegStart IS NOT NULL
	AND RegEND > GETDATE()
	AND o.OrganizationStatusId = 30
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
