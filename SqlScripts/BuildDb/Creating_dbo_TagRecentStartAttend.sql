CREATE PROC [dbo].[TagRecentStartAttend]( 
	@progid INT,
	@divid INT,
	@org INT,
	@orgtype INT,
	@days0 INT,
	@days INT,
	@tagid INT)
AS
BEGIN
	SELECT OrganizationId INTO #orgs
	FROM dbo.Organizations
	WHERE (ISNULL(@orgtype, 0) = 0 OR OrganizationTypeId = @orgtype)
	AND (ISNULL(@org, 0) = 0 OR OrganizationId = @org)
	AND (ISNULL(@divid, 0) = 0 
			OR EXISTS(SELECT NULL FROM dbo.DivOrg WHERE OrgId = OrganizationId AND DivId = @divid))
	AND (ISNULL(@progid, 0) = 0 
			OR EXISTS(SELECT NULL FROM dbo.DivOrg dd WHERE dd.OrgId = OrganizationId
				AND EXISTS(SELECT NULL FROM dbo.ProgDiv pp WHERE pp.DivId = dd.DivId AND pp.ProgId = @progid)))

	SELECT PeopleId INTO #AttendsAfterDate
	FROM Attend a
	JOIN #orgs o ON o.OrganizationId = a.OrganizationId
	WHERE a.AttendanceFlag = 1
	AND a.MeetingDate >= DATEADD(dd, -@days, GETDATE())

	SELECT PeopleId INTO #AttendsBeforeDate
	FROM Attend a
	JOIN #orgs o ON o.OrganizationId = a.OrganizationId
	WHERE a.AttendanceFlag = 1
	AND a.MeetingDate > DATEADD(dd, -ISNULL(@days0, 365), GETDATE())
	AND a.MeetingDate < DATEADD(dd, -@days, GETDATE())

	;WITH att AS (
		SELECT t1.PeopleId, COUNT(*) Cnt
		FROM #AttendsAfterDate t1
		LEFT JOIN #AttendsBeforeDate t2 ON t2.PeopleId = t1.PeopleId
		WHERE t2.PeopleId IS NULL
		GROUP BY t1.PeopleId
	)
	INSERT dbo.TagPerson ( Id, PeopleId )
	SELECT @tagid, PeopleId
	FROM att
	WHERE Cnt > 0
    
	DROP TABLE #orgs, #AttendsAfterDate, #AttendsBeforeDate

	RETURN 
END

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
