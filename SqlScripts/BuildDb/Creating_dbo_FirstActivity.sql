CREATE FUNCTION [dbo].[FirstActivity] ()
RETURNS DATETIME
AS
BEGIN
	DECLARE @Result DATETIME

	SELECT TOP 1 @Result = ActivityDate
	FROM dbo.ActivityLog a
	JOIN dbo.Users u ON u.UserId = a.UserId
	JOIN dbo.People p ON p.PeopleId = u.PeopleId
	WHERE p.EmailAddress NOT LIKE '%bvcms.com%'
	ORDER BY Id	

	RETURN @Result

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
