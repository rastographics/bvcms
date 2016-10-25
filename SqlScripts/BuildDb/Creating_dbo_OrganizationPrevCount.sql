CREATE FUNCTION [dbo].[OrganizationPrevCount](@oid INT) 
RETURNS INT
AS
BEGIN
	DECLARE @c INT
	SELECT @c = COUNT(*)
	FROM dbo.EnrollmentTransaction et 
	WHERE OrganizationId = @oid
	AND TransactionStatus = 0
	AND TransactionTypeId > 3
	AND TransactionDate = (
		SELECT MAX(TransactionDate) 
		FROM dbo.EnrollmentTransaction 
		WHERE PeopleId = et.PeopleId 
		AND OrganizationId = et.OrganizationId 
		AND TransactionTypeId > 3
	) 
	AND NOT EXISTS(
		SELECT NULL 
		FROM dbo.OrganizationMembers 
		WHERE PeopleId = et.PeopleId 
		AND OrganizationId = et.OrganizationId
	)
	RETURN @c
END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
