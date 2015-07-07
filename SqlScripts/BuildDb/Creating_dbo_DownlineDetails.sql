CREATE FUNCTION [dbo].[DownlineDetails]
(	
	@categoryid INT,
	@leaderid INT,
	@pagenum INT,
	@pagesize INT
)
RETURNS TABLE 
AS
RETURN 
(
	WITH downlineids AS (
		SELECT 
			[level] = MIN(Generation)
			,OrgId
			,LeaderId
			,DiscipleId 
			,TRACE
		FROM dbo.Downline
		WHERE DownlineId = @leaderid
		AND CategoryId = @categoryid
		GROUP BY OrgId, LeaderId, DiscipleId, TRACE
	), TempCount AS (
	    SELECT COUNT(*) AS MaxRows FROM downlineids
	)
	SELECT 
		[Level]
		,OrganizationName
		,Leader = leader.Name2
		,Student = follow.Name2
		,Trace
		,d.OrgId
		,d.LeaderId
		,d.DiscipleId
		,MaxRows 
	FROM downlineids d
	JOIN dbo.Organizations o ON o.OrganizationId = d.OrgId
	JOIN dbo.People leader ON leader.PeopleId = d.leaderid
	JOIN dbo.People follow ON follow.PeopleId = d.DiscipleId
	JOIN TempCount c ON 1 = 1
	ORDER BY [level], o.OrganizationName, leader.Name2, follow.Name2
    OFFSET (@pagenum-1)*@pagesize ROWS
	FETCH NEXT @pagesize ROWS ONLY
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
