CREATE PROCEDURE [dbo].[UpdateAllAttendStrBig] (@orgid INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @start DATETIME = CURRENT_TIMESTAMP;
	DECLARE @pid INT, @n INT
	
	DECLARE @a nvarchar(200) = '', -- attendance string
			@mindt DATETIME, 
			@dt DATETIME,
			@tct INT, -- total count
			@act INT, -- attended count
			@pct REAL,
			@lastattend DATETIME
    DECLARE @yearago DATETIME,
			@lastmeet DATETIME,
			@maxfuturemeeting DATETIME,
			@tzoffset INT,
			@earlycheckinhours INT = 10 -- to include future meetings
			
	SELECT @tzoffset = CONVERT(INT, Setting) FROM dbo.Setting WHERE Id = 'TZOffset'
	SELECT @maxfuturemeeting = DATEADD(hh ,ISNULL(@tzoffset,0) + @earlycheckinhours, GETDATE())
		
    SELECT @lastmeet = MAX(MeetingDate) 
    FROM dbo.Meetings
    WHERE OrganizationId = @orgid
    AND MeetingDate <= @maxfuturemeeting
    
    SELECT @yearago = DATEADD(year, -1, @lastmeet)
			
	CREATE TABLE #t
	(
		PeopleId INT NOT NULL,
		Attended BIT,
		[Year] INT NOT NULL,
		[Week] INT NOT NULL,
		AttendCreditCode INT NOT NULL,
		AttendanceTypeId INT
	)
	INSERT INTO #t
	        ( PeopleId,
	          Attended,
	          [Year],
	          [Week],
	          AttendCreditCode,
	          AttendanceTypeId
	        )
	SELECT
		PeopleId,
		CONVERT(BIT, MAX(Attended)) AS Attended,
		[Year],
		[Week],
		AttendCredit,
		MAX(AttendanceTypeId) AS AttendanceTypeId
	FROM (
		SELECT	PeopleId,
				CONVERT(INT, EffAttendFlag) AS Attended, 
				DATEPART(yy, m.MeetingDate) AS [Year], 
				DATEPART(ww, m.MeetingDate) AS [Week], 
				s.ScheduleId,
				AttendanceTypeId,
				CASE WHEN ISNULL(m.AttendCreditId, 1) = 1 
					THEN AttendId + 20 -- make every meeting count, 20 gets it out of the way of AttendCredit codes
					ELSE m.AttendCreditId
				END AS AttendCredit
		FROM dbo.Attend AS a
		JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
		LEFT JOIN dbo.OrgSchedule s 
			ON m.OrganizationId = s.OrganizationId 
			AND s.ScheduleId = dbo.ScheduleId(NULL, a.MeetingDate)
		WHERE m.OrganizationId = @orgid
			AND m.MeetingDate >= @yearago
			AND m.MeetingDate <= @maxfuturemeeting
	) AS InlineView
	GROUP BY PeopleId, [Year], [Week], AttendCredit
	        
	CREATE INDEX IDX_PeopleId ON #t(PeopleId, [Year] DESC, [Week] DESC)
	
	DECLARE cur CURSOR FOR 
	SELECT PeopleId 
	FROM dbo.OrganizationMembers
	WHERE OrganizationId = @orgid
	
	OPEN cur
	
	FETCH NEXT FROM cur INTO @pid
	WHILE @@FETCH_STATUS = 0
	BEGIN	
		raiserror ('%d %d', 10,1, @orgid, @pid) with nowait
		
	    SELECT @tct = COUNT(*) 
		FROM (
				SELECT TOP 52 * 
				FROM #t 
				WHERE PeopleId = @pid 
				ORDER BY [Year] DESC, [Week] DESC
			  ) tt 
		WHERE Attended IS NOT NULL
	    
	    SELECT @act = COUNT(*) 
		FROM (
				SELECT TOP 52 * 
				FROM #t 
				WHERE PeopleId = @pid 
				ORDER BY [Year] DESC, [Week] DESC
			  ) tt 
	    WHERE Attended = 1
	       
		if @tct = 0
			SELECT @pct = 0
		else
			SELECT @pct = @act * 100.0 / @tct
				
		SELECT TOP 52 @a = 
			CASE 
			WHEN Attended IS NULL THEN
				CASE AttendanceTypeId
				WHEN 20 THEN 'V'
				WHEN 70 THEN 'I'
				WHEN 90 THEN 'G'
				WHEN 80 THEN 'O'
				WHEN 110 THEN '*'
				ELSE '*'
				END
			WHEN Attended = 1 THEN 'P'
			ELSE '.'
			END + @a
		FROM #t
		WHERE PeopleId = @pid
		ORDER BY [Year] DESC, [Week] DESC
		
		SELECT @lastattend = MAX(m.MeetingDate) 
		FROM dbo.Meetings m
		WHERE m.OrganizationId = @orgid 
		AND EXISTS
		(
			SELECT NULL 
			FROM dbo.Attend 
			WHERE MeetingId = m.MeetingId 
			AND PeopleId = @pid 
			AND AttendanceFlag = 1
		)

		UPDATE dbo.OrganizationMembers SET
			AttendPct = @pct,
			AttendStr = @a,
			LastAttended = @lastattend
		WHERE OrganizationId = @orgid AND PeopleId = @pid

		
		FETCH NEXT FROM cur INTO @pid
	END
	CLOSE cur
	DEALLOCATE cur
		
	INSERT INTO dbo.ActivityLog
	        ( ActivityDate , UserId , Activity , Machine )
	VALUES  ( GETDATE(), NULL , 'UpdateAllAttendStr (' + CONVERT(nvarchar, @orgid) + ',' + CONVERT(nvarchar, DATEDIFF(ms, @start, CURRENT_TIMESTAMP) / 1000) + ')', 'DB' )
END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
