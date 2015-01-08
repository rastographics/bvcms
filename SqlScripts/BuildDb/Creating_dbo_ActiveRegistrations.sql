

CREATE VIEW [dbo].[ActiveRegistrations]
AS

	SELECT o.OrganizationId 
	FROM dbo.Organizations o
	LEFT JOIN dbo.MasterOrgs p ON p.OrganizationId = o.OrganizationId
	WHERE p.OrganizationId IS NULL
	AND o.RegistrationTypeId > 0
	AND ISNULL(o.RegistrationClosed, 0) = 0
	AND ISNULL(o.ClassFilled, 0) = 0
	AND (o.RegStart IS NULL OR o.RegStart < GETDATE())
	AND (o.RegEnd IS NULL OR o.RegEnd > GETDATE())

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
