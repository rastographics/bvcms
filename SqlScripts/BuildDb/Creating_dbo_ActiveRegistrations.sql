
CREATE VIEW [dbo].[ActiveRegistrations]
AS

	SELECT o.OrganizationId 
	FROM dbo.Organizations o
	LEFT JOIN dbo.MasterOrgs p ON p.PickListOrgId = o.OrganizationId
	WHERE p.OrganizationId IS NULL
	AND o.RegistrationTypeId > 0
	AND ISNULL(o.RegistrationClosed, 0) = 0
	AND ISNULL(o.ClassFilled, 0) = 0
	AND (o.RegStart IS NULL OR o.RegStart <= DATEADD(DAY, 21, GETDATE()))
	AND (o.RegEnd IS NULL OR o.RegEnd > GETDATE())


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
