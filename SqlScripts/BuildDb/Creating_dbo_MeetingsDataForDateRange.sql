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
		, SUM(m.MaxCount) cnt
	FROM dbo.Organizations o
	JOIN dbo.Meetings m ON m.OrganizationId = o.OrganizationId
	JOIN SplitInts(@orgs) i ON i.Value = o.OrganizationId
	WHERE m.MeetingDate >= @startdate AND CAST(m.MeetingDate AS DATE) <= @enddate
	GROUP BY o.OrganizationName, o.LeaderName, dbo.SundayForDate(m.MeetingDate)
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
