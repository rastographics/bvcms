CREATE FUNCTION [dbo].[OrganizationMemberCount2](@oid int) 
RETURNS int
AS
BEGIN
	DECLARE @c INT, @ct INT
	SELECT @c = count(*) from dbo.OrganizationMembers om
	JOIN dbo.Organizations o on o.OrganizationId = om.OrganizationId
	where om.OrganizationId = @oid 
	AND (Pending IS NULL OR Pending = 0)
	AND MemberTypeId NOT IN (230, 311)
	AND (ISNULL(o.IsMissionTrip, 0) = 0 
		OR EXISTS(SELECT NULL 
				  FROM dbo.OrgMemMemTags omt 
				  JOIN dbo.MemberTags mt ON mt.Id = omt.MemberTagId 
				  WHERE omt.OrgId = @oid 
				  AND omt.PeopleId = om.PeopleId 
				  AND mt.Name = 'Goer'))

	SELECT @ct = SUM(ISNULL(Tickets, 0)) - COUNT(*) FROM dbo.OrganizationMembers om
	JOIN dbo.Organizations o on o.OrganizationId = om.OrganizationId
	where om.OrganizationId = @oid 
	AND (Pending IS NULL OR Pending = 0)
	AND MemberTypeId NOT IN (230, 311)
	AND Tickets > 0

	RETURN @c + ISNULL(@ct, 0)
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
