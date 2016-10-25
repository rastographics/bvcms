CREATE VIEW [dbo].[AppRegistrations]
AS
(
	SELECT 
		o.OrganizationId
		,NULLIF(dbo.RegexMatch(o.RegSetting, '(?<=^Title:\s)(.*)$'), '') AS Title
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
	WHERE ISNULL(o.AppCategory, '') <> 'Invitation Only'

)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
