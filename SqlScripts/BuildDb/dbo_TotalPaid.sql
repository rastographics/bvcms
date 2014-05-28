

CREATE FUNCTION [dbo].[TotalPaid](@oid int, @pid int) 
RETURNS int
AS
BEGIN
	DECLARE @c MONEY, @mt BIT, @tranid INT

	SELECT
		@tranid = TranId,
		@mt = IsMissionTrip 
	FROM dbo.OrganizationMembers om
	JOIN dbo.Organizations o on o.OrganizationId = om.OrganizationId
	where om.OrganizationId = @oid AND om.PeopleId = @pid 

	IF @mt = 1 AND EXISTS(SELECT NULL FROM dbo.GoerSenderAmounts WHERE OrgId = @oid)
		SELECT @c = SUM(Amount) FROM dbo.GoerSenderAmounts
		WHERE GoerId = @pid
		AND OrgId = @oid
		AND ISNULL(InActive, 0) = 0
	ELSE
		SELECT @c = IndPaid FROM dbo.TransactionSummary WHERE RegId = @tranid

	RETURN @c
END


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
