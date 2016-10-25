CREATE FUNCTION [dbo].[UserRoleList](@uid int)
RETURNS nvarchar(500)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @roles nvarchar(500)
	select @roles = coalesce(@roles + '|', '|') + r.RoleName FROM dbo.UserRole ur JOIN dbo.Roles r ON ur.RoleId = r.RoleId
	WHERE ur.UserId = @uid

	RETURN @roles + '|'

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
