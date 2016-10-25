CREATE FUNCTION [dbo].[OrganizationMemberCount](@oid int) 
RETURNS int
AS
BEGIN
	DECLARE @c int
	SELECT @c = count(*) from dbo.OrganizationMembers 
	where OrganizationId = @oid
	AND (Pending = 0 OR Pending IS NULL)
	AND MemberTypeId NOT IN (230, 311)
	RETURN @c
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
