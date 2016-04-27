

CREATE FUNCTION [dbo].[RollList]
(
	@mid INT
	,@meetingdt DATETIME
	,@oid INT
	,@current BIT
	,@FromMobile BIT
    ,@subgroupIds VARCHAR(100)  --Filter by subgroup IDs (comma separated). Returns everyone who is in at least 1 of these subgroups. Empty string disables subgroup filtering
	,@IncludeLeaderless BIT = 0  --Include those who are not in any subgroup with a leader (used in conjunction with @subgroupIds)
)
RETURNS @table TABLE 
(
	Section INT, -- Member = 1, Visitor = 2, Other = 3
	PeopleId INT, 
	Name NVARCHAR(100),
	Last NVARCHAR(100),
	FamilyId INT,
	First NVARCHAR(50),
	Email NVARCHAR(100),
	Attended BIT, 
	CommitmentId INT, 
	CurrMemberType NVARCHAR(100), 
	MemberType NVARCHAR(100), 
	AttendType NVARCHAR(100),
	OtherAttends INT,
	CurrMember BIT,
	Conflict BIT,
	ChurchMemberStatus NVARCHAR(100)
)
AS
BEGIN
	DECLARE @AppRollListCommittedOnly BIT = 0
	DECLARE @FilterBySubgroup BIT = 0
	DECLARE @subgroupIdsTable TABLE (Id int)
	INSERT INTO @subgroupIdsTable (Id) SELECT * FROM dbo.SplitInts(@subgroupIds)
	IF (SELECT Count(*) FROM @subgroupIdsTable) > 0 
	Begin
		-- Only filter by Subgroup if some subgroup Ids were passed into this function.
		SET @FilterBySubgroup = 1 
	End

	IF @FromMobile = 1 AND 
		EXISTS(	SELECT NULL 
				FROM dbo.OrganizationExtra
				WHERE OrganizationId = @oid
				AND Field = 'AppRollListCommittedOnly'
				AND BitValue = 1)
		SET @AppRollListCommittedOnly = 1

	--People who attended the meeting or were committed to come
	DECLARE @attends TABLE 
	(
		PeopleId INT, 
		CommitmentId INT, 
		Attended BIT, 
		membertype INT, 
		attendtype INT,
		OtherAttends INT
	)
	--Members at the time of meeting
	DECLARE @members TABLE 
	(
		PeopleId INT, 
		joindt DATETIME, 
		membertype INT
	)
	--Recent Visitors
	DECLARE @visitors TABLE 
	(
		PeopleId INT
	)

	--Subgroups with at least 1 leader
	DECLARE @subgroupsWithLeader TABLE
	(
		MemberTagId INT
	)

	INSERT @attends
	SELECT a.PeopleId 
		   ,a.Commitment
		   ,a.AttendanceFlag
		   ,a.MemberTypeId
		   ,a.AttendanceTypeId
		   ,a.OtherAttends
	FROM dbo.Attend a
	WHERE MeetingId = @mid
	AND (a.EffAttendFlag IS NULL OR a.EffAttendFlag = 1 OR a.Commitment IS NOT NULL)

	DECLARE @dt DATETIME = @meetingdt
	IF(@current = 1)
		SET @dt = GETDATE()

	INSERT @members
	SELECT 
		PeopleId, 
		joined, 
		MemberTypeId 
	FROM dbo.OrgMembersAsOfDate(@oid, @dt)

	INSERT @visitors
	SELECT PeopleId 
	FROM dbo.OrgVisitorsAsOfDate(@oid, @meetingdt, 0)

	IF @FilterBySubgroup = 1 AND @IncludeLeaderless = 1
	BEGIN
		INSERT @subgroupsWithLeader 
		SELECT DISTINCT MemberTagId
		FROM dbo.OrgMemMemTags mt
		WHERE mt.IsLeader = 1 AND mt.OrgId = @oid
	END
	


	-- start building results, members first
	INSERT @table
	SELECT DISTINCT 1 -- Members
		   ,m.PeopleId
		   ,p.Name2
		   ,p.LastName
		   ,p.FamilyId
		   ,p.PreferredName
		   ,p.EmailAddress
		   ,a.Attended
		   ,a.CommitmentId
		   ,cmt.Description CurrMemberType
		   ,mt.Description MemberType
		   ,at.Description Attendtype
		   ,a.OtherAttends
		   ,CASE WHEN om.PeopleId IS NULL THEN 0 ELSE 1 END
		   ,CASE WHEN GETDATE() > @meetingdt OR mc.PeopleId IS NULL THEN 0 ELSE 1 END
		   ,ms.Description ChurchMemberStatus
	FROM @members m
	LEFT JOIN @attends a ON a.PeopleId = m.PeopleId
	JOIN dbo.People p ON p.PeopleId = m.PeopleId
	LEFT JOIN lookup.MemberType cmt ON cmt.Id = m.membertype
	LEFT JOIN lookup.MemberType mt ON mt.Id = a.membertype
	LEFT JOIN lookup.AttendType at ON at.Id = a.attendtype
	LEFT JOIN lookup.MemberStatus ms ON ms.Id = p.MemberStatusId
	LEFT JOIN dbo.OrganizationMembers om ON om.PeopleId = p.PeopleId AND om.OrganizationId = @oid
	LEFT JOIN dbo.MeetingConflicts mc ON mc.PeopleId = a.PeopleId AND mc.MeetingDate = @meetingdt AND @oid IN (mc.OrgId1, mc.OrgId2)
    
    -- Join Subgroups ONLY IF we were given at least 1 subgroup to filter by   --JoelS
	LEFT JOIN dbo.OrgMemMemTags sg ON ((@FilterBySubgroup = 1) AND (m.PeopleId = sg.PeopleId))
	
    WHERE (@current = 1 OR @meetingdt > m.joindt OR (a.PeopleId IS NOT NULL AND attendtype NOT IN (40,50,60,110)))
	AND (@AppRollListCommittedOnly = 0 OR CommitmentId IN (1,4))
    
	
	-- Subgroup Filtering...
	AND (
		(@FilterBySubgroup = 0) --If subgroup filtering not activated, just include everyone and ignore the rest of these WHERE conditions
		OR (
			(sg.MemberTagId IN (SELECT Id FROM @subgroupIdsTable)) -- if they are in a matching subgroup, include them
			OR 
			-- If @includeLeaderless, then include... 
			(@includeLeaderless = 1 AND 
				(	
					(sg.MemberTagId NOT IN (SELECT MemberTagId FROM @subgroupsWithLeader)) -- ...members NOT in any subgroup that has a leader
					OR
					(sg.MemberTagID IS NULL)  -- ...members NOT in any subgroup at all
				)
			) 
		)
	)

	

	-- recent visitors who have not become members as of the meeting date
	INSERT @table
	SELECT 2 -- Visitors
		   ,v.PeopleId
		   ,p.Name2
		   ,p.LastName
		   ,p.FamilyId
		   ,p.PreferredName
		   ,p.EmailAddress
		   ,a.Attended
		   ,a.CommitmentId
		   ,NULL
		   ,mt.Description MemberType
		   ,at.Description Attendtype
	       ,a.OtherAttends
		   ,CASE WHEN om.PeopleId IS NULL THEN 0 ELSE 1 END
		   ,0
		   ,ms.Description ChurchMemberStatus
	FROM @visitors v
	LEFT JOIN @attends a ON a.PeopleId = v.PeopleId
	JOIN dbo.People p ON p.PeopleId = v.PeopleId
	LEFT JOIN lookup.MemberType mt ON mt.Id = a.membertype
	LEFT JOIN lookup.AttendType at ON at.Id = a.attendtype
	LEFT JOIN lookup.MemberStatus ms ON ms.Id = p.MemberStatusId
	LEFT JOIN dbo.OrganizationMembers om ON om.PeopleId = p.PeopleId AND om.OrganizationId = @oid
	WHERE NOT EXISTS(SELECT NULL FROM @members m WHERE m.PeopleId = v.PeopleId)

	 -- If we are filtering by Subgroup, do NOT show visitors unless the @IncludeLeaderless flag is true   --JoelS
	-- AND ((@FilterBySubgroup = 0) OR (@IncludeLeaderless = 1 ))
    
	-- now catch the odd ducks who slipped through the cracks, (should not be any if all is well)
	INSERT @table
	SELECT DISTINCT 3 -- Other
		   ,a.PeopleId
		   ,p.Name2
		   ,p.LastName
		   ,p.FamilyId
		   ,p.PreferredName
		   ,p.EmailAddress
		   ,a.Attended
		   ,a.CommitmentId
		   ,NULL
		   ,mt.Description MemberType
		   ,at.Description Attendtype
	       ,a.OtherAttends
		   ,CASE WHEN om.PeopleId IS NULL THEN 0 ELSE 1 END
		   ,0
		   ,ms.Description ChurchMemberStatus
	FROM @attends a
	LEFT JOIN @table t ON t.PeopleId = a.PeopleId 
	JOIN dbo.People p ON p.PeopleId = a.PeopleId
	LEFT JOIN lookup.MemberType mt ON mt.Id = a.membertype
	LEFT JOIN lookup.AttendType at ON at.Id = a.attendtype
	LEFT JOIN lookup.MemberStatus ms ON ms.Id = p.MemberStatusId
	LEFT JOIN dbo.OrganizationMembers om ON om.PeopleId = p.PeopleId AND om.OrganizationId = @oid
	
	 -- Join Subgroups ONLY IF Filtering by Subgroup   --JoelS
	LEFT JOIN dbo.OrgMemMemTags sg ON ((@FilterBySubgroup = 1) AND (a.PeopleId = sg.PeopleId))

	WHERE t.PeopleId IS NULL

	-- Subgroup Filtering...
	AND (
		(@FilterBySubgroup = 0) --If subgroup filtering not activated, just include everyone and ignore the rest of these WHERE conditions
		OR (
			(sg.MemberTagId IN (SELECT Id FROM @subgroupIdsTable)) -- if they are in a matching subgroup, include them
			OR 
			-- If @includeLeaderless, then include... 
			(@includeLeaderless = 1 AND 
				(	
					(sg.MemberTagId NOT IN (SELECT MemberTagId FROM @subgroupsWithLeader)) -- ...members NOT in any subgroup that has a leader
					OR
					(sg.MemberTagID IS NULL)  -- ...members NOT in any subgroup at all
				)
			) 
		)
	)

	RETURN

END







GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
