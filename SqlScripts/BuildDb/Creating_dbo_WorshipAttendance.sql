CREATE PROCEDURE [dbo].[WorshipAttendance] (@tagid INT)
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT OFF
	SET ANSI_WARNINGS OFF

	CREATE TABLE #t (PeopleId INT, OrganizationId INT, WeekDate DATE, Attended INT)
	DECLARE
		@w04 DATE = DATEADD(ww, -4, GETDATE()),
		@w12 DATE = DATEADD(ww, -12, GETDATE()),
		@w26 DATE = DATEADD(ww, -26, GETDATE()),
		@w52 DATE = DATEADD(ww, -52, GETDATE())
	DECLARE @wid INT = ISNULL((SELECT Setting FROM dbo.Setting WHERE Id = 'WorshipId'), 0)

	INSERT #t
	SELECT ac.PeopleId, ac.OrganizationId, ac.WeekDate, ac.Attended
	FROM AttendCredits ac
	JOIN 
	(
		SELECT p.PeopleId, p.BibleFellowshipClassId 
		FROM TagPerson t
		JOIN dbo.People p ON p.PeopleId = t.PeopleId
		WHERE Id = @tagid
	) tt ON tt.PeopleId = ac.PeopleId
	WHERE ac.OrganizationId IN (tt.BibleFellowshipClassId, @wid)
	AND WeekDate > DATEADD(ww, -52, GETDATE())
	AND Attended IS NOT NULL

	SELECT 
		 t.PeopleId
		,p.Name
		,p.Age

		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekDate >= @w04 AND Attended = 1 AND OrganizationId = @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekDate >= @w04 AND OrganizationId = @wid AND PeopleId = t.PeopleId), 0) * 100 AS Wor04

		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekDate >= @w04 AND Attended = 1 AND OrganizationId <> @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekDate >= @w04 AND OrganizationId <> @wid AND PeopleId = t.PeopleId), 0) * 100 AS MF04
	
		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekDate >= @w12 AND Attended = 1 AND OrganizationId = @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekDate >= @w12 AND OrganizationId = @wid AND PeopleId = t.PeopleId), 0) * 100 AS Wor12

		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekDate >= @w12 AND Attended = 1 AND OrganizationId <> @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekDate >= @w12 AND OrganizationId <> @wid AND PeopleId = t.PeopleId), 0) * 100 AS MF12
	
		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekDate >= @w26 AND Attended = 1 AND OrganizationId = @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekDate >= @w26 AND OrganizationId = @wid AND PeopleId = t.PeopleId), 0) * 100 AS Wor26

		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekDate >= @w26 AND Attended = 1 AND OrganizationId <> @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekDate >= @w26 AND OrganizationId <> @wid AND PeopleId = t.PeopleId), 0) * 100 AS MF26
	
		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekDate >= @w52 AND Attended = 1 AND OrganizationId = @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekDate >= @w52 AND OrganizationId = @wid AND PeopleId = t.PeopleId), 0) * 100 AS Wor52

		,CONVERT(FLOAT, (SELECT COUNT(*) FROM #t WHERE WeekDate >= @w52 AND Attended = 1 AND OrganizationId <> @wid AND PeopleId = t.PeopleId))
		/ NULLIF((SELECT COUNT(*) FROM #t WHERE WeekDate >= @w52 AND OrganizationId <> @wid AND PeopleId = t.PeopleId), 0) * 100 AS MF52


		,om.AttendStr WorshipAttStr

	from #t t
	JOIN dbo.People p ON p.PeopleId = t.PeopleId
	LEFT JOIN dbo.OrganizationMembers om ON om.PeopleId = p.PeopleId AND om.OrganizationId = @wid
	GROUP BY t.PeopleId, p.Name, p.Age, om.AttendStr

	DROP TABLE #t

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
