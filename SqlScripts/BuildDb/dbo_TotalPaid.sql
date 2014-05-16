

CREATE FUNCTION [dbo].[TotalPaid](@oid int, @pid int) 
RETURNS int
AS
BEGIN
	DECLARE @c MONEY, @mt BIT
	SELECT @c = AmountPaid, @mt = IsMissionTrip from dbo.OrganizationMembers om
	JOIN dbo.Organizations o on o.OrganizationId = om.OrganizationId
	where om.OrganizationId = @oid AND om.PeopleId = @pid 

	IF @mt = 1
		SELECT @c = SUM(Amount) FROM dbo.GoerSenderAmounts
		WHERE GoerId = @pid
		AND OrgId = @oid
		AND ISNULL(InActive, 0) = 0

	RETURN @c
END


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
