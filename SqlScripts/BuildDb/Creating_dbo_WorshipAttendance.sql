

CREATE PROCEDURE [dbo].[WorshipAttendance] (@tagid INT)
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT OFF
	SET ANSI_WARNINGS OFF

	DECLARE @wid INT = ISNULL((SELECT Setting FROM dbo.Setting WHERE Id = 'WorshipId'), 0)

	DECLARE @lastmeet DATETIME = dbo.MaxMeetingDate(@wid)

	DECLARE @lastWeekNumber INT = dbo.WeekNumber(@lastmeet)
    
	DECLARE
		@w04 INT = dbo.WeekNumber(DATEADD(ww, -3, @lastmeet)),
		@w12 INT = dbo.WeekNumber(DATEADD(ww, -11, @lastmeet)),
		@w26 INT = dbo.WeekNumber(DATEADD(ww, -25, @lastmeet)),
		@w52 INT = dbo.WeekNumber(DATEADD(ww, -51, @lastmeet))

	SELECT ac.* INTO #t
	FROM AttendCredits2 ac
	JOIN 
	(
		SELECT p.PeopleId, p.BibleFellowshipClassId 
		FROM TagPerson t
		JOIN dbo.People p ON p.PeopleId = t.PeopleId
		WHERE Id = @tagid
	) tt ON tt.PeopleId = ac.PeopleId
	WHERE ac.OrganizationId IN (tt.BibleFellowshipClassId, @wid)
	AND WeekNumber > dbo.WeekNumber(DATEADD(ww, -52, @lastmeet))
	AND Attended IS NOT NULL

	SELECT 
		 t.PeopleId
		,p.Name2 AS Name
		,p.Age
		,dbo.SundayForWeekNumber(@lastWeekNumber) Dt00

		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w04 AND Attended = 1 AND OrganizationId = @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w04 AND OrganizationId = @wid AND PeopleId = t.PeopleId), 0) * 100 AS Wor04

		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w04 AND Attended = 1 AND OrganizationId <> @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w04 AND OrganizationId <> @wid AND PeopleId = t.PeopleId), 0) * 100 AS MF04
		,dbo.SundayForWeekNumber(@w04) Dt04
	
		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w12 AND Attended = 1 AND OrganizationId = @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w12 AND OrganizationId = @wid AND PeopleId = t.PeopleId), 0) * 100 AS Wor12

		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w12 AND Attended = 1 AND OrganizationId <> @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w12 AND OrganizationId <> @wid AND PeopleId = t.PeopleId), 0) * 100 AS MF12
		,dbo.SundayForWeekNumber(@w12) Dt12
	
		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w26 AND Attended = 1 AND OrganizationId = @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w26 AND OrganizationId = @wid AND PeopleId = t.PeopleId), 0) * 100 AS Wor26

		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w26 AND Attended = 1 AND OrganizationId <> @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w26 AND OrganizationId <> @wid AND PeopleId = t.PeopleId), 0) * 100 AS MF26
		,dbo.SundayForWeekNumber(@w26) Dt26
	
		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w52 AND Attended = 1 AND OrganizationId = @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w52 AND OrganizationId = @wid AND PeopleId = t.PeopleId), 0) * 100 AS Wor52

		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w52 AND Attended = 1 AND OrganizationId <> @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekNumber >= @w52 AND OrganizationId <> @wid AND PeopleId = t.PeopleId), 0) * 100 AS MF52
		,mf.AttendPct MFPct
		,dbo.SundayForWeekNumber(@w52) Dt52


		,om.AttendStr WorshipAttStr

	from #t t
	JOIN dbo.People p ON p.PeopleId = t.PeopleId
	LEFT JOIN dbo.OrganizationMembers om ON om.PeopleId = p.PeopleId AND om.OrganizationId = @wid
	LEFT JOIN dbo.OrganizationMembers mf ON mf.PeopleId = p.PeopleId AND mf.OrganizationId = p.BibleFellowshipClassId
	GROUP BY t.PeopleId, p.Name2, p.Age, om.AttendStr, mf.AttendPct

	DROP TABLE #t

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
