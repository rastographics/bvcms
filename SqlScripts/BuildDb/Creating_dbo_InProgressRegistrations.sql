--DECLARE @pid INT = 0


CREATE VIEW [dbo].[InProgressRegistrations] AS 
(
	SELECT 
		p.Name
        ,o.OrganizationName
        ,e.Stamp
		,p.PeopleId
		,o.OrganizationId
        ,e.Id RegDataId
	FROM dbo.OrgSearch('regsetting:AllowSaveProgress', NULL, NULL, NULL, NULL, NULL, 30, 96, NULL, NULL) o
	JOIN dbo.RegistrationData e ON e.OrganizationId = o.OrganizationId
	JOIN dbo.People p ON p.PeopleId = e.UserPeopleId
	WHERE e.Stamp > ISNULL(o.RegStart, DATEADD(DAY, -30, GETDATE()))
	AND ISNULL(e.abandoned, 0) = 0
	AND ISNULL(e.completed, 0) = 0
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
