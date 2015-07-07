-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[DownlineSummary]
(	
	@categoryid INT,
	@leaderid INT,
	@pagenum INT,
	@pagesize INT
)
RETURNS 
@t TABLE
(
	PeopleId int,
	LeaderName varchar(100),
	Cnt int,
	MaxRows int
)
AS
BEGIN
	DECLARE @maxrows INT 

	;WITH totalcount AS (
		SELECT DownlineId
		FROM Downline
		WHERE CategoryId = @categoryid
		AND (DownlineId = @leaderid OR @leaderid IS NULL)
		GROUP BY CategoryId, DownlineId
	)
    SELECT @maxrows = COUNT(*) FROM totalcount
	
	;WITH uniquedisciples AS (
		SELECT CategoryId, DownlineId, DiscipleId
		FROM dbo.Downline
		WHERE CategoryId = @categoryid
		AND (DownlineId = @leaderid OR @leaderid IS NULL)
		GROUP BY CategoryId, DownlineId, DiscipleId
	),
	countdisciples AS (
		SELECT DownlineId, cnt = COUNT(*) 
		FROM uniquedisciples
		GROUP BY CategoryId, DownlineId
	),
	levels AS (
		SELECT DownlineId, maxlevel = MAX(Generation)
		FROM dbo.Downline
		WHERE CategoryId = @categoryid
		AND (DownlineId = @leaderid OR @leaderid IS NULL)
		GROUP BY CategoryId, DownlineId
	)
	INSERT @t (PeopleId, LeaderName, Cnt, MaxRows)
	SELECT p.PeopleId, p.NAME, c.cnt, @maxrows
	FROM countdisciples c
	JOIN dbo.People p ON p.PeopleId = c.DownlineId
	ORDER BY c.cnt DESC, p.PeopleId
    OFFSET (@pagenum-1)*@pagesize ROWS
	FETCH NEXT @pagesize ROWS ONLY

	RETURN 
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
