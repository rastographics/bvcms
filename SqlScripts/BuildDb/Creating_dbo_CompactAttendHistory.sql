CREATE FUNCTION [dbo].[CompactAttendHistory](@pid INT)
RETURNS VARCHAR(100) 
AS
BEGIN
	DECLARE @ret VARCHAR(100); 
	WITH lastdtsrev AS  (
				SELECT TOP 2 MeetingDate
	            FROM    dbo.Attend
	            WHERE   AttendanceFlag = 1
	            AND PeopleId = @pid
	            GROUP BY MeetingDate
	            ORDER BY MeetingDate DESC 
	), dts AS ( 
		SELECT   
			firstdts = ( 
				SELECT TOP 2 FORMAT(MeetingDate, 'M/d/yy') + ',  ' AS [text()]
				FROM   dbo.Attend
				WHERE  AttendanceFlag = 1
				AND PeopleId = @pid
				GROUP BY MeetingDate
				ORDER BY MeetingDate
				FOR XML PATH('') )
		   ,lastdts = ( 
				SELECT FORMAT(MeetingDate, 'M/d/yy') + ',  ' AS [text()]
	            FROM    lastdtsrev
	            ORDER BY MeetingDate 
	            FOR XML PATH('') )
	       ,totalattends = ( 
				SELECT COUNT(*)
	            FROM   dbo.Attend
	            WHERE  AttendanceFlag = 1
	            AND PeopleId = @pid )
	), result AS (
		SELECT result = dts.firstdts 
			+ ' (..' + FORMAT(dts.totalattends, 'n0') + '..), ' 
			+ dts.lastdts
		FROM dts
	)
	SELECT	@ret = REVERSE(STUFF(REVERSE(result), 3, 1, '')) FROM result;
	RETURN @ret
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
