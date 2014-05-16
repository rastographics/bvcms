-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[LastIdInTrans] 
( @oid INT, @pid INT )
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @mt INT

	-- Add the T-SQL statements to compute the return value here
SELECT TOP 1 @mt = TransactionId FROM dbo.EnrollmentTransaction et
WHERE OrganizationId = @oid AND PeopleId = @pid
AND TransactionTypeId <= 3
ORDER BY TransactionId DESC
	-- Return the result of the function
	RETURN @mt

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
