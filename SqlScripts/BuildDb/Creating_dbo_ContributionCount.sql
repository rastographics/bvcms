-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ContributionCount](@pid INT, @days INT, @fundid INT)
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
	
	IF (@option = 2)
		SELECT @cnt = COUNT(*)
		FROM dbo.Contribution c
		WHERE 
		c.ContributionDate >= @mindt
		AND (c.FundId = @fundid OR @fundid IS NULL)
		AND c.ContributionStatusId = 0 --Recorded
		AND c.ContributionTypeId NOT IN (6,7,8) --Reversed or returned
		AND c.PeopleId IN (@spouse, @pid)
	else
		SELECT @cnt = COUNT(*)
		FROM dbo.Contribution c
		WHERE 
		c.ContributionDate >= @mindt
		AND (c.FundId = @fundid OR @fundid IS NULL)
		AND c.ContributionStatusId = 0 --Recorded
		AND c.ContributionTypeId NOT IN (6,7,8) --Reversed or returned
		AND c.PeopleId = @pid
	
/*	AND ((@option <> 2 AND c.PeopleId = @pid)
		 OR (@option = 2 AND c.PeopleId IN (@pid, @spouse))) */

	-- Return the result of the function
	RETURN @cnt

END



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
