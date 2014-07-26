CREATE PROCEDURE [dbo].[DropOrgMemberAndET](@oid INT, @pid INT)
AS
BEGIN
	SET NOCOUNT ON

	DELETE dbo.OrgMemMemTags 
	WHERE PeopleId = @pid AND OrgId = @oid

	DELETE dbo.OrganizationMembers 
	WHERE PeopleId = @pid AND OrganizationId = @oid

	DELETE dbo.EnrollmentTransaction
	WHERE OrganizationId = @oid 
	AND PeopleId = @pid 

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
