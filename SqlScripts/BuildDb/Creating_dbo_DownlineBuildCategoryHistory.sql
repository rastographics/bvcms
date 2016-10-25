-- ================================================
CREATE PROCEDURE [dbo].[DownlineBuildCategoryHistory] 
	@categoryid INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @orgs TABLE (orgid INT)
	DECLARE @mf BIT, @progs VARCHAR(100), @divs VARCHAR(100)

	SELECT 
		@mf = mainfellowship
		,@progs = programs
		,@divs = divisions
	FROM dbo.DownlineCategories(@categoryid)

	IF OBJECT_ID('tempdb..#DownlineData') IS NOT NULL
		DROP TABLE #DownlineData

	CREATE TABLE #DownlineData (
		[OrgId] [INT] NULL,
		[LeaderId] [INT] NULL,
		[DiscId] [INT] NULL,
		[StartDt] [DATETIME] NULL,
		[EndDt] [DATETIME] NULL
	)

	CREATE INDEX IXDownlineDataLeaderIdEndDt ON #DownlineData
	(
		[LeaderId] ASC,
		[EndDt] ASC
	)

	;WITH porgs AS (
		SELECT oid = o.OrganizationId 
		FROM dbo.Organizations o
		LEFT JOIN dbo.OrganizationExtra e ON e.OrganizationId = o.OrganizationId AND e.Field = 'NoDownline'
		WHERE e.OrganizationId IS NULL
		AND EXISTS(
			SELECT NULL
			FROM dbo.DivOrg dd
			WHERE dd.OrgId = o.OrganizationId
			AND	EXISTS(
				SELECT NULL
				FROM dbo.ProgDiv pd
				WHERE pd.DivId = dd.DivId
				AND pd.ProgId IN (SELECT Value FROM dbo.SplitInts(@progs))
			)
		)
	),
	dorgs AS (
		SELECT oid = o.OrganizationId 
		FROM dbo.Organizations o
		LEFT JOIN dbo.OrganizationExtra e ON e.OrganizationId = o.OrganizationId AND e.Field = 'NoDownline'
		WHERE e.OrganizationId IS NULL
		AND EXISTS(
			SELECT NULL
			FROM dbo.DivOrg dd
			WHERE dd.OrgId = o.OrganizationId
			AND dd.DivId IN (SELECT Value FROM dbo.SplitInts(@divs))
		)
	),
	mainfellowships AS (
		SELECT oid = o.OrganizationId
		FROM dbo.Organizations o
		LEFT JOIN dbo.OrganizationExtra e ON e.OrganizationId = o.OrganizationId AND e.Field = 'NoDownline'
		WHERE e.OrganizationId IS NULL
		AND IsBibleFellowshipOrg = 1
		AND @mf = 1
	), combined AS (
		SELECT oid FROM porgs
		UNION SELECT oid FROM dorgs
		UNION SELECT oid FROM mainfellowships
	)
	INSERT @orgs ( orgid )
	SELECT DISTINCT oid
	FROM combined

	DECLARE @cnt INT
	SELECT @cnt = COUNT(*) FROM @orgs
	RAISERROR ('inserted %i orgs', 0, 1, @cnt) WITH NOWAIT

	-- Build the history table of start dates and end dates from EnrollmentTransaction table
	-- Our goal is to have each row contain the org, person, leader status, started when, ended when

	IF OBJECT_ID('tempdb..#enrollhistory') IS NOT NULL
		DROP TABLE #enrollhistory

	CREATE TABLE #enrollhistory
	(
		oid INT,
		pid INT, 
		lead BIT, 
		startdt DATETIME,
		enddt DATETIME
	)

	INSERT #enrollhistory ( oid, pid, lead, startdt, enddt )
	SELECT 
		t.OrganizationId,
		PeopleId,

		-- flag to indicate leader or not 
		CASE WHEN MemberTypeId = o.LeaderMemberTypeId THEN 1 ELSE 0 END,

		-- start date
		TransactionDate,

		-- end date is null to begin with
		NULL

	FROM dbo.EnrollmentTransaction t
	JOIN dbo.Organizations o ON o.OrganizationId = t.OrganizationId
	JOIN @orgs oo ON oo.orgid = o.OrganizationId

	-- looking for enrollments=1 and changes=3
	WHERE TransactionTypeId IN (1,3)

	ORDER BY TransactionDate

	-- find change dates to update the end dates on the rows with the right start date
	UPDATE #enrollhistory
	SET enddt = (
		-- find the closet transaction
		SELECT MIN(TransactionDate)
		FROM dbo.EnrollmentTransaction

		-- that was a change
		WHERE TransactionTypeId = 3

		-- in the same org
		AND OrganizationId = oid

		-- for the same person
		AND PeopleId = pid

		-- where it is after the starting date
		AND TransactionDate > startdt
	)
	FROM #enrollhistory

	-- only update the end dates where they have not already been updated
	WHERE enddt IS NULL

	-- find drop dates to update the end dates on the rows with the right start date
	UPDATE #enrollhistory
	SET enddt = (

		-- find the closet transaction
		SELECT MIN(TransactionDate)
		FROM dbo.EnrollmentTransaction

		-- that was a drop
		WHERE TransactionTypeId = 5

		-- in the same org
		AND OrganizationId = oid

		-- for the same person
		AND PeopleId = pid

		-- where it is after the starting date
		AND TransactionDate > startdt
	)
	FROM #enrollhistory

	-- only update the end dates where they have not already been updated
	WHERE enddt IS NULL

	SELECT @cnt = COUNT(*) FROM #enrollhistory
	RAISERROR ('inserted %i enrollhistory records', 0, 1, @cnt) WITH NOWAIT

	;WITH data AS (
		SELECT 
		leader.oid 
		,leaderid = leader.pid
		,discid = disc.pid
		,leadersdt = leader.startdt -- starting date
		,leaderedt = ISNULL(leader.enddt, '1/1/3000') -- ending date
		,discsdt = disc.startdt -- starting date
		,discedt = ISNULL(disc.enddt, '1/1/3000') -- ending date
		FROM #enrollhistory leader
		-- join leaders and followers 
		JOIN #enrollhistory disc

		ON leader.oid = disc.oid -- leader and disciple in the same org
		AND leader.lead = 1 -- leader is a leader
		AND disc.lead = 0 -- disciple is not a leader
		AND leader.pid <> disc.pid -- leader is not the disciple

		-- look for an overlap in time frame
		AND leader.startdt < ISNULL(disc.enddt, '1/1/3000') -- leader started before disciple quit
		AND disc.startdt < ISNULL(leader.enddt, '1/1/3000') -- disciple started before leader quit
	)

	INSERT #DownlineData
	        ( 
			  OrgId ,
	          LeaderId ,
	          DiscId ,
	          StartDt ,
	          EndDt
	        )
	SELECT 
		oid,
		leaderid, 
		discid, 
		IIF(leadersdt > discsdt, leadersdt, discsdt),
		IIF(leaderedt < discedt, leaderedt, discedt)
	FROM data t
	--GROUP BY t.oid, t.leaderid, t.discid

	SELECT @cnt = COUNT(*) FROM #DownlineData
	RAISERROR ('inserted %i downlinedata records', 0, 1, @cnt) WITH NOWAIT


	;WITH leaders AS (
		SELECT
			oid, 
			pid,
			CAST(startdt AS DATE) startdt,
			CAST(enddt AS DATE) enddt
		FROM #enrollhistory e
		WHERE LEAD = 1
	),
	pass2 AS (
		SELECT
			oid, 
			startdt, 
			enddt, 
			MIN(pid) p1, 
			MAX(pid) p2 
		FROM leaders l
		GROUP BY oid, startdt, enddt
		HAVING COUNT(*) = 2 
	),
	females AS (
		SELECT
			oid, 
			pid = IIF(p1.GenderId = 2, p1.PeopleId, p2.PeopleId),
			sd = l.startdt,
			ed = l.enddt
		FROM pass2 l
		JOIN dbo.People p1 ON p1.PeopleId = l.p1
		JOIN dbo.People p2 ON p2.PeopleId = l.p2
		WHERE p1.GenderId <> p2.GenderId
	)
	DELETE #DownlineData 
	FROM #DownlineData dd
	WHERE EXISTS(
		SELECT oid, pid, sd, ed
		FROM females
		WHERE oid = dd.OrgId AND dd.LeaderId = pid AND CAST(dd.StartDt AS DATE) = sd AND CAST(dd.EndDt AS DATE) = ed
	)


	EXEC dbo.DownlineAddLeaders @categoryid
	

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
