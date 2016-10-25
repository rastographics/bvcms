CREATE FUNCTION [dbo].[DownlineLevels]
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
			Generation
			,OrgId
			,LeaderId
			,DiscipleId 
			,StartDt
			,EndDt
		FROM dbo.Downline
		WHERE DownlineId = @leaderid
		AND CategoryId = @categoryid
	), levels AS (
		SELECT
			[level] = downlineids.Generation,
			OrgId,
			LeaderId,
			StartDt = MIN(downlineids.StartDt),
			EndDt = MAX(downlineids.EndDt),
			Cnt = COUNT(*)
		FROM downlineids
		GROUP BY Generation, OrgId, LeaderId
	), TempCount AS (
	    SELECT COUNT(*) AS MaxRows FROM levels
	)
	SELECT 
		[Level]
		,OrganizationName
		,Leader = leader.Name2
		,d.OrgId
		,d.LeaderId
		,d.Cnt
		,d.StartDt
		,EndDt = NULLIF(d.EndDt, '1/1/3000')
		,c.MaxRows 
	FROM levels d
	JOIN dbo.Organizations o ON o.OrganizationId = d.OrgId
	JOIN dbo.People leader ON leader.PeopleId = d.leaderid
	JOIN TempCount c ON 1 = 1
	ORDER BY [level], o.OrganizationName, leader.Name2
    OFFSET (@pagenum-1)*@pagesize ROWS
	FETCH NEXT @pagesize ROWS ONLY
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
