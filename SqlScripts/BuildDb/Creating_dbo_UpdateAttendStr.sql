CREATE PROCEDURE [dbo].[UpdateAttendStr] @orgid INT, @pid INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @a nvarchar(200) = '', -- attendance string
			@mindt DATETIME, 
			@dt DATETIME,
			@tct INT, -- total count
			@act INT, -- attended count
			@pct REAL,
			@lastattend DATETIME
			
	DECLARE @t TABLE (
		Attended BIT,
		[Year] INT,
		[Week] INT,
		AttendCreditCode INT,
		AttendanceTypeId INT
	)
	INSERT INTO @t SELECT TOP 52 * FROM dbo.AttendanceCredits(@orgid, @pid) 
	
	SELECT @tct = COUNT(*) FROM @t WHERE Attended IS NOT NULL
    SELECT @act = COUNT(*) FROM @t WHERE Attended = 1
       
	if @tct = 0
		SELECT @pct = 0
	ELSE
		SELECT @pct = @act * 100.0 / @tct
			
	SELECT TOP 52 @a = @a +
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
		END
	FROM @t
	
	SELECT @lastattend = MAX(a.MeetingDate) FROM dbo.Attend a
	WHERE a.AttendanceFlag = 1 
	AND a.OrganizationId = @orgid 
	AND a.PeopleId = @pid

	UPDATE dbo.OrganizationMembers SET
		AttendPct = @pct,
		AttendStr = @a,
		LastAttended = @lastattend
	WHERE OrganizationId = @orgid AND PeopleId = @pid

END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
