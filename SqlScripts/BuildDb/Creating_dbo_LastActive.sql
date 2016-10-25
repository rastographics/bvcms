CREATE FUNCTION [dbo].[LastActive] (@uid INT)
RETURNS DATETIME
AS
	BEGIN
	DECLARE @dt DATETIME
		SELECT @dt = MAX(a.ActivityDate) 
		FROM dbo.ActivityLog a
		WHERE @uid = a.UserId
	RETURN @dt
	END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
