-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[PledgeCount](@pid INT, @days INT, @fundid INT)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @cnt int

	-- Add the T-SQL statements to compute the return value here
	DECLARE @mindt DATETIME = DATEADD(D, -@days, GETDATE())
	DECLARE @option INT
	DECLARE @spouse INT
	SELECT	@option = ISNULL(ContributionOptionsId,1), 
			@spouse = SpouseId
	FROM dbo.People 
	WHERE PeopleId = @pid
	
	
	SELECT @cnt = COUNT(*)
	FROM dbo.Contribution c
	WHERE 
	c.ContributionDate >= @mindt
	AND (c.FundId = @fundid OR @fundid IS NULL)
	AND c.ContributionStatusId = 0 --Recorded
	AND c.ContributionTypeId = 8
	AND ((@option <> 2 AND c.PeopleId = @pid)
		 OR (@option = 2 AND c.PeopleId IN (@pid, @spouse)))

	-- Return the result of the function
	RETURN @cnt

END



GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
