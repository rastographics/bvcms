CREATE FUNCTION [dbo].[TPStats] (@emails VARCHAR(MAX))
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		nrecs = (SELECT COUNT(*) FROM dbo.People)
		,nmembers = (SELECT COUNT(*) FROM dbo.People WHERE MemberStatusId = 10)
		,olgive = CASE WHEN EXISTS(
					SELECT NULL 
					FROM dbo.ActivityLog 
					WHERE Activity LIKE '%giving%' 
					AND ActivityDate > DATEADD(DAY, -30, GETDATE())
				) THEN CAST(1 AS BIT) ELSE 0 END
		,give = CASE WHEN (
					SELECT COUNT(*) 
					FROM dbo.Contribution 
					WHERE ContributionDate > DATEADD(DAY, -60, GETDATE())
				) > 20 THEN CAST(1 AS BIT) ELSE 0 END
		,wag = CASE WHEN ( SELECT COUNT(*) FROM dbo.Division WHERE ReportLine > 0 ) > 2 
						AND EXISTS(SELECT NULL FROM dbo.Program WHERE EndHoursOffset > 0)
					THEN 1 ELSE 0 
			   END
		,checkin = CASE WHEN (
					SELECT COUNT(*) 
					FROM dbo.ActivityLog 
					WHERE Activity LIKE 'checkin %' 
					AND ActivityDate > DATEADD(DAY, -7, GETDATE())
				) > 20 THEN CAST(1 AS BIT) ELSE 0 END
		,[lastdt] = ISNULL(( SELECT TOP 1 ActivityDate
					FROM dbo.ActivityLog a
					JOIN dbo.Users u ON u.UserId = a.UserId
					WHERE NOT EXISTS(SELECT NULL FROM dbo.Split(@emails, ',') WHERE u.EmailAddress LIKE Value)
					ORDER BY a.ActivityDate DESC
				), '1/1/1900')
		,[lastp] = ISNULL(( SELECT TOP 1 u.Name2
					FROM dbo.ActivityLog a
					JOIN dbo.Users u ON u.UserId = a.UserId
					WHERE NOT EXISTS(SELECT NULL FROM dbo.Split(@emails, ',') WHERE u.EmailAddress LIKE Value)
					ORDER BY a.ActivityDate DESC
				), '(null)')
		,[nlogs] = (SELECT COUNT(*)
					FROM dbo.ActivityLog a
					JOIN dbo.Users u ON u.UserId = a.UserId
					WHERE NOT EXISTS(SELECT NULL FROM dbo.Split(@emails, ',') WHERE u.EmailAddress LIKE Value)
					AND a.ActivityDate > DATEADD(DAY, -7, GETDATE())
				)
		,[nlogs30] = (	SELECT COUNT(*)
						FROM dbo.ActivityLog a
						JOIN dbo.Users u ON u.UserId = a.UserId
						WHERE NOT EXISTS(SELECT NULL FROM dbo.Split(@emails, ',') WHERE u.EmailAddress LIKE Value)
						AND a.ActivityDate > DATEADD(DAY, -30, GETDATE())
					)
		,norgs = (SELECT COUNT(*) FROM dbo.Organizations)
		,created = (SELECT CONVERT(DATETIME, Setting) 
					FROM dbo.Setting
					WHERE Id = 'DbCreatedDate'
				)
		,converted = (SELECT CONVERT(DATETIME, Setting) 
					FROM dbo.Setting
					WHERE Id = 'DbConvertedDate'
				)
		,nusers = (	SELECT COUNT(*)
					FROM dbo.Users u
					WHERE NOT EXISTS(SELECT NULL FROM dbo.Split(@emails, ',') WHERE u.EmailAddress LIKE Value)
					AND EXISTS(
							SELECT NULL 
							FROM dbo.UserRole ur
							JOIN dbo.Roles r ON r.RoleId = ur.RoleId
							WHERE ur.UserId = u.UserId
							AND r.RoleName = 'Access'
						)
				)
		,nmydata = (SELECT COUNT(*)
					FROM dbo.Users u
					WHERE NOT EXISTS(SELECT NULL FROM dbo.Split(@emails, ',') WHERE u.EmailAddress LIKE Value)
					AND NOT EXISTS(
							SELECT NULL 
							FROM dbo.UserRole ur
							JOIN dbo.Roles r ON r.RoleId = ur.RoleId
							WHERE ur.UserId = u.UserId
							AND r.RoleName = 'Access'
						)
				)
		,nadmins = (SELECT COUNT(*)
					FROM dbo.Users u
					WHERE NOT EXISTS(SELECT NULL FROM dbo.Split(@emails, ',') WHERE u.EmailAddress LIKE Value)
					AND EXISTS(
							SELECT NULL 
							FROM dbo.UserRole ur
							JOIN dbo.Roles r ON r.RoleId = ur.RoleId
							WHERE ur.UserId = u.UserId
							AND r.RoleName = 'Admin'
						)
				)
		,reg = CASE WHEN EXISTS(
					SELECT NULL
					FROM dbo.OrganizationMembers om
					JOIN dbo.Organizations o ON o.OrganizationId = om.OrganizationId
					WHERE om.EnrollmentDate > DATEADD(DAY, -120, GETDATE())
					AND o.RegistrationTypeId > 0
				) THEN CAST(1 AS BIT) ELSE 0 END
		,firstactive = (SELECT TOP 1 a.ActivityDate
					FROM dbo.ActivityLog a
					JOIN dbo.Users u ON u.UserId = a.UserId
					WHERE NOT EXISTS(SELECT NULL FROM dbo.Split(@emails, ',') WHERE u.EmailAddress LIKE Value)
					ORDER BY a.ActivityDate)
		,notam = CASE WHEN EXISTS(
					SELECT NULL 
					FROM dbo.Content
					WHERE Name = 'notam'
				) THEN CAST(1 AS BIT) ELSE 0 END
		,campuses = (SELECT ISNULL(NULLIF(MIN(Cnt), 0), 1)
					 FROM (
						SELECT cnt = COUNT(*)
						FROM (
							SELECT COUNT(*) Cnt , CampusId, c.Description Campus
							FROM dbo.People p
							JOIN lookup.Campus c ON c.Id = p.CampusId
							GROUP BY CampusId, c.Description
							HAVING COUNT(*) > 50
						) tt
						UNION 
						SELECT COUNT(*)
						FROM (
							SELECT COUNT(*) Cnt , CampusId, c.Description Campus
							FROM dbo.Organizations o
							JOIN lookup.Campus c ON c.Id = o.CampusId
							GROUP BY CampusId, c.Description
							HAVING COUNT(*) > 5
						) tt) ttt)
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
