CREATE PROCEDURE [dbo].[RecentAbsentsSP2]
(
	 @name nvarchar(100)
	,@prog INT
	,@div INT 
	,@campus INT
	,@sched INT 
	,@status INT
	,@onlinereg INT
	,@orgtypeid INT
)
AS
BEGIN

SET NOCOUNT ON

DECLARE @to TABLE (orgid INT PRIMARY KEY, cathreshhold INT, orgname NVARCHAR(100), leader NVARCHAR(70))

INSERT @to
SELECT  OrganizationId, ConsecutiveAbsentsThreshold, OrganizationName, LeaderName
FROM Organizations o 
	WHERE (@name IS NULL OR o.OrganizationName LIKE '%' + @name + '%')
	
	AND (ISNULL(@prog, 0) = 0
		OR EXISTS(SELECT NULL 
					FROM dbo.DivOrg dd
					JOIN dbo.Division di ON dd.DivId = di.Id
					JOIN dbo.ProgDiv pp ON di.Id = pp.DivId
					WHERE dd.OrgId = o.OrganizationId AND pp.ProgId = @prog))
					
	AND (ISNULL(@div, 0) = 0
		OR EXISTS(SELECT NULL 
					FROM dbo.DivOrg dd
					WHERE dd.OrgId = o.OrganizationId AND dd.DivId = @div))
					
	AND (ISNULL(@campus, 0) = 0 OR o.CampusId = @campus)
	
	AND (ISNULL(@sched, 0) = 0
		OR EXISTS(SELECT NULL 
					FROM dbo.OrgSchedule os 
					WHERE os.OrganizationId = o.OrganizationId AND os.ScheduleId = @sched))
					
	AND (ISNULL(@status, 0) = 0 OR o.OrganizationStatusId = @status)
	
	AND ((@onlinereg = 0 AND ISNULL(o.RegistrationTypeId, 0) = 0) 
		  OR (@onlinereg = 99 AND ISNULL(o.RegistrationTypeId, 0) > 0) 
		  OR (@onlinereg > 0 AND ISNULL(o.RegistrationTypeId, 0) = @onlinereg) 
		  OR @onlinereg = -1
		)
        
	AND (CASE 
			WHEN @orgtypeid = 0 THEN 1
			WHEN @orgtypeid > 0 AND o.OrganizationTypeId = @orgtypeid THEN 1
			WHEN @orgtypeid = -1 AND o.OrganizationTypeId IS NULL THEN 1
			WHEN @orgtypeid = -3 AND o.IsBibleFellowshipOrg = 1 THEN 1
			WHEN @orgtypeid = -2 AND ISNULL(o.IsBibleFellowshipOrg, 0) = 0 THEN 1
			WHEN @orgtypeid = -4 AND o.SuspendCheckin = 1 THEN 1
			WHEN @orgtypeid = -5 AND EXISTS(SELECT NULL FROM Organizations co WHERE co.ParentOrgId = o.OrganizationId) THEN 1
			WHEN @orgtypeid = -6 AND o.ParentOrgId > 0 THEN 1
			ELSE 0
		 END) = 1

CREATE TABLE #t (
	OrganizationId INT,
	PeopleId INT,
	Attended BIT,
	WeekDate DATE
)
INSERT #t SELECT * FROM dbo.AttendCredits ac
WHERE ac.OrganizationId IN (SELECT OrgId FROM @to)

	SELECT
		o.OrganizationId,
		o.OrganizationName,
		o.LeaderName,
		ISNULL(o.ConsecutiveAbsentsThreshold, 2) ConsecutiveAbsentsThreshold,
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
		lastattend,
			(SELECT MAX(MeetingDate)
			 FROM dbo.Attend
			 WHERE AttendanceFlag = 1 
			 AND OrganizationId = tt1.OrganizationId)
		lastmeeting
	FROM 
		(SELECT 
			OrganizationId,
			PeopleId, 
			COUNT(*) consecutive
		 FROM #t a
		 WHERE a.WeekDate > (SELECT MAX(WeekDate)
							  FROM #t
							  WHERE Attended = 1 
							  AND a.PeopleId = PeopleId
							  AND a.OrganizationId = OrganizationId
							  GROUP BY PeopleId)
		 AND EXISTS(SELECT NULL FROM #t WHERE Attended = 1 AND WeekDate = a.WeekDate AND OrganizationId = a.OrganizationId)
		 GROUP BY OrganizationId, PeopleId
	    ) tt1
	JOIN OrganizationMembers om ON tt1.OrganizationId = om.OrganizationId AND tt1.PeopleId = om.PeopleId
	JOIN dbo.People p ON om.PeopleId = p.PeopleId
	JOIN Organizations o ON om.OrganizationId = o.OrganizationId
	JOIN lookup.MemberType mt ON om.MemberTypeId = mt.Id
	JOIN lookup.AttendType at ON at.Id = mt.AttendanceTypeId
	WHERE consecutive >= ISNULL(o.ConsecutiveAbsentsThreshold, 2)
	AND at.Id NOT IN (70, 100) --inservice and homebound
	AND om.MemberTypeId NOT IN (230, 310) --inactive
	ORDER BY o.OrganizationName, o.OrganizationId, consecutive, p.Name2
	DROP TABLE #t
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
