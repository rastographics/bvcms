CREATE FUNCTION [dbo].[OrganizationPrevMemberCount](@oid int) 
RETURNS int
AS
BEGIN
	DECLARE @c int
	
SELECT @c = COUNT(*)
FROM dbo.EnrollmentTransaction et
WHERE TransactionStatus = 0
AND OrganizationId = @oid
AND TransactionTypeId > 3
AND TransactionDate = (SELECT MAX(TransactionDate) FROM dbo.EnrollmentTransaction WHERE PeopleId = et.PeopleId AND OrganizationId = et.OrganizationId AND TransactionTypeId > 3 AND TransactionStatus = 0)
AND NOT EXISTS(SELECT NULL FROM dbo.OrganizationMembers WHERE PeopleId = et.PeopleId AND OrganizationId = et.OrganizationId)
	
	RETURN @c
END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
