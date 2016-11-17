CREATE PROC [dbo].[CreateRole] (@role VARCHAR(50), @hardwired BIT) 
AS 
BEGIN 
	IF NOT EXISTS(SELECT NULL FROM dbo.Roles WHERE RoleName = @role) 
		INSERT dbo.Roles ( RoleName, hardwired ) VALUES  ( @role, 1 ) 
	ELSE 
		IF NOT EXISTS(SELECT NULL FROM dbo.Roles WHERE RoleName = @role AND hardwired = @hardwired ) 
			UPDATE dbo.Roles SET hardwired = @hardwired WHERE RoleName = @role 
END 
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
