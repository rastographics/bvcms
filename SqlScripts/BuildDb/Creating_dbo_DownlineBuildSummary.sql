-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DownlineBuildSummary](@categoryid INT)
AS
BEGIN
	SET NOCOUNT ON;

	;WITH uniquedisciples AS (
		SELECT CategoryId, DownlineId, DiscipleId, MAX(Generation) Generation
		FROM dbo.Downline
		WHERE CategoryId = @categoryid
		GROUP BY CategoryId, DownlineId, DiscipleId
	),
	Counts AS (
		SELECT DownlineId, cnt = COUNT(*), MaxLevels = MAX(Generation) 
		FROM uniquedisciples
		GROUP BY CategoryId, DownlineId
	)
	INSERT dbo.DownlineLeaders
	        ( CategoryId ,
	          PeopleId ,
	          Name ,
	          Cnt ,
	          Levels
	        )
	SELECT @categoryid, p.PeopleId, p.Name, c.cnt, c.MaxLevels
	FROM Counts c
	JOIN dbo.People p ON p.PeopleId = c.DownlineId
	ORDER BY c.cnt DESC, p.PeopleId
	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
