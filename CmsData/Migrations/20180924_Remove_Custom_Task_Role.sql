DECLARE @RoleID INT
DECLARE @RoleName VARCHAR(100)
SET @RoleName = 'Tasks'
IF EXISTS(SELECT 1 FROM [Roles] WHERE RoleName like @RoleName )
BEGIN
	IF (SELECT COUNT(roleid) FROM [dbo].[Roles] WHERE RoleName like @RoleName) < 2
	BEGIN 	
		SET @RoleID = (select roleId from [Roles] WHERE RoleName like @RoleName)
		DELETE FROM [UserRole] WHERE RoleId = @RoleID
		DELETE FROM [Roles] WHERE RoleId like @RoleID
		PRINT N'Role ''' + @RoleName + ''' deleted.';
	END
	ELSE
		PRINT N'Operation not valid, more than 2 roles to delete.';  
END
ELSE
	PRINT N'Role ''' + @RoleName + ''' not found.';
 
GO
