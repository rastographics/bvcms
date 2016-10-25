CREATE PROCEDURE [dbo].[DownlineAddLeaders] 
	@categoryid INT
AS
BEGIN
	SET NOCOUNT ON

	IF OBJECT_ID('tempdb..#leaders') IS NOT NULL
		DROP TABLE #leaders

	SELECT LeaderId
	INTO #leaders
	FROM #DownlineData
	JOIN dbo.People p ON p.PeopleId = LeaderId
	GROUP BY LeaderId
	ORDER BY LeaderId

	DECLARE @leaderid INT = 0

	DECLARE @nleaders INT = (SELECT COUNT(*) FROM #leaders)

	DECLARE @n INT = 1

	-- Iterate over all leaders
	WHILE (1 = 1) 
	BEGIN  

		-- Get next leaderid
		SELECT TOP 1 @leaderid = LeaderId
		FROM #leaders
		WHERE LeaderId > @leaderid 
		ORDER BY LeaderId

		-- Exit loop if no more leaders
		IF @@ROWCOUNT = 0 
			BREAK

		RAISERROR ('%i of %i Calling DownlineAddLeader %i, %i', 0, 1, @n, @nleaders, @categoryid, @leaderid) WITH NOWAIT
		SET @n = @n + 1

		EXEC dbo.DownlineAddLeader @categoryid, @leaderid

	END		
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
