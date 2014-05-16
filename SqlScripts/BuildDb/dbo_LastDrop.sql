CREATE FUNCTION [dbo].[LastDrop] (@orgid INT, @pid INT)
RETURNS DATETIME
AS
	BEGIN
	DECLARE @dt DATETIME
	
	SELECT @dt = TransactionDate FROM dbo.EnrollmentTransaction
	WHERE TransactionTypeId > 3
	AND PeopleId = @pid
	AND OrganizationId = @orgid
	AND MemberTypeId <> 311
	IF @dt IS NULL
		SET @dt = '1/1/1900'
	
	RETURN @dt
	END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
