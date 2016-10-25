CREATE VIEW [dbo].[InProgressRegistrations] AS 
(
	SELECT 
		p.Name
        ,o.OrganizationName
        ,e.Stamp
		,p.PeopleId
		,o.OrganizationId
        ,e.Id RegDataId
	FROM dbo.Organizations o 
	JOIN dbo.RegistrationData e ON e.OrganizationId = o.OrganizationId
	JOIN dbo.People p ON p.PeopleId = e.UserPeopleId
	WHERE e.Stamp > ISNULL(o.RegStart, DATEADD(DAY, -30, GETDATE()))
	AND ISNULL(e.abandoned, 0) = 0
	AND ISNULL(e.completed, 0) = 0
	AND RegSettingXml IS NOT NULL
	AND o.OrganizationStatusId = 30
	AND ISNULL(o.RegistrationClosed, 0) = 0
	AND ISNULL(o.ClassFilled, 0) = 0
	AND (o.RegStart IS NULL OR o.RegStart <= DATEADD(DAY, 21, GETDATE()))
	AND (o.RegEnd IS NULL OR o.RegEnd > GETDATE())
	AND RegSettingXml.value('(//AllowSaveProgress)[1]', 'bit') = 1
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
