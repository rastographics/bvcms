CREATE FUNCTION [dbo].[OrganizationProspectCount](@oid INT) 
RETURNS INT
AS
BEGIN
	DECLARE @c INT
	SELECT @c = COUNT(*) FROM dbo.OrganizationMembers 
	WHERE OrganizationId = @oid
	AND ISNULL(Pending, 0) = 0
	AND MemberTypeId = 311
	RETURN @c
END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
