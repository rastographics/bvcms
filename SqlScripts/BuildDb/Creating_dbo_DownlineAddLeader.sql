CREATE PROCEDURE [dbo].[DownlineAddLeader] (@categoryid INT, @leaderid INT)
AS
BEGIN
	SET NOCOUNT ON

	IF EXISTS(SELECT NULL FROM dbo.Downline
		WHERE CategoryId = @categoryid
		AND DownlineId = @leaderid
	)
		RETURN

	CREATE TABLE #t
	(
		Generation INT ,
		OrgId INT ,
		LeaderId INT ,
		DiscipleId INT ,
		StartDt DATETIME ,
		EndDt DATETIME ,
		Trace VARCHAR(400),
		PRIMARY KEY (LeaderId, DiscipleId)
	)

	-- Add First Level
	DECLARE @level INT = 1
	INSERT #t (
	          Generation ,
	          OrgId ,
	          LeaderId ,
	          DiscipleId ,
	          StartDt ,
	          EndDt ,
	          Trace
	        )
	SELECT  @level, 
			MAX(OrgId), 
			LeaderId, 
			DiscId, 
			MIN(StartDt), 
			MAX(EndDt),
			CAST(LeaderId AS VARCHAR(10)) + '/' + CAST(DiscId AS VARCHAR(10))
	FROM #DownlineData
	WHERE LeaderId = @leaderid
	GROUP BY LeaderId, DiscId

	-- Add remaining levels 2 and under
	SET @level = 2

	DECLARE @cnt INT

	-- need some place to stop the loop, this should be out of the way
	DECLARE @maxlevels INT = 30

	-- loop through the levels
	WHILE @level < @maxlevels + 1
	BEGIN
		;WITH thislevel AS (
		    SELECT 
				nextlevel.OrgId
				,nextlevel.LeaderId 
				,nextlevel.DiscId 
				,nextlevel.StartDt
				,nextlevel.EndDt
				,Trace = prevlevel.Trace + '/' + CAST(nextlevel.DiscId AS VARCHAR(10))
		    FROM #t prevlevel
		    JOIN #DownlineData nextlevel 
			

			-- limit search to previous level for this since there should be nothing new in levels prior to the previous
			ON prevlevel.Generation = @level - 1 

			-- the follower has become a leader
			AND prevlevel.DiscipleId = nextlevel.LeaderId 

			-- the new leader was previously a follower
			--AND prevlevel.StartDt < nextlevel.EndDt
			AND (prevlevel.StartDt <= nextlevel.EndDt)

			-- no need to include someone already counted at a previous time
			AND NOT EXISTS(
				SELECT NULL FROM #t t
				WHERE t.DiscipleId = nextlevel.DiscId AND t.LeaderId = nextlevel.LeaderId
			)
		)
	    INSERT #t (
			Generation, 
			OrgId, 
			Leaderid, 
			DiscipleId, 
			StartDt, 
			EndDt, 
			Trace
		)
		SELECT 
			@level, 
			MAX(OrgId), 
			LeaderId, 
			DiscId, 
			MIN(StartDt), 
			MAX(EndDt), 
			MAX(Trace)
		FROM thislevel t
		GROUP BY t.LeaderId, t.DiscId

		SELECT @cnt = @@rowcount 
		--RAISERROR ('Level %i cnt=%i', 0, 1, @level, @cnt) WITH NOWAIT


		-- no need to continue if there were no records in this level
		IF(@cnt = 0)
			BREAK;

		--SET @cnt = (SELECT COUNT(*) FROM dbo.Downline)

		-- go to the next level down
	    SELECT @level = @level + 1;
	END

	INSERT dbo.Downline
	        ( CategoryId ,
	          DownlineId ,
	          Generation ,
	          OrgId ,
	          LeaderId ,
	          DiscipleId ,
	          StartDt ,
	          Trace
	        )
	SELECT 
		@categoryid,
		@leaderid,
		Generation ,
		OrgId ,
		LeaderId ,
		DiscipleId ,
		StartDt ,
		Trace
	FROM #t

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
