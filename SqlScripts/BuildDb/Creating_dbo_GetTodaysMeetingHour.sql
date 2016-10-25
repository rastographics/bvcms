CREATE FUNCTION [dbo].[GetTodaysMeetingHour]
    (
      @thisday INT,
      @MeetingTime DATETIME,
      @SchedDay INT
    )
RETURNS DATETIME
AS 
    BEGIN
        DECLARE 
			@DefaultHour DATETIME,
            @DefaultDay INT,
            @prevMidnight DATETIME
            
        IF @thisday IS NULL
			SELECT @thisday = DATEPART(dw, GETDATE()) - 1
			
		DECLARE @plusdays INT = @thisday - (DATEPART(dw, GETDATE())-1) + 7
		IF @plusdays > 6
			SELECT @plusdays = @plusdays - 7
		SELECT @prevMidnight = dateadd(dd,0, datediff(dd,0,GETDATE())) + @plusdays
        
        SELECT  @DefaultHour = @MeetingTime,
                @DefaultDay = ISNULL(@SchedDay, 0)
        
        DECLARE @meetingdate DATETIME
        
		DECLARE @DefaultTime DATETIME = DATEADD(dd, -DATEDIFF(dd, 0, @DefaultHour), @DefaultHour)
		
		IF @DefaultDay = @thisday OR @DefaultDay = 10
			SELECT @meetingdate = @prevMidnight + @DefaultTime
					
        RETURN @meetingdate
    END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
