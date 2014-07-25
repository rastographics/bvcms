CREATE FUNCTION [dbo].[AttendanceCredits](@orgid INT, @pid INT)
RETURNS @t TABLE (
	Attended BIT,
	[Year] INT,
	[Week] INT,
	AttendCreditCode INT,
	AttendanceTypeId INT
)
AS
BEGIN
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
			
	INSERT INTO @t
	SELECT
		CONVERT(BIT, MAX(Attended)) AS Attended,
		[Year],
		[Week],
		AttendCredit,
		MAX(AttendanceTypeId) AS AttendanceTypeId
	FROM (
		SELECT	CONVERT(INT, EffAttendFlag) AS Attended, 
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
			AND PeopleId = @pid
			AND m.MeetingDate >= @yearago
			AND m.MeetingDate <= @maxfuturemeeting
	) AS InlineView
	GROUP BY [Year], [Week], AttendCredit
	ORDER BY [Year] DESC, [Week] DESC
	
	RETURN
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
