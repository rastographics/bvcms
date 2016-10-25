CREATE PROC [dbo].[AddRoleToUser](@rolename VARCHAR(50), @user VARCHAR(50))
AS
BEGIN
	INSERT dbo.UserRole ( RoleId, UserId ) VALUES ( 
		(SELECT RoleId FROM dbo.Roles WHERE RoleName = @rolename),
		(SELECT UserId FROM Users WHERE Username = @user)
	)
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
