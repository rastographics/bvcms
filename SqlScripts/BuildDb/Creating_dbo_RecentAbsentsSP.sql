CREATE PROCEDURE [dbo].[RecentAbsentsSP](@orgid INT, @orgidfilter INT = NULL)
AS
BEGIN
CREATE TABLE #t (
	OrganizationId INT,
	PeopleId INT,
	Attended BIT,
	WeekDate DATE
)
INSERT #t SELECT * FROM dbo.AttendCredits ac
WHERE ac.OrganizationId = @orgid
	AND (ISNULL(@orgidfilter, 0) = 0 OR EXISTS(SELECT NULL FROM dbo.OrganizationMembers WHERE OrganizationId = @orgidfilter AND PeopleId = ac.PeopleId))

	SELECT
		@orgid OrganizationId,
		consecutive, 
		om.PeopleId,
		p.Name2,
		p.HomePhone,
		p.CellPhone,
		p.EmailAddress,
			(SELECT MAX(mm.MeetingDate)
			FROM Attend
			JOIN dbo.Meetings mm ON dbo.Attend.MeetingId = mm.MeetingId
			WHERE AttendanceFlag = 1
			AND mm.OrganizationId = o.OrganizationId
			AND Attend.PeopleId = om.PeopleId)
		lastattend
	FROM 
		(SELECT 
			PeopleId, 
			COUNT(*) consecutive
		 FROM #t a
		 WHERE a.WeekDate > (SELECT MAX(WeekDate)
							  FROM #t
							  WHERE Attended = 1 
							  AND a.PeopleId = PeopleId
							  GROUP BY PeopleId)
		 --AND EXISTS(SELECT NULL FROM #t WHERE Attended = 1 AND WeekDate = a.WeekDate)
		 GROUP BY PeopleId
	    ) tt1
	JOIN OrganizationMembers om ON @orgid = om.OrganizationId AND tt1.PeopleId = om.PeopleId
	JOIN dbo.People p ON om.PeopleId = p.PeopleId
	JOIN Organizations o ON om.OrganizationId = o.OrganizationId
	JOIN lookup.MemberType mt ON om.MemberTypeId = mt.Id
	JOIN lookup.AttendType at ON at.Id = mt.AttendanceTypeId
	where o.OrganizationId = @orgid
	AND consecutive >= ISNULL(o.ConsecutiveAbsentsThreshold, 2)
	AND at.Id NOT IN (70, 100) --inservice and homebound
	AND om.MemberTypeId NOT IN (230, 310) --inactive
	AND (ISNULL(@orgidfilter, 0) = 0 OR EXISTS(SELECT NULL FROM dbo.OrganizationMembers WHERE OrganizationId = @orgidfilter AND PeopleId = p.PeopleId))
	ORDER BY o.OrganizationName, o.OrganizationId, consecutive, p.Name2
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
