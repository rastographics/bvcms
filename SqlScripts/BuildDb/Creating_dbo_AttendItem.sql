CREATE FUNCTION [dbo].[AttendItem] (@pid INT, @n INT)
RETURNS DATETIME
AS
	BEGIN
	DECLARE @dt DATETIME
	
	IF (SELECT COUNT(*) FROM dbo.Attend WHERE PeopleId = @pid) < @n
		RETURN NULL
	
	SELECT @dt = MeetingDate
	FROM
	(
		SELECT MeetingDate, ROW_NUMBER() OVER(ORDER BY MeetingDate) AS SeqNo
        FROM
        (
			SELECT DISTINCT TOP(@n) CAST(MeetingDate AS DATE) MeetingDate
			FROM Attend a
	        WHERE PeopleId = @pid AND AttendanceFlag = 1
	        ORDER BY MeetingDate
        ) tt
     ) yy
     WHERE SeqNo = @n

	
	RETURN @dt
	END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
