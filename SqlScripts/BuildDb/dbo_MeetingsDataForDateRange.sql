-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[MeetingsDataForDateRange] (@orgs VARCHAR(max), @startdate DateTime, @enddate DateTime)

RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		o.OrganizationName [Organization]
		, o.LeaderName
	    , dbo.SundayForDate(m.MeetingDate) dt
		, SUM(m.NumPresent) cnt
	FROM dbo.Organizations o
	JOIN dbo.Meetings m ON m.OrganizationId = o.OrganizationId
	JOIN SplitInts(@orgs) i ON i.Value = o.OrganizationId
	WHERE m.MeetingDate >= @startdate AND CAST(m.MeetingDate AS DATE) <= @enddate
	GROUP BY o.OrganizationName, o.LeaderName, dbo.SundayForDate(m.MeetingDate)
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
