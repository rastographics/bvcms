-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[ContributionChange](@pid INT, @dt1 DATETIME, @dt2 DATETIME)
RETURNS FLOAT
AS
BEGIN
	DECLARE @pct FLOAT
	DECLARE @amt1 MONEY
	DECLARE @amt2 MONEY
	DECLARE @option INT
	DECLARE @spouse INT
	SELECT	@option = ISNULL(ContributionOptionsId,1), 
			@spouse = SpouseId
	FROM dbo.People 
	WHERE PeopleId = @pid
	
	SELECT @amt1 = SUM(ContributionAmount)
	FROM dbo.Contribution
	WHERE 
	ContributionDate >= @dt1
	AND ContributionDate < @dt2
	AND ContributionStatusId = 0 --Recorded
	AND ContributionTypeId NOT IN (6,7) --Reversed or returned
	AND ((@option <> 2 AND PeopleId = @pid)
		 OR (@option = 2 AND PeopleId IN (@pid, @spouse)))
		 
	SELECT @amt2 = SUM(ContributionAmount)
	FROM dbo.Contribution
	WHERE 
	ContributionDate >= @dt2
	AND ContributionStatusId = 0 --Recorded
	AND ContributionTypeId NOT IN (6,7) --Reversed or returned
	AND ((@option <> 2 AND PeopleId = @pid)
		 OR (@option = 2 AND PeopleId IN (@pid, @spouse)))

	-- Return the result of the function
	IF @amt1 = 0
		SET @amt1 = .01
	IF @amt2 is NULL	
		SET @amt2 = 0
	SET @pct = @amt2 / @amt1 * 100
	RETURN @pct

END



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
