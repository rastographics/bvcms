-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateETAttendPct] (@tid INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-----------------------------------------------------------------
    DECLARE @yearago DATETIME
    DECLARE @lastmeet DATETIME 
    DECLARE @tct INT 
    DECLARE @act INT
    DECLARE @pct REAL
    DECLARE @fromdt DATETIME
    DECLARE @pid INT, @orgid INT
		
	SELECT @fromdt = TransactionDate, @pid = PeopleId, @orgid = OrganizationId
	FROM dbo.EnrollmentTransaction WHERE @tid = TransactionId
		
    SELECT @lastmeet = MAX(MeetingDate) FROM dbo.Meetings
    WHERE OrganizationId = @orgid AND MeetingDate <= @fromdt
    
    SELECT @yearago = DATEADD(year,-1,@lastmeet)
    
	SELECT @tct = COUNT(*) FROM dbo.Attend
     WHERE PeopleId = @pid
       AND OrganizationId = @orgid
       AND EffAttendFlag IS NOT NULL
       AND MeetingDate >= @yearago
       AND MeetingDate <= @fromdt
       
    SELECT @act = COUNT(*) FROM dbo.Attend
     WHERE PeopleId = @pid
       AND OrganizationId = @orgid
       AND EffAttendFlag = 1
       AND MeetingDate >= @yearago
       AND MeetingDate <= @fromdt
      
       
	if @tct = 0
			select @pct = 0
		else
			SELECT @pct = @act * 100.0 / @tct
			
	--------------------------------------------	
	
		
	UPDATE dbo.EnrollmentTransaction SET
		AttendancePercentage = @pct
	WHERE TransactionId = @tid

END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
