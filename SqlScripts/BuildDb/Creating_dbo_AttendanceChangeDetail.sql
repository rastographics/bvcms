CREATE FUNCTION [dbo].[AttendanceChangeDetail]  
(  
	@orgids VARCHAR(MAX),  
	@MeetingDate1 DATETIME,  
	@MeetingDate2 DATETIME 
)  
RETURNS TABLE AS RETURN  
(  
		SELECT  a.PeopleId ,  
				a.OrganizationId, 
		        Attended ,  
		        WeekDate 
 
		FROM AttendCredits a 
		WHERE a.OrganizationId IN (SELECT Value FROM dbo.SplitInts(@orgids))  
		AND WeekDate BETWEEN DATEADD(DAY, -DATEDIFF(DAY, @MeetingDate1, @MeetingDate2), @MeetingDate1) AND @MeetingDate2  
) 
 
 
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
