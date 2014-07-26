CREATE FUNCTION [dbo].[GetPeopleIdFromACS](@famnum int, @indnum int)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @id int

	-- Add the T-SQL statements to compute the return value here
	SELECT @id = PeopleId 
	FROM dbo.PeopleExtra
	WHERE Field = 'IndividualNumber' 
	AND IntValue = @famnum 
	AND IntValue2 = @indnum

	-- Return the result of the function
	RETURN @id

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
