CREATE PROCEDURE [dbo].[UpdateAllOrganizations]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE dbo.Organizations
	SET MemberCount = dbo.OrganizationMemberCount(OrganizationId),
		PrevMemberCount = dbo.OrganizationPrevCount(OrganizationId),
		ProspectCount = dbo.OrganizationProspectCount(OrganizationId)

	UPDATE dbo.People
	SET Grade = ISNULL(dbo.SchoolGrade(PeopleId), Grade),
	BibleFellowshipClassId = dbo.BibleFellowshipClassId(PeopleId)

	UPDATE dbo.Organizations
	SET LeaderId = dbo.OrganizationLeaderId(o.OrganizationId),
	LeaderName = dbo.OrganizationLeaderName(o.OrganizationId)
	FROM dbo.Organizations o

END

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
