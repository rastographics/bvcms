CREATE PROC [dbo].[AddUserFromOtherData2](@name VARCHAR(50), @user VARCHAR(50), @password VARCHAR(100), @fieldname VARCHAR(50), @id VARCHAR(50), @roles VARCHAR(500))
AS
BEGIN
	IF EXISTS(SELECT PeopleId FROM dbo.PeopleExtra WHERE Field = @fieldname AND Data = @id)
	BEGIN
		INSERT  INTO dbo.Users ( Username ,[Password] , TempPassword,Name ,PeopleId, MustChangePassword, IsApproved )
		VALUES  (@user, 'thisisanencryptedpassword' ,@password ,@name
		        ,( SELECT PeopleId FROM dbo.PeopleExtra
		           WHERE Field = @fieldname AND Data = @id ), 1, 1)

		DECLARE @userid INT = @@IDENTITY

		INSERT dbo.UserRole ( RoleId, UserId )
		SELECT r.RoleId, @userid
		FROM dbo.Split(@roles, ',') rr
		JOIN dbo.Roles r ON r.RoleName = rr.Value
	END

END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
