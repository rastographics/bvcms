CREATE PROC [dbo].[AttendUpdateN] (@pid INT, @max INT)
AS
BEGIN

	IF (SELECT COUNT(*) FROM Attend WHERE PeopleId = @pid AND AttendanceFlag = 1) > @max
		RETURN 0;

	UPDATE dbo.Attend
	SET SeqNo = NULL
	WHERE PeopleId = @pid;
	
	WITH vv (AttendId, Rnk)
	AS
	(
	SELECT a.AttendId, yy.Rnk FROM Attend a
	JOIN
	(
		SELECT MeetingDate, ROW_NUMBER() OVER(ORDER BY MeetingDate) AS Rnk
	    FROM
	    (
			SELECT DISTINCT TOP(5) CAST(MeetingDate AS DATE) MeetingDate
			FROM Attend
	        WHERE PeopleId = @pid AND AttendanceFlag = 1
			ORDER BY MeetingDate
	    ) tt
	) yy ON CAST(a.MeetingDate AS DATE) = yy.MeetingDate
	WHERE a.PeopleId = @pid
	) 
	UPDATE Attend
	SET SeqNo = vv.Rnk
	FROM vv
	WHERE vv.AttendId = Attend.AttendId

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
