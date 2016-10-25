CREATE FUNCTION [dbo].[DownlineSummary]
(	
	@categoryid INT,
	@pagenum INT,
	@pagesize INT
)
RETURNS 
@t TABLE
(
	[Rank] INT,
	PeopleId INT,
	LeaderName VARCHAR(100),
	Cnt INT,
    Levels INT,
	MaxRows INT
)
AS
BEGIN

	DECLARE @maxrows INT = (SELECT COUNT(*) FROM DownlineLeaders WHERE CategoryId = @categoryid)
	
	INSERT @t ([Rank], PeopleId, LeaderName, Cnt, Levels, MaxRows)
	SELECT rownum = ROW_NUMBER() OVER (ORDER BY c.Cnt DESC, c.PeopleId),
		p.PeopleId, p.Name, c.Cnt, c.Levels, @maxrows
	FROM dbo.DownlineLeaders c
	JOIN dbo.People p ON p.PeopleId = c.PeopleId
	WHERE c.CategoryId = @categoryid
	ORDER BY c.Cnt DESC, c.PeopleId
    OFFSET (@pagenum-1)*@pagesize ROWS
	FETCH NEXT @pagesize ROWS ONLY
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
