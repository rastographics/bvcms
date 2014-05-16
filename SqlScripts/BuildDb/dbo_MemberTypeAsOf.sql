CREATE FUNCTION [dbo].[MemberTypeAsOf]
( @oid INT, @pid INT, @dt DATETIME )
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @mt INT

	-- Add the T-SQL statements to compute the return value here
SELECT TOP 1 @mt = MemberTypeId FROM dbo.EnrollmentTransaction et
WHERE OrganizationId = @oid AND PeopleId = @pid
AND TransactionDate <= @dt 
AND (NextTranChangeDate >= @dt OR NextTranChangeDate IS NULL)
AND TransactionTypeId <= 3
ORDER BY TransactionDate DESC
	RETURN @mt

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
