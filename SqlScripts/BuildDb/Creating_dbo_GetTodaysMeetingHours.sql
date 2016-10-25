CREATE FUNCTION [dbo].[GetTodaysMeetingHours]
    (
      @orgid INT ,
      @thisday INT
    )
RETURNS @ta TABLE ( [hour] DateTime)
AS 
    BEGIN
        DECLARE 
			--@DefaultHour DATETIME,
            --@DefaultDay INT,
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
        
        --SELECT  @DefaultHour = MeetingTime,
        --        @DefaultDay = ISNULL(SchedDay, 0)
        --FROM    dbo.Organizations
        --WHERE   OrganizationId = @orgid

		--SET @plusdays = @DefaultDay - (DATEPART(dw, GETDATE())-1) + 7
		--IF @plusdays > 6
		--	SELECT @plusdays = @plusdays - 7
			
        DECLARE @defaultPrevMidnight DATETIME = dateadd(dd,0, datediff(dd,0,GETDATE())) + @plusdays

        --DECLARE @DefaultTime DATETIME = DATEADD(dd, -DATEDIFF(dd, 0, @DefaultHour), @DefaultHour) + @defaultPrevMidnight
        --IF (@DefaultDay = @thisday)
			--INSERT INTO @ta (hour) VALUES(@DefaultTime)
			
		INSERT INTO @ta 
			SELECT dbo.GetTodaysMeetingHour(@thisday, MeetingTime, SchedDay) 
			FROM dbo.OrgSchedule
			WHERE OrganizationId = @orgid
			AND dbo.GetTodaysMeetingHour(@thisday, MeetingTime, SchedDay) IS NOT NULL
			ORDER BY Id
        
		INSERT INTO @ta 
			SELECT MeetingDate FROM dbo.Meetings
			WHERE MeetingDate NOT IN (SELECT hour FROM @ta)
			AND OrganizationId = @orgid
			AND MeetingDate >= @prevMidnight
			AND MeetingDate < @nextMidnight
			ORDER BY MeetingDate
			
		RETURN

    END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
