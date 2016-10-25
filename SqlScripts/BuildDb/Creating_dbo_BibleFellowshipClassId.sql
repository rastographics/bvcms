
CREATE FUNCTION [dbo].[BibleFellowshipClassId]
	(
	@pid int
	)
RETURNS int
AS
	BEGIN
	declare @oid INT

	select top 1 @oid = om.OrganizationId from dbo.OrganizationMembers AS om 
	JOIN dbo.Organizations AS o ON om.OrganizationId = o.OrganizationId
	WHERE o.IsBibleFellowshipOrg = 1 
	AND om.PeopleId = @pid
	AND ISNULL(om.Pending, 0) = 0

	return @oid
	END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
