
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE TRIGGER [dbo].[delMeeting] 
   ON  [dbo].[Meetings]
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @oid INT, @mid INT, @att INT
	
	SELECT @mid = MeetingId, @oid = OrganizationId FROM DELETED;
	IF @oid IS NULL
		RETURN
	
	--DECLARE attendcursor CURSOR FOR
	--SELECT PeopleId, AttendanceFlag
	--FROM dbo.Attend
	--WHERE MeetingId = @mid
	
	--OPEN attendcursor;
	--FETCH NEXT FROM attendcursor INTO @pid, @att;
	
	--WHILE @@FETCH_STATUS = 0
	--BEGIN
	--	IF @att = 1
	--		EXEC dbo.RecordAttend @mid, @pid, 0
	--	FETCH NEXT FROM attendcursor INTO @pid, @att;
	--END;
	--CLOSE attendcursor;
	--DEALLOCATE attendcursor;
	
	DECLARE @dialog UNIQUEIDENTIFIER
	BEGIN DIALOG CONVERSATION @dialog
	  FROM SERVICE UpdateAttendStrService
	  TO SERVICE 'UpdateAttendStrService'
	  ON CONTRACT UpdateAttendStrContract
	  WITH ENCRYPTION = OFF;
	SEND ON CONVERSATION @dialog MESSAGE TYPE UpdateAttendStrMessage (@oid)
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
