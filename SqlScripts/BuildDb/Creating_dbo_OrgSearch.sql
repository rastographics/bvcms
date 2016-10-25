CREATE FUNCTION [dbo].[OrgSearch]
(	
	 @name nvarchar(100)
	,@prog INT
	,@div INT 
	,@type INT 
	,@campus INT
	,@sched INT 
	,@status INT
	,@onlinereg INT
	,@UserId INT
	,@targetDiv INT
)
RETURNS TABLE 
AS
RETURN 
(
	WITH filterRoles AS (
		SELECT OrganizationId oid
		FROM dbo.Organizations o
		JOIN dbo.Roles r ON r.RoleName = o.LimitToRole 
		JOIN dbo.UserRole ur ON ur.RoleId = r.RoleId
		JOIN dbo.Users u ON u.UserId = ur.UserId
		WHERE @UserId = u.UserId OR @UserId IS NULL
	),
	filterLeaders1 AS ( 
		SELECT o.OrganizationId oid, o.ParentOrgId
		FROM dbo.Organizations o
		JOIN OrganizationMembers om ON om.OrganizationId = o.OrganizationId 
		JOIN dbo.Users u ON u.PeopleId = om.PeopleId AND u.UserId = @UserId
		JOIN lookup.MemberType mt ON mt.Id = om.MemberTypeId
	),
	filterLeaders2 AS (
		SELECT o2.OrganizationId oid, o2.ParentOrgId
		FROM dbo.Organizations o2
		WHERE o2.ParentOrgId IN (SELECT oid FROM filterLeaders1)
	),
	filterLeaders3 AS (
		SELECT o3.OrganizationId oid, o3.ParentOrgId
		FROM dbo.Organizations o3
		WHERE o3.ParentOrgId IN (SELECT oid FROM filterLeaders2)
	),
	filterLeaders AS (
		SELECT oid, ParentOrgId FROM filterLeaders1
		UNION SELECT oid, ParentOrgId FROM filterLeaders2
		UNION SELECT oid, ParentOrgId FROM filterLeaders3
	),
	filterProg AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		WHERE EXISTS(
			SELECT NULL 
			FROM dbo.DivOrg di
			JOIN dbo.ProgDiv pp ON pp.DivId = di.DivId
			WHERE di.OrgId = o.OrganizationId 
			AND pp.ProgId = @prog
		)
	),
	filterDiv AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		WHERE EXISTS(
			SELECT NULL 
			FROM dbo.DivOrg dd
			WHERE dd.OrgId = o.OrganizationId AND dd.DivId = @div
		)
	),
	filterSched AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		WHERE (@sched = -1 
			AND NOT EXISTS(
				SELECT NULL 
				FROM dbo.OrgSchedule 
				WHERE OrganizationId = o.OrganizationId
			)
		)
		OR EXISTS(
			SELECT NULL 
			FROM dbo.OrgSchedule 
			WHERE OrganizationId = o.OrganizationId 
			AND ScheduleId = @sched
		)
	),
	filterType AS (
		SELECT o.OrganizationId oid
		FROM dbo.Organizations o
		WHERE ( @type > 0 
				AND o.OrganizationTypeId = @type
		)
		OR (@type = -1 -- No Org Type
			AND o.OrganizationTypeId IS NULL )
		OR (@type = -2 -- Not Main Fellowship
			AND ISNULL(o.IsBibleFellowshipOrg, 0) = 0 )
		OR (@type = -3 -- Main Fellowship
			AND o.IsBibleFellowshipOrg = 1 )
		OR (@type = -4 -- Suspended Checkin
			AND o.SuspendCheckin = 1 )
		OR (@type = -5 -- Parent Org
			AND EXISTS(
				SELECT NULL 
				FROM dbo.Organizations 
				WHERE ParentOrgId = o.OrganizationId
			)
		)
		OR (@type = -6 -- Child Org
			AND o.ParentOrgId > 0 
		)
		OR (@type = -7 -- Fees
			AND EXISTS(
				SELECT NULL 
				FROM dbo.OrgsWithFees f
				WHERE f.OrganizationId = o.OrganizationId
			)
		)
		OR (@type = -8 -- No Fees
			AND EXISTS(
				SELECT NULL 
				FROM dbo.OrgsWithoutFees f
				WHERE f.OrganizationId = o.OrganizationId
			)
		)
	),
	schedules AS (
		SELECT
			OrganizationId,
			ScheduleId,
			SchedTime,
			dbo.GetScheduleDesc(MeetingTime) ScheduleDescription
		FROM dbo.OrgSchedule
		WHERE ((ISNULL(@sched, 0) = 0 AND Id = 1) OR ScheduleId = @sched)
	), 
	divisions AS (
		SELECT
			OrganizationId,
			Divisions = STUFF(( 
				SELECT N', ' + d.Name + ' (' + CONVERT(VARCHAR(10), d.Id) + ')'
				FROM dbo.DivOrg dd
				JOIN dbo.Division d ON d.Id = dd.DivId
				WHERE dd.OrgId = o.OrganizationId
				FOR XML PATH(''), TYPE
			).value('text()[1]','nvarchar(max)'),1,2,N'')
		FROM dbo.Organizations o
	),
	filterName AS (
		SELECT oid
		FROM dbo.FilterOrgSearchName(@name)
	),
	filterReg AS (
		SELECT oid
		FROM dbo.FilterOnlineReg(@onlinereg)
	)
	--SELECT * FROM schedules
	
	SELECT 
		o.OrganizationId
		, o.OrganizationName
		, o.OrganizationStatusId
		, p.Name Program
		, p.Id ProgramId
		, d.Name Division
		, ds.Divisions
		, sc.ScheduleDescription
		, sc.ScheduleId
		, sc.SchedTime
		, c.[Description] Campus
		, mmt.[Description] LeaderType
		, o.LeaderName
		, o.Location
		, o.ClassFilled
		, o.RegistrationClosed
		, o.AppCategory
		, o.RegStart
		, o.RegEnd
		, o.PublicSortOrder
		, o.FirstMeetingDate
		, o.LastMeetingDate
		, o.MemberCount
		, o.RegistrationTypeId
		, o.CanSelfCheckin
		, o.LeaderId
		, o.PrevMemberCount
		, o.ProspectCount
		, o.[Description]
		, o.UseRegisterLink2
		, o.DivisionId
		, o.BirthDayStart
		, o.BirthDayEnd
		, Tag = CASE WHEN ISNULL(@targetDiv, 0) = 0
			THEN ''
			ELSE (CASE WHEN EXISTS(
					SELECT NULL
			        FROM dbo.DivOrg dd
					WHERE dd.OrgId = o.OrganizationId
					AND dd.DivId = @targetDiv
				) THEN 'Remove' ELSE 'Add' END
			)
			END
		, ChangeMain = CASE WHEN (
				o.DivisionId IS NULL 
				OR o.DivisionId <> @targetDiv
			)
			AND EXISTS(
				SELECT NULL
				FROM dbo.DivOrg d
				WHERE d.OrgId = o.OrganizationId
				AND d.DivId = @targetDiv
			)
			THEN 1 ELSE 0 END
	FROM dbo.Organizations o
	JOIN divisions ds ON ds.OrganizationId = o.OrganizationId
	LEFT JOIN lookup.MemberType mmt ON mmt.Id = o.LeaderMemberTypeId
	LEFT JOIN dbo.Division d ON d.Id = o.DivisionId
	LEFT JOIN dbo.Program p ON p.Id = d.ProgId
	LEFT JOIN schedules sc ON sc.OrganizationId = o.OrganizationId
	LEFT JOIN lookup.Campus c ON c.Id = o.CampusId
	WHERE (o.LimitToRole IS NULL OR o.OrganizationId IN (SELECT oid FROM filterRoles))
	AND (ISNULL(@name, '') = '' OR o.OrganizationId IN (SELECT oid FROM filterName))
	AND (ISNULL(@status, 0) = 0 OR o.OrganizationStatusId = @status)
	AND (ISNULL(@campus, 0) = 0 OR o.CampusId = @campus)

	AND (ISNULL(@type, 0) = 0 OR o.OrganizationId IN (SELECT oid FROM filterType))
	AND (ISNULL(@prog, 0) = 0 OR o.OrganizationId IN (SELECT oid FROM filterProg))
	AND (ISNULL(@div, 0) = 0 OR o.OrganizationId IN (SELECT oid FROM filterDiv))
	AND (ISNULL(@sched, 0) = 0 OR o.OrganizationId IN (SELECT oid FROM filterSched))
	AND (ISNULL(@onlinereg, -1) = -1 OR o.OrganizationId IN (SELECT oid FROM filterReg))
	AND (CASE WHEN EXISTS(
				SELECT NULL
				FROM dbo.Roles r
				JOIN dbo.UserRole ur ON ur.RoleId = r.RoleId
				WHERE ur.UserId = @UserId
				AND r.RoleName = 'OrgLeadersOnly'
		) THEN 1 ELSE 0 END = 0
		OR o.OrganizationId IN (SELECT oid FROM filterLeaders)
	)
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
