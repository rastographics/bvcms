CREATE PROC [dbo].[AddRoleToUser](@rolename VARCHAR(50), @user VARCHAR(50))
AS
BEGIN
	INSERT dbo.UserRole ( RoleId, UserId ) VALUES ( 
		(SELECT RoleId FROM dbo.Roles WHERE RoleName = @rolename),
		(SELECT UserId FROM Users WHERE Username = @user)
	)
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
