CREATE FUNCTION [dbo].[GetTodaysMeetingHours3]
    (
      @thisday INT
    )
RETURNS @ta TABLE ( [hour] DATETIME, OrganizationId INT)
AS 
    BEGIN
        DECLARE 
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
        
        DECLARE @defaultPrevMidnight DATETIME = dateadd(dd,0, datediff(dd,0,GETDATE())) + @plusdays

		INSERT INTO @ta 
			SELECT dbo.GetTodaysMeetingHour(@thisday, MeetingTime, SchedDay), o.OrganizationId 
			FROM dbo.OrgSchedule os
			JOIN dbo.Organizations o ON os.OrganizationId = o.OrganizationId
			WHERE dbo.GetTodaysMeetingHour(@thisday, MeetingTime, SchedDay) IS NOT NULL
			AND ISNULL(o.NotWeekly, 0) = 0
			ORDER BY Id
        
		INSERT INTO @ta 
			SELECT MeetingDate, OrganizationId
			FROM dbo.Meetings m
			WHERE MeetingDate NOT IN (SELECT hour FROM @ta t WHERE t.OrganizationId = m.OrganizationId)
			AND MeetingDate >= @prevMidnight
			AND MeetingDate < @nextMidnight
			ORDER BY MeetingDate
			
		RETURN

    END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
