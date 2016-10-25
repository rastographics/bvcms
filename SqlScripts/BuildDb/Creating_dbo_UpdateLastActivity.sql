CREATE PROCEDURE [dbo].[UpdateLastActivity] ( @userid INT )
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    -- Insert statements for procedure here
    UPDATE dbo.Users
    SET LastActivityDate = GETDATE() WHERE UserId = @userid
    -- changed
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
