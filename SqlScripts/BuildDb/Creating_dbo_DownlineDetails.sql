CREATE FUNCTION [dbo].[DownlineDetails]
(	
	@categoryid INT,
	@leaderid INT,
	@level INT,
	@pagenum INT,
	@pagesize INT
)
RETURNS TABLE 
AS
RETURN 
(
	WITH downlineids AS (
		SELECT 
			OrgId
			,LeaderId
			,DiscipleId 
			,StartDt
			,EndDt
			,Trace
		FROM dbo.Downline
		WHERE DownlineId = @leaderid
		AND CategoryId = @categoryid
		AND Generation = @level
	), TempCount AS (
	    SELECT COUNT(*) AS MaxRows FROM downlineids
	)
	SELECT 
		OrganizationName
		,Leader = leader.Name2
		,Student = follow.Name2
		,Trace
		,d.OrgId
		,d.LeaderId
		,d.DiscipleId
		,d.StartDt
		,EndDt = NULLIF(d.EndDt, '1/1/3000')
		,MaxRows 
	FROM downlineids d
	JOIN dbo.Organizations o ON o.OrganizationId = d.OrgId
	JOIN dbo.People leader ON leader.PeopleId = d.leaderid
	JOIN dbo.People follow ON follow.PeopleId = d.DiscipleId
	JOIN TempCount c ON 1 = 1
	ORDER BY o.OrganizationName, leader.Name2, follow.Name2
    OFFSET (@pagenum-1)*@pagesize ROWS
	FETCH NEXT @pagesize ROWS ONLY
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
