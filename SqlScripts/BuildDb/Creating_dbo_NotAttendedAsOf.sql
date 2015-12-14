CREATE FUNCTION [dbo].[NotAttendedAsOf]( 
	@progid INT,
	@divid INT,
	@org INT,
	@dt1 DATETIME, 
	@dt2 DATETIME,
	@guestonly BIT
	)
RETURNS TABLE
AS
RETURN
(
	SELECT p.PeopleId
	FROM dbo.People p
	LEFT JOIN dbo.AttendedAsOf(@progid, @divid, @org, @dt2, @dt2, @guestonly) a ON a.PeopleId = p.PeopleId
	WHERE a.PeopleId IS NULL
)


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
