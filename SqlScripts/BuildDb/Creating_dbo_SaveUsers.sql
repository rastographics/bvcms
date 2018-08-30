-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
CREATE PROCEDURE [dbo].[SaveUsers]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT Name, EmailAddress, 
		LEFT(Roles, LEN(Roles) -1) AS Roles
	FROM (SELECT DISTINCT Name, EmailAddress,
			(SELECT RoleName + ',' AS [text()] 
			FROM Roles r
			JOIN dbo.UserRole ur ON r.RoleId = ur.RoleId
			WHERE ur.UserId = u.UserId
			FOR XML PATH ('')) Roles
	FROM dbo.Users u
	WHERE EmailAddress NOT LIKE '%@touchpointsoftware.com'
	) tt

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
