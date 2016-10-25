CREATE PROCEDURE [dbo].[MeetingsForDateRange] @orgs VARCHAR(MAX), @startdate DATETIME, @enddate DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @cols AS NVARCHAR(MAX), @query  AS NVARCHAR(MAX)
	select @cols = STUFF((SELECT ',' + QUOTENAME(convert(char(10), dt, 1)) 
                    from dbo.MeetingsDataForDateRange(@orgs, @startdate, @enddate)
                    group by dt
                    order by dt
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
	set @query = '
	SELECT * from dbo.MeetingsDataForDateRange(''' + @orgs + ''',
		''' + CAST(@startdate AS VARCHAR(20)) + ''',
		''' + CAST(@enddate AS VARCHAR(20)) + ''')
	pivot ( sum(cnt) for dt in (' + @cols + ') ) as WeeklyCount
	order by Organization
	'
	EXEC (@query)
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
