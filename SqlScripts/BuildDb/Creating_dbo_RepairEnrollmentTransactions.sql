CREATE PROC [dbo].[RepairEnrollmentTransactions](@oid INT)
AS 
BEGIN
	DECLARE @t TABLE (id INT, typ INT, dt DATETIME, pid INT)
	INSERT @t (id, typ, dt, pid)
	SELECT TransactionId, TransactionTypeId, TransactionDate, PeopleId FROM dbo.EnrollmentTransaction WHERE OrganizationId = @oid

	UPDATE dbo.EnrollmentTransaction
	SET NextTranChangeDate = (SELECT TOP 1 dt FROM @t WHERE e.TransactionTypeId <= 3 AND pid = e.PeopleId AND dt > e.TransactionDate ORDER BY dt) 
	  , EnrollmentTransactionId = (SELECT TOP 1 id FROM @t WHERE typ <= 2 AND pid = e.PeopleId AND dt < e.TransactionDate AND e.TransactionTypeId >= 3 ORDER BY dt DESC) 
	FROM dbo.EnrollmentTransaction e
	WHERE OrganizationId = @oid AND e.TransactionId = TransactionId

	UPDATE dbo.Organizations
	SET MemberCount = dbo.OrganizationMemberCount(OrganizationId),
		ProspectCount = dbo.OrganizationProspectCount(OrganizationId)
	WHERE OrganizationId = @oid
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
