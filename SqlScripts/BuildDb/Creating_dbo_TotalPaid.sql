CREATE FUNCTION [dbo].[TotalPaid](@oid int, @pid int) 
RETURNS MONEY
AS
BEGIN
	DECLARE @c MONEY, @mt BIT, @tranid INT, @joindt DATETIME

	SELECT
		@tranid = TranId,
		@mt = IsMissionTrip,
		@joindt = om.EnrollmentDate
	FROM dbo.OrganizationMembers om
	JOIN dbo.Organizations o on o.OrganizationId = om.OrganizationId
	where om.OrganizationId = @oid AND om.PeopleId = @pid 


	-- get amount paid by person
	SET @c = ISNULL((SELECT IndPaid FROM dbo.TransactionSummary WHERE RegId = @tranid AND PeopleId = @pid), 0)

	-- if this is a mission trip then add amount from other people supporting this person
	IF @mt = 1 AND EXISTS(SELECT NULL FROM dbo.GoerSenderAmounts WHERE OrgId = @oid)
		SET @c = ISNULL(@c,0) + 
			ISNULL((SELECT SUM(Amount) 
			FROM dbo.GoerSenderAmounts
			WHERE GoerId = @pid
			AND OrgId = @oid
			AND Created > @joindt
		 	--AND ISNULL(InActive, 0) = 0
			AND SupporterId <> @pid),0)

	RETURN @c
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
