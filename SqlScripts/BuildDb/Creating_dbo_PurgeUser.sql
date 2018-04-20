CREATE PROCEDURE [dbo].[PurgeUser](@uid INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE dbo.UserRole
	FROM dbo.UserRole ur
	JOIN dbo.Users u ON u.UserId = ur.UserId
	WHERE u.UserId = @uid
	
	DELETE dbo.ActivityLog
	FROM dbo.ActivityLog a
	JOIN dbo.Users u ON a.UserId = u.UserId
	WHERE u.UserId = @uid
	
	delete dbo.Preferences
	FROM dbo.Preferences p
	JOIN dbo.Users u ON u.UserId = p.UserId
	WHERE u.UserId = @uid
	
	DELETE dbo.VolunteerForm
	FROM dbo.VolunteerForm vf
	JOIN dbo.Users u ON vf.UploaderId = u.UserId
	WHERE u.UserId = @uid
	
	DELETE dbo.Coupons
	FROM dbo.Coupons c
	JOIN dbo.Users u ON c.UserId = u.UserId
	WHERE u.UserId = @uid
    
	DELETE 
	FROM dbo.ApiSession
	WHERE UserId = @uid

	DELETE dbo.SMSGroupMembers
	FROM dbo.SMSGroupMembers m
	JOIN dbo.Users u ON u.UserId = m.UserID
	WHERE u.UserId = @uid
	
	DELETE dbo.Users
	WHERE UserId = @uid

	DELETE
	FROM dbo.MobileAppDevices
	WHERE userID = @uid
	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
