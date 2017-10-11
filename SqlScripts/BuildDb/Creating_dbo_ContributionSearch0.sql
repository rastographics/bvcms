CREATE FUNCTION [dbo].[ContributionSearch0]
(
	@name NVARCHAR (100)
	,@comments NVARCHAR(100)
	,@bundletype INT
    ,@type INT
	,@status INT
	,@minamt MONEY
	,@maxamt MONEY 
	,@startdate DATETIME
	,@enddate DATETIME
	,@taxnontax VARCHAR(20)
	,@fundid INT
	,@campusid INT
	,@year INT
	,@includeunclosedbundles BIT
    ,@mobile BIT
	,@online INT
)
RETURNS 
@t TABLE (ContributionId INT)
AS
BEGIN
	INSERT @t (ContributionId)
		SELECT ContributionId 
		FROM ContributionSearch(@name, @comments, @minamt, @maxamt, @startdate, @enddate, @campusid, @fundid, @online, @status, @taxnontax, @year, @type, @bundletype, @includeunclosedbundles, @mobile, NULL, NULL)
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
