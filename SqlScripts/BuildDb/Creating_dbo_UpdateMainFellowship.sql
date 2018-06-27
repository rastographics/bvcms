CREATE PROCEDURE [dbo].[UpdateMainFellowship] (@orgid INT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE dbo.People
	SET BibleFellowshipClassId = dbo.BibleFellowshipClassId(p.PeopleId)
	FROM dbo.People p
	JOIN dbo.OrganizationMembers om ON p.PeopleId = om.PeopleId
	WHERE om.OrganizationId = @orgid

	update dbo.Organizations
	set MemberCount = dbo.OrganizationMemberCount(OrganizationId),
		ProspectCount = dbo.OrganizationProspectCount(OrganizationId)
	WHERE OrganizationId = @orgid
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
