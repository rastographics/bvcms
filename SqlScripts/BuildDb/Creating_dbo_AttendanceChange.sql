CREATE FUNCTION [dbo].[AttendanceChange] 
( 
	@orgids AS VARCHAR(MAX), 
	@MeetingDate1 DATETIME, 
	@MeetingDate2 DATETIME 
) 
RETURNS TABLE AS RETURN 
( 
	WITH attends AS ( 
		SELECT  pid = PeopleId , 
		        att = Attended , 
		        dt = WeekDate  
		FROM AttendCredits 
		WHERE OrganizationId IN (SELECT Value FROM dbo.SplitInts(@orgids)) 
		AND WeekDate BETWEEN DATEADD(DAY, -DATEDIFF(DAY, @MeetingDate1, @MeetingDate2), @MeetingDate1) AND @MeetingDate2 
	), 
	totals AS ( 
		SELECT 
			pid = t.pid, 
			Attends1 = (SELECT COUNT(*) FROM attends a WHERE a.att = 1 AND a.pid = t.pid AND a.Dt BETWEEN DATEADD(DAY, -DATEDIFF(DAY, @MeetingDate1, @MeetingDate2), @MeetingDate1) AND @MeetingDate1), 
			Total1 = (SELECT COUNT(*) FROM attends a WHERE a.pid = t.pid AND a.Dt BETWEEN DATEADD(DAY, -DATEDIFF(DAY, @MeetingDate1, @MeetingDate2), @MeetingDate1) AND @MeetingDate1), 
			Attends2 = (SELECT COUNT(*) FROM attends a WHERE a.att = 1 AND a.pid = t.pid AND a.Dt BETWEEN @MeetingDate1 AND @MeetingDate2), 
			Total2 = (SELECT COUNT(*) FROM attends a WHERE a.pid = t.pid AND a.Dt BETWEEN @MeetingDate1 AND @MeetingDate2) 
		FROM attends t 
		GROUP BY t.pid 
	), 
	pcts AS ( 
		SELECT pid 
			,Pct1 = (t.Attends1 * 1.0) / NULLIF(t.Total1, 0) * 100 
			,Pct2 = (t.Attends2 * 1.0) / NULLIF(t.Total2, 0) * 100 
		FROM totals t 
	), 
	pcts2 AS ( 
		SELECT pid, 
		Pct1 = ISNULL(Pct1, 0),  
		Pct2 = ISNULL(Pct2, 0),  
		PctOfPrevious = (NULLIF(Pct2, 0) / NULLIF(Pct1, 0)) * 100 
		FROM pcts 
	) 
	SELECT pid PeopleId, 
		Pct1,  
		Pct2,  
		PctChange = CASE  
			WHEN PctOfPrevious IS NULL AND Pct1 = 0 AND Pct2 = 0 THEN 0 
			WHEN PctOfPrevious IS NULL AND Pct1 = 0 THEN 100000.0 
			WHEN PctOfPrevious IS NULL AND Pct2 = 0 THEN -100000.0 
			ELSE PctOfPrevious - 100.0 
		END, 
		CHANGE = CASE  
			WHEN PctOfPrevious IS NULL AND Pct1 = 0 AND Pct2 = 0 THEN 'No Change' 
			WHEN PctOfPrevious IS NULL AND Pct1 = 0 THEN 'Started' 
			WHEN PctOfPrevious IS NULL AND Pct2 = 0 THEN 'Stopped' 
			WHEN PctOfPrevious > 100 THEN FORMAT(-(Pct1 - Pct2) / Pct1, '0%') + ' Increase' 
			WHEN PctOfPrevious < 100 THEN FORMAT((Pct1 - Pct2) / Pct1, '0%') + ' Decrease' 
			ELSE 'No Change' 
		END 
	FROM pcts2 
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
