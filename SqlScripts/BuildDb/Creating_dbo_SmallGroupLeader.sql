CREATE FUNCTION [dbo].[SmallGroupLeader]
(
	@oid INT, @pid INT
)
RETURNS VARCHAR(100)
AS
BEGIN
	DECLARE @ret VARCHAR(100)

	SELECT TOP 1 @ret = mtsg.Name
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
