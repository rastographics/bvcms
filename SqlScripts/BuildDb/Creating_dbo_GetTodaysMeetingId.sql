CREATE FUNCTION [dbo].[GetTodaysMeetingId]
    (
      @orgid INT ,
      @thisday INT
    )
RETURNS INT 
AS 
    BEGIN
        DECLARE 
			@DefaultHour DATETIME,
            @DefaultDay INT,
            @prevMidnight DATETIME,
            @ninetyMinutesAgo DATETIME,
            @nextMidnight DATETIME
            
        DECLARE @dt DATETIME = GETDATE()
		DECLARE @d DATETIME = DATEADD(dd, 0, DATEDIFF(dd, 0, @dt))
		DECLARE @t DATETIME = @dt - @d
		DECLARE @simulatedTime DATETIME

        IF @thisday IS NULL
			SELECT @thisday = DATEPART(dw, GETDATE()) - 1
			
		DECLARE @plusdays INT = @thisday - (DATEPART(dw, GETDATE())-1) + 7
		IF @plusdays > 6
			SELECT @plusdays = @plusdays - 7
		SELECT @prevMidnight = dateadd(dd,0, datediff(dd,0,GETDATE())) + @plusdays
        SELECT @nextMidnight = @prevMidnight + 1
		SELECT @simulatedTime = @prevMidnight + @t
        SELECT @ninetyMinutesAgo = DATEADD(mi, -90, @simulatedTime)
        
        SELECT  @DefaultHour = MeetingTime,
                @DefaultDay = ISNULL(SchedDay, 0)
        FROM    dbo.OrgSchedule
        WHERE   OrganizationId = @orgid AND Id = 1
        
        DECLARE @meetingid INT, @meetingdate DATETIME
        
        SELECT TOP 1 @meetingid = MeetingId FROM dbo.Meetings
        WHERE OrganizationId = @orgid
        AND MeetingDate >= @ninetyMinutesAgo
        AND MeetingDate < @nextMidnight
        ORDER BY MeetingDate
        
        IF @meetingid IS NULL
			SELECT TOP 1 @meetingid = MeetingId FROM dbo.Meetings
			WHERE OrganizationId = @orgid
			AND MeetingDate >= @prevMidnight
			AND MeetingDate < @nextMidnight
			ORDER BY MeetingDate
			
			RETURN @meetingid
		--RETURN ISNULL(@meetingid, 0)

    END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
