

CREATE VIEW [dbo].[UserLeaders]
AS
SELECT 
	p.PeopleId
	, p.Name 
	, u.Username
	, u.UserId
	, Access = CASE WHEN EXISTS(
			SELECT NULL
			FROM dbo.UserRole ur
			JOIN dbo.Roles r ON r.RoleId = ur.RoleId
			WHERE r.RoleName = 'Access'
			AND ur.UserId = u.UserId
	) THEN 'Access' ELSE NULL END 
	, OrgLeaderOnly = CASE WHEN EXISTS(
			SELECT NULL
			FROM dbo.UserRole ur
			JOIN dbo.Roles r ON r.RoleId = ur.RoleId
			WHERE r.RoleName = 'Access'
			AND ur.UserId = u.UserId
	) THEN 'OrgLeadersOnly' ELSE NULL END 
FROM dbo.People p
LEFT JOIN dbo.Users u ON u.PeopleId = p.PeopleId


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
