CREATE PROCEDURE [dbo].[RecordAttend2]
( @orgid INT, @mdt DATETIME, @pid INT)
AS
BEGIN
	DECLARE @mid INT
	
	EXEC @mid = dbo.CreateMeeting @orgid, @mdt
	EXEC  dbo.RecordAttend @mid, @pid, 1
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
