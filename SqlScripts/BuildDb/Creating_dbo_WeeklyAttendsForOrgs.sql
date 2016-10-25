-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[WeeklyAttendsForOrgs]
(
	@orgs VARCHAR(MAX),
	@firstdate datetime
)
RETURNS 
@tt TABLE 
(
	PeopleId INT NOT NULL,
	Attended BIT NOT NULL,
	Sunday DATETIME NOT NULL
)
AS
BEGIN

	DECLARE @t TABLE (id INT)
	INSERT INTO @t SELECT i.Value FROM dbo.SplitInts(@orgs) i

	INSERT INTO @tt
	SELECT PeopleId
		, MAX(CAST(AttendanceFlag AS INT)) Attended
		, dbo.SundayForDate(MIN(MeetingDate)) Sunday
	FROM
	(
		SELECT
			a.PeopleId,
			a.MeetingDate,
			a.AttendanceFlag,
			DATEPART(yy, a.MeetingDate) YearNum,
			DATEPART(isowk, a.MeetingDate) WeekNum
		FROM dbo.Attend a
		JOIN dbo.People p ON p.PeopleId = a.PeopleId
		WHERE a.OrganizationId IN (SELECT id FROM @t)
		AND a.MeetingDate > @firstdate
		-- exclude RecentVisitor, NewVisitor, Prospect, Group
		AND a.AttendanceTypeId NOT IN (50, 60, 190, 90)
	) t1
	GROUP BY t1.PeopleId, t1.YearNum, t1.WeekNum
	
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
