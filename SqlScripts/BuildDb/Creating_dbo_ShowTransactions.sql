CREATE PROCEDURE [dbo].[ShowTransactions](@pid INT, @orgid INT)
AS
BEGIN
	SELECT
		TransactionId, 
		TransactionDate, 
		TransactionTypeId, 
		OrganizationId, 
		PeopleId, 
		NextTranChangeDate,
		dbo.NextTranChangeDate(PeopleId, OrganizationId, TransactionDate, TransactionTypeId) NextTranChangeDate0,
		EnrollmentTransactionId,
		dbo.EnrollmentTransactionId(PeopleId, OrganizationId, TransactionDate, TransactionTypeId) EnrollmentTransactionId0,
		CreatedDate
	FROM dbo.EnrollmentTransaction
	WHERE PeopleId = @pid AND (OrganizationId = @orgid OR @orgid = 0)
	ORDER BY TransactionDate

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
