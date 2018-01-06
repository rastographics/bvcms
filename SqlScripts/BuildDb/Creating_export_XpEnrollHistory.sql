CREATE VIEW [export].[XpEnrollHistory] AS 
SELECT 
	TransactionId ,
    OrganizationId ,
    PeopleId ,
    TransactionDate ,
    OrganizationName ,
    MemberTypeId ,
    TransactionType = (CASE TransactionTypeId WHEN 1 THEN 'Join' WHEN 5 THEN 'Drop' END),
    EnrollmentTransactionId 
FROM dbo.EnrollmentTransaction
WHERE TransactionTypeId IN (1,5)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
