-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[BaptismAgeRange](@age int) 
RETURNS nvarchar(20)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @r nvarchar(20)

	-- Add the T-SQL statements to compute the return value here
	SELECT @r = 
		CASE 
		WHEN @age IS NULL THEN ' NA'
		WHEN @age < 12 THEN '0-11'
		WHEN @age < 19 THEN '12-18'
		WHEN @age < 24 THEN '19-23'
		WHEN @age < 31 THEN '24-30'
		WHEN @age < 41 THEN '31-40'
		WHEN @age < 51 THEN '41-50'
		WHEN @age < 61 THEN '51-60'
		WHEN @age < 71 THEN '61-70'
		ELSE 'Over 70'
		END
		

	-- Return the result of the function
	RETURN @r

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
