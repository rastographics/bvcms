CREATE PROCEDURE [dbo].[UpdateAllAttendStr] (@orgid INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @start DATETIME = CURRENT_TIMESTAMP;
	
	IF OBJECT_ID('tempdb..#t') IS NOT NULL
	   DROP TABLE #t

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
    
    SELECT @yearago = DATEADD(YEAR, -1, @lastmeet)
		
	SELECT 
		PeopleId,
		CONVERT(BIT, MAX(Attended)) AS Attended,
		[Year],
		[Week],
		AttendCredit,
		MAX(AttendanceTypeId) AS AttendanceTypeId
	INTO #t	
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
			AND m.MeetingDate >= dbo.MinMeetingDate(m.OrganizationId, a.PeopleId, @yearago)
			AND m.MeetingDate <= @maxfuturemeeting
	) AS InlineView
	GROUP BY PeopleId, [Year], [Week], AttendCredit

	CREATE INDEX IDX_PeopleId ON #t(PeopleId, [Year] DESC, [Week] DESC)

	UPDATE dbo.OrganizationMembers
	SET LastAttended = t2.LastAttended,
		AttendStr = t2.AttStr,
		AttendPct = t2.AttPct
	FROM dbo.OrganizationMembers m
	JOIN
	(
	SELECT PeopleId
		, (SELECT MAX(MeetingDate) 
			FROM dbo.Attend
			WHERE PeopleId = om.PeopleId
			AND OrganizationId = om.OrganizationId
			AND AttendanceFlag = 1) LastAttended
		, CONVERT(FLOAT, ISNULL((SELECT COUNT(PeopleId)
			FROM (
					SELECT TOP 52 PeopleId, Attended
					FROM #t 
					WHERE PeopleId = om.PeopleId
					ORDER BY [Year] DESC, [Week] DESC
				  ) tt 
		    WHERE Attended = 1),0))
			/
			NULLIF((SELECT COUNT(PeopleId) 
			FROM (
					SELECT TOP 52 PeopleId, Attended 
					FROM #t 
					WHERE PeopleId = om.PeopleId
					ORDER BY [Year] DESC, [Week] DESC
				  ) tt 
			WHERE Attended IS NOT NULL),0) * 100.0
		 AttPct
		,(SELECT (SELECT 
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
			ELSE '-'
			END AS [text()]
				  FROM #t t
				  WHERE t.PeopleId = om.PeopleId
				  FOR XML PATH(''))
		 ) AttStr 	 
	FROM dbo.OrganizationMembers om WHERE OrganizationId = @orgid
	) t2 ON t2.PeopleId = m.PeopleId
	WHERE m.OrganizationId = @orgid

	DROP TABLE #t

	INSERT INTO dbo.ActivityLog
	        ( ActivityDate , UserId , Activity , Machine )
	VALUES  ( GETDATE(), NULL , 'UpdateAllAttendStr (' + CONVERT(nvarchar, @orgid) + ',' + CONVERT(nvarchar, DATEDIFF(ms, @start, CURRENT_TIMESTAMP) / 1000) + ')', 'DB' )
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
