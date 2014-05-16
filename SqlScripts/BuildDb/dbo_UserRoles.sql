

CREATE VIEW [dbo].[UserRoles]
AS
SELECT Name, EmailAddress, 
	LEFT(Roles, LEN(Roles) -1) AS Roles
FROM (SELECT DISTINCT Name, EmailAddress,
		(SELECT RoleName + ',' AS [text()] 
		FROM Roles r
		JOIN dbo.UserRole ur ON r.RoleId = ur.RoleId
		WHERE ur.UserId = u.UserId
		FOR XML PATH ('')) Roles
FROM dbo.Users u
WHERE EmailAddress NOT IN ('david@bvcms.com','karen@bvcms.com', 'support@bvcms.com')
) tt

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
