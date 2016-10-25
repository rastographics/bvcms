CREATE FUNCTION [dbo].[IsSmallGroupLeaderOnly]
(
	@oid INT, @pid INT
)
RETURNS INT
AS
BEGIN
	DECLARE @ret INT

	SELECT TOP 1 @ret = mtsg.Id
	FROM dbo.OrganizationMembers om
	JOIN dbo.OrgMemMemTags ommt ON ommt.OrgId = om.OrganizationId AND ommt.PeopleId = om.PeopleId
	JOIN dbo.MemberTags mt ON mt.Id = ommt.MemberTagId
	JOIN dbo.MemberTags mtsg ON mtsg.OrgId = @oid AND mtsg.Name = SUBSTRING(mt.Name, 1, LEN(mt.Name) - 9)
	WHERE om.OrganizationId = @oid AND om.PeopleId = @pid
	AND mt.Name LIKE '%-sgleader'

	RETURN @ret

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
